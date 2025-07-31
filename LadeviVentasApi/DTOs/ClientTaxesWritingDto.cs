namespace LadeviVentasApi.DTOs;

using AutoMapper;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;

[AutoMap(typeof(ClientTaxes), ReverseMap = true)]
public class ClientTaxesWritingDto : BaseEntity
{
    public string Value;
    public long TaxTypeId;
}