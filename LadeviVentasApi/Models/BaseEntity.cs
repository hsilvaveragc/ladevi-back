using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LadeviVentasApi.Data;
using LadeviVentasApi.Models.Domain;
using Newtonsoft.Json;

namespace LadeviVentasApi.Models
{
    public abstract class BaseEntity : IValidatableObject
    {
        public long Id { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedUser { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShouldDelete { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var applicationUser = httpContextAccessor.HttpContext.Items["current-user"] as Lazy<ApplicationUser>;
            var ctx = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            return PerformValidate(validationContext, ctx, applicationUser);
        }

        protected virtual IList<ValidationResult> PerformValidate(ValidationContext validationContext,
            ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return new List<ValidationResult>();
        }
    }
    public sealed class FkCheck : ValidationAttribute
    {
        public Type TypeToCheck { get; set; }

        protected override ValidationResult IsValid(object entityId, ValidationContext validationContext)
        {
            var ctx = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var result = entityId == null || ctx.Find(TypeToCheck, long.TryParse(entityId.ToString(), out var id) ? id : entityId) != null
                ? ValidationResult.Success
                : new ValidationResult($"No existe el valor elegido!");
            return result;
        }
    }
    public class MustHaveOneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return value is IEnumerable collection && collection.GetEnumerator().MoveNext()
                ? ValidationResult.Success
                : new ValidationResult($"Al menos debe tener un elemento!");
        }
    }
}