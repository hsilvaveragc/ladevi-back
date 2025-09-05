namespace LadeviVentasApi.DTOs;

using LadeviVentasApi.Helpers.Attributes;
using LadeviVentasApi.Models;

public class ApplicationUserWritingDto : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    [RequiredId]
    public long ApplicationRoleId { get; set; }
    public string FullName { get; set; }
    public string Initials { get; set; }
    public double CommisionCoeficient { get; set; }
    [RequiredId] public long CountryId { get; set; }
}