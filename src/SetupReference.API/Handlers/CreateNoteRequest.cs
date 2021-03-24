using System;
using MediatR;
using SetupReference.API.ResponseModels;

namespace SetupReference.API.Handlers
{
    public class CreateNoteRequest : IRequest<Note>
    {
        public CreateNoteRequest(int tenantId, string title, string message, DateTime? dueDate)
        {
            TenantId = tenantId;
            Title = title;
            Message = message;
            DueDate = dueDate;
        }

        public int TenantId { get; }

        public string Title { get; }

        public string Message { get; }

        public DateTime? DueDate { get; }
    }
}
