namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(District), ReverseMap = true)]
public class DistrictWritingDto : BaseEntity
{
    public string Name { get; set; }
    public long StateId { get; set; }
}