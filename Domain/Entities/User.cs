using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Users")]
    public class User : AuditEntity
    {
        public int Id { get; set; }
        public string FullNameEn { get; set; }
        public string FullNameAr { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
