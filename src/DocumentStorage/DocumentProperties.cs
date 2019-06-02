using System;

namespace DocumentStorage
{
    public class DocumentProperties
    {
        public Guid Id { get; }

        public string Title { get; }

        public DocumentProperties(Guid id, string title)
        {
            Id = Id;
            Title = title;
        }
    }
}
