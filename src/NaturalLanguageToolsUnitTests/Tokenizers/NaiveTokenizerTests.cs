using System.Linq;
using Xunit;

using NaturalLanguageTools.Tokenizers;
using System.Collections.Generic;

namespace NaturalLanguageToolsUnitTests.Tokenizers
{
    public class NaiveTokenizerTests
    {
        [Fact]
        public void TokenizerTests()
        {
            AssertEqual(new (int, int)[] { }, "");
            AssertEqual(new[] { (0, 4) }, "word"); ;
            AssertEqual(new[] { (0, 3), (4, 9) }, "two words");
            AssertEqual(new[] { (0, 5), (6, 12), (15, 19) }, "extra spaces   here");
        }

        private static void AssertEqual((int From, int To)[] expected, string text)
        {
            Assert.Equal(expected, NaiveTokenizer.Tokenize(text.ToCharArray()).ToArray());
        }
    }
}
