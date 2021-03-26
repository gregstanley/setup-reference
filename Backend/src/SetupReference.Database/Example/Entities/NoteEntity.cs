using System;

namespace SetupReference.Database.ExampleContext.Entities
{
    public class NoteEntity
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
