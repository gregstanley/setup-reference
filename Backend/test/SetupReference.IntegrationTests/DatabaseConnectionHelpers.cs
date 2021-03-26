using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SetupReference.Database.Example;

namespace SetupReference.IntegrationTests
{
    public static class DatabaseConnectionHelpers
    {
        public const string BaseConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=True";
        public const string DatabaseNamePrefix = "ExampleIntegration";

        public static string GetConnectionString(string baseConnectionString, string databaseName)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = databaseName
            };

            return builder.ToString();
        }

        public static string GenerateDatabaseName(string databaseNamePrefix) => $"{databaseNamePrefix}-{Guid.NewGuid()}";

        public static string GenerateConnectionString(string baseConnectionString, string databaseNamePrefix) =>
            GetConnectionString(baseConnectionString, GenerateDatabaseName(databaseNamePrefix));

        public static string GenerateConnectionString(string databaseNamePrefix) => GenerateConnectionString(BaseConnectionString, databaseNamePrefix);

        public static string GenerateConnectionString() => GenerateConnectionString(BaseConnectionString, DatabaseNamePrefix);

        public static ExampleContext GetContext(string connectionString, int tenantId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExampleContext>();

            optionsBuilder.UseSqlServer(connectionString); //, x => x.UseNetTopologySuite());

            return new ExampleContext(optionsBuilder.Options, new TestTenantProvider(tenantId));
        }
    }
}
