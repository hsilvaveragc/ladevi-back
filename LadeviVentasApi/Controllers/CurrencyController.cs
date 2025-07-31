namespace LadeviVentasApi.Controllers;

using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class CurrencyController : RestController<Currency, CurrencyWritingDto>
{
    public CurrencyController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public override async Task<IActionResult> Put(long id, CurrencyWritingDto x)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        Currency oldCurrency = Context.Currency.AsNoTracking().Single(y => y.Id == id);

        List<CurrencyParity> oldParities = Context.CurrencyParities.AsNoTracking().Where(y => y.CurrencyId == x.Id).ToList();

        Currency currencyToUpdate = Mapper.Map<Currency>(x);

        //Si usa Euro no modifico las paridades en caso que existiesen.
        if (x.UseEuro)
        {
            currencyToUpdate.CurrencyParities = oldCurrency.CurrencyParities;
        }

        var local = Context.Set<Currency>()
                        .Local
                        .FirstOrDefault(entry => entry.Id.Equals(id));

        if (local != null)
        {
            Context.Entry(local).State = EntityState.Detached;
        }

        if (!x.UseEuro)
        {
            foreach (var parity in currencyToUpdate.CurrencyParities.ToList())
            {
                if (parity.Id > 0)
                {
                    Context.Entry(parity).State = parity.ShouldDelete.HasValue && parity.ShouldDelete.Value ? EntityState.Deleted : EntityState.Modified;
                }
                else
                {
                    Context.Add(parity);
                }
            }
        }
        try
        {
            Context.Entry(currencyToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }

        Auditory auditCurrency = new Auditory();
        auditCurrency.Date = DateTime.Now;
        auditCurrency.Entity = "Moneda";
        auditCurrency.UserId = CurrentAppUser.Value.Id;
        auditCurrency.User = CurrentAppUser.Value.FullName;
        auditCurrency.AuditMessage = "Se modificó la moneda " + x.Name + ". " + string.Join("|", Utils.GenerateAuditLogMessages(oldCurrency, currencyToUpdate));
        Context.Add(auditCurrency);
        await Context.SaveChangesAsync();

        //Acá hago la auditoria de las entidades relacionadas
        try
        {
            #region Auditoria Paridades de dolar                

            foreach (var oldP in oldParities)
            {
                CurrencyParity actualParity = x.CurrencyParities.FirstOrDefault(y => y.Id == oldP.Id);
                //Se elimino una paridad
                if (actualParity == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Moneda";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se eliminó una paridad de la moneda " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
                else
                {
                    List<string> mods = Utils.GenerateAuditLogMessages(oldP, actualParity);
                    if (mods.Count > 0)
                    {
                        Auditory audit = new Auditory();
                        audit.Date = DateTime.Now;
                        audit.Entity = "Moneda";
                        audit.UserId = CurrentAppUser.Value.Id;
                        audit.User = CurrentAppUser.Value.FullName;
                        audit.AuditMessage = "Modificación de paridad de la moneda " + x.Name + ". " + string.Join('|', mods);
                        Context.Add(audit);
                        await Context.SaveChangesAsync();
                    }
                }
            }

            foreach (var actualP in x.CurrencyParities)
            {
                CurrencyParity oldParity = oldParities.FirstOrDefault(y => y.Id == actualP.Id);
                // Se agrego una entidad
                if (oldParity == null)
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Moneda";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;
                    audit.AuditMessage = "Se Agregó una paridad a la moneda " + x.Name;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
            }
            #endregion
        }
        catch (Exception ex)
        {

        }

        return NoContent();
        // return result;
    }

    public override Task<ActionResult<Currency>> Delete(long id)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        return base.Delete(id);
    }


    protected override IQueryable<Currency> GetQueryableWithIncludes()
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);

        bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();

        var allCurrencies = base.GetQueryableWithIncludes()
                .Include(c => c.CurrencyParities);

        return allCurrencies;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var result = GetSearchQueryable()
            .Join(
                Context.Country,
                cu => cu.CountryId,
                co => co.Id,
                (cu, co) => new { Currency = cu, Country = co })
            .Select(x => new
            {
                x.Currency.Id,
                x.Currency.Name,
                x.Currency.CountryId,
                Country = x.Country.Name,
                x.Currency.DeletedDate,
                CanDelete = !Context.Contracts.Any(c => c.CurrencyId == x.Currency.Id),
                    CurrencyParities = !x.Currency.UseEuro ? x.Currency.CurrencyParities.Where(pcp => !pcp.Deleted.HasValue || pcp.Deleted.Value)
                    .OrderBy(pcp => pcp.Start).ToList() : null,
                x.Currency.UseEuro
            })
            .Where(x => !x.DeletedDate.HasValue)
            .ToDataSourceResult(request);

        result.Data = result.Data.Cast<dynamic>().Select(x => new CurrencyItemDto
        {
            Id = x.Id,
            Name = x.Name,
            CountryId = x.CountryId,
            Country = x.Country,
            DeletedDate = x.DeletedDate,
            CanDelete = x.CanDelete,
            CurrencyParities = x.CurrencyParities ?? new List<CurrencyParity>(),
            UseEuro = x.UseEuro
        }).ToList();

        return result;
    }
}
