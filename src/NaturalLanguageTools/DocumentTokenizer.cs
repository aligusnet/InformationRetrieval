using System.Collections.Generic;

using NaturalLanguageTools.Transformers;

namespace NaturalLanguageTools
{
    public class DocumentTokenizer : StorageTransformer<string, IEnumerable<string>>
    {
        public DocumentTokenizer(ITokenizer tokenizer) : base(d => tokenizer.Tokenize(d))
        {
        }
    }
}
