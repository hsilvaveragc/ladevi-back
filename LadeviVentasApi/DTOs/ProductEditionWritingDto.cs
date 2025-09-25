namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(ProductEdition), ReverseMap = true)]
public class ProductEditionWritingDto : BaseEntity
{
    public string Code, Name;
    public long ProductId;
    public bool Closed;
    public DateTime End;
    public int? PageCount;
    public List<InventoryAdvertisingSpaceDto> InventoryAdvertisingSpaces;
}