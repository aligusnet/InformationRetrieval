using System.IO;

namespace Corpus
{
    public interface IDocumentDataSerializer<T>
    {
        void Serialize(Stream stream, T data);

        T Deserialize(Stream stream);

        string FileExtension { get; }
    }
}
