using MediatR;

namespace SetupReference.API.Handlers
{
    public class DeleteNoteRequest : IRequest
    {
        public DeleteNoteRequest(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
