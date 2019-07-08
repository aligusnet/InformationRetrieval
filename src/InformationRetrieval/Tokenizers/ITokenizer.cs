using System.Collections.Generic;

namespace InformationRetrieval.Tokenizers
{
    /// <summary>
    /// Generic Tokenizer Interface
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Splits the given text into tokens
        /// </summary>
        /// <param name="text">The text to tokenize</param>
        /// <returns>The list of tokens</returns>
        IEnumerable<string> Tokenize(string text);
    }
}
