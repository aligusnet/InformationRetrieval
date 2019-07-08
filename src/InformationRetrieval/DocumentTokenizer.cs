using System.Collections.Generic;

using InformationRetrieval.Tokenizers;
using InformationRetrieval.Transformers;

namespace InformationRetrieval
{
    public class DocumentTokenizer : CorpusTransformer<string, IEnumerable<string>>
    {
        public DocumentTokenizer(ITokenizer tokenizer) : base(d => tokenizer.Tokenize(d))
        {
        }
    }
}
