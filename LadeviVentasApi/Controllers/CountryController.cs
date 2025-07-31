namespace LadeviVentasApi.Controllers;

using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CountryController : RestController<Country>
{
    public CountryController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        return GetSearchQueryable()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.CodigoTelefonico,
                    p.XubioCode,
                })
                .ToDataSourceResult(request);
    }

    [HttpGet("Options")]
    public async Task<IActionResult> Options()
    {
        var countries = await Context.Country
                                    .AsNoTracking()
                                    .Where(p => !p.Deleted.HasValue || !p.Deleted.Value)
                                    .Select(p => new
                                    {
                                        p.Id,
                                        p.Name,
                                    })
                                    .OrderBy(c => c.Name)
                                    .ToListAsync();

        return Ok(countries);
    }
}