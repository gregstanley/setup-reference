using System.Threading.Tasks;
using SetupReference.Database.Example;
using Xunit;

namespace SetupReference.IntegrationTests.XUnit
{
    // Derived from information here: https://www.jvandertil.nl/posts/2020-04-02_sqlserverintegrationtesting/
    public class DatabaseFixtureEntityFramework : IAsyncLifetime
    {
        public ExampleContext ExampleContext { get; }

        public DatabaseFixtureEntityFramework()
        {
            var connectionString = DatabaseConnectionHelpers.GenerateConnectionString();

            ExampleContext = DatabaseConnectionHelpers.GetContext(connectionString, 1);
        }

        public async Task InitializeAsync()
        {
            await ExampleContext.Database.EnsureDeletedAsync();
            await ExampleContext.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await ExampleContext.Database.EnsureDeletedAsync();
            await ExampleContext.DisposeAsync();
        }
    }
}
