using System.Collections.Generic;
using System.Linq;

namespace NaturalLangugeTools
{
    public class WordSpaceTokenizer : ITokenizer
    {
        public IEnumerable<string> Tokenize(string text)
        {
            return text.Split().Where(s => s != string.Empty);
        }
    }
}
