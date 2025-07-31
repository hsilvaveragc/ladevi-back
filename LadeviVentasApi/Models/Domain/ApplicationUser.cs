using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LadeviVentasApi.Models.Domain
{
    public class ApplicationUser : BaseEntity, IValidatableObject
    {
        [Required] public string FullName { get; set; }
        [Required] public string Initials { get; set; }
        [Required] public double CommisionCoeficient { get; set; }
        public ApplicationRole? ApplicationRole { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ApplicationRole))] public long ApplicationRoleId { get; set; }
        public Country? Country { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Country))] public long CountryId { get; set; }
        public IdentityUser? CredentialsUser { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(IdentityUser))] public string CredentialsUserId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            if (!new EmailAddressAttribute().IsValid(CredentialsUser?.Email))
            {
                validationResults.Add(new ValidationResult("Email invalido"));
            }

            return validationResults;
        }
    }
}
