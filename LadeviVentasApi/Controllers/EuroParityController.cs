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
public class EuroParityController : RestController<EuroParity, EuroParityWritingDto>
{
    public EuroParityController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public override async Task<IActionResult> Post(EuroParityWritingDto x)
    {
        using (var tx = await Context.Database.BeginTransactionAsync())
        {
            var euroParitiesLatestStartDate = await Context.EuroParities
                                                   .OrderByDescending(date => date)
                                                   .FirstOrDefaultAsync();

            if (euroParitiesLatestStartDate != null)
            {
                if (euroParitiesLatestStartDate.Start >= x.Start)
                {
                    x.End = euroParitiesLatestStartDate.Start;
                }
                else
                {
                    euroParitiesLatestStartDate.End = x.Start;
                }

                Context.Update(euroParitiesLatestStartDate);
            }

            var result = await base.Post(x);
            tx.Commit();

            return result;
        }

    }

    public override async Task<IActionResult> Put(long id, EuroParityWritingDto x)
    {
        throw new NotImplementedException();
    }

    public override Task<ActionResult<EuroParity>> Delete(long id)
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        return base.Delete(id);
    }


    protected override IQueryable<EuroParity> GetQueryableWithIncludes()
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);

        var allCurrencies = base.GetQueryableWithIncludes()
                                .Where(x => !x.DeletedDate.HasValue);

        return allCurrencies;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var anyUseEuro = Context.Contracts
                                .Include(c => c.Currency)
                                .Any(c => c.Currency.UseEuro);

        var euroParityNotDeleteId = GetSearchQueryable()
                                .Where(x => x.End >= DateTime.UtcNow && x.Start <= DateTime.UtcNow).OrderByDescending(x => x.Start)
                                .FirstOrDefault()?.Id ?? 0;


        var result = GetSearchQueryable()
            .Select(x => new
            {
                x.Id,
                x.EuroToDollarExchangeRate,
                x.Start,
                x.End,
                CanDelete = euroParityNotDeleteId == 0 || x.Id != euroParityNotDeleteId || (x.Id == euroParityNotDeleteId && !anyUseEuro),
            })
            .OrderByDescending(x => x.Start)
            .ToDataSourceResult(request);

        return result;
    }
}
