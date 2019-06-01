using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using DocumentStorage;
using NaturalLangugeTools;
using Wikidump;

namespace NaturalLanguageApp
{
    class Program
    {
        static readonly string basePath = @"F:\wikipedia";
        static readonly string wikiPath = Path.Combine(basePath, "enwiki");
        static readonly string tokenizedPath = Path.Combine(basePath, "enwiki.tokenized");
        static readonly string wordCountsPath = Path.Combine(basePath, "word_counts.json");

        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            CountWords();

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);

            PrintTopWordCounts(10);
        }

        static void CountWords()
        {
            var reader = new StorageZipReader(tokenizedPath);
            var tokenizer = new WordSpaceTokenizer();

            var wordCounts = VocabularyAssisstant.CountWords(
                reader,
                tokenizer,
                s => s.ToLower());

            using var stream = CreateOutputStream(wordCountsPath);
            VocabularyAssisstant.SerializeWordCounts(wordCounts, stream);
        }

        static void PrintTopWordCounts(int top)
        {
            using var stream = File.OpenRead(wordCountsPath);
            var wordCounts = VocabularyAssisstant.DeserializeWordCounts(stream);

            foreach (var (word, count) in wordCounts.Take(top))
            {
                Console.WriteLine($"{word}: {count}");
            }
        }

        static void TokenizeWikipedia()
        {
            var outputWikipediaPath = tokenizedPath;

            PrepareOutputDirectory(outputWikipediaPath);

            IStorageReader reader = new StorageZipReader(wikiPath);
            IStorageWriter writer = new StorageZipWriter(outputWikipediaPath);

            var wikipediaTokenizer = new WikipediaTokenizer(new WordRegexTokenizer());
            wikipediaTokenizer.Tokenize(reader, writer);
        }

        static void TransformWikiDump()
        {
            string dumpFilePath = Path.Combine(basePath, "enwiki-20190101-pages-articles-multistream.xml");
            string pathToSave = wikiPath;

            PrepareOutputDirectory(pathToSave);

            using (var xmlReader = new WikiDumpXmlReader(dumpFilePath))
            {
                IStorageReader reader = new WikipediaReader(xmlReader, WikipediaReader.DefaultFilter, 1000);
                IStorageWriter writer = new StorageZipWriter(pathToSave);

                writer.Write(reader.Read());
            }
        }

        static void PrepareOutputDirectory(string outputDirectory)
        {
            if (Directory.Exists(outputDirectory))
            {
                string tmpDir = $"{outputDirectory}.tmp";
                Directory.Move(outputDirectory, tmpDir);
                Directory.Delete(tmpDir, recursive: true);
            }

            Directory.CreateDirectory(outputDirectory);
        }

        static Stream CreateOutputStream(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.Open(filePath, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                return File.OpenWrite(filePath);
            }
        }
    }
}
