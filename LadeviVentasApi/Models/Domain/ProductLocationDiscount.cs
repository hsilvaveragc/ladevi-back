using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductLocationDiscount : BaseEntity
    {
        [Required] public double Discount { get; set; }
        [Required] public long ProductId { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(AdvertisingSpaceLocationType))] public long AdvertisingSpaceLocationTypeId { get; set; }
    }
}