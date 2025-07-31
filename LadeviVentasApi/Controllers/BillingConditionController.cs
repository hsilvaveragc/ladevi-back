namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class BillingConditionController : RestController<BillingCondition>
{
    public BillingConditionController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}