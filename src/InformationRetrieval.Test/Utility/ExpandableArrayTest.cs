using Xunit;
using InformationRetrieval.Utility;

namespace InformationRetrieval.Test.Utility
{
    public class ExpandableArrayTest
    {
        [Fact]
        public void ExpandableArraySmokeTest()
        {
            var array = new ExpandableArray<int> { 1, 5, 10, 11 };

            Assert.Equal(4, array.Count);
            Assert.Equal(new[] { 1, 5, 10, 11 }, array);

            array[3] += 11;

            Assert.Equal(22, array.Buffer[3]);

            array.Add(new int[] { 2, 3, 5, 7, 11, 13, 17, 19 });

            Assert.Equal(12, array.Count);
            Assert.Equal(new[] { 1, 5, 10, 22, 2, 3, 5, 7, 11, 13, 17, 19 }, array);
        }
    }
}
