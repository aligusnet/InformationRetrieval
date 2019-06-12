using System.Linq;
using DocumentStorage;

namespace NaturalLanguageTools.Transformers
{
    class DocumentCollectionTransformer<TIn, TOut> : IDocumentCollectionTransformer<TIn, TOut>
    {
        private readonly IDocumentTransformer<TIn, TOut> documentTransformer;

        public DocumentCollectionTransformer(IDocumentTransformer<TIn, TOut> documentTransformer)
        {
            this.documentTransformer = documentTransformer;
        }

        public DocumentCollection<TOut> Transform(DocumentCollection<TIn> source)
        {
            var target = source.Documents.Select(d => documentTransformer.Transform(d)).ToList();
            return DocumentCollection<TOut>.Make(source.Metadata.Id, target);
        }
    }
}
