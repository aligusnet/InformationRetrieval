using System;
using System.Collections.Generic;
using System.Linq;
using DocumentStorage;

namespace NaturalLanguageTools.Transformers
{
    public class StorageTransformer<TIn, TOut> : IStorageTransformer<TIn, TOut>
    {
        private readonly IDocumentCollectionTransformer<TIn, TOut> collectionTransformer;

        public StorageTransformer(Func<TIn, TOut> processData) : this(new DocumentTransformer<TIn, TOut>(processData))
        {
        }

        public StorageTransformer(IDocumentTransformer<TIn, TOut> documentTransformer) : this(new DocumentCollectionTransformer<TIn, TOut>(documentTransformer))
        {
        }

        public StorageTransformer(IDocumentCollectionTransformer<TIn, TOut> collectionTransformer)
        {
            this.collectionTransformer = collectionTransformer;
        }

        public IEnumerable<DocumentCollection<TOut>> Transform(IEnumerable<DocumentCollection<TIn>> source)
        {
            return source.Select(dc => collectionTransformer.Transform(dc));
        }

        public void Transform(IStorageReader<TIn> reader, IStorageWriter<TOut> writer)
        {
            var source = reader.Read();
            var target = Transform(source);
            writer.Write(target);
        }
    }
}
