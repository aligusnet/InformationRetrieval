using System;
using System.Diagnostics;
using System.IO;
using NaturalLangugeTools;
using Wikipedia;
using static Wikidump.WikiDumpTransformer;

namespace NaturalLanguageApp
{
    class Program
    {
        static readonly string basePath = @"F:\wikipedia";

        static void Main(string[] args)
        {
            TransformWikiDump();
        }

        static void TokenizeWikipedia()
        {
            var timer = new Stopwatch();
            timer.Start();

            var inputWikipediaPath = Path.Combine(basePath, "enwiki.raw.test");
            var outputWikipediaPath = Path.Combine(basePath, "enwiki.tokenized");

            if (Directory.Exists(outputWikipediaPath))
            {
                Directory.Delete(outputWikipediaPath, recursive: true);
            }

            Directory.CreateDirectory(outputWikipediaPath);

            var storage = new WikipediaZipStorage();

            var wikipediaTokenizer = new WikipediaTokenizer(new WordRegexTokenizer());
            wikipediaTokenizer.Tokenize(storage, inputWikipediaPath, storage, outputWikipediaPath);

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);
        }

        static void TransformWikiDump()
        {
            string dumpFilePath = Path.Combine(basePath, "enwiki-20190101-pages-articles-multistream.xml");
            string pathToSave = Path.Combine(basePath, "enwiki");

            var timer = new Stopwatch();
            timer.Start();
            Transform(dumpFilePath, pathToSave, count: 5000);
            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);
        }
    }
}
