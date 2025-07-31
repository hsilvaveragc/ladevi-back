using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class ApplicationRole : BaseEntity
    {
        public const string SuperuserRole = "Administrador";
        public const string NationalSellerRole = "Vendedor Nacional";
        public const string COMTURSellerRole = "Vendedor COMTUR";
        public const string SupervisorRole = "Supervisor";

        [Required] public string Name { get; set; }
        [Required] public bool ShouldHaveCommission { get; set; }

        public bool IsSeller()
        {
            return Name.Equals(NationalSellerRole) || Name.Equals(COMTURSellerRole);
        }

        public bool IsSuperuser() => Name.Equals(SuperuserRole);
        public bool IsNationalSeller() => Name.Equals(NationalSellerRole);
        public bool IsCOMTURSeller() => Name.Equals(COMTURSellerRole);
        public bool IsSupervisor() => Name.Equals(SupervisorRole);
    }
}
