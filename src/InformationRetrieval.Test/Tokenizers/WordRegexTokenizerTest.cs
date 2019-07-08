using Xunit;

using InformationRetrieval.Tokenizers;

namespace InformationRetrieval.Test.Tokenizers
{
    public class WordRegexTokenizerTests
    {
        [Fact]
        public void TokenizerSmokeTest()
        {
            var text = "He is? The dog ran. The sun is out!";
            var expected = new string[] { "he", "is", "the", "dog", "ran", "the", "sun", "is", "out" };
            var actual = new WordRegexTokenizer(lowerCase: true).Tokenize(text);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TokenizeWikiText()
        {
            var text = @"{{Use dmy dates|date=June 2013}}
                        '''Altenberg''' ([[German language|German]] for ""old mountain"") may refer to:

                        = Places =

                        === Germany ===
                        * [[Altenberg, Saxony]], a town in the Free State of Saxony
                        __NOTOC__

                        { { disambiguation}
                                    }
                        [[Category: Place name disambiguation pages]]";

            var expected = new string []
                {"Use", "dmy", "dates", "date", "June", "2013",
                 "Altenberg", "German", "language", "German", "for", "old", "mountain", "may", "refer", "to",
                 "Places",
                 "Germany",
                 "Altenberg", "Saxony", "a", "town", "in", "the", "Free", "State", "of", "Saxony",
                 "__NOTOC__",
                 "disambiguation",
                 "Category", "Place", "name", "disambiguation", "pages"};

            var actual = new WordRegexTokenizer(lowerCase: false).Tokenize(text);

            Assert.Equal(expected, actual);
        }
    }
}
