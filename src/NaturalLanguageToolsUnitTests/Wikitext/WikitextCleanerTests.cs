using System.Linq;
using Xunit;

using NaturalLanguageTools.Wikitext;


namespace NaturalLanguageToolsUnitTests.Wikitext
{
    public class WikitextCleanerTests
    {
        [Theory]
        [InlineData("[[simple link]]", "simple link")]
        [InlineData("[[some link| some text]]", " some text")]
        [InlineData("[not internal link]", "[not internal link]")]
        [InlineData("[http://target external link]", "[http://target external link]")]
        public void CleanLinksTest(string input, string expected)
        {
            AssertClean(input, expected);
        }

        [Theory]
        [InlineData("{{template}}", "")]
        [InlineData("{{template: [[link]]}}hi", "hi")]
        [InlineData("{{template: {{tplt2}} }}hi", "hi")]
        [InlineData("Oops: {{template: {{tplt2}}}}hi", "Oops: }hi")]
        [InlineData("{Not template }hi", "{Not template }hi")]
        public void CleanTemplatesTest(string input, string expected)
        {
            AssertClean(input, expected);
        }

        [Theory]
        [InlineData("<tag>Hello world</tag> after", "Hello world after")]
        [InlineData("<br  />", "")]
        [InlineData("<tag><template>{{template}}pss</template><link>[[ Hi there]]</link></tag> after", "pss Hi there after")]
        [InlineData("<a href=\"http://there.com/page.html?d=rt&rt=d\">there</a>", "there")]
        [InlineData("a < 5 && b > 6", "a < 5 && b > 6")]
        [InlineData("oops: a <b && b > 6", "oops: a  6")]
        public void CleanTagsTest(string input, string expected)
        {
            AssertClean(input, expected);
        }

        private void AssertClean(string input, string expected)
        {
            var actual = WikitextCleaner.Clean(input.ToCharArray());

            Assert.Equal(expected, new string(actual.ToArray()));
        }
    }
}
