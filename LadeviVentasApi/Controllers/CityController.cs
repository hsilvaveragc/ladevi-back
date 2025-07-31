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
public class CityController : RestController<City, CityWritingDto>
{
    public CityController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override IQueryable<City> GetQueryableWithIncludes()
    {
        return base.GetQueryableWithIncludes()
            .Include(x => x.District.State.Country);
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        return GetSearchQueryable()
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.DistrictId,
                DistrictName = x.District.Name,
                StateName = x.District.State.Name,
                CountryName = x.District.State.Country.Name,
                x.CodigoTelefonico
            })
            .ToDataSourceResult(request);
    }
}