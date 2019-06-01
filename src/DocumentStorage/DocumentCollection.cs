using System;
using System.Collections.Generic;

namespace DocumentStorage
{
    /// <summary>
    /// Represents a collection of documents
    /// </summary>
    public class DocumentCollection
    {
        /// <summary>
        /// Document properties of the collection
        /// </summary>
        public IDictionary<Guid, DocumentProperties> Metadata { get; set; }

        /// <summary>
        /// List of documents in the collection
        /// </summary>
        public IList<Document> Documents { get; set; }
    }
}
