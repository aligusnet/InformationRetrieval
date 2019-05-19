using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NaturalLangugeTools;
using Wikipedia;

namespace NaturalLanguageApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            var basePath = @"F:\wikipedia";
            var inputWikipediaPath = Path.Combine(basePath, "enwiki.raw.test");
            var outputWikipediaPath = Path.Combine(basePath, "enwiki.tokenized");

            if (Directory.Exists(outputWikipediaPath))
            {
                Directory.Delete(outputWikipediaPath, recursive: true);
            }

            Directory.CreateDirectory(outputWikipediaPath);

            var storage = new WikipediaZipStorage();
            var wikipedia = storage.Read(inputWikipediaPath);
            var processedWikipedia = ProcessWikipedia(wikipedia);
            storage.Write(processedWikipedia, outputWikipediaPath);

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);
        }

        static IEnumerable<WikiCollection> ProcessWikipedia(IEnumerable<WikiCollection> wikipedia)
        {
            var tokenizer = new WordRegexTokenizer();

            foreach (var collection in wikipedia)
            {
                yield return ProcessWikiCollection(collection, tokenizer);
            }
        }

        static WikiCollection ProcessWikiCollection(WikiCollection collection, ITokenizer tokenizer)
        {
            return new WikiCollection
            {
                Contents = collection.Contents,
                Pages = ProcessWikiPages(collection.Pages, tokenizer),
            };
        }

        static IEnumerable<WikiPage> ProcessWikiPages(IEnumerable<WikiPage> pages, ITokenizer tokenizer)
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
