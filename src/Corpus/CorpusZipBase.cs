using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Corpus
{
    public abstract class CorpusZipBase
    {
        protected const string METADATA_ENTRY_NAME = "_METADATA_.json";

        private readonly string path;

        protected CorpusZipBase(string path, IFileSystem fileSystem)
        {
            this.path = path;
            this.FileSystem = fileSystem;
        }

        protected IFileSystem FileSystem { get; }

        protected string GetBlockPath(ushort blockId)
        {
            return Path.Combine(path, $"{BlockMetadata.IdString(blockId)}.zip");
        }

        protected IEnumerable<string> GetBlocksPaths()
        {
            return FileSystem.Directory.GetFiles(path, "*.zip")
                     .OrderBy(fn => BlockMetadata.ParseId(Path.GetFileNameWithoutExtension(fn)));
        }
    }
}
