using Corpus;

namespace NaturalLanguageTools.Transformers
{
    public interface IDocumentTransformer<TIn, TOut> : ITransformer<Document<TIn>, Document<TOut>>
    {
    }
}
