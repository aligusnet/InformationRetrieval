using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using InformationRetrieval.Utility;

namespace Corpus
{
    public class BlockMetadata
    {
        private const string BlockIdPropertyName = "id";
        private const string DocumentMetadataListPropertyName = "documents";
        private const string DocumentIdPropertyName = "id";
        private const string TitlePropertyName = "title";

        private readonly IList<DocumentMetadata> metadata;

        public static ushort ParseId(ReadOnlySpan<char> hex) => NumberParser.ParseUInt16(hex, 16);

        public static string IdString(ushort blockId) => string.Format($"{blockId:X4}");

        public ushort Id { get; }

        public string IdString() => IdString(Id);

        public DocumentMetadata this[DocumentId key]
        {
            get
            {
                return metadata[key.LocalId];
            }
        }

        public int Count => metadata.Count;

        public BlockMetadata(ushort id, IList<DocumentMetadata> metadata)
        {
            this.Id = id;
            this.metadata = metadata;
        }

        public static BlockMetadata Make<T>(ushort id, IList<Document<T>> docs)
        {
            return new BlockMetadata(id, docs.Select(d => d.Metadata).ToArray());
        }

        public void Serialize(Stream stream)
        {
            var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

            writer.WriteStartObject();

            writer.WriteString(BlockIdPropertyName, IdString());

            writer.WriteStartArray(DocumentMetadataListPropertyName);
            foreach (var dm in metadata)
            {
                writer.WriteStartObject();
                writer.WriteString(DocumentIdPropertyName, dm.Id.ToString());
                writer.WriteString(TitlePropertyName, dm.Title);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            
            writer.WriteEndObject();

            writer.Flush();
        }

        public static BlockMetadata Deserialize(Stream stream)
        {
            var reader = new Utf8JsonStreamReader(stream);

            return ReadBlockMetadata(ref reader);
        }

        private static BlockMetadata ReadBlockMetadata(ref Utf8JsonStreamReader reader)
        {
            ushort id = 0;
            IList<DocumentMetadata> docs = Array.Empty<DocumentMetadata>();

            JsonReaderHelper.ReadToken(ref reader, JsonTokenType.StartObject);

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();
                switch (propertyName)
                {
                    case BlockIdPropertyName:
                        id = ParseId(JsonReaderHelper.ReadString(ref reader).AsSpan());
                        break;
                    case DocumentMetadataListPropertyName:
                        docs = ReadDocumentsMetadata(ref reader);
                        break;
                    default:
                        throw new InvalidDataException($"Unexpected property at position {reader.Position}: {propertyName}");

                }
            }

            return new BlockMetadata(id, docs);
        }

        private static IList<DocumentMetadata> ReadDocumentsMetadata(ref Utf8JsonStreamReader reader)
        {
            var docs = new List<DocumentMetadata>();

            JsonReaderHelper.ReadToken(ref reader, JsonTokenType.StartArray);
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var doc = ReadDocumentMetadata(ref reader);
                docs.Add(doc);
            }

            return docs;
        }

        private static DocumentMetadata ReadDocumentMetadata(ref Utf8JsonStreamReader reader)
        {
            var id = DocumentId.Zero;
            var title = string.Empty;

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                switch(propertyName)
                {
                    case DocumentIdPropertyName:
                        id = DocumentId.Parse(JsonReaderHelper.ReadString(ref reader).AsSpan());
                        break;
                    case TitlePropertyName:
                        title = JsonReaderHelper.ReadString(ref reader);
                        break;
                    default:
                        throw new InvalidDataException($"Unexpected property at position {reader.Position}: {propertyName}");

                }
            }

            return new DocumentMetadata(id, title);
        }
    }
}
