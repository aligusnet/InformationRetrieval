using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using DocumentStorage.Json;

namespace DocumentStorage
{
    public class DocumentCollectionMetadata
    {
        private const string IdPropertyName = "id";
        private const string TitlePropertyName = "title";

        private readonly IDictionary<DocumentId, DocumentMetadata> metadata;

        public DocumentMetadata this[DocumentId key]
        {
            get
            {
                return metadata[key];
            }
        }

        public int Count => metadata.Count;

        public DocumentCollectionMetadata() : this(new Dictionary<DocumentId, DocumentMetadata>())
        {
        }

        public DocumentCollectionMetadata(IDictionary<DocumentId, DocumentMetadata> metadata)
        {
            this.metadata = metadata;
        }

        public static DocumentCollectionMetadata Make<T>(IList<Document<T>> docs)
        {
            return new DocumentCollectionMetadata(docs.Select(d => d.Metadata).ToDictionary(p => p.Id));
        }

        public void Serialize(Stream stream)
        {
            var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

            writer.WriteStartObject();
            
            foreach (var kv in metadata)
            {
                writer.WriteStartObject(kv.Value.Id.ToString());
                writer.WriteNumber(IdPropertyName, kv.Value.Id.Id);
                writer.WriteString(TitlePropertyName, kv.Value.Title);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            writer.Flush();
        }

        public static DocumentCollectionMetadata Deserialize(Stream stream)
        {
            var reader = new Utf8JsonStreamReader(stream);

            var dict = ReadDictionary(ref reader);

            return new DocumentCollectionMetadata(dict);
        }

        private static IDictionary<DocumentId, DocumentMetadata> ReadDictionary(ref Utf8JsonStreamReader reader)
        {
            var dict = new Dictionary<DocumentId, DocumentMetadata>();

            ReadToken(ref reader, JsonTokenType.StartObject);

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var position = reader.Position;
                var key = reader.GetString();
                var prop = ReadDocumentProperties(ref reader);

                if (key != prop.Id.ToString())
                {
                    throw new InvalidDataException($"Incorrect ket at position {position}");
                }

                dict.Add(prop.Id, prop);
            }

            return dict;
        }

        private static DocumentMetadata ReadDocumentProperties(ref Utf8JsonStreamReader reader)
        {
            ReadToken(ref reader, JsonTokenType.StartObject);

            uint id = 0;
            var title = string.Empty;

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                switch(propertyName)
                {
                    case IdPropertyName:
                        id = ReadUInt32(ref reader);
                        break;
                    case TitlePropertyName:
                        title = ReadString(ref reader);
                        break;
                    default:
                        throw new InvalidDataException($"Unexpected property at position {reader.Position}: {propertyName}");

                }
            }

            return new DocumentMetadata(new DocumentId(id), title);
        }

        private static void Read(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read())
            {
                throw new InvalidDataException("Unexpected end of stream");
            }
        }

        private static void ReadToken(ref Utf8JsonStreamReader reader, JsonTokenType token)
        {
            Read(ref reader);

            if (reader.TokenType != token)
            {
                throw new InvalidDataException($"Unexpected token at position {reader.Position}: expected {token} but got {reader.TokenType}");
            }
        }

        private static uint ReadUInt32(ref Utf8JsonStreamReader reader)
        {
            ReadToken(ref reader, JsonTokenType.Number);
            return reader.GetUInt32();
        }

        private static string ReadString(ref Utf8JsonStreamReader reader)
        {
            ReadToken(ref reader, JsonTokenType.String);
            return reader.GetString();
        }
    }
}
