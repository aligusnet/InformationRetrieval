using System;

namespace DocumentStorage
{
    /// <summary>
    /// Class represents text document
    /// </summary>
    public class Document<T>
    {
        /// <summary>
        /// Gets/sets unique ID of the page
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets/sets title of the document
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets/sets text data of the document
        /// </summary>
        public T Data { get; }

        public Document(Guid id, string title, T data)
        {
            Id = id;
            Title = title;
            Data = data;
        }
    }
}
