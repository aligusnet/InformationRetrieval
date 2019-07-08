using System.Collections.Generic;
using System.Linq;

namespace InformationRetrieval.Tokenizers
{
    public class WordSpaceTokenizer : ITokenizer
    {
        public IEnumerable<string> Tokenize(string text)
        {
            return text.Split().Where(s => s != string.Empty);
        }
    }
}
