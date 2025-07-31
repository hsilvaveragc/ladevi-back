namespace LadeviVentasApi.Controllers;

using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TaxCategoryController : RestController<TaxCategory, TaxCategoryWritingDto>
{

    public TaxCategoryController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}