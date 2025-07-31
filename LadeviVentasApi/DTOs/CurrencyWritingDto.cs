namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(Currency), ReverseMap = true)]
public class CurrencyWritingDto : BaseEntity
{
    public long CountryId;
    public string Name;
    public bool UseEuro;
    public IEnumerable<CurrencyParity> CurrencyParities;
}