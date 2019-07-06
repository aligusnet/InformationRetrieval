using Corpus;

namespace NaturalLanguageTools.Transformers
{
    public interface IBlockTransformer<TIn, TOut> : ITransformer<Block<TIn>, Block<TOut>>
    {
    }
}
