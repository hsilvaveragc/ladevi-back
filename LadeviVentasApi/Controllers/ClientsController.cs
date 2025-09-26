namespace LadeviVentasApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Services.Xubio;
using LadeviVentasApi.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : RestV2Controller<Client, ClientWritingDto>
{
  private readonly XubioService xubioService;

  public ClientsController(ApplicationDbContext context, IMapper mapper, XubioService xubioService) : base(context, mapper)
  {
    this.xubioService = xubioService;
  }

  public override async Task<IActionResult> Post(ClientWritingDto x)
  {
    try
    {
      ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;

      var clientDb = Mapper.Map<Client>(x);

      TryValidateModel(clientDb);
      if (!ModelState.IsValid) return ActionResultForModelStateValidation();

      using var transaction = await Context.Database.BeginTransactionAsync();

      try
      {
        Context.Add(clientDb);
        await Context.SaveChangesAsync();

        await ProcessXubioIntegration(clientDb);

        await RegisterAuditoryRecord(clientDb, "Creación");
        await transaction.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = clientDb.Id },
            Mapper.Map<ClientWritingDto>(clientDb));
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
    catch (ValidationExtensions.ValidationException ex)
    {
      return HandleValidationException(ex);
    }
    catch (Exception ex)
    {
      return HandleGenericException(ex);
    }
  }

  public override async Task<IActionResult> Put(long id, ClientWritingDto x)
  {
    try
    {
      ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;

      if (id != x.Id) return BadRequest();

      var clientToUpdate = Mapper.Map<Client>(x);
      var oldEntity = GetByIdNoTracking(id);
      clientToUpdate.Id = id;

      // Validación inicial
      TryValidateModel(clientToUpdate);
      if (!ModelState.IsValid) return ActionResultForModelStateValidation();

      // Usar transacción para consistencia
      using var transaction = await Context.Database.BeginTransactionAsync();

      try
      {
        // Detach local entity si existe
        var local = Context.Set<Client>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(id));

        if (local != null)
        {
          Context.Entry(local).State = EntityState.Detached;
        }

        // Verificar si el CUIT cambió y podría generar conflictos
        if (oldEntity != null && oldEntity.IdentificationValue != clientToUpdate.IdentificationValue)
        {
          await ValidateIdentificationValueChange(clientToUpdate, id);
        }

        // Actualizar cliente
        Context.Entry(clientToUpdate).State = EntityState.Modified;
        await Context.SaveChangesAsync();

        // Registrar auditoría
        await RegisterAuditoryRecord(clientToUpdate, "Modificación", oldEntity);

        await transaction.CommitAsync();
        return NoContent();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
    catch (ValidationExtensions.ValidationException ex)
    {
      return HandleValidationException(ex);
    }
    catch (Exception ex)
    {
      return HandleGenericException(ex);
    }
  }

  public override async Task<ActionResult<Client>> Delete(long id)
  {
    var hasContractsClient = Context.Contracts.Any(c => c.ClientId == id && (!c.Deleted.HasValue || !c.Deleted.Value));

    if (!hasContractsClient)
    {
      var client = Context.Clients.AsNoTracking().SingleOrDefault(c => c.Id == id);
      var xubioId = client.XubioId;
      var result = await base.Delete(id);

      // if (xubioId != null && !Context.Clients.Any(c => c.XubioId == xubioId && c.Id != id))
      // {
      //   var response = await this.xubioService.DeleteClientAsync(client.XubioId.Value, client.IsComtur);
      // }
      return result;
    }

    return BadRequest();
  }

  [HttpGet("Options")]
  public async Task<IActionResult> Options()
  {
    bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
    long userId = CurrentAppUser.Value.Id;

    var allClientsQuery = Context.Clients.AsNoTracking().Where(x => !x.Deleted.HasValue || !x.Deleted.Value);

    if (isSeller)
    {
      allClientsQuery = allClientsQuery.Where(client => client.ApplicationUserSellerId == userId);
    }

    var allClientsOptions = await allClientsQuery.Select(x => new
    {
      x.Id,
      x.BrandName,
      x.LegalName,
      x.IsEnabled
    }).ToListAsync();

    return Ok(allClientsOptions);
  }

  [HttpGet("OptionsFull")]
  public async Task<IActionResult> OptionsFull(bool onlyArgentina = false, bool onlyComtur = false, bool onlyEnabled = false)
  {
    bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
    long userId = CurrentAppUser.Value.Id;

    var allClientsQuery = Context.Clients
                                .Include(c => c.Country)
                                .AsNoTracking()
                                .Where(x => !x.Deleted.HasValue || !x.Deleted.Value);

    if (isSeller)
    {
      allClientsQuery = allClientsQuery.Where(client => client.ApplicationUserSellerId == userId);
    }

    if (onlyArgentina)
    {
      allClientsQuery = allClientsQuery.Where(c => c.CountryId == 4);
    }

    if (onlyComtur)
    {
      allClientsQuery = allClientsQuery.Where(c => c.IsComtur && c.BillingPointOfSale.Trim().TrimStart('0') == "99");
    }

    if (onlyEnabled)
    {
      allClientsQuery = allClientsQuery.Where(c => c.IsEnabled);
    }

    var allClientsOptions = await allClientsQuery.Select(x => new
    {
      x.Id,
      x.BrandName,
      x.LegalName,
      x.IsEnabled,
      x.ApplicationUserSellerId,
      x.IsAgency,
      x.IsComtur,
      x.CountryId,
      CountryName = x.Country != null ? x.Country.Name : string.Empty,
      x.XubioId,
      x.IsBigCompany
    })
    .OrderBy(c => c.BrandName)
    .ToListAsync();

    return Ok(allClientsOptions);
  }

  [HttpPost("SyncXubioArgentinaNotComturClients")]
  [AllowAnonymous]
  public async Task<IActionResult> SyncXubioArgentinaNotComturClients()
  {
    try
    {
      // Get Argentina clients from Xubio (not Comtur)
      var xubioClients = await this.xubioService.GetClientsAsync(false);

      if (xubioClients == null)
      {
        return BadRequest("No se pudo obtener clientes de la API de Xubio");
      }

      // Filter clients by Argentina
      var argentinaXubioClients = xubioClients.Where(c => c.Pais != null && c.Pais.Codigo == "ARGENTINA").ToList();

      // Retrieve Argentina no comtur clients from database
      var dbClients = await Context.Clients
          .Where(c => (!c.Deleted.HasValue || !c.Deleted.Value) && c.CountryId == 4 && !c.IsComtur && !c.XubioId.HasValue)
          .ToListAsync();

      var dbTaxCategories = await Context.TaxCategories
          .Where(tc => !tc.Deleted.HasValue || !tc.Deleted.Value)
          .ToListAsync();

      int updatedCount = 0;
      var updatedClients = new List<object>();

      foreach (var argentinaXubioClient in argentinaXubioClients)
      {
        // Find matching client in database
        var matchingClient = dbClients.FirstOrDefault(c => c.IdentificationValue == argentinaXubioClient.Cuit);

        if (matchingClient != null && matchingClient.XubioId != argentinaXubioClient.ClientId)
        {
          // Update XubioId
          matchingClient.XubioId = argentinaXubioClient.ClientId;
          matchingClient.TaxCategoryId = dbTaxCategories.SingleOrDefault(tc => tc.Code == argentinaXubioClient.CategoriaFiscal.Codigo)?.Id;
          Context.Update(matchingClient);
          updatedCount++;

          updatedClients.Add(new
          {
            XubioClientId = argentinaXubioClient.ClientId,
            ClientName = matchingClient.LegalName,
            XubioName = argentinaXubioClient.Nombre,
            matchingClient.IdentificationValue,
            XubioCuit = argentinaXubioClient.Cuit
          });
        }
      }

      // Save changes to database
      await Context.SaveChangesAsync();

      #region Auditory
      try
      {
        Auditory audit = new Auditory();
        audit.Date = DateTime.Now;
        audit.Entity = "Cliente";
        audit.UserId = CurrentAppUser.Value.Id;
        audit.User = CurrentAppUser.Value.FullName;
        audit.AuditMessage = $"Sincronización con Xubio. Total clientes sincronizados: {updatedCount}";
        Context.Add(audit);
        await Context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        // Ignoramos errores en auditoría para no interrumpir el proceso principal
      }
      #endregion

      return Ok(new
      {
        Mensaje = $"Sincronización completada. Se actualizaron {updatedCount} clientes.",
        TotalClientesXubio = argentinaXubioClients.Count,
        ClientesActualizados = updatedCount,
        DetalleClientesActualizados = updatedClients
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Mensaje = "Ocurrió un error durante la sincronización", Error = ex.Message });
    }
  }

  [HttpPost("SyncXubioComturClients")]
  [AllowAnonymous]
  public async Task<IActionResult> SyncXubioComturClients()
  {
    try
    {
      // Get Comtur Xubio clients
      var xubioClients = await this.xubioService.GetClientsAsync(true);

      if (xubioClients == null)
      {
        return BadRequest("No se pudo obtener clientes de la API de Xubio");
      }

      // Filter clients
      var comturXubioClients = xubioClients.Where(c => c.Pais != null && !string.IsNullOrEmpty(c.Cuit)).ToList();

      // Get the cuit field whose value is equivalent to the database id
      var clientDbIds = comturXubioClients.Select(x => x.Cuit.TrimStart('0')).ToList();

      // Find clients by Id
      var dbClients = await Context.Clients
          .Where(c => !c.XubioId.HasValue && clientDbIds.Contains(c.Id.ToString()))
          .ToListAsync();

      var dbTaxCategories = await Context.TaxCategories
          .Where(tc => !tc.Deleted.HasValue || !tc.Deleted.Value)
          .ToListAsync();

      int updatedCount = 0;
      var updatedClients = new List<object>();

      foreach (var comturXubioClient in comturXubioClients)
      {
        // Find matching client in database
        var matchingClient = dbClients.FirstOrDefault(c => c.Id.ToString() == comturXubioClient.Cuit.TrimStart('0'));

        if (matchingClient != null && matchingClient.XubioId != comturXubioClient.ClientId)
        {
          // Update XubioId
          matchingClient.XubioId = comturXubioClient.ClientId;
          matchingClient.TaxCategoryId = dbTaxCategories.SingleOrDefault(tc => tc.Code == comturXubioClient.CategoriaFiscal.Codigo)?.Id;
          Context.Update(matchingClient);
          updatedCount++;

          updatedClients.Add(new
          {
            XubioClientId = comturXubioClient.ClientId,
            ClientName = matchingClient.LegalName,
            XubioName = comturXubioClient.Nombre,
            XubioIdentification = comturXubioClient.Cuit
          });
        }
      }

      // Save changes to database
      await Context.SaveChangesAsync();

      #region Auditory
      try
      {
        Auditory audit = new Auditory();
        audit.Date = DateTime.Now;
        audit.Entity = "Cliente";
        audit.UserId = CurrentAppUser.Value.Id;
        audit.User = CurrentAppUser.Value.FullName;
        audit.AuditMessage = $"Sincronización con Xubio. Total clientes sincronizados: {updatedCount}";
        Context.Add(audit);
        await Context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        // Ignoramos errores en auditoría para no interrumpir el proceso principal
      }
      #endregion

      return Ok(new
      {
        Mensaje = $"Sincronización completada. Se actualizaron {updatedCount} clientes.",
        TotalClientesXubio = comturXubioClients.Count,
        ClientesActualizados = updatedCount,
        DetalleClientesActualizados = updatedClients
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Mensaje = "Ocurrió un error durante la sincronización", Error = ex.Message });
    }
  }

  // Crear cliente y asociar en una sola operación:
  [HttpPost("CreateAndAssociate")]
  public async Task<IActionResult> CreateAndAssociate([FromBody] CreateOrEditAndAssociateDto request)
  {
    try
    {
      ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;

      var clientDb = Mapper.Map<Client>(request.ClientData);

      TryValidateModel(clientDb);
      if (!ModelState.IsValid) return ActionResultForModelStateValidation();

      using var transaction = await Context.Database.BeginTransactionAsync();

      try
      {
        // Crear cliente sin validación Xubio
        Context.Add(clientDb);
        await Context.SaveChangesAsync();

        // Asociar directamente con el XubioId proporcionado
        clientDb.XubioId = request.XubioId;
        Context.Update(clientDb);
        await Context.SaveChangesAsync();

        await RegisterAuditoryRecord(clientDb, "Creación");
        await transaction.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = clientDb.Id },
            Mapper.Map<ClientWritingDto>(clientDb));
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
    catch (ValidationExtensions.ValidationException ex)
    {
      return HandleValidationException(ex);
    }
    catch (Exception ex)
    {
      return HandleGenericException(ex);
    }
  }

  // Endpoint simple para confirmar asociación
  [HttpPut("EditAndAssociate/{id}")]
  public async Task<IActionResult> EditAndAssociate(long id, [FromBody] CreateOrEditAndAssociateDto x)
  {
    try
    {
      ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;

      if (id != x.ClientData.Id) return BadRequest();

      var clientToUpdate = Mapper.Map<Client>(x.ClientData);
      var oldEntity = GetByIdNoTracking(id);
      clientToUpdate.Id = id;
      clientToUpdate.XubioId = x.XubioId;

      // Validación inicial
      TryValidateModel(clientToUpdate);
      if (!ModelState.IsValid) return ActionResultForModelStateValidation();

      // Usar transacción para consistencia
      using var transaction = await Context.Database.BeginTransactionAsync();

      try
      {
        // Detach local entity si existe
        var local = Context.Set<Client>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(id));

        if (local != null)
        {
          Context.Entry(local).State = EntityState.Detached;
        }

        // Actualizar cliente
        Context.Entry(clientToUpdate).State = EntityState.Modified;
        await Context.SaveChangesAsync();

        // Registrar auditoría
        await RegisterAuditoryRecord(clientToUpdate, "Modificación", oldEntity);

        await transaction.CommitAsync();
        return NoContent();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
    catch (ValidationExtensions.ValidationException ex)
    {
      return HandleValidationException(ex);
    }
    catch (Exception ex)
    {
      return HandleGenericException(ex);
    }
  }

  protected override IQueryable<Client> GetQueryableWithIncludes()
  {
    bool isSeller = CurrentAppUser.Value.ApplicationRole.IsSeller();
    bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();

    long userId = CurrentAppUser.Value.Id;
    long countryId = CurrentAppUser.Value.CountryId;

    var allClients = base.GetQueryableWithIncludes()
        .Include(c => c.City)
        .Include(c => c.District)
        .Include(c => c.State)
        .Include(c => c.Country)
        .Include(c => c.ApplicationUserSeller)
        .Include(c => c.ApplicationUserDebtCollector)
        .Include(c => c.TaxCategory);

    if (isSeller)
    {
      return allClients.Where(client => client.ApplicationUserSellerId == userId);
    }
    return allClients;
  }

  protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
  {
    var result = GetSearchQueryable()
        .Select(x => new
        {
          x.XubioId,
          x.Address,
          x.AlternativeEmail,
          x.ApplicationUserDebtCollector,
          x.ApplicationUserDebtCollectorId,
          x.ApplicationUserSeller,
          x.ApplicationUserSellerId,
          x.BillingPointOfSale,
          x.BrandName,
          FullName = x.BrandName + " - " + x.LegalName,
          x.CityId,
          x.City,
          x.DistrictId,
          x.District,
          x.StateId,
          x.State,
          x.CountryId,
          x.Country,
          x.ElectronicBillByMail,
          x.ElectronicBillByPaper,
          x.Id,
          x.IsAgency,
          x.IsComtur,
          x.IsEnabled,
          x.LegalName,
          x.MainEmail,
          x.PostalCode,
          x.ShouldDelete,
          x.TelephoneAreaCode,
          x.TelephoneCountryCode,
          x.TelephoneNumber,
          x.TaxPercentage,
          x.TaxTypeId,
          x.IdentificationValue,
          x.Contact,
          CanDelete = !Context.Contracts.Any(c => c.ClientId == x.Id && (!c.Deleted.HasValue || !c.Deleted.Value)),
          x.TaxCategoryId,
          x.IsBigCompany
        })
        .ToDataSourceResult(request);

    return result;
  }

  private async Task ProcessXubioIntegration(Client clientDb)
  {
    // Verificar si se debe sincronizar con Xubio
    if (!ShouldSyncToXubio(clientDb))
    {
      return;
    }

    var identificationXubioValue = clientDb.ShouldSyncToXubioArgentina() ? clientDb.IdentificationValue : clientDb.Id.ToString().PadLeft(8, '0');

    // Verificar si el cliente ya existe en Xubio
    var xubioClients = await xubioService.GetClientsAsync(clientDb.IsComtur, identificationXubioValue);

    if (xubioClients != null && xubioClients.Any())
    {
      if (clientDb.ShouldSyncToXubioComtur())
      {
        throw new ValidationExtensions.ValidationException("Ya existe un cliente en Xubio para el cliente que quiere crear.", new[] { "brandName" });
      }

      // Cliente ya existe en Xubio
      var existingXubioClient = xubioClients.First();

      // Verificar si ya existe un cliente local con ese XubioId
      var existingLocalClient = await Context.Clients
          .Where(c => c.XubioId == existingXubioClient.ClientId &&
                     c.Id != clientDb.Id && // Excluir el cliente actual
                     (!c.Deleted.HasValue || !c.Deleted.Value))
          .FirstOrDefaultAsync();

      if (existingLocalClient != null)
      {
        // En vez de lanzar error normal, lanzar error especial
        throw new ValidationExtensions.ValidationException(
            $"DUPLICATE_CUIT|{existingXubioClient.ClientId}|{existingLocalClient.LegalName}|{clientDb.IdentificationValue}",
            new[] { "identificationValue" });
      }

      // Si no hay conflicto, vincular con el XubioId existente
      clientDb.XubioId = existingXubioClient.ClientId;
      Context.Update(clientDb);
      await Context.SaveChangesAsync();
    }
    else
    {
      // Crear cliente en Xubio
      var response = await xubioService.CreateClientAsync(clientDb);

      if (response.Error != null)
      {
        // Manejar errores específicos de creación en Xubio
        if (response.Error.Description.ToUpper().Contains("CUIT inválido".ToUpper()))
        {
          ValidationExtensions.ThrowIfInvalid(true,
              response.Error.Description, "identificationValue");
        }
        else if (response.Error.Description.ToUpper().Contains("El número de identificación ya ha sido cargado en el sistema".ToUpper()))
        {
          ValidationExtensions.ThrowIfInvalid(true,
              "El número de identificación ya existe en Xubio pero no se pudo vincular correctamente. " +
              "Contacte al administrador del sistema.", "identificationValue");
        }
        else
        {
          ValidationExtensions.ThrowIfInvalid(true,
              $"Error al crear cliente en Xubio: {response.Error.Description}");
        }
      }

      // Asignar XubioId y actualizar
      clientDb.XubioId = response.ClientId;
      Context.Update(clientDb);
      await Context.SaveChangesAsync();

      // Completar datos en Xubio
      await xubioService.EditClientAsync(clientDb);
    }
  }

  // Método auxiliar para validar cambios en el CUIT
  private async Task ValidateIdentificationValueChange(Client clientToUpdate, long clientId)
  {
    // Solo validar con Xubio si se debe sincronizar
    if (!ShouldSyncToXubio(clientToUpdate))
    {
      return;
    }

    // Verificar si el nuevo CUIT ya existe en Xubio
    var xubioClients = await xubioService.GetClientsAsync(clientToUpdate.IsComtur, clientToUpdate.IdentificationValue);

    if (xubioClients != null && xubioClients.Any())
    {
      var existingXubioClient = xubioClients.First();

      // Verificar si ya existe otro cliente local con ese XubioId
      var existingLocalClient = await Context.Clients
          .Where(c => c.XubioId == existingXubioClient.ClientId &&
                     c.Id != clientId && // Excluir el cliente actual
                     (!c.Deleted.HasValue || !c.Deleted.Value))
          .FirstOrDefaultAsync();

      if (existingLocalClient != null)
      {
        throw new ValidationExtensions.ValidationException(
            $"DUPLICATE_CUIT|{existingXubioClient.ClientId}|{existingLocalClient.LegalName}|{existingLocalClient.IdentificationValue}",
            new[] { "identificationValue" });
      }

      // Si no hay conflicto, actualizar XubioId
      clientToUpdate.XubioId = existingXubioClient.ClientId;
    }
    else
    {
      // El nuevo CUIT no existe en Xubio, limpiar XubioId
      // (se podría crear en Xubio posteriormente si se requiere)
      clientToUpdate.XubioId = null;
    }
  }

  // Método auxiliar para determinar si se debe sincronizar con Xubio
  private bool ShouldSyncToXubio(Client client)
  {
    return client.ShouldSyncToXubioArgentina() || client.ShouldSyncToXubioComtur();
  }

  // Método auxiliar para registrar auditoría
  private async Task RegisterAuditoryRecord(Client client, string action, Client oldEntity = null)
  {
    try
    {
      var audit = new Auditory
      {
        Date = DateTime.Now,
        Entity = GetRealNameEntity(client.GetType().Name),
        UserId = CurrentAppUser.Value.Id,
        User = CurrentAppUser.Value.FullName
      };

      // Construir mensaje base
      string name = GetName(client.GetType().Name, client);
      var messageBuilder = new System.Text.StringBuilder();
      messageBuilder.Append($"{action} de {audit.Entity}. Id={client.Id}");

      if (!string.IsNullOrEmpty(name))
      {
        messageBuilder.Append($". Nombre= {name}");
      }

      // Si hay entidad anterior, agregar detalles de cambios
      if (oldEntity != null)
      {
        var changes = Utils.GenerateAuditLogMessages(oldEntity, client);
        if (changes.Any())
        {
          messageBuilder.Append($". {string.Join('|', changes)}");
        }
      }

      audit.AuditMessage = messageBuilder.ToString();

      Context.Add(audit);
      await Context.SaveChangesAsync();
    }
    catch (Exception)
    {
      // Log del error sin interrumpir el flujo principal
      // _logger?.LogWarning("No se pudo registrar la auditoría para el cliente {ClientId}", client.Id);
    }
  }
}
