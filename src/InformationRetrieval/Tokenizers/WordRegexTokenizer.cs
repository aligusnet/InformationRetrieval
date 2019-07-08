using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InformationRetrieval.Tokenizers
{
    /// <summary>
    /// Tokenizer implementation that uses predefined regex to split text into words
    /// </summary>
    public class WordRegexTokenizer : ITokenizer
    {
        private const string GroupName = "word";

        private static readonly Regex reWordTokenize;
        private readonly bool lowerCase;

        static WordRegexTokenizer()
        {
            string nonWordSymbols = @",\.\?\!'"":\[\]\{})=\|";
            var wordTokenize = $@"(?<{GroupName}>[\w\d-]+)[{nonWordSymbols}]*\s*";
            reWordTokenize = new Regex(wordTokenize, RegexOptions.Compiled);
        }

        public WordRegexTokenizer(bool lowerCase)
        {
            this.lowerCase = lowerCase;
        }

        /// <summary>
        /// Splits the given text into words
        /// </summary>
        /// <param name="text">The text to tokenize</param>
        /// <returns>The list of words</returns>
        public IEnumerable<string> Tokenize(string text)
        {
            text = lowerCase ? text.ToLower() : text;
            var matches = reWordTokenize.Matches(text);
            var result = new string[matches.Count];

            return matches.Cast<Match>().Select(m => m.Groups[GroupName].Value);
        }
    }
}
