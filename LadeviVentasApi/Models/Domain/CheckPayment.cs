using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class CheckPayment : BaseEntity
    {
        [Required] public int Order { get; set; }
        [Required] public DateTime Date { get; set; }
        [Required] public double Total { get; set; }

        public Contract? Contract { get; set; }
        [Required] public long ContractId { get; set; }
    }
}
