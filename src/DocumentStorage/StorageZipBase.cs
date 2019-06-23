using System.IO;
using System.IO.Abstractions;

namespace DocumentStorage
{
    public abstract class StorageZipBase
    {
        protected const string METADATA_ENTRY_NAME = "_METADATA_.json";

        private readonly string path;

        protected StorageZipBase(string path, IFileSystem fileSystem)
        {
            this.path = path;
            this.FileSystem = fileSystem;
        }

        protected IFileSystem FileSystem { get; }

        protected string GetCollectionPath(ushort collectionId)
        {
            return Path.Combine(path, $"{DocumentCollectionMetadata.IdString(collectionId)}.zip");
        }

        protected string[] GetCollectionsPaths()
        {
            return FileSystem.Directory.GetFiles(path, "*.zip");
        }
    }
}
