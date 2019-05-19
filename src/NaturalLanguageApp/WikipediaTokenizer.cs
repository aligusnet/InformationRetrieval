using System;
using System.Collections.Generic;
using NaturalLangugeTools;
using Wikipedia;

namespace NaturalLanguageApp
{
    public class WikipediaTokenizer
    {
        private readonly ITokenizer tokenizer;

        public WikipediaTokenizer(ITokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public void Tokenize(IWikipediaReader reader, IWikipediaWriter writer)
        {
            var wikipedia = reader.Read();
            var processedWikipedia = ProcessWikipedia(wikipedia);
            writer.Write(processedWikipedia);
        }

        private IEnumerable<WikiCollection> ProcessWikipedia(IEnumerable<WikiCollection> wikipedia)
        {
            foreach (var collection in wikipedia)
            {
                yield return ProcessWikiCollection(collection);
            }
        }

        private WikiCollection ProcessWikiCollection(WikiCollection collection)
        {
            return new WikiCollection
            {
                Contents = collection.Contents,
                Pages = ProcessWikiPages(collection.Pages),
            };
        }

        private IEnumerable<WikiPage> ProcessWikiPages(IEnumerable<WikiPage> pages)
        {
            foreach (var page in pages)
            {
                var data = string.Join(' ', tokenizer.Tokenize(page.Data));
                var title = string.Join(' ', tokenizer.Tokenize(page.Title));

                yield return new WikiPage
                {
                    Id = page.Id,
                    Title = page.Title,
                    Data = string.Join(Environment.NewLine, title, data)
                };
            }
        }
    }
}
