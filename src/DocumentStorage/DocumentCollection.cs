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
        /// Contents (ID -> Title map) of the collection
        /// </summary>
        public IDictionary<Guid, string> Contents { get; set; }

        /// <summary>
        /// List of documents in the collection
        /// </summary>
        public IEnumerable<Document> Pages { get; set; }
    }
}
