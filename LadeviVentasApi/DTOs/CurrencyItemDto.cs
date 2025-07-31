namespace LadeviVentasApi.DTOs;

using LadeviVentasApi.Models.Domain;

public class CurrencyItemDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long CountryId { get; set; }
    public string Country { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<CurrencyParity> CurrencyParities { get; set; }
    public bool UseEuro { get; set; }
}