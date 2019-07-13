using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Corpus;
using InformationRetrieval.Utility;

namespace InformationRetrieval.Indexing.External
{
    /// <summary>
    /// Buildable ExternalIndex that able to index the corpus block by block
    /// and then merge all blocks' indices into one external index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlockedExternalBuildableIndex<T> : IBuildableIndex<T>, IDisposable
        where T : IComparable<T>
    {
        private readonly BuildableIndexManager indexManager;
        private readonly Stream stream;

        public BlockedExternalBuildableIndex(Func<Stream, IExternalBuildableIndex<T>> createIndex, string basePath) : 
            this(createIndex, basePath, new FileSystem()) { }

        public BlockedExternalBuildableIndex(Func<Stream, IExternalBuildableIndex<T>> createIndex, string basePath, IFileSystem fileSystem)
        {
            this.indexManager = new BuildableIndexManager(createIndex, basePath, fileSystem);
            string indexPath = Path.Combine(basePath, ExternalIndexSerializer<T>.IndexFileName);
            this.stream = fileSystem.File.Open(indexPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void IndexTerm(DocumentId id, T term, int position)
        {
            var index = indexManager.Get(id.BlockId);
            index.IndexTerm(id, term, position);
        }

        public ExternalIndex<T> Build()
        {
            var composer = new ExternalIndexComposer<T>(stream);

            var indexInfoList = indexManager.GetIndices();
            var indices = indexManager.GetIndices().Where(i => i.GetCount() > 0).ToArray();

            AddAllDocs(composer, indices);

            var minHeapComparer = Comparer<IEnumerator<(T Term, IReadOnlyCollection<DocumentId> PostingsList)>>.Create(
                (x, y) => ComparePostingLists(y.Current, x.Current));

            var queue = new PriorityQueue<IEnumerator<(T Term, IReadOnlyCollection<DocumentId> PostingsList)>>(
                indices.Length,
                minHeapComparer);

            foreach (var index in indices)
            {
                var enumerator = ReadIndex(index);
                if (enumerator.MoveNext())
                {
                    queue.Push(enumerator);
                }
            }

            var docs = new ListChain<DocumentId>(indices.Length);

            T currentTerm = default!;

            while (queue.Count > 0)
            {
                var e = queue.Pop();

                if (e.Current.Term.CompareTo(currentTerm) != 0)
                {
                    if (currentTerm != default && docs.Count > 0)
                    {
                        composer.AddPostingsList(currentTerm, docs);
                    }

                    docs.Clear();
                    currentTerm = e.Current.Term;
                }

                docs.Add(e.Current.PostingsList);

                if (e.MoveNext())
                {
                    queue.Push(e);
                }
            }

            composer.AddPostingsList(currentTerm, docs);

            indexManager.Clear();
            return composer.Compose();
        }

        private void AddAllDocs(ExternalIndexComposer<T> composer, ExternalIndex<T>[] indices)
        {
            var allDocs = new ListChain<DocumentId>(indices.Length);

            foreach (var index in indices)
            {
                allDocs.Add(index.GetAll().ToList());
            }

            composer.AddAllDocuments(allDocs);
        }

        private static int ComparePostingLists((T Term, IReadOnlyCollection<DocumentId> PostingsList) lhs, (T Term, IReadOnlyCollection<DocumentId> PostingsList) rhs)
        {
            int cmpTerms = lhs.Term.CompareTo(rhs.Term);
            if (cmpTerms != 0)
            {
                return cmpTerms;
            }
            else
            {
                return lhs.PostingsList.First().CompareTo(rhs.PostingsList.First());
            }
        }

        private IEnumerator<(T Term, IReadOnlyCollection<DocumentId> PostingsList)> ReadIndex(ExternalIndex<T> index)
        {
            foreach (var pair in index.Offsets.OrderBy(p => p.Key))
            {
                yield return (pair.Key, index.Search(pair.Key));
            }
        }

        public void Dispose()
        {
            indexManager.Dispose();
        }

        ISearchableIndex<T> IBuildableIndex<T>.Build()
        {
            return Build();
        }

        private struct IndexInfo
        {
            public string Path;
            public IDictionary<T, long> Offsets;

            public IndexInfo(string path, IDictionary<T, long> offsets)
            {
                Path = path;
                Offsets = offsets;
            }
        }

        private class BuildableIndexManager : IDisposable
        {
            private readonly string basePath;
            private readonly IFileSystem fileSystem;
            private IExternalBuildableIndex<T>? currentIndex;
            private ushort currentBlockId;
            private readonly IList<ExternalIndex<T>> indices;
            private readonly Func<Stream, IExternalBuildableIndex<T>> createIndex;

            public BuildableIndexManager(Func<Stream, IExternalBuildableIndex<T>> createIndex, string basePath, IFileSystem fileSystem)
            {
                var dirName = $"run_{DateTime.Now:yyyy-MM-dd_HHmmss.FFFFFFF}";
                this.basePath = Path.Combine(basePath, dirName);
                this.fileSystem = fileSystem;
                this.indices = new List<ExternalIndex<T>>();
                this.currentBlockId = 0;
                this.createIndex = createIndex;
            }

            public IBuildableIndex<T> Get(ushort blockId)
            {
                if (currentIndex == null)
                {
                    currentIndex = createIndex(OpenIndexStrem(blockId));
                    currentBlockId = blockId;
                }
                else if (currentBlockId != blockId)
                {
                    Build();
                    currentIndex = createIndex(OpenIndexStrem(blockId));
                    currentBlockId = blockId;
                }

                return currentIndex;
            }

            public IList<ExternalIndex<T>> GetIndices()
            {
                Build();
                return indices;
            }

            public void Clear()
            {
                foreach (var index in indices)
                {
                    index.Dispose();
                }
                indices.Clear();

                fileSystem.Directory.Delete(basePath, recursive: true);
            }

            public void Dispose()
            {
                Clear();
            }

            private void Build()
            {
                if (currentIndex != null)
                {
                    var externalIndex = currentIndex.BuildExternalIndex();
                    indices.Add(externalIndex);
                    currentIndex = null;
                }
            }

            private Stream OpenIndexStrem(ushort blockId)
            {
                if (!fileSystem.Directory.Exists(basePath))
                {
                    fileSystem.Directory.CreateDirectory(basePath);
                }

                string path = BuildIndexPath(blockId);
                return fileSystem.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }

            private string BuildIndexPath(ushort blockId) =>
                Path.Combine(basePath, $"block-{blockId:00000}.index");
        }
    }
}
