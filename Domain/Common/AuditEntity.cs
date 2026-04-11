namespace Domain.Common
{
    public class AuditEntity
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public int CompanyId { get; set; }
    }
}
