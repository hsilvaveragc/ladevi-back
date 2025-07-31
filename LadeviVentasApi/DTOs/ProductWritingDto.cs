namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(Product), ReverseMap = true)]
public class ProductWritingDto : BaseEntity
{
    public long ProductTypeId, CountryId;
    public string Name;
    public string? XubioProductCode, ComturXubioProductCode;
    public double DiscountForCheck, DiscountForLoyalty, DiscountForAgency,
        DiscountForSameCountry, DiscountForOtherCountry, DiscountSpecialBySeller,
        DiscountByManager, MaxAplicableDiscount, AliquotForSalesCommission, IVA;
    public IEnumerable<ProductVolumeDiscount> ProductVolumeDiscounts;
    public IEnumerable<ProductLocationDiscount> ProductLocationDiscounts;
}