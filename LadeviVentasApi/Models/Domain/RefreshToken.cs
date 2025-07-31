namespace LadeviVentasApi.Models.Domain
{
    public class RefreshToken : BaseEntity
    {
        public string Username { get; set; }
        public string Refreshtoken { get; set; }
        public bool Revoked { get; set; }
    }
}