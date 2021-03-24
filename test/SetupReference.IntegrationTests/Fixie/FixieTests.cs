using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Shouldly;
using SetupReference.Database.Example;
using SetupReference.Database.ExampleContext.Entities;

namespace SetupReference.IntegrationTests.Fixie
{
    public class FixieTests
    {
        private string connectionString;

        private ExampleContext exampleContext;

        // Respawn checkpoint configuration. See: https://github.com/jbogard/Respawn
        private static Checkpoint checkpoint = new Checkpoint
        {
            TablesToIgnore = new[]
            {
                "sysdiagrams",
                "tblUser",
                "tblObjectType",
            }
        };

        public async Task Empty_Database_Returns_Null_Assessment_Instance()
        {
            await checkpoint.Reset(connectionString);

            var x = await exampleContext.Notes.FirstOrDefaultAsync();

            x.ShouldBeNull();
        }

        // TODO: Tests with tenancy
        public async Task Note_Saved_And_Retrieved_Correctly()
        {
            // Arrange
            await checkpoint.Reset(connectionString);

            var note = new NoteEntity
            {
                TenantId = 1,
                Title = "Test note",
                Message = "The message"
            };

            exampleContext.Notes.Add(note);

            await exampleContext.SaveChangesAsync();

            // Act
            var x = await exampleContext.Notes.FirstOrDefaultAsync();

            // Assert
            x.ShouldNotBeNull();
            x.Id.ShouldBePositive();
            x.TenantId.ShouldBe(1);
            x.Title.ShouldBe("Test note");
            x.Message.ShouldBe("The message");
        }

        public async Task SetUp()
        {
            connectionString = DatabaseConnectionHelpers.GenerateConnectionString();

            exampleContext = DatabaseConnectionHelpers.GetContext(connectionString, 1);

            await exampleContext.Database.EnsureDeletedAsync();
            await exampleContext.Database.EnsureCreatedAsync();
        }

        public async Task TearDown() => await exampleContext.Database.EnsureDeletedAsync();
    }
}
