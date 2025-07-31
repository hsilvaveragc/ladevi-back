namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(ProductAdvertisingSpace), ReverseMap = true)]
public class ProductAdvertisingSpaceWritingDto : BaseEntity
{
    public string Name;
    public long ProductId;
    public double DollarPrice, Height, Width, DiscountForCheck, DiscountForLoyalty, DiscountForAgency,
        DiscountForSameCountry, DiscountForOtherCountry;
    public bool Show;
    public IEnumerable<ProductAdvertisingSpaceVolumeDiscount> ProductAdvertisingSpaceVolumeDiscounts;
    public IEnumerable<ProductAdvertisingSpaceLocationDiscount> ProductAdvertisingSpaceLocationDiscounts;
}