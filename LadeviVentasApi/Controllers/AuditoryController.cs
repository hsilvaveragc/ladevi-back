namespace LadeviVentasApi.Controllers;

using LadeviVentasApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class AuditoryController : ControllerBase
{
    protected ApplicationDbContext Context { get; set; }

    public AuditoryController(ApplicationDbContext context)
    {
        Context = context;
    }

    [HttpGet("GetAuditory")]
    public async Task<ActionResult> GetAuditory()
    {
        var audits = Context.Auditory.OrderByDescending(a => a.Date).Take(100).ToList();
        return Ok(audits);
    }
}