namespace Corpus
{
    public class DocumentMetadata
    {
        public DocumentId Id { get; }

        public string Title { get; }

        public DocumentMetadata(DocumentId id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
