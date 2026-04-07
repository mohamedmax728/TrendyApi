using Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Persistence.Identity
{
    public class TenantProvider : ITenantProvider
    {
        public int CompanyId { get; }

        public TenantProvider(IHttpContextAccessor accessor)
        {
            var claim = accessor.HttpContext?
                .User
                .FindFirst("companyId");

            CompanyId = int.Parse(claim!.Value);
        }
    }
}
