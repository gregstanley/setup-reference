using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SetupReference.API.ResponseModels;
using SetupReference.Database.Example;
using SetupReference.Database.ExampleContext.Entities;

namespace SetupReference.API.Handlers
{
    public class CreateNoteRequestHandler : IRequestHandler<CreateNoteRequest, Note>
    {
        private readonly ExampleContext context;

        public CreateNoteRequestHandler(ExampleContext context)
        {
            this.context = context;
        }

        public async Task<Note> Handle(CreateNoteRequest request, CancellationToken cancellationToken)
        {
            var entity = new NoteEntity
            {
                TenantId = request.TenantId,
                Title = request.Title,
                Message = request.Message,
                DueDate = request.DueDate
            };

            context.Add(entity);

            await context.SaveChangesAsync();

            return new Note(entity.Id, entity.Title, entity.Message, entity.DueDate);
        }
    }
}
