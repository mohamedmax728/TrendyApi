using Domain.Common;
using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Roles")]
    public class Role : AuditEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleCodeEnum RoleCode { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
