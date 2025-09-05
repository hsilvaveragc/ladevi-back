namespace LadeviVentasApi.Controllers;

using System.Diagnostics;
using System.Reflection;
using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RestV2Controller<T, TWrite> : ControllerBase
    where T : BaseEntity
    where TWrite : BaseDto
{
    protected ApplicationDbContext Context { get; set; }
    protected IMapper Mapper { get; }
    protected Lazy<ApplicationUser> CurrentAppUser;

    public RestV2Controller(ApplicationDbContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
        CurrentAppUser = new Lazy<ApplicationUser>(GetAppUser, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private ApplicationUser GetAppUser()
    {
        //todo seba tuvo que agregar esto porque sino al correr los seeds del integration controller pinchaba el currency controller linea 25
        var userMail = User?.Identity?.IsAuthenticated != true
            //? null
            ? throw new InvalidOperationException("not logged in")
            : User.Claims.ToDictionary(c => c.Type, c => c.Value)["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
        return
            //userMail == null
            //    ? new ApplicationUser { ApplicationRole = new ApplicationRole { Name = ApplicationRole.SuperuserRole } }
            //    :
            Context.ApplicationUsers.Include(u => u.ApplicationRole)
            .Include(u => u.Country)
            .Single(u => u.CredentialsUser.Email.ToLower() == userMail.ToLower());
    }

    protected static object GetSimpleWsErrorDto(string msg)
    {
        return new { errors = new { error = new[] { msg } } };
    }

    [HttpPost("Search")]
    public virtual DataSourceResult Search([FromBody] KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        return GetSearchDataSourceResult(request);
    }

    protected virtual DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var searchDataSourceResult = GetSearchQueryable().ToDataSourceResult(request);
        foreach (var o in searchDataSourceResult.Data)
        {
            CleanFields(o);
        }
        return searchDataSourceResult;
    }

    protected virtual IQueryable<T> GetSearchQueryable()
    {
        return GetQueryableWithIncludes();
    }

    protected virtual IQueryable<T> GetQueryableWithIncludes()
    {
        return Context.Set<T>().Where(x => !x.Deleted.HasValue || !x.Deleted.Value);
    }

    protected T GetByIdNoTracking(long id)
    {
        var x = Context.Set<T>().AsNoTracking().SingleOrDefault(o => o.Id == id);
        // var x = GetQueryableWithIncludes().AsNoTracking().SingleOrDefaultAsync(o => o.Id == id).Result;
        return x;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<T>> GetById(long id)
    {
        var x = await GetByIdCleaned(id);
        if (x == null) return NotFound();
        return Ok(x);
    }

    protected async Task<T> GetByIdCleaned(long id)
    {
        var x = await GetQueryableWithIncludes().SingleOrDefaultAsync(o => o.Id == id);
        if (x != null) CleanFields(x);
        return x;
    }

    protected virtual void CleanFields(object x)
    {
    }

    [HttpPut("Put/{id}")]
    public virtual async Task<IActionResult> Put(long id, TWrite x)
    {
        try
        {
            ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;
            if (id != x.Id) return BadRequest();
            var xAux = x is T entity ? entity : Mapper.Map<T>(x);
            return await PerformUpdate(id, xAux);
        }
        catch (ValidationExtensions.ValidationException ex)
        {
            return HandleValidationException(ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is ValidationExtensions.ValidationException validationEx)
        {
            return HandleValidationException(validationEx);
        }
        catch (Exception ex)
        {
            return HandleGenericException(ex);
        }
    }

    protected virtual async Task<IActionResult> PerformUpdate(long id, T xAux)
    {
        TryValidateModel(xAux);
        if (!ModelState.IsValid) return ActionResultForModelStateValidation();

        var oldEntity = GetByIdNoTracking(id);

        try
        {
            Upsert(xAux);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        try
        {
            #region Auditory
            try
            {
                Auditory audit = new Auditory();
                audit.Date = DateTime.Now;
                audit.Entity = GetRealNameEntity(xAux.GetType().Name);
                audit.UserId = CurrentAppUser.Value.Id;
                audit.User = CurrentAppUser.Value.FullName;
                string name = GetName(xAux.GetType().Name, xAux);
                audit.AuditMessage = "Id=" + id.ToString() + (string.IsNullOrEmpty(name) ? "" : ". Nombre= " + name) + ". Modificación de " + audit.Entity + ". " + string.Join('|', Utils.GenerateAuditLogMessages(oldEntity, xAux));
                Context.Add(audit);
                await Context.SaveChangesAsync();
            }
            //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
            catch (Exception ex)
            {
            }
            #endregion

            await Context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(id))
            {
                return NotFound();
            }
            throw;
        }
    }

    private void Upsert(T entity)
    {
        //Context.ChangeTracker.TrackGraph(entity, e =>
        //{
        //    e.Entry.State = EntityState.Detached;
        //});
        Context.ChangeTracker.TrackGraph(entity, e =>
        {
            e.Entry.State = e.Entry.IsKeySet
                ? e.Entry.Entity is BaseEntity o && (o.ShouldDelete ?? false)
                    ? EntityState.Deleted
                    : EntityState.Modified
                : EntityState.Added;
        });

#if DEBUG
        foreach (var entry in Context.ChangeTracker.Entries())
        {
            Debug.WriteLine($"Entity: {entry.Entity.GetType().Name} State: {entry.State.ToString()}");
        }
#endif
    }

    [HttpPost("Post")]
    public virtual async Task<IActionResult> Post(TWrite x)
    {
        try
        {
            ControllerContext.HttpContext.Items["current-user"] = CurrentAppUser;
            var xAux = x is T entity ? entity : Mapper.Map<T>(x);
            TryValidateModel(xAux);

            if (ModelState.IsValid)
            {
                Context.Add(xAux);
                await Context.SaveChangesAsync();

                xAux = GetQueryableWithIncludes().Single(o => o.Id == xAux.Id);

                #region Auditory
                try
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = GetRealNameEntity(xAux.GetType().Name);
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    string name = GetName(xAux.GetType().Name, xAux);

                    string descOP = string.Empty;
                    bool isOP = false;

                    if (xAux.GetType() == typeof(PublishingOrder))
                    {
                        ProductAdvertisingSpace pas = Context.ProductAdvertisingSpaces.AsNoTracking().Single(y => y.Id == (long)x.GetType().GetRuntimeField(nameof(PublishingOrder.ProductAdvertisingSpaceId)).GetValue(x));
                        Contract c = Context.Contracts.AsNoTracking().FirstOrDefault(y => y.Id == (long)x.GetType().GetRuntimeField(nameof(PublishingOrder.ContractId)).GetValue(x));
                        descOP = (c == null ? "" : " .Contrato= " + c.Name) + " .Tipo de Espacio: " + pas.Name;
                        isOP = true;
                    }

                    audit.AuditMessage = "Creación de " + audit.Entity + ". Id=" + xAux.Id.ToString() + (isOP ? descOP : (string.IsNullOrEmpty(name) ? "" : ". Nombre= " + name));

                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
                //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
                catch (Exception ex)
                {
                }
                #endregion

                return CreatedAtAction(nameof(GetById), new { id = xAux.Id },
                    xAux is TWrite oAux ? oAux : Mapper.Map<TWrite>(xAux));
            }

            return ActionResultForModelStateValidation();
        }
        catch (ValidationExtensions.ValidationException ex)
        {
            return HandleValidationException(ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is ValidationExtensions.ValidationException validationEx)
        {
            return HandleValidationException(validationEx);
        }
        catch (Exception ex)
        {
            return HandleGenericException(ex);
        }
    }

    /// <summary>
    /// Maneja las ValidationException del sistema de BaseEntity y las convierte al formato esperado por React.
    /// Mapea errores específicos de campos para mostrar en los formularios del frontend.
    /// </summary>
    /// <param name="ex">ValidationException con campos específicos</param>
    /// <returns>BadRequest con estructura de errores compatible con React</returns>
    protected virtual IActionResult HandleValidationException(ValidationExtensions.ValidationException ex)
    {
        var namingConvention = new CamelCaseNamingStrategy();
        var errors = new Dictionary<string, string[]>();

        if (ex.Fields?.Length > 0)
        {
            // Mapear errores a campos específicos usando camelCase
            foreach (var field in ex.Fields)
            {
                var camelCaseField = namingConvention.GetPropertyName(field, false);

                if (errors.ContainsKey(camelCaseField))
                {
                    var existingErrors = errors[camelCaseField].ToList();
                    existingErrors.Add(ex.Message);
                    errors[camelCaseField] = existingErrors.ToArray();
                }
                else
                {
                    errors[camelCaseField] = new[] { ex.Message };
                }
            }
        }
        else
        {
            // Error general sin campo específico
            errors[""] = new[] { ex.Message };
        }

        Response.StatusCode = 400;
        Response.ContentType = "application/json";
        return new JsonResult(new { Errors = errors });
    }

    /// <summary>
    /// Versión para métodos que retornan ActionResult&lt;T&gt;
    /// </summary>
    protected virtual ActionResult<T> HandleValidationExceptionForActionResult(ValidationExtensions.ValidationException ex)
    {
        var namingConvention = new CamelCaseNamingStrategy();
        var errors = new Dictionary<string, string[]>();

        if (ex.Fields?.Length > 0)
        {
            foreach (var field in ex.Fields)
            {
                var camelCaseField = namingConvention.GetPropertyName(field, false);

                if (errors.ContainsKey(camelCaseField))
                {
                    var existingErrors = errors[camelCaseField].ToList();
                    existingErrors.Add(ex.Message);
                    errors[camelCaseField] = existingErrors.ToArray();
                }
                else
                {
                    errors[camelCaseField] = new[] { ex.Message };
                }
            }
        }
        else
        {
            errors[""] = new[] { ex.Message };
        }

        return BadRequest(new { Errors = errors });
    }

    /// <summary>
    /// Maneja excepciones genéricas y las convierte en errores estandarizados para React.
    /// </summary>
    /// <param name="ex">Excepción genérica</param>
    /// <returns>BadRequest con mensaje de error general</returns>
    protected virtual IActionResult HandleGenericException(Exception ex)
    {
        // TODO: Agregar logging real aquí
        // _logger?.LogError(ex, "Error inesperado en {Controller}", GetType().Name);

        Response.StatusCode = 400;
        Response.ContentType = "application/json";
        return new JsonResult(new
        {
            Errors = new Dictionary<string, string[]>
            {
                { "", new[] { "Ocurrió un error interno al procesar la solicitud." } }
            }
        });
    }

    /// <summary>
    /// Versión para métodos que retornan ActionResult&lt;T&gt;
    /// </summary>
    protected virtual ActionResult<T> HandleGenericExceptionForActionResult(Exception ex)
    {
        // TODO: Agregar logging real aquí
        // _logger?.LogError(ex, "Error inesperado en {Controller}", GetType().Name);

        return BadRequest(new
        {
            Errors = new Dictionary<string, string[]>
            {
                { "", new[] { "Ocurrió un error interno al procesar la solicitud." } }
            }
        });
    }

    /// <summary>
    /// Convierte los errores de ModelState al formato estándar de la aplicación.
    /// Mantiene la funcionalidad original pero asegura compatibilidad con el manejo de ValidationException.
    /// </summary>
    /// <returns>JsonResult con errores de validación en formato camelCase</returns>
    protected IActionResult ActionResultForModelStateValidation()
    {
        var namingConvention = new CamelCaseNamingStrategy();
        var dict = ModelState.ToDictionary(
            kvp => namingConvention.GetPropertyName(kvp.Key, false),
            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
        );
        Response.StatusCode = 400;
        Response.ContentType = "application/json";
        return new JsonResult(new
        {
            Errors = dict
        });
    }

    [HttpDelete("Delete/{id}")]
    public virtual async Task<ActionResult<T>> Delete(long id)
    {
        try
        {
            var x = await Context.FindAsync<T>(id);
            if (x == null)
            {
                return NotFound();
            }

            //Context.Remove(x);

            x.Deleted = true;
            x.DeletedDate = DateTime.Now;
            x.DeletedUser = CurrentAppUser.Value.FullName;

            Context.ChangeTracker.TrackGraph(x, e =>
            {
                e.Entry.State = EntityState.Modified;
            });
            await Context.SaveChangesAsync();

            #region Auditory
            try
            {
                Auditory audit = new Auditory();
                audit.Date = DateTime.Now;
                audit.Entity = GetRealNameEntity(x.GetType().Name);
                audit.UserId = CurrentAppUser.Value.Id;
                audit.User = CurrentAppUser.Value.FullName;
                string name = GetName(x.GetType().Name, x);
                audit.AuditMessage = "Eliminación de " + audit.Entity + ". Id=" + id.ToString() + (string.IsNullOrEmpty(name) ? "" : ". Nombre= " + name);
                Context.Add(audit);
                await Context.SaveChangesAsync();
            }
            //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
            catch (Exception ex)
            {
            }
            #endregion

            return x;
        }
        catch (ValidationExtensions.ValidationException ex)
        {
            return HandleValidationExceptionForActionResult(ex);
        }
        catch (Exception ex)
        {
            return HandleGenericExceptionForActionResult(ex);
        }
    }

    private bool Exists(long id)
    {
        return Context.Set<T>().Any(e => e.Id == id);
    }

    protected long GetUserCountryId()
    {
        return CurrentAppUser.Value.CountryId;
    }

    protected void EnsureUserRole(params UsersRole[] roles)
    {
        var results = roles.Select(rol =>
            rol == UsersRole.Superuser && CurrentAppUser.Value.ApplicationRole.IsSuperuser()
            || rol == UsersRole.NationalSeller && CurrentAppUser.Value.ApplicationRole.IsNationalSeller()
            || rol == UsersRole.COMTURSeller && CurrentAppUser.Value.ApplicationRole.IsCOMTURSeller()
            || rol == UsersRole.Supervisor && CurrentAppUser.Value.ApplicationRole.IsSupervisor()
        );
    }

    public enum UsersRole { NationalSeller, COMTURSeller, Superuser, Supervisor }

    protected void ValidateUserIsFromCountry(long countryId)
    {
        if (countryId != GetUserCountryId())
        {
            throw new UnauthorizedAccessException();
        }
    }

    protected string GetName(string className, T entity)
    {
        string name = "";

        switch (className)
        {
            case nameof(ApplicationUser):
                name = entity.GetType().GetProperty("FullName").GetValue(entity, null).ToString();
                break;
            case nameof(Client):
                name = entity.GetType().GetProperty("BrandName").GetValue(entity, null).ToString() + " - " + entity.GetType().GetProperty("LegalName").GetValue(entity, null).ToString();
                break;
            default:
                PropertyInfo pi = entity.GetType().GetProperty("Name");
                name = pi == null ? "" : pi.GetValue(entity, null).ToString();
                break;
        }

        return name;
    }

    protected string GetRealNameEntity(string className)
    {
        string entity = "";
        switch (className)
        {
            case nameof(PublishingOrder):
                entity = "Órden de Publicación";
                break;
            case nameof(Product):
                entity = "Producto";
                break;
            case nameof(ProductType):
                entity = "Tipo de Producto";
                break;
            case nameof(AdvertisingSpaceLocationType):
                entity = "Ubicación";
                break;
            case nameof(ProductVolumeDiscount):
                entity = "Descuento por volumen";
                break;
            case nameof(ProductLocationDiscount):
                entity = "Descuento por ubicación";
                break;
            case nameof(ProductAdvertisingSpace):
                entity = "Tipo de Espacio";
                break;
            case nameof(ProductEdition):
                entity = "Edición";
                break;
            case nameof(PaymentMethod):
                entity = "Metodo de Pago";
                break;
            case nameof(Currency):
                entity = "Moneda";
                break;
            case nameof(CurrencyParity):
                entity = "Paridad dolar";
                break;
            case nameof(Contract):
                entity = "Contrato";
                break;
            case nameof(Country):
                entity = "Pais";
                break;
            case nameof(ApplicationRole):
                entity = "Rol";
                break;
            case nameof(ApplicationUser):
                entity = "Usuario";
                break;
            case nameof(State):
                entity = "Provincia";
                break;
            case nameof(District):
                entity = "Municipio";
                break;
            case nameof(City):
                entity = "Ciudad";
                break;
            case nameof(TaxType):
                entity = "Identificación tributaria";
                break;
            case nameof(Client):
                entity = "Cliente";
                break;
            case nameof(EuroParity):
                entity = "Euro";
                break;
        }
        return entity;
    }
}