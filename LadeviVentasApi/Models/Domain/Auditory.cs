namespace LadeviVentasApi.Models.Domain
{
    public class Auditory
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Entity { get; set; }
        public string AuditMessage { get; set; }
    }
}
