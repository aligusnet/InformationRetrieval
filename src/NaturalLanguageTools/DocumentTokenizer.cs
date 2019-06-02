using System.Collections.Generic;

using NaturalLanguageTools.Transformers;

namespace NaturalLanguageTools
{
    using Tokens = IEnumerable<string>;

    public class DocumentTokenizer : StorageTransformer<string, Tokens>
    {
        public DocumentTokenizer(ITokenizer tokenizer) : 
            base(new DocumentTransformer<string, Tokens>(d => tokenizer.Tokenize(d)))
        {
        }
    }
}
