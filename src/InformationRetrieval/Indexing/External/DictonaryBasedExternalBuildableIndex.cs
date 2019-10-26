using System;
using System.IO;
using Corpus;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Indexing.External
{
    public class DictonaryBasedExternalBuildableIndex<T> : IExternalBuildableIndex<T> where T : notnull
    {
        public static Func<Stream, IExternalBuildableIndex<T>> GetCreateMethodWithMixedPostingsLists(int rangeThreshold)
            => s => new DictonaryBasedExternalBuildableIndex<T>(new MixedPostingsListBuilder<T>(rangeThreshold), s);

        private readonly Stream postingsStream;
        private readonly IPostingsListBuilder<T> builder;

        public DictonaryBasedExternalBuildableIndex(IPostingsListBuilder<T> postingListBuilder, Stream postingsStream)
        {
            builder = postingListBuilder;
            this.postingsStream = postingsStream;
        }

        public ISearchableIndex<T> Build() 
            => BuildExternalIndex();

        public ExternalIndex<T> BuildExternalIndex()
        {
            var composer = new ExternalIndexComposer<T>(postingsStream);

            composer.AddAllDocuments(builder.Documents);

            foreach (var postings in builder.PostingsLists)
            {
                composer.AddPostingsList(postings.Key, postings.Value);
            }

            return composer.Compose();
        }

        public void IndexTerm(DocumentId id, T term, int position) 
            => builder.AddTerm(id, term);
    }
}
