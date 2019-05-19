using System;
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
            TokenizeWikipedia();
        }

        static void TokenizeWikipedia()
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

            var wikipediaTokenizer = new WikipediaTokenizer(new WordRegexTokenizer());
            wikipediaTokenizer.Tokenize(storage, inputWikipediaPath, storage, outputWikipediaPath);

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);
        }
    }
}
