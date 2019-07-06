namespace Corpus
{
    /// <summary>
    /// Class represents text document
    /// </summary>
    public class Document<T>
    {
        /// <summary>
        /// Gets/sets metadata of the document
        /// </summary>
        public DocumentMetadata Metadata { get; }

        /// <summary>
        /// Gets/sets text data of the document
        /// </summary>
        public T Data { get; }

        public Document(DocumentMetadata metadata, T data)
        {
            Metadata = metadata;
            Data = data;
        }
    }
}
