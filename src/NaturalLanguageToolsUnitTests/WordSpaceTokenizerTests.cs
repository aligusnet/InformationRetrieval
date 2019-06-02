using Xunit;

using NaturalLanguageTools;

namespace NaturalLanguageToolsUnitTests
{
    public class WordSpaceTokenizerTests
    {
        [Fact]
        public void SpaceTokenizerTest()
        {
            var text = "He is? The dog ran. The sun is out!";
            var expected = new string[] { "He", "is?", "The", "dog", "ran.", "The", "sun", "is", "out!" };
            var actual = new WordSpaceTokenizer().Tokenize(text);

            Assert.Equal(expected, actual);
        }
    }
}
