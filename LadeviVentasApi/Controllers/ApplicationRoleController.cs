namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ApplicationRoleController : RestController<ApplicationRole>
{
    public ApplicationRoleController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet("Options")]
    public async Task<IActionResult> Options()
    {
        var allRoles = await Context.ApplicationRole.AsNoTracking()
                                                    .Where(x => !x.Deleted.HasValue || !x.Deleted.Value)
                                                    .Select(r => new
                                                    {
                                                        r.Id,
                                                        r.Name
                                                    })
                                                    .ToListAsync();
        return Ok(allRoles);
    }
}