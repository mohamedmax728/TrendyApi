using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;


namespace Domain.Entities
{
    public class PasswordResetToken : AuditEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OTP { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; }
    }
}
