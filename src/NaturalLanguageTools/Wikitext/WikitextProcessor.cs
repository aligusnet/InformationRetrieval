using System.Collections.Generic;
using System.Linq;

using NaturalLanguageTools.Tokenizers;
using NaturalLanguageTools.Transformers;
using NaturalLanguageTools.Utility;

namespace NaturalLanguageTools.Wikitext
{
    /// <summary>
    /// Clean, tokenize, and hash wikitext documents.
    /// </summary>
    public class WikitextProcessor : StorageTransformer<IList<char>, IEnumerable<int>>
    {
        public WikitextProcessor() : base(Process)
        {
        }

        private static IEnumerable<int> Process(IList<char> text)
        {
            var cleaned = WikitextCleaner.Clean(text);
            var tokenized = StateMachineTokenizer.Tokenize(cleaned, lowerCase: true);
            return NaiveTokenizer.Tokenize(tokenized)
                                 .Select(ft => TextHasher.CalculateHashCode(tokenized, ft.From, ft.To));
        }
    }
}
