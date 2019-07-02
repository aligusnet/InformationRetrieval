using static System.Threading.Tasks.Parallel;
using DocumentStorage;

namespace NaturalLanguageTools.Transformers.Parallel
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
            var target = new Document<TOut>[source.Documents.Count];
            For(0, target.Length, index => target[index] = documentTransformer.Transform(source.Documents[index]));
            return DocumentCollection<TOut>.Make(source.Metadata.Id, target);
        }
    }
}
