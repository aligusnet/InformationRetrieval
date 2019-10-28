using System;
using System.Linq;
using Xunit;

using InformationRetrieval.Utility;
using InformationRetrieval.Wikitext;
using System.Collections.Generic;
using Corpus;

namespace InformationRetrieval.Test.Wikitext
{
    using Tokens = IList<char>;

    public class WikitextProcessorTests
    {
        private static readonly WikitextProcessor processor = new WikitextProcessor();
        [Fact]
        public void ProcessorTest()
        {
            string text0 = @"{{template and {{subtemplate}}}}
                            == Header ==
                            <a href=""http://www.link.to/"">link</a>";
            string text1 = "Some long text;!!! with  punctuations; And!!!!?";
            var expected = new[]
            {
                CreateExpectedResult("header link"),
                CreateExpectedResult("some long text with punctuations and"),
            };

            var corpus = CreateCorpus(text0, text1);

            var processed = processor.Transform(corpus).First().Documents.Select(d => d.Data).ToArray();

            Assert.Equal(expected, processed);
        }

        private static IEnumerable<Block<Tokens>> CreateCorpus(params string[] texts)
        {
            var docs = new List<Document<Tokens>>(texts.Length);
            for (ushort i = 0; i < texts.Length; ++i)
            {
                docs.Add(CreateDocument(texts[i], i));
            };

            return new List<Block<Tokens>>
            {
                Block<Tokens>.Make(0, docs),
            };
        }

        private static Document<Tokens> CreateDocument(string text, ushort index)
        {
            return new Document<Tokens>(
                    new DocumentMetadata(new DocumentId(index), $"Title {index}"),
                    text.ToCharArray());
        }

        private static IEnumerable<int> CreateExpectedResult(string words)
        {
            return words.Split().Select(w => TextHasher.CalculateHashCode(w.AsSpan()));
        }
    }
}
