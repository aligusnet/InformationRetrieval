using System.Collections.Generic;

namespace InformationRetrieval.Tokenizers
{
    public static class NaiveTokenizer
    {
        private const char Space = '\u0020';

        public static IEnumerable<(int From, int To)> Tokenize(IList<char> text)
        {
            int from = 0;

            for (int i = 0; i < text.Count; ++i)
            {
                if (text[i] == Space)
                {
                    if (i - from > 0)
                    {
                        yield return (from, i);
                    }
                    
                    from = i + 1;
                }
            }

            if (text.Count - from > 0)
            {
                yield return (from, text.Count);
            }
        }
    }
}
