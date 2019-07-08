using Corpus;

namespace InformationRetrieval.Transformers
{
    public interface IBlockTransformer<TIn, TOut> : ITransformer<Block<TIn>, Block<TOut>>
    {
    }
}
