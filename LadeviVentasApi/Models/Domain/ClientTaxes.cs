using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ClientTaxes : BaseEntity
    {
        public string Value { get; set; }
        [Required] public long ClientId { get; set; }
        public TaxType TaxType { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(TaxType))] public long TaxTypeId { get; set; }
    }
}
