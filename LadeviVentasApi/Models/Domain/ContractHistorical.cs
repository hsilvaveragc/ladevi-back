using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ContractHistorical : BaseEntity
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string Changes { get; set; }
        [Required]
        public long ContractId { get; set; }
    }
}
