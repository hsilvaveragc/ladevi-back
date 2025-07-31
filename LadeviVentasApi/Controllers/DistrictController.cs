namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : RestController<District, DistrictWritingDto>
{
    public DistrictController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet("grouped-by-state")]
    public async Task<IActionResult> GetGroupedByState()
    {
        var districts = await Context.District
                                .AsNoTracking()
                                .Where(d => !d.Deleted.HasValue || !d.Deleted.Value)
                                .GroupBy(d => d.StateId)
                                .Select(dg => new
                                {
                                    StateId = dg.Key,
                                    Districts = dg
                                        .Select(d => new
                                        {
                                            d.Id,
                                            d.Name
                                        })
                                        .OrderBy(d => d.Name)
                                        .ToList()
                                })
                                .ToListAsync();

        return Ok(districts);
    }

    protected override IQueryable<District> GetQueryableWithIncludes()
    {
        return base.GetQueryableWithIncludes()
            .Include(x => x.State.Country);
    }
}