using System.IO;

namespace DocumentStorage
{
    public interface IDocumentDataSerializer<T>
    {
        void Serialize(Stream stream, T data);

        T Deserialize(Stream stream);

        string FileExtension { get; }
    }
}
