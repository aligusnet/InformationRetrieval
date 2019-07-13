using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using DawgSharp;
using ProtoBuf;

using Corpus;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Indexing.InMemory
{
    public class DawgSearchableIndex : ISearchableIndex<string>
    {
        private readonly Dawg<RangePostingsList> dawg;
        private readonly RangePostingsList allDocuments;

        public DawgSearchableIndex(Dawg<RangePostingsList> dawg, RangePostingsList allDocuments)
        {
            this.dawg = dawg;
            this.allDocuments = allDocuments;
        }

        public IEnumerable<DocumentId> GetAll()
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

        private static void SerializePayload(BinaryWriter writer, RangePostingsList payload)
        {
            Serializer.SerializeWithLengthPrefix(writer.BaseStream, payload, PrefixStyle.Base128);
        }

        public static DawgSearchableIndex Deserialize(Stream stream)
        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
            var allDocuments = Serializer.DeserializeWithLengthPrefix<RangePostingsList>(gzipStream, PrefixStyle.Base128);
            var dawg = Dawg<RangePostingsList>.Load(gzipStream, readPayload: DeserializePayload);
            return new DawgSearchableIndex(dawg, allDocuments);
        }

        private static RangePostingsList DeserializePayload(BinaryReader reader)
        {
            return Serializer.DeserializeWithLengthPrefix<RangePostingsList>(reader.BaseStream, PrefixStyle.Base128);
        }

        public int GetCount(string word)
        {
            return dawg[word]?.Count ?? 0;
        }

        public int GetCount()
        {
            return allDocuments.Count;
        }
    }
}
