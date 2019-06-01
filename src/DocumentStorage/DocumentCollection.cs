using System;
using System.Collections.Generic;

namespace DocumentStorage
{
    /// <summary>
    /// Represents a collection of documents
    /// </summary>
    public class DocumentCollection<T>
    {
        /// <summary>
        /// Document properties of the collection
        /// </summary>
        public IDictionary<Guid, DocumentProperties> Metadata { get; set; }

        /// <summary>
        /// List of documents in the collection
        /// </summary>
        public IList<Document<T>> Documents { get; set; }
    }
}
