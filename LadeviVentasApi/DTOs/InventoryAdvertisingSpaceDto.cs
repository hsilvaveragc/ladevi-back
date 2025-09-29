namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(InventoryAdvertisingSpace), ReverseMap = true)]
public class InventoryAdvertisingSpaceDto : BaseDto
{
    public long ProductEditionId;
    public long ProductAdvertisingSpaceId;
    public int? Quantity;
}