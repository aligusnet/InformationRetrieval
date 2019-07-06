using System.Collections.Generic;

namespace Corpus
{
    /// <summary>
    /// Represents a block of documents,
    /// a subset of document corpus small enough to be processed in memory.
    /// </summary>
    public class Block<T>
    {
        /// <summary>
        /// Document properties of the block
        /// </summary>
        public BlockMetadata Metadata { get; }

        /// <summary>
        /// List of documents in the block
        /// </summary>
        public IList<Document<T>> Documents { get; }

        public Block(IList<Document<T>> docs, BlockMetadata metadata)
        {
            Documents = docs;
            Metadata = metadata;
        }

        /// <summary>
        /// Make new block from a list of documents
        /// </summary>
        /// <param name="id">The block's id</param>
        /// <param name="docs">The list of documents</param>
        /// <returns>The new block</returns>
        public static Block<T> Make(ushort id, IList<Document<T>> docs)
        {
            return new Block<T>(docs, BlockMetadata.Make<T>(id, docs));
        }
    }
}
