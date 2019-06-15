using System;
using System.Collections.Generic;
using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    public class DictionaryIndex<T> : IBuildableIndex<T>
    {
        private readonly IDictionary<T, IList<DocumentIdRangeCollection>> wordIndex;

        public DictionaryIndex()
        {
            wordIndex = new Dictionary<T, IList<DocumentIdRangeCollection>>();
        }

        public void IndexWord(DocumentId id, T word, int position)
        {
            DocumentIdRangeCollection collection;
            if (wordIndex.TryGetValue(word, out var collectionList))
            {
                if (collectionList[^1].CollectionId == id.CollectionId)
                {
                    collection = collectionList[^1];
                }
                else
                {
                    collection = new DocumentIdRangeCollection(id.CollectionId);
                    collectionList.Add(collection);
                }
            }
            else
            {
                collectionList = new List<DocumentIdRangeCollection>();
                wordIndex.Add(word, collectionList);

                collection = new DocumentIdRangeCollection(id.CollectionId);
                collectionList.Add(collection);
            }

            collection.AppendDocument(id.LocalId);
        }

        public IEnumerable<DocumentId> this[T word]
        {
            get
            {
                if (wordIndex.TryGetValue(word, out var collectionList))
                {
                    foreach (var collection in collectionList)
                    {
                        foreach (var range in collection.Ranges)
                        {
                            for (ushort i = 0; i < range.Length; ++i)
                            {
                                yield return new DocumentId(collection.CollectionId, (ushort)(range.Start + i));
                            }
                        }
                    }
                }
            }
        }
    }

    internal struct DocumentIdRange
    {
        public ushort Start { get; }
        public int End => Start + Length;
        public ushort Length { get; set; }

        public DocumentIdRange(ushort localId)
        {
            Start = localId;
            Length = 1;
        }
    }

    internal class DocumentIdRangeCollection
    {
        public ushort CollectionId { get; }

        public IList<DocumentIdRange> Ranges { get; }

        public DocumentIdRangeCollection(ushort id)
        {
            CollectionId = id;
            Ranges = new List<DocumentIdRange>();
        }

        public void AppendDocument(ushort localId)
        {
            if (Ranges.Count == 0)
            {
                Ranges.Add(new DocumentIdRange(localId));
            }
            else
            {
                Index lastIndex = ^1;

                var lastRange = Ranges[lastIndex];
                if (lastRange.End == localId)
                {
                    lastRange.Length += 1;
                    Ranges[lastIndex] = lastRange;
                }
                else if (lastRange.End < localId)
                {
                    Ranges.Add(new DocumentIdRange(localId));
                }
            }
        }
    }
}
