using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace SetupReference.IntegrationTests.XUnit
{
    // From https://www.jvandertil.nl/posts/2020-04-02_sqlserverintegrationtesting/
    // "Notice: Due to the time it takes to initialize and destroy databases you are generally better off using a collection fixture instead of an IClassFixture<T>.
    // These will share the database fixture across all tests in the collection, but this does limit the amount of parallelization xUnit can do.
    // Be sure to read the documentation!"
    public class ConnectionTestsEntityFramework : IClassFixture<DatabaseFixtureEntityFramework>
    {
        private readonly DatabaseFixtureEntityFramework fixture;

        public ConnectionTestsEntityFramework(DatabaseFixtureEntityFramework fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Can_Connect()
        {
            var x = await fixture.ExampleContext.Notes.FirstOrDefaultAsync();

            x.ShouldBeNull();
        }
    }
}