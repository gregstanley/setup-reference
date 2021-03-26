using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SetupReference.API.ResponseModels;
using SetupReference.Database.Example;

namespace SetupReference.API.Handlers
{
    public class GetNoteRequestHandler : IRequestHandler<GetNoteRequest, Note>
    {
        private readonly ExampleContext context;

        public GetNoteRequestHandler(ExampleContext context)
        {
            this.context = context;
        }

        public async Task<Note> Handle(GetNoteRequest request, CancellationToken cancellationToken)
        {
            var note = await context.Notes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (note is null)
                return null;

            return new Note(note.Id, note.Title, note.Message, note.DueDate);
        }
    }
}
