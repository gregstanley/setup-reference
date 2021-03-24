using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SetupReference.Database;
using SetupReference.Database.Exceptions;

namespace SetupReference.API
{
    public class WebTenantProvider : ITenantProvider<int>
    {
        public WebTenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext is null)
            {
                // We are not in an HTTP context so ignore requirement for tenant id
                TenantId = 0;
                return;
            }

            var tenantIdAsString = httpContextAccessor.HttpContext.GetRouteValue("tenantId")?.ToString() ?? string.Empty;

            // Assuming this is resolved for a Controller then this *should* be safe but if not then worth checking
            if (!int.TryParse(tenantIdAsString, out var tenantIdAsInt))
                throw new InvalidTenantIdException($"The value '{tenantIdAsInt}' is not a valid tenant id.");

            TenantId = tenantIdAsInt;
        }

        public int TenantId { get; }
    }
}