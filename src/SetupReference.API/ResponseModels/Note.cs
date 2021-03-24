using System;

namespace SetupReference.API.ResponseModels
{
    public class Note
    {
        public Note(int id, string title, string message, DateTime? dueDate)
        {
            Id = id;
            Title = title;
            Message = message;
            DueDate = dueDate;
        }

        public int Id { get; }

        public string Title { get; }

        public string Message { get; }

        public DateTime? DueDate { get; }
    }
}
