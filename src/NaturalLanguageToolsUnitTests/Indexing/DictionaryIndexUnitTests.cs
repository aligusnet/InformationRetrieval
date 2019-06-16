using System;
using System.IO;
using System.Linq;

using Xunit;

using DocumentStorage;
using NaturalLanguageTools.Indexing;

namespace NaturalLanguageToolsUnitTests.Indexing
{
    public class DictionaryIndexUnitTests
    {
        [Fact]
        public void IndexSearchTest()
        {
            var index = CreateIndex();

            var actualLargestDocIds = index.Search("largest").ToArray();
            var expectedLargestDocIds = new[]
            {
                new DocumentId(0, 0),
                new DocumentId(1, 2), new DocumentId(1, 3),
            };

            Assert.Equal(expectedLargestDocIds, actualLargestDocIds);

            var actualTheDocIds = index.Search("the").ToArray();
            var expectedTheDocIds = new[]
            {
                new DocumentId(0, 0), new DocumentId(0, 1), new DocumentId(0, 2),
                new DocumentId(1, 0), new DocumentId(1, 1), new DocumentId(1, 2), new DocumentId(1, 3),
            };

            Assert.Equal(expectedTheDocIds, actualTheDocIds);

            var actualMoonDocIds = index.Search("moon").ToArray();
            var expectedMoonDocIds = Array.Empty<DocumentId>();

            Assert.Equal(expectedMoonDocIds, actualMoonDocIds);

            var expectedAllDocIds = new[]
            {
                new DocumentId(0, 0), new DocumentId(0, 1), new DocumentId(0, 2),
                new DocumentId(1, 0), new DocumentId(1, 1), new DocumentId(1, 2), new DocumentId(1, 3),
            };
            Assert.Equal(expectedAllDocIds, index.AllDocuments());
        }

        [Fact]
        public void SerializationTest()
        {
            using var stream = new MemoryStream();

            var index = CreateIndex();
            index.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var deserializedIndex = DictionaryIndex<string>.Deserialize(stream);

            foreach (var word in new[] { "largest", "the", "moon" })
            {
                Assert.Equal(index.Search(word), deserializedIndex.Search(word));
            }

            Assert.Equal(index.AllDocuments(), deserializedIndex.AllDocuments());
        }

        private static DictionaryIndex<string>  CreateIndex()
        {
            var storage = GetTestSentenceCollections();

            var index = new DictionaryIndex<string>();
            BuildIndex(index, storage);

            return index;
        }

        private static string[][] GetTestSentenceCollections()
        {
            var collection1 = new[]
            {
                "The Great Barrier Reef in Australia is the world’s largest reef system",
                "The waste hierarchy or 3 R’s are in order of importance reduce reuse and recycle",
                "Around 75% of the volcanoes on Earth are found in the Pacific Ring of Fire an area around the Pacific Ocean where tectonic plates meet",
            };

            var collection2 = new[]
            {
                "Despite it name the Killer Whale Orca is actually a type of dolphin",
                "Giant water lilies in the Amazon can grow over 6 feet in diameter",
                "The largest ocean on Earth is the Pacific Ocean",
                "The largest individual flower on Earth is from a plant called Rafflesia arnoldii Its flowers reach up to 1 metre 3 feet in diameter and weigh around 10kg",
            };

            return new[]
            {
                collection1,
                collection2,
            };
        }

        private static void BuildIndex(DictionaryIndex<string> index, string[][] sentenceCollections)
        {
            for (var collectionId = 0; collectionId < sentenceCollections.Length; ++collectionId)
            {
                var collection = sentenceCollections[collectionId];
                for (var localId = 0; localId < collection.Length; ++localId)
                {
                    var doc = collection[localId].ToLower().Split();
                    var docId = new DocumentId((ushort)collectionId, (ushort)localId);
                    for (var position = 0; position < doc.Length; ++position)
                    {
                        index.IndexWord(docId, doc[position], position);
                    }
                }
            }
        }
    }
}
