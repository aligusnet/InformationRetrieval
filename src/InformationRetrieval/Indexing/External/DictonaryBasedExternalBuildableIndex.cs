using System;
using System.IO;
using Corpus;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Indexing.External
{
    public class DictonaryBasedExternalBuildableIndex<T> : IExternalBuildableIndex<T> where T : notnull
    {
        public static Func<Stream, IExternalBuildableIndex<T>> GetCreateMethod(int rangeThreshold) 
            => s => new DictonaryBasedExternalBuildableIndex<T>(rangeThreshold, s);

        private readonly Stream postingsStream;
        private readonly MixedPostingsListBuilder<T> builder;

        public DictonaryBasedExternalBuildableIndex(int rangeThreshold, Stream postingsStream)
        {
            builder = new MixedPostingsListBuilder<T>(rangeThreshold);
            this.postingsStream = postingsStream;
        }

        public ISearchableIndex<T> Build() 
            => BuildExternalIndex();

        public ExternalIndex<T> BuildExternalIndex()
        {
            var composer = new ExternalIndexComposer<T>(postingsStream);

            composer.AddAllDocuments(builder.AllDocuments);

            foreach (var postings in builder.RangedPostingsLists)
            {
                composer.AddPostingsList(postings.Key, postings.Value);
            }

            foreach (var postings in builder.UncompressedPostingsLists)
            {
                composer.AddPostingsList(postings.Key, postings.Value);
            }

            return composer.Compose();
        }

        public void IndexTerm(DocumentId id, T term, int position) 
            => builder.Add(id, term);
    }
}
