namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(State), ReverseMap = true)]
public class StateWritingDto : BaseEntity
{
    public string Name { get; set; }
    public long CountryId { get; set; }
}