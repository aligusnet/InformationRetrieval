using System;
using System.Linq;
using Xunit;

using NaturalLanguageTools.BooleanSearch;


namespace NaturalLanguageToolsUnitTests.BooleanSearch
{
    public class BooleanQueryLanguageUnitTests
    {
        [Theory]
        [InlineData("hello1191", "hello1191")]
        public void TermIsASequenceOfAlphaNumericCharacters(string input, string word)
        {
            var term = BooleanQueryLanguage.ParseQuery(input);
            AssertTerm(term, word);
        }

        [Theory]
        [InlineData("(NOT earth)", "earth")]
        public void OperatorNotTakesExactlyOneArgument(string input, string word)
        {
            var query = BooleanQueryLanguage.ParseQuery(input);
            AssertNot(query, word);
        }

        [Theory]
        [InlineData("(NOT moon earth)")]
        public void OperatorNotThrowsExceptionInCaseOfInvalidNumberOfArguments(string input)
        {
            Assert.Throws<InvalidOperationException>(() => BooleanQueryLanguage.ParseQuery(input));
        }

        [Theory]
        [InlineData("(AND hi)", "hi")]
        [InlineData("(AND hello world)", "hello world")]
        [InlineData(" ( AND karpov chess world ) ", "karpov chess world")]
        public void OperatorAndTakesAtLeastOneArgument(string input, string expected)
        {
            var query = BooleanQueryLanguage.ParseQuery(input);
            AssertAnd(query, expected.Split());
        }

        [Theory]
        [InlineData("(OR \t hi)", "hi")]
        [InlineData(" ( OR hello world)", "hello world")]
        public void OperatorOrTakesAtLeastOneArgument(string input, string expected)
        {
            var query = BooleanQueryLanguage.ParseQuery(input);
            AssertOr(query, expected.Split());
        }

        [Fact]
        public void NestedQueryTest()
        {
            var input = " ( AND  (\tAND mir da) (NOT me) (OR tra (AND b c)))";
            var query = (BooleanQueryOperationAnd)BooleanQueryLanguage.ParseQuery(input);
            var orQuery = (BooleanQueryOperationOr)query.Elements[2];

            AssertAnd(query.Elements[0], "mir da".Split());
            AssertNot(query.Elements[1], "me");
            AssertTerm(orQuery.Elements[0], "tra");
            AssertAnd(orQuery.Elements[1], "b c".Split());
        }

        private void AssertTerm(BooleanQuery query, string word)
        {
            Assert.IsType<BooleanQueryTerm>(query);
            Assert.Equal(word, ((BooleanQueryTerm)query).Word);
        }

        private void AssertNot(BooleanQuery query, string word)
        {
            Assert.IsType<BooleanQueryOperationNot>(query);
            AssertTerm(((BooleanQueryOperationNot)query).Element, word);
        }

        private void AssertAnd(BooleanQuery query, params string[] words)
        {
            Assert.IsType<BooleanQueryOperationAnd>(query);

            var actual = ((BooleanQueryOperationAnd)query).Elements.Cast<BooleanQueryTerm>().Select(q => q.Word).ToArray();
            Assert.Equal(words, actual);
        }

        private void AssertOr(BooleanQuery query, params string[] words)
        {
            Assert.IsType<BooleanQueryOperationOr>(query);

            var actual = ((BooleanQueryOperationOr)query).Elements.Cast<BooleanQueryTerm>().Select(q => q.Word).ToArray();
            Assert.Equal(words, actual);
        }
    }
}
