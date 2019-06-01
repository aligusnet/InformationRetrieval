using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Make new Document collection from a list of documents
        /// </summary>
        /// <param name="docs">The list of documents</param>
        /// <returns>The new collection</returns>
        public static DocumentCollection<T> Make(IList<Document<T>> docs)
        {
            return new DocumentCollection<T>
            {
                Metadata = BuildMetadata(docs),
                Documents = docs,
            };
        }

        private static IDictionary<Guid, DocumentProperties> BuildMetadata(IList<Document<T>> docs)
        {
            return docs.Select(ToProperties).ToDictionary(p => p.Id);
        }

        private static DocumentProperties ToProperties(Document<T> doc)
        {
            return new DocumentProperties
            {
                Id = doc.Id,
                Title = doc.Title,
            };
        }
    }
}
