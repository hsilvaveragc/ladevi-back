namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TaxTypeController : RestController<TaxType, TaxTypeWritingDto>
{

    public TaxTypeController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override IQueryable<TaxType> GetQueryableWithIncludes()
    {
        bool isSupervisor = CurrentAppUser.Value.ApplicationRole.IsSupervisor();

        var allTaxType = base.GetQueryableWithIncludes()
            .Include(x => x.Country);

        return allTaxType;
    }
}