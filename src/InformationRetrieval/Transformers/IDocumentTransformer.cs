using Corpus;

namespace InformationRetrieval.Transformers
{
    public interface IDocumentTransformer<TIn, TOut> : ITransformer<Document<TIn>, Document<TOut>>
    {
    }
}
