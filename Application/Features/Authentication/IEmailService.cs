using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Authentication
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
