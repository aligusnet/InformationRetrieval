using DocumentStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NaturalLanguageApp
{
    public static class VocabularyAssisstant
    {
        public static IList<(string Word, int Count)> CountWords(
            IStorageReader<IEnumerable<string>> reader,
            Func<string, string> wordTransformer)
        {
            var counter = new Dictionary<string, int>();

            foreach (var collection in reader.Read())
            {
                foreach (var doc in collection.Documents)
                {
                    foreach (var word in doc.Data.Select(wordTransformer))
                    {
                        counter.TryGetValue(word, out int count);
                        counter[word] = count + 1;
                    }
                }
            }

            return counter.OrderByDescending(p => p.Value).Select(p => (Word: p.Key, Count: p.Value)).ToArray();
        }

        public static void SerializeWordCounts(IList<(string Word, int Count)> wordCounts, Stream stream)
        {
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

            writer.WriteStartArray();
            foreach (var wc in wordCounts)
            {
                writer.WriteStartObject();
                writer.WriteString("word", wc.Word);
                writer.WriteNumber("count", wc.Count);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public static IList<(string Word, int Count)> DeserializeWordCounts(Stream stream)
        {
            var reader = new Utf8JsonStreamReader(stream);

            var wordCounts = new List<(string Word, int Count)>();
            string word = string.Empty;
            int count = 0;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        wordCounts.Add((word, count));
                        break;
                    case JsonTokenType.String:
                        word = reader.GetString();
                        break;
                    case JsonTokenType.Number:
                        count = reader.GetInt32();
                        break;
                }
            }

            return wordCounts;
        }
    }
}
