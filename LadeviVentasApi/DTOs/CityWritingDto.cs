namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(City), ReverseMap = true)]
public class CityWritingDto : BaseEntity
{
    public string Name { get; set; }
    public long DistrictId { get; set; }
    public string CodigoTelefonico { get; set; }
}