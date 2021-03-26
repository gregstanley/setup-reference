using System.Collections.Generic;
using MediatR;
using SetupReference.API.ResponseModels;

namespace SetupReference.API.Handlers
{
    public class GetNotesRequest : IRequest<IEnumerable<Note>>
    {
        public GetNotesRequest(int page, int size)
        {
            Page = page;
            Size = size;
        }

        public int Page { get; }

        public int Size { get; }
    }
}
