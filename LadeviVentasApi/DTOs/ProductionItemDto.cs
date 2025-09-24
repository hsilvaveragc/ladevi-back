using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.DTOs;

namespace LadeviVentasApi.Models.Domain
{
    public class ProductionItemDto : BaseDto
    {
        public long ProductEditionId { get; set; }
        public int PageNumber { get; set; }
        public int Slot { get; set; }
        public long InventoryProductAdvertisingSpaceId { get; set; }
        public string ProductAdvertisingSpaceName { get; set; }
        public bool IsEditorial { get; set; }
        public bool IsCA { get; set; }
        public string Observations { get; set; }

        public long? PublishingOrderId { get; set; }
        public long? ContractId { get; set; }
        public string ContractName { get; set; }
        public string ClientName { get; set; }
        public string SellerName { get; set; }
    }
}