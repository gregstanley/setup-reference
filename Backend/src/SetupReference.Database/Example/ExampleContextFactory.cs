using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SetupReference.Database.Example
{
    // We may need this if attempting to run migrations with a 'tenant' based contenxt:
    // https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
    //
    // The EF migrations will use this to create the context instance and avoid the need to provide
    // the runtime only dependencies e.g. an ITenantProvider instance.
    // 
    // Unfortunately it does mean that the connection string here may need adjusting
    // for each local environmant as it does not use the standard project one.
    public class ExampleContextFactory : IDesignTimeDbContextFactory<ExampleContext>
    {
        public ExampleContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExampleContext>();
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=SetupReference;Integrated Security=True");//,x => x.UseNetTopologySuite());

            return new ExampleContext(optionsBuilder.Options);
        }
    }
}
