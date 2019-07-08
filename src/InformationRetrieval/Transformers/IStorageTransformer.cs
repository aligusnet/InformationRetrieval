using System.Collections.Generic;

using Corpus;

namespace InformationRetrieval.Transformers
{
    public interface ICorpusTransformer<TIn, TOut> : ITransformer<IEnumerable<Block<TIn>>, IEnumerable<Block<TOut>>>
    {
        void Transform(ICorpusReader<TIn> reader, ICorpusWriter<TOut> writer);
    }
}
