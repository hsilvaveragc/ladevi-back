using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class SoldSpace : BaseEntity
    {
        //Ubicacion
        public AdvertisingSpaceLocationType? AdvertisingSpaceLocationType { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(AdvertisingSpaceLocationType))] public long AdvertisingSpaceLocationTypeId { get; set; }
        //Tipo de Espacio
        public ProductAdvertisingSpace? ProductAdvertisingSpace { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ProductAdvertisingSpace))] public long ProductAdvertisingSpaceId { get; set; }
        [Required] public long ContractId { get; set; }
        public short? TypeSpecialDiscount { get; set; }
        public string DescriptionSpecialDiscount { get; set; }
        public double SpecialDiscount { get; set; }
        public short TypeGerentialDiscount { get; set; }
        public string DescriptionGerentialDiscount { get; set; }
        public double GerentialDiscount { get; set; }
        public double LocationDiscount { get; set; }
        public double SpacePrice { get; set; }
        public double Quantity { get; set; }
        public double Balance { get; set; }
        public double Total { get; set; }
        public double SubTotal { get; set; }
        public double TotalTaxes { get; set; }
        public double TotalDiscounts { get; set; }
        public double UnitPriceWithDiscounts { get; set; }
        public bool ApplyDiscountForCheck { get; set; }
        public bool ApplyDiscountForLoyalty { get; set; }
        public bool ApplyDiscountForSameCountry { get; set; }
        public bool ApplyDiscountForOtherCountry { get; set; }
        public bool AppyDiscountForAgency { get; set; }
        public bool ApplyDiscountForVolume { get; set; }
        public double? DiscountForCheck { get; set; }
        public double? DiscountForLoyalty { get; set; }
        public double? DiscountForAgency { get; set; }
        public double? DiscountForSameCountry { get; set; }
        public double? DiscountForOtherCountry { get; set; }
        public double? DiscountForVolume { get; set; }
        public Contract? Contract { get; set; }
        public string? XubioDocumentNumber { get; set; }
    }
}
