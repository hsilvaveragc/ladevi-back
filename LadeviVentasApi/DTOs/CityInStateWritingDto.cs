namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(City), ReverseMap = true)]
public class CityInStateWritingDto : BaseEntity
{
    public string Name { get; set; }
}
