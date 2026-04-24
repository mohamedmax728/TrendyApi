using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Models
{
    public class EmailSettings
    {
        public string From { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
