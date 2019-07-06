using System.Linq;
using Xunit;

using NaturalLanguageTools.Wikitext;

namespace NaturalLanguageTools.Test.Wikitext
{
    public class WikitextParserTests
    {
        [Fact]
        public void ParseSimpleTextTest()
        {
            var wiki = "blah-blah. OK!";
            var doc = WikitextParser.Parse(wiki);
            var elements = doc.Elements.Cast<WikitextPlainText>().ToArray();

            Assert.Single(elements);
            Assert.Equal(wiki, elements[0].Value);
        }

        [Fact]
        public void ParseTextWithHeader()
        {
            var wiki = @"
                          = Hello =

                          blah-blah. OK!";
            var doc = WikitextParser.Parse(wiki);
            var elements = doc.Elements.Cast<IWikitextValue>().ToArray();

            Assert.Equal(2, elements.Length);
            Assert.Equal("Hello", elements[0].Value);
            Assert.Equal("blah-blah. OK!", elements[1].Value);
        }

        [Fact]
        public void ParseBadText()
        {
            var wiki = @"
                          = Hello =
                          blah-blah. OK!";
            Assert.Throws<Sprache.ParseException>(() => WikitextParser.Parse(wiki));
        }

    }
}
