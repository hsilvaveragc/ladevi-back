namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(TaxType), ReverseMap = true)]
public class TaxTypeWritingDto : BaseEntity
{
    public string Name;
    public string[] Options;
    public long CountryId, Order;
    public bool IsIdentificationField;
}

