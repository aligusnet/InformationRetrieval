using DocumentStorage;

namespace NaturalLanguageTools.Transformers
{
    public interface IDocumentCollectionTransformer<TIn, TOut> : ITransformer<DocumentCollection<TIn>, DocumentCollection<TOut>>
    {
    }
}
