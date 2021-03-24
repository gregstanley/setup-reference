using System;
using System.Threading.Tasks;
using Shouldly;
using SetupReference.API.Handlers;
using SetupReference.Database.Example;
using SetupReference.Database.ExampleContext.Entities;

using static SetupReference.IntegrationTests.Fixie.SliceFixture;

namespace SetupReference.IntegrationTests.Fixie
{
    public class CreateNoteHandlerTests
    {
        public async Task CreateNote()
        {
            var request = new CreateNoteRequest(1, "The new title", "The new message", DateTime.Now);

            var result = await SendAsync(request);

            result.ShouldNotBeNull();
            result.Title.ShouldBe("The new title");
            result.Message.ShouldBe("The new message");
        }


        public async Task ClassSetUp() => await RecreateAndMigrateDatabaseAsync();

        public async Task ResetCheckpoint() => await SliceFixture.ResetCheckpoint();

        public async Task SetUp()
        {
            await InsertAsync<ExampleContext>(new NoteEntity
            {
                TenantId = 1,
                Title = "The title",
                Message = "The message"
            });
        }

        // public async Task TearDown() => throw new NotImplementedException();

        public async Task ClassTearDown() => await DeleteDatabaseAsync();
    }
}
