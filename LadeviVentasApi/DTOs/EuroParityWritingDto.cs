namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(EuroParity), ReverseMap = true)]
public class EuroParityWritingDto : BaseEntity
{
    public DateTime Start;
    public DateTime End;
    public double EuroToDollarExchangeRate;
}