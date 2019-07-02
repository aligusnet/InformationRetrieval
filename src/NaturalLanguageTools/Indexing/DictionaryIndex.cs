using System;
using System.Collections.Generic;
using DocumentStorage;
using System.IO;
using System.IO.Compression;
using ProtoBuf;
using System.Linq;

namespace NaturalLanguageTools.Indexing
{
    [ProtoContract]
    public class DictionaryIndex<T> : IBuildableIndex<T>, ISearchableIndex<T>
    {
        [ProtoMember(1)]
        private readonly IDictionary<T, DocumentIdRangeCollectionList> wordIndex;

        [ProtoMember(2)]
        private readonly IDictionary<T, IList<uint>> rareWordIndex;

        [ProtoMember(3)]
        private readonly DocumentIdRangeCollectionList allDocuments;

        [ProtoMember(4)]
        private readonly int rareWordThreshold;

        private const int DefaultCapacity = 8;

        // for protobuf deserialization
        private DictionaryIndex() : this(0)
        {
        }

        public DictionaryIndex(int rareWordThreshold)
        {
            wordIndex = new Dictionary<T, DocumentIdRangeCollectionList>();
            rareWordIndex = new Dictionary<T, IList<uint>>();
            allDocuments = new DocumentIdRangeCollectionList();
            this.rareWordThreshold = rareWordThreshold;
        }

        public void IndexWord(DocumentId id, T word, int position)
        {
            if (wordIndex.TryGetValue(word, out var collectionList))
            {
                collectionList.Add(id);
            }
            else
            {
                if (!rareWordIndex.TryGetValue(word, out var ids))
                {
                    ids = new List<uint>(DefaultCapacity);
                    rareWordIndex.Add(word, ids);
                    ids.Add(id.Id);
                }
                else if (ids[^1] != id.Id)
                {
                    ids.Add(id.Id);
                }

                int count = ids.Count;
                if (count >= rareWordThreshold && id.Id - ids[count-rareWordThreshold] == (rareWordThreshold-1))
                {
                    MoveToIndex(word, ids);
                }
            }

            allDocuments.Add(id);
        }

        private void MoveToIndex(T word, IList<uint> ids)
        {
            rareWordIndex.Remove(word);

            var list = new DocumentIdRangeCollectionList();
            foreach (var id in ids)
            {
                list.Add(id);
            }

            wordIndex.Add(word, list);
        }

        public IEnumerable<DocumentId> Search(T word)
        {
            if (wordIndex.TryGetValue(word, out var collectionList))
            {
                return collectionList;
            }

            if (rareWordIndex.TryGetValue(word, out var ids))
            {
                return ids.Select(id => new DocumentId(id));
            }

            return Array.Empty<DocumentId>();
        }

        public IEnumerable<DocumentId> GetAll()
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

        public int GetCount(T word)
        {
            if (wordIndex.TryGetValue(word, out var collectionList))
            {
                return collectionList.DocumentsCount;
            }

            if (rareWordIndex.TryGetValue(word, out var ids))
            {
                return ids.Count;
            }

            return 0;
        }

        public int GetCount()
        {
            return allDocuments.DocumentsCount;
        }
    }
}
