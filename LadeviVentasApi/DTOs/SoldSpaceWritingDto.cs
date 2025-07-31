namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(SoldSpace), ReverseMap = true)]
public class SoldSpaceWritingDto : BaseEntity
{
    public long AdvertisingSpaceLocationTypeId, ProductAdvertisingSpaceId;
    public short? TypeSpecialDiscount, TypeGerentialDiscount;
    public double Quantity, SpacePrice;
    public double? SpecialDiscount, GerentialDiscount, LocationDiscount;
    public string DescriptionSpecialDiscount, DescriptionGerentialDiscount;
    public double Total { get; set; }
    public double UnitPriceWithDiscounts { get; set; }
    public bool ApplyDiscountForCheck, ApplyDiscountForLoyalty, ApplyDiscountForSameCountry, ApplyDiscountForOtherCountry;
    public bool AppyDiscountForAgency, ApplyDiscountForVolume;
    public double? DiscountForCheck, DiscountForLoyalty, DiscountForAgency, DiscountForSameCountry, DiscountForOtherCountry, DiscountForVolume;
}
