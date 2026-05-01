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

            if (claim != null && !string.IsNullOrEmpty(claim.Value) && int.TryParse(claim.Value, out var companyId))
            {
                CompanyId = companyId;
            }
            else
            {
                // Default to CompanyId 1 when no valid claim is found
                // This handles cases like user registration where there's no auth context yet
                CompanyId = 1;
            }
        }
    }
}
