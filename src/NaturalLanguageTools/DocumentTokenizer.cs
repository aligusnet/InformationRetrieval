using System.Collections.Generic;

using DocumentStorage;
using NaturalLanguageTools.Transformers;

namespace NaturalLanguageTools
{
    using Tokens = IEnumerable<string>;

    public class DocumentTokenizer : StorageTransformer<string, Tokens>
    {

        public DocumentTokenizer(ITokenizer tokenizer) : base(new Transformer(tokenizer))
        {
        }

        private class Transformer : IDocumentTransformer<string, Tokens>
        {
            private readonly ITokenizer tokenizer;

            public Transformer(ITokenizer tokenizer)
            {
                this.tokenizer = tokenizer;
            }

            public Document<Tokens> Transform(Document<string> source)
            {
                return new Document<IEnumerable<string>>(
                    source.Id,
                    source.Title,
                    tokenizer.Tokenize(source.Data));
            }
        }
    }
}
