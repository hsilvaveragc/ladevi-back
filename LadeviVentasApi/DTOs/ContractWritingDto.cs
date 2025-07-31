namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(Contract), ReverseMap = true)]
public class ContractWritingDto : BaseEntity
{
    public long ProductId, ClientId, SellerId, BillingConditionId, PaymentMethodId, BillingCountryId, Number;
    public long? CurrencyId;
    public bool UseEuro;
    public DateTime Start, End, ContractDate;
    public bool? PaidOut;
    public double? CurrencyParity;
    public double IVA;
    public string Name, InvoiceNumber, Observations;
    public int? CheckQuantity, DaysToFirstPayment, DaysBetweenChecks;
    public IEnumerable<SoldSpaceWritingDto> SoldSpaces;
}