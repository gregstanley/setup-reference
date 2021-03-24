using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SetupReference.API.ResponseModels;
using SetupReference.Database.Example;

namespace SetupReference.API.Handlers
{
    public class GetNotesRequestHandler : IRequestHandler<GetNotesRequest, IEnumerable<Note>>
    {
        private readonly ExampleContext context;

        public GetNotesRequestHandler(ExampleContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Note>> Handle(GetNotesRequest request, CancellationToken cancellationToken)
        { 
            // TODO: FluentValidation
            if (request.Page < 1)
                throw new IndexOutOfRangeException(nameof(request.Page));

            var notes = await context.Notes
                .OrderBy(x => x.Id)
                .Skip(request.Page - 1)
                .Take(request.Size)
                .Select(note => new Note(note.Id, note.Title, note.Message, note.DueDate)).ToListAsync();

            return notes;
        }
    }
}
