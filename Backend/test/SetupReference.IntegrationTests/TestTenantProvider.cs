using SetupReference.Database;

namespace SetupReference.IntegrationTests
{
    public class TestTenantProvider : ITenantProvider<int>
    {
        public TestTenantProvider(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; }
    }
}
