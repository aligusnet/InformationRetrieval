using System.Collections.Generic;

using DocumentStorage;

namespace NaturalLanguageTools.Transformers
{
    public interface IStorageTransformer<TIn, TOut> : ITransformer<IEnumerable<DocumentCollection<TIn>>, IEnumerable<DocumentCollection<TOut>>>
    {
        void Transform(IStorageReader<TIn> reader, IStorageWriter<TOut> writer);
    }
}
