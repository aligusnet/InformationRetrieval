using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Corpus;
using InformationRetrieval;
using InformationRetrieval.Indexing;
using InformationRetrieval.Indexing.External;
using InformationRetrieval.Indexing.InMemory;
using InformationRetrieval.Tokenizers;
using InformationRetrieval.Transformers;
using InformationRetrieval.Utility;
using InformationRetrieval.Wikitext;
using Wikidump;

namespace NaturalLanguage.App
{
    class Program
    {
        private const int RangeThreshold = 5;
        static readonly string basePath = @"F:\wikipedia";
        static readonly string wikiDumpFilePath = Path.Combine(basePath, "enwiki-20190101-pages-articles-multistream.xml");
        static readonly string wikiPath = Path.Combine(basePath, "enwiki");
        static readonly string cleanedPath = Path.Combine(basePath, "enwiki.cleaned");
        static readonly string tokenizedPath = Path.Combine(basePath, "enwiki.tokenized");
        static readonly string hashedPath = Path.Combine(basePath, "enwiki.hashed");
        static readonly string wordCountsPath = Path.Combine(basePath, "word_counts.json");
        static readonly string indexPath = Path.Combine(basePath, "index.bin");
        static readonly string dawgIndexPath = Path.Combine(basePath, "dawg_index.bin");
        static readonly string externalIndexPath = Path.Combine(basePath, "external_index");

        static readonly IDocumentDataSerializer<string> stringDataSerializer = new StringDocumentDataSerializer();
        static readonly IDocumentDataSerializer<IEnumerable<string>> tokenizedDataSerializer = new TokenizedDocumentDataSerializer();
        static readonly IDocumentDataSerializer<int[]> hashedDataSerializer = new NumberedDocumentDataSerializer();
        static readonly IDocumentDataSerializer<IList<char>> charDataSerializer = new CharDocumentDataSerializer();

        static readonly Stopwatch timer = new Stopwatch();

        static void Main(string[] args)
        {
            var arguments = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            arguments.UnionWith(args);

            Console.Write("Arguments:");
            foreach (var arg in args)
            {
                Console.Write(" {0}", arg);
            }
            Console.WriteLine("\n");

            var actions = new Action[]
            {
                TransformWikiDump,
                CleanWikitext,
                TokenizeWikipedia,
                HashWikipedia,
                IndexWikipedia,
                ProcessAndIndexWikipedia,
                PrintIndexStats,
                BuildExternalIndex,
                PrintExternalIndexStats,
            };

            foreach (var action in actions)
            {
                if (arguments.Contains(action.Method.Name))
                {
                    Run(action);
                }
            }
        }

        static void Run(Action action)
        {
            Console.WriteLine($"Running {action.Method.Name}");
            timer.Restart();
            action();
            timer.Stop();
            Console.WriteLine($"{action.Method.Name} completed in {timer.Elapsed:g}\n");
        }

        static void ProcessAndIndexWikipedia()
        {
            var reader = new CorpusZipReader<IList<char>>(wikiPath, charDataSerializer);
            var index = new DictionaryIndex<int>(rareWordThreshold: 5);
            var indexBuilder = new IndexBuilder<int, IEnumerable<int>>(index);
            var processor = new WikitextProcessor();
            indexBuilder.IndexCorpus(processor.Transform(reader.Read()));

            Console.WriteLine("Serializing index...");
            using var file = File.Create(indexPath);
            index.Serialize(file);
        }

        static void BuildExternalIndex()
        {
            PrepareOutputDirectory(externalIndexPath);

            var reader = new CorpusZipReader<IList<char>>(wikiPath, charDataSerializer);
            using var buildableIndex = new BlockedExternalBuildableIndex<int>(
                DictonaryBasedExternalBuildableIndex<int>.GetCreateMethod(RangeThreshold), 
                externalIndexPath);
            var indexBuilder = new IndexBuilder<int, IEnumerable<int>>(buildableIndex);
            var processor = new WikitextProcessor();
            indexBuilder.IndexCorpus(processor.Transform(reader.Read()));

            using var index = buildableIndex.Build();

            var serializer = new ExternalIndexSerializer<int>();
            serializer.Serialize(externalIndexPath, index);
        }

        static void PrintExternalIndexStats()
        {
            var serializer = new ExternalIndexSerializer<int>();
            using var index = serializer.Deserialize(externalIndexPath);

            Console.WriteLine($"The: {index.Search(TextHasher.CalculateHashCode("the".AsSpan())).Count()}");
        }

        static void BuildDawgIndex()
        {
            var reader = new CorpusZipReader<IEnumerable<string>>(tokenizedPath, tokenizedDataSerializer);
            var buildableIndex = new DawgBuildableIndex();
            var indexBuilder = new IndexBuilder<string, IEnumerable<string>>(buildableIndex);
            indexBuilder.IndexCorpus(reader.Read());

            var index = buildableIndex.Build();

            using var file = File.Create(dawgIndexPath);
            index.Serialize(file);
        }

        static void PrintDawgIndexStats()
        {
            using var file = File.OpenRead(dawgIndexPath);
            var index = DawgSearchableIndex.Deserialize(file);

            Console.WriteLine($"The: {index.Search("the").Count()}");
        }

        static void IndexWikipedia()
        {
            var reader = new CorpusZipReader<int[]>(hashedPath, hashedDataSerializer);
            var index = new DictionaryIndex<int>(rareWordThreshold: RangeThreshold);
            var indexBuilder = new IndexBuilder<int, int[]>(index);
            indexBuilder.IndexCorpus(reader.Read());

            using var file = File.Create(indexPath);
            index.Serialize(file);
        }

        static void PrintIndexStats()
        {
            using var file = File.OpenRead(indexPath);
            var index = DictionaryIndex<int>.Deserialize(file);

            Console.WriteLine($"The: {index.Search(TextHasher.CalculateHashCode("the".AsSpan())).Count()}");
        }

        static void CountWords()
        {
            var reader = new CorpusZipReader<IEnumerable<string>>(tokenizedPath, tokenizedDataSerializer);

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

            var reader = new CorpusZipReader<IEnumerable<string>>(tokenizedPath, tokenizedDataSerializer);
            var writer = new CorpusZipWriter<int[]>(outputWikipediaPath, hashedDataSerializer);

            var hasher = new DocumentHasher();
            hasher.Transform(reader, writer);
        }

        static void TokenizeWikipedia()
        {
            var outputWikipediaPath = tokenizedPath;

            PrepareOutputDirectory(outputWikipediaPath);

            var reader = new CorpusZipReader<IList<char>>(cleanedPath, charDataSerializer);
            var writer = new CorpusZipWriter<IList<char>>(outputWikipediaPath, charDataSerializer);

            var tokenizer = new CorpusTransformer<IList<char>, IList<char>>(t => StateMachineTokenizer.Tokenize(t, lowerCase: true));
            tokenizer.Transform(reader, writer);
        }

        static void CleanWikitext()
        {
            var outputWikipediaPath = cleanedPath;

            PrepareOutputDirectory(outputWikipediaPath);

            var reader = new CorpusZipReader<IList<char>>(wikiPath, charDataSerializer);
            var writer = new CorpusZipWriter<IList<char>>(outputWikipediaPath, charDataSerializer);

            var cleaner = new CorpusTransformer<IList<char>, IList<char>>(WikitextCleaner.Clean);
            cleaner.Transform(reader, writer);
        }

        static void TransformWikiDump()
        {
            string pathToSave = wikiPath;

            PrepareOutputDirectory(pathToSave);

            using var xmlReader = new WikiDumpXmlReader(wikiDumpFilePath);

            ICorpusReader<string> reader = new WikipediaReader(xmlReader, WikipediaReader.DefaultFilter, 5_000, count: 100_000);
            ICorpusWriter<string> writer = new CorpusZipWriter<string>(pathToSave, stringDataSerializer);

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
