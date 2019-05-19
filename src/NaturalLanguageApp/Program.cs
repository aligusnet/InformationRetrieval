﻿using System;
using System.Diagnostics;
using System.IO;
using NaturalLangugeTools;
using Wikidump;
using DocumentStorage;

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

            IStorageReader reader = new StorageZipReader(inputWikipediaPath);
            IStorageWriter writer = new StorageZipWriter(outputWikipediaPath);

            var wikipediaTokenizer = new WikipediaTokenizer(new WordRegexTokenizer());
            wikipediaTokenizer.Tokenize(reader, writer);

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);
        }

        static void TransformWikiDump()
        {
            var timer = new Stopwatch();
            timer.Start();

            string dumpFilePath = Path.Combine(basePath, "enwiki-20190101-pages-articles-multistream.xml");
            string pathToSave = Path.Combine(basePath, "enwiki");

            if (Directory.Exists(pathToSave))
            {
                Directory.Delete(pathToSave, recursive: true);
            }

            Directory.CreateDirectory(pathToSave);

            using (var xmlReader = new WikiDumpXmlReader(dumpFilePath))
            {
                IStorageReader reader = new WikipediaReader(xmlReader, WikipediaReader.DefaultFilter, 1000, 5000);
                IStorageWriter writer = new StorageZipWriter(pathToSave);

                writer.Write(reader.Read());
            }

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);
        }
    }
}
