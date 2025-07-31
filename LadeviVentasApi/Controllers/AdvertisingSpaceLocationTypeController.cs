namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AdvertisingSpaceLocationTypeController : RestController<AdvertisingSpaceLocationType>
{
    public AdvertisingSpaceLocationTypeController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override IQueryable<AdvertisingSpaceLocationType> GetQueryableWithIncludes()
    {
        var spaceLocationTypes = base.GetQueryableWithIncludes();
        return spaceLocationTypes;
    }
}
