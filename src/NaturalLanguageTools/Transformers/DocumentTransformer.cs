using System;

using DocumentStorage;

namespace NaturalLanguageTools.Transformers
{
    public class DocumentTransformer<TIn, TOut> : IDocumentTransformer<TIn, TOut>
    {
        private readonly Func<TIn, TOut> transformData;

        public DocumentTransformer(Func<TIn, TOut> transformData)
        {
            this.transformData = transformData;
        }

        public Document<TOut> Transform(Document<TIn> source)
        {
            return new Document<TOut>(
                    source.Metadata,
                    transformData(source.Data));
        }
    }
}
