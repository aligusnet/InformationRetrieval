using System;
using System.Collections.Generic;

using Corpus;
using InformationRetrieval.Indexing;

namespace InformationRetrieval.Test.Indexing
{
    public static class IndexHelper
    {
        public static string[][] GetTestSentenceBlocks()
        {
            var block1 = new[]
            {
                "The Great Barrier Reef in Australia is the world’s largest reef system",
                "The waste hierarchy or 3 R’s are in order of importance reduce reuse and recycle",
                "Around 75% of the volcanoes on Earth are found in the Pacific Ring of Fire an area around the Pacific Ocean where tectonic plates meet",
            };

            var block2 = new[]
            {
                "Despite it name the Killer Whale Orca is actually a type of dolphin",
                "Giant water lilies in the Amazon can grow over 6 feet in diameter",
                "The largest ocean on Earth is the Pacific Ocean",
                "The largest individual flower on Earth is from a plant called Rafflesia arnoldii Its flowers reach up to 1 metre 3 feet in diameter and weigh around 10kg",
            };

            return new[]
            {
                block1,
                block2,
            };
        }

        public static IDictionary<string, DocumentId[]> Results = new Dictionary<string, DocumentId[]>
        {
            {
                "largest",
                new[]
                {
                    new DocumentId(0),
                    new DocumentId(12), new DocumentId(13),
                }
            },
            {
                "the",
                new[]
                {
                    new DocumentId(0), new DocumentId(1), new DocumentId(2),
                    new DocumentId(10), new DocumentId(11), new DocumentId(12), new DocumentId(13),
                }
            },
            {
                "moon",
                Array.Empty<DocumentId>()
            },
        };

        public static DocumentId[] AllDocuments = new[]
        {
            new DocumentId(0), new DocumentId(1), new DocumentId(2),
            new DocumentId(10), new DocumentId(11), new DocumentId(12), new DocumentId(13),
        };

        public static void BuildIndex(IBuildableIndex<string> index, string[][] sentenceBlocks)
        {
            for (var blockId = 0; blockId < sentenceBlocks.Length; ++blockId)
            {
                var block = sentenceBlocks[blockId];
                for (var localId = 0; localId < block.Length; ++localId)
                {
                    var doc = block[localId].ToLower().Split();
                    var docId = new DocumentId((uint)(blockId * 10 + localId));
                    for (var position = 0; position < doc.Length; ++position)
                    {
                        index.IndexTerm(docId, doc[position], position);
                    }
                }
            }
        }
    }
}
