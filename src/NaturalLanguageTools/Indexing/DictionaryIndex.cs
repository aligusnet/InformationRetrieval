using System;
using System.Collections.Generic;
using ProtoBuf;
using DocumentStorage;
using System.IO;
using System.IO.Compression;
using System.Collections;

namespace NaturalLanguageTools.Indexing
{
    [ProtoContract]
    public class DictionaryIndex<T> : IBuildableIndex<T>, ISearchableIndex<T>
    {
        [ProtoMember(1)]
        private readonly IDictionary<T, DocumentIdRangeCollectionList> wordIndex;

        [ProtoMember(2)]
        private readonly DocumentIdRangeCollectionList allDocuments;

        public DictionaryIndex()
        {
            wordIndex = new Dictionary<T, DocumentIdRangeCollectionList>();
            allDocuments = new DocumentIdRangeCollectionList();
        }

        public void IndexWord(DocumentId id, T word, int position)
        {
            if (!wordIndex.TryGetValue(word, out var collectionList))
            {
                collectionList = new DocumentIdRangeCollectionList();
                wordIndex.Add(word, collectionList);
            }

            collectionList.Add(id);
            allDocuments.Add(id);
        }

        public IEnumerable<DocumentId> Search(T word)
        {
            if (wordIndex.TryGetValue(word, out var collectionList))
            {
                return collectionList;
            }

            return Array.Empty<DocumentId>();
        }

        public IEnumerable<DocumentId> AllDocuments()
        {
            return allDocuments;
        }

        public void Serialize(Stream stream)
        {
            using var gzipStream = new GZipStream(stream, CompressionLevel.Optimal, leaveOpen: true);
            Serializer.Serialize(gzipStream, this);
        }

        public static DictionaryIndex<T> Deserialize(Stream stream)
        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
            return Serializer.Deserialize<DictionaryIndex<T>>(gzipStream);
        }
    }

    [ProtoContract]
    internal struct DocumentIdRange
    {
        [ProtoMember(1)]
        public ushort Start { get; }
        public int End => Start + Length;
        [ProtoMember(2)]
        public ushort Length { get; set; }

        public DocumentIdRange(ushort localId)
        {
            Start = localId;
            Length = 1;
        }
    }

    [ProtoContract]
    internal class DocumentIdRangeCollection
    {
        [ProtoMember(1)]
        public ushort CollectionId { get; }

        [ProtoMember(2)]
        public IList<DocumentIdRange> Ranges { get; }

        // for protobuf deserialization
        private DocumentIdRangeCollection() : this(0)
        {
        }

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

    [ProtoContract]
    internal class DocumentIdRangeCollectionList : IEnumerable<DocumentId>
    {
        [ProtoMember(1)]
        private readonly IList<DocumentIdRangeCollection> list = new List<DocumentIdRangeCollection>();

        public void Add(DocumentId id)
        {
            DocumentIdRangeCollection collection;

            if (list.Count == 0 || list[^1].CollectionId != id.CollectionId)
            {
                collection = new DocumentIdRangeCollection(id.CollectionId);
                list.Add(collection);
            }
            else
            {
                collection = list[^1];
            }

            collection.AppendDocument(id.LocalId);
        }

        public IEnumerator<DocumentId> GetEnumerator()
        {
            foreach (var collection in list)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
