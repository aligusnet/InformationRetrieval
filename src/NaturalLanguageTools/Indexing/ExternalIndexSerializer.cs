using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

using ProtoBuf;

namespace NaturalLanguageTools.Indexing
{
    public class ExternalIndexSerializer<T>
    {
        public const string IndexFileName = "corpus.index";
        public const string OffsetsFileName = "corpus.offsets";

        private readonly FileSystem fileSystem;

        public ExternalIndexSerializer() : this(new FileSystem()) { }

        public ExternalIndexSerializer(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ExternalIndex<T> Deserialize(string basePath)
        {
            var postingStream = fileSystem.File.OpenRead(Path.Combine(basePath, IndexFileName));
            using var offsetsStream = fileSystem.File.OpenRead(Path.Combine(basePath, OffsetsFileName));
            var offsets = DeserializeOffsets(offsetsStream);
            return new ExternalIndex<T>(offsets, postingStream);
        }

        public void Serialize(string basePath, ExternalIndex<T> index)
        {
            using var offsetsStream = fileSystem.File.OpenWrite(Path.Combine(basePath, OffsetsFileName));
            SerializeOffsets(offsetsStream, index.Offsets);
        }

        public static void SerializeOffsets(Stream stream, IDictionary<T, long> offsets)
        {
            Serializer.Serialize(stream, offsets);
        }

        public static IDictionary<T, long> DeserializeOffsets(Stream stream)
        {
            return Serializer.Deserialize<Dictionary<T, long>>(stream);
        }
    }
}
