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
        public DocumentCollectionMetadata Metadata { get; }

        /// <summary>
        /// List of documents in the collection
        /// </summary>
        public IList<Document<T>> Documents { get; }

        public DocumentCollection(IList<Document<T>> docs, DocumentCollectionMetadata metadata)
        {
            Documents = docs;
            Metadata = metadata;
        }

        /// <summary>
        /// Make new Document collection from a list of documents
        /// </summary>
        /// <param name="docs">The list of documents</param>
        /// <returns>The new collection</returns>
        public static DocumentCollection<T> Make(IList<Document<T>> docs)
        {
            return new DocumentCollection<T>(docs, DocumentCollectionMetadata.Make<T>(docs));
        }
    }
}
