using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProtoBuf;

using DawgSharp;
using DocumentStorage;
using System.IO.Compression;

namespace NaturalLanguageTools.Indexing
{
    public class DawgSearchableIndex : ISearchableIndex<string>
    {
        private readonly Dawg<DocumentIdRangeCollectionList> dawg;
        private readonly DocumentIdRangeCollectionList allDocuments;

        public DawgSearchableIndex(Dawg<DocumentIdRangeCollectionList> dawg, DocumentIdRangeCollectionList allDocuments)
        {
            this.dawg = dawg;
            this.allDocuments = allDocuments;
        }

        public IEnumerable<DocumentId> AllDocuments()
        {
            return allDocuments;
        }

        public IEnumerable<DocumentId> Search(string word)
        {
            return dawg[word] ?? Enumerable.Empty<DocumentId>();
        }

        public void Serialize(Stream stream)
        {
            using var gzipStream = new GZipStream(stream, CompressionLevel.Optimal, leaveOpen: true);
            Serializer.SerializeWithLengthPrefix(gzipStream, allDocuments, PrefixStyle.Base128);
            dawg.SaveTo(gzipStream, writePayload: SerializePayload);
        }

        private static void SerializePayload(BinaryWriter writer, DocumentIdRangeCollectionList payload)
        {
            Serializer.SerializeWithLengthPrefix(writer.BaseStream, payload, PrefixStyle.Base128);
        }

        public static DawgSearchableIndex Deserialize(Stream stream)
        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
            var allDocuments = Serializer.DeserializeWithLengthPrefix<DocumentIdRangeCollectionList>(gzipStream, PrefixStyle.Base128);
            var dawg = Dawg<DocumentIdRangeCollectionList>.Load(gzipStream, readPayload: DeserializePayload);
            return new DawgSearchableIndex(dawg, allDocuments);
        }

        private static DocumentIdRangeCollectionList DeserializePayload(BinaryReader reader)
        {
            return Serializer.DeserializeWithLengthPrefix<DocumentIdRangeCollectionList>(reader.BaseStream, PrefixStyle.Base128);
        }
    }
}
