using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using InformationRetrieval.Utility;
using H = InformationRetrieval.Utility.JsonReaderHelper;

namespace Corpus
{
    public class CorpusZipMetadata
    {
        private const string FirstDocumentIdInBlockPropertyName = "FirstDocumentIdInBlock";

        public IReadOnlyList<DocumentId> FirstDocumentIdInBlock { get; }

        public CorpusZipMetadata(IReadOnlyList<DocumentId> firstDocumentIdInBlock)
        {
            FirstDocumentIdInBlock = firstDocumentIdInBlock;
        }

        public void Serialize(Stream stream)
        {
            var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

            writer.WriteStartObject();
            writer.WritePropertyName(FirstDocumentIdInBlockPropertyName);

            writer.WriteStartArray();
            foreach (var docId in FirstDocumentIdInBlock)
            {
                writer.WriteNumberValue(docId.Id);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();

            writer.Flush();
        }

        public int GetBlockId(DocumentId docId)
        {
            int blockId;
            for (blockId = 1; blockId < FirstDocumentIdInBlock.Count;  ++blockId)
            {
                if (FirstDocumentIdInBlock[blockId].CompareTo(docId) > 0)
                {
                    break;
                }
            }
            return blockId - 1;
        }

        public static CorpusZipMetadata Deserialize(Stream stream)
        {
            var reader = new Utf8JsonStreamReader(stream);

            return ReadCorpusMetadata(ref reader);
        }

        private static CorpusZipMetadata ReadCorpusMetadata(ref Utf8JsonStreamReader reader)
        {
            IReadOnlyList<DocumentId> firstDocumentIdInBlock = Array.Empty<DocumentId>();

            H.ReadToken(ref reader, JsonTokenType.StartObject);
            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                switch (propertyName)
                {
                    case FirstDocumentIdInBlockPropertyName:
                        firstDocumentIdInBlock = ReadFirstDocumentIdInBlock(ref reader);
                        break;
                    default:
                        throw new InvalidDataException($"Unexpected property at position {reader.Position}: {propertyName}");
                }
            }

            if (firstDocumentIdInBlock.Count == 0)
            {
                throw new InvalidDataException("Mandatory field ReadFirstDocumentIdInBlock was not found");
            }

            return new CorpusZipMetadata(firstDocumentIdInBlock);
        }

        private static IReadOnlyList<DocumentId> ReadFirstDocumentIdInBlock(ref Utf8JsonStreamReader reader)
        {
            var firstDocumentIdInBlock = new List<DocumentId>();

            H.ReadToken(ref reader, JsonTokenType.StartArray);
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                firstDocumentIdInBlock.Add(new DocumentId(reader.GetUInt32()));
            }

            return firstDocumentIdInBlock;
        }
    }
}
