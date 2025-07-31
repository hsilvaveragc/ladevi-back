namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(PublishingOrder), ReverseMap = true)]
public class PublishingOrderWritingDto : BaseEntity
{
    public bool Latent;
    public long ClientId;
    public long AdvertisingSpaceLocationTypeId, ProductAdvertisingSpaceId, ProductEditionId;
    public long? SellerId;
    public long? ContractId, SoldSpaceId;
    public string PageNumber;
    public string InvoiceNumber;
    public bool? PaidOut;
    public double Quantity;
    public string Observations;
    public DateTime? LastUpdate;
    public DateTime? CreationDate;
    public bool CanDelete;
}