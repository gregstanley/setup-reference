namespace SetupReference.Database
{
    // https://gunnarpeipman.com/ef-core-global-query-filters/
    public interface ITenantProvider<T>
    {
        T TenantId { get; }
    }
}
