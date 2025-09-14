namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(InventoryProductAdvertisingSpace), ReverseMap = true)]
public class InventoryProductAdvertisingSpaceDto : BaseDto
{
    public long ProductEditionId;
    public long ProductAdvertisingSpaceId;
    public int? Quantity;
}