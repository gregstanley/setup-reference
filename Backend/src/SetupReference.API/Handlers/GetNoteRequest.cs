using MediatR;
using SetupReference.API.ResponseModels;

namespace SetupReference.API.Handlers
{
    public class GetNoteRequest : IRequest<Note>
    {
        public GetNoteRequest(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
