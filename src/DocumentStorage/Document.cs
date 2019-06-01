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
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets title of the document
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets text data of the document
        /// </summary>
        public T Data { get; set; }
    }
}
