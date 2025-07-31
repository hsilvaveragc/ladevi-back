namespace LadeviVentasApi.Controllers;

using System.Linq.Dynamic.Core;
using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class StateController : RestController<State, StateWritingDto>
{

    public StateController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet("grouped-by-country")]
    public async Task<IActionResult> GetGroupedByCountry()
    {
        var states = await Context.State
                                .AsNoTracking()
                                .Where(s => !s.Deleted.HasValue || !s.Deleted.Value)
                                .GroupBy(s => s.CountryId)
                                .Select(sg => new
                                {
                                    CountryId = sg.Key,
                                    States = sg
                                        .Select(s => new
                                        {
                                            s.Id,
                                            s.Name,
                                            s.XubioCode,
                                        })
                                        .OrderBy(s => s.Name)
                                        .ToList()
                                })
                                .ToListAsync();

        return Ok(states);
    }

    protected override IQueryable<State> GetQueryableWithIncludes()
    {
        return base.GetQueryableWithIncludes()
            .Include(x => x.Country);
    }
}