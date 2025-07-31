using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductAdvertisingSpaceLocationDiscount : BaseEntity
    {
        [Required] public double Discount { get; set; }
        [Required] public long ProductAdvertisingSpaceId { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(AdvertisingSpaceLocationType))] public long AdvertisingSpaceLocationTypeId { get; set; }
    }
}