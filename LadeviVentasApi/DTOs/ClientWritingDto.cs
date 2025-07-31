namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(Client), ReverseMap = true)]
public class ClientWritingDto : BaseDto
{
    public string BrandName, LegalName, Address,
        PostalCode, TelephoneCountryCode, TelephoneAreaCode,
        MainEmail, TelephoneNumber, BillingPointOfSale;
    public string? Contact, AlternativeEmail;
    public bool ElectronicBillByMail, ElectronicBillByPaper, IsEnabled, IsAgency, IsComtur;
    public long CountryId, StateId, ApplicationUserDebtCollectorId, ApplicationUserSellerId;
    public long Id, TaxTypeId, TaxCategoryId;
    public string IdentificationValue;
    public double TaxPercentage;
    public long? DistrictId, CityId, XubioId;
    public bool? IsBigCompany;
}