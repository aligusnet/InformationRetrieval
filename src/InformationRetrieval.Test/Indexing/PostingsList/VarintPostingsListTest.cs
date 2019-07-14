using System.IO;
using System.Linq;

using Xunit;

using Corpus;
using InformationRetrieval.Indexing.PostingsList;
using InformationRetrieval.Utility;


namespace InformationRetrieval.Test.Indexing.PostingsList
{
    public class VarintPostingsListTest
    {
        [Fact]
        public void VarintPostingsListSmokeTest()
        {
            var postingsList = new VarintPostingsList()
            {
                new DocumentId(1),
                new DocumentId(2),
                new DocumentId(11),
                new DocumentId(15),
                new DocumentId(119),
            };

            Assert.Equal(PostingsListTest.GetDocIds(1, 2, 11, 15, 119), postingsList.ToArray());
        }
    }
}
