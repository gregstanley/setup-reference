using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using SetupReference.API.Handlers;
using SetupReference.API.ResponseModels;

namespace SetupReference.API.Controllers
{
    [ApiController]
    [Route("api/{tenantId:int}/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly ILogger<NotesController> logger;
        private readonly IMediator mediator;

        public NotesController(ILogger<NotesController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> Get(int tenantId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var notes = await mediator.Send(new GetNotesRequest(page, pageSize), cancellationToken);

            return Ok(notes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Note>> Get(int tenantId, int id, CancellationToken cancellationToken)
        {
            var note = await mediator.Send(new GetNoteRequest(id), cancellationToken);

            return note is null ? NotFound() : (ActionResult<Note>)note;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tenantId, string title, string message, DateTime? dueDate, CancellationToken cancellationToken)
        {
            var note = await mediator.Send(new CreateNoteRequest(tenantId, title, message, dueDate), cancellationToken);

            return Ok(note);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int tenantId, int id, CancellationToken cancellationToken)
        {
            await mediator.Send(new DeleteNoteRequest(id), cancellationToken);

            return NoContent();
        }
    }
}
