namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodController : RestController<PaymentMethod>
{
    public PaymentMethodController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}