using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using DocumentStorage;
using NaturalLanguageTools;
using NaturalLanguageTools.Indexing;
using Wikidump;

namespace NaturalLanguageApp
{
    class Program
    {
        static readonly string basePath = @"F:\wikipedia";
        static readonly string wikiDumpFilePath = Path.Combine(basePath, "enwiki-20190101-pages-articles-multistream.xml");
        static readonly string wikiPath = Path.Combine(basePath, "enwiki");
        static readonly string tokenizedPath = Path.Combine(basePath, "enwiki.tokenized");
        static readonly string hashedPath = Path.Combine(basePath, "enwiki.hashed");
        static readonly string wordCountsPath = Path.Combine(basePath, "word_counts.json");
        static readonly string indexPath = Path.Combine(basePath, "index.bin");

        static readonly IDocumentDataSerializer<string> stringDataSerializer = new StringDocumentDataSerializer();
        static readonly IDocumentDataSerializer<IEnumerable<string>> tokenizedDataSerializer = new TokenizedDocumentDataSerializer();
        static readonly IDocumentDataSerializer<int[]> hashedDataSerializer = new NumberedDocumentDataSerializer();

        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            IndexWikipedia();

            timer.Stop();
            Console.WriteLine("Finished in {0}", timer.Elapsed);

            PrintTopWordCounts(10);
        }

        static void IndexWikipedia()
        {
            var reader = new StorageZipReader<int[]>(hashedPath, hashedDataSerializer);
            var index = new DictionaryIndex<int>();
            var indexBuilder = new IndexBuilder<int, int[]>(index);
            indexBuilder.IndexStorage(reader.Read());

            using var file = File.Create(indexPath);
            index.Serialize(file);
        }

        static void PrintIndexStats()
        {
            using var file = File.OpenRead(indexPath);
            var index = DictionaryIndex<int>.Deserialize(file);

            Console.WriteLine($"The: {index[DocumentHasher.CalculateHashCode("the".AsSpan())].Count()}");
        }

        static void CountWords()
        {
            var reader = new StorageZipReader<IEnumerable<string>>(tokenizedPath, tokenizedDataSerializer);

            var wordCounts = VocabularyAssisstant.CountWords(
                reader,
                s => s.ToLower());

            using var stream = CreateOutputStream(wordCountsPath);
            VocabularyAssisstant.SerializeWordCounts(wordCounts, stream);
        }

        static void PrintTopWordCounts(int top)
        {
            using var stream = File.OpenRead(wordCountsPath);
            var wordCounts = VocabularyAssisstant.DeserializeWordCounts(stream);

            foreach (var wc in wordCounts.Take(top))
            {
                Console.WriteLine($"{wc.Word}: {wc.Count}");
            }
        }

        static void HashWikipedia()
        {
            var outputWikipediaPath = hashedPath;

            PrepareOutputDirectory(outputWikipediaPath);

            var reader = new StorageZipReader<IEnumerable<string>>(tokenizedPath, tokenizedDataSerializer);
            var writer = new StorageZipWriter<int[]>(outputWikipediaPath, hashedDataSerializer);

            var hasher = new DocumentHasher();
            hasher.Transform(reader, writer);
        }

        static void TokenizeWikipedia()
        {
            var outputWikipediaPath = tokenizedPath;

            PrepareOutputDirectory(outputWikipediaPath);

            var reader = new StorageZipReader<string>(wikiPath, stringDataSerializer);
            var writer = new StorageZipWriter<IEnumerable<string>>(outputWikipediaPath, tokenizedDataSerializer);

            var wikipediaTokenizer = new DocumentTokenizer(new WordRegexTokenizer());
            wikipediaTokenizer.Transform(reader, writer);
        }

        static void TransformWikiDump()
        {
            string pathToSave = wikiPath;

            PrepareOutputDirectory(pathToSave);

            using var xmlReader = new WikiDumpXmlReader(wikiDumpFilePath);

            IStorageReader<string> reader = new WikipediaReader(xmlReader, WikipediaReader.DefaultFilter, 1000, count: 20_000);
            IStorageWriter<string> writer = new StorageZipWriter<string>(pathToSave, stringDataSerializer);

            writer.Write(reader.Read());
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
