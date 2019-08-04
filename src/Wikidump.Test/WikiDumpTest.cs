using System.IO;
using System.Reflection;
using Xunit;

using System.Linq;

namespace Wikidump.Test
{
    public class WikiDumpTests
    {
        [Fact]
        public void WikiDumpXmlReaderTest()
        {
            var xmlDump = GetXmlDump();

            using var reader = new WikiDumpXmlReader(xmlDump);
            var pages = reader.ReadPages().ToArray();

            Assert.True(pages[0].IsRedirect);
            Assert.False(pages[0].IsSpecial);
            Assert.False(pages[0].IsContent);
            Assert.Equal("RedirectPage", pages[0].Title);
            Assert.Equal("Simple page", pages[0].RedirectTitle);
            Assert.StartsWith("#REDIRECT [[Simple page]]", pages[0].Text);

            Assert.False(pages[1].IsRedirect);
            Assert.False(pages[1].IsSpecial);
            Assert.True(pages[1].IsContent);
            Assert.Equal("Simple page", pages[1].Title);
            Assert.Equal("=Simple Page=\nSome text", pages[1].Text);

            Assert.False(pages[3].IsRedirect);
            Assert.True(pages[3].IsSpecial);
            Assert.False(pages[3].IsContent);
            Assert.Equal("Category:about", pages[3].Title);
            Assert.Equal("This is wikipedia", pages[3].Text);
        }

        [Fact]
        public void WikipediaReaderTest()
        {
            var xmlDump = GetXmlDump();

            using var xmlReader = new WikiDumpXmlReader(xmlDump);
            var wikiReader = new WikipediaReader(
                xmlReader, 
                WikipediaReader.DefaultFilter,
                blockSize: 1);
            var corpus = wikiReader.Read().ToArray();

            Assert.Equal(2, corpus.Length);

            Assert.Equal(1, corpus[0].Documents.Count);
            Assert.Equal(1, corpus[0].Metadata.Count);
            var doc1 = corpus[0].Documents[0];
            Assert.Equal("Simple page", doc1.Metadata.Title);
            Assert.Equal("=Simple Page=\nSome text", doc1.Data);
            Assert.Equal("Simple page", corpus[0].Metadata[doc1.Metadata.Id].Title);
            Assert.Equal(doc1.Metadata.Id, corpus[0].Metadata[doc1.Metadata.Id].Id);

            Assert.Equal(1, corpus[1].Documents.Count);
            Assert.Equal(1, corpus[1].Metadata.Count);
            var doc2 = corpus[1].Documents[0];
            Assert.Equal("Another page", doc2.Metadata.Title);
            Assert.Equal("Hello world", doc2.Data);
            Assert.Equal("Another page", corpus[1].Metadata[doc2.Metadata.Id].Title);
            Assert.Equal(doc2.Metadata.Id, corpus[1].Metadata[doc2.Metadata.Id].Id);
        }

        private static Stream GetXmlDump()
        {
            var assembly = typeof(WikiDumpTests).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream("Wikidump.Test.wikidump.xml")!;
        }
    }
}
