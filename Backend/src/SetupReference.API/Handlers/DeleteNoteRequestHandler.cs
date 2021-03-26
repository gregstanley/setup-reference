using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SetupReference.Database.Example;

namespace SetupReference.API.Handlers
{
    public class DeleteNoteRequestHandler : IRequestHandler<DeleteNoteRequest, Unit>
    {
        private readonly ExampleContext context;

        public DeleteNoteRequestHandler(ExampleContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(DeleteNoteRequest request, CancellationToken cancellationToken)
        {
            var note = await context.Notes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (note is null)
                return Unit.Value;

            context.Notes.Remove(note);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
