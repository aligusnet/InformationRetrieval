using System;

namespace DocumentStorage
{
    public class DocumentProperties
    {
        public DocumentId Id { get; }

        public string Title { get; }

        public DocumentProperties(DocumentId id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
