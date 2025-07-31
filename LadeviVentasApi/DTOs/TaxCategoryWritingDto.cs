namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(TaxCategory), ReverseMap = true)]
public class TaxCategoryWritingDto : BaseEntity
{
    public string Name;
    public string Code;
}

