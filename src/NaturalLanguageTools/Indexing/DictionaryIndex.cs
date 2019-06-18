using System;
using System.Collections.Generic;
using ProtoBuf;
using DocumentStorage;
using System.IO;
using System.IO.Compression;

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

            return 0;
        }

        public int GetCount()
        {
            return allDocuments.DocumentsCount;
        }
    }
}
