using System.Collections.Generic;
using System.Linq;

namespace InformationRetrieval.BooleanSearch
{
    public abstract class BooleanQuery
    {
        public static BooleanQuery CreateTerm(string word)
        {
            return new BooleanQueryTerm(word);
        }

        public static BooleanQuery CreateAnd(params string[] words)
        {
            return new BooleanQueryOperationAnd(words.Select(CreateTerm).ToList());
        }

        public static BooleanQuery CreateAnd(params BooleanQuery[] elements)
        {
            return new BooleanQueryOperationAnd(elements);
        }

        public static BooleanQuery CreateOr(params string[] words)
        {
            return new BooleanQueryOperationOr(words.Select(CreateTerm).ToList());
        }

        public static BooleanQuery CreateOr(params BooleanQuery[] elements)
        {
            return new BooleanQueryOperationOr(elements);
        }

        public static BooleanQuery CreateNot(BooleanQuery element)
        {
            return new BooleanQueryOperationNot(element);
        }
    }

    public class BooleanQueryTerm : BooleanQuery
    {
        public string Word { get; }

        public BooleanQueryTerm(string word)
        {
            Word = word;
        }
    }

    public class BooleanQueryOperationAnd : BooleanQuery
    {
        public IList<BooleanQuery> Elements { get; }

        public BooleanQueryOperationAnd(IList<BooleanQuery> elements)
        {
            Elements = elements;
        }
    }

    public class BooleanQueryOperationOr : BooleanQuery
    {
        public IList<BooleanQuery> Elements { get; }

        public BooleanQueryOperationOr(IList<BooleanQuery> elements)
        {
            Elements = elements;
        }
    }

    public class BooleanQueryOperationNot : BooleanQuery
    {
        public BooleanQuery Element { get; }

        public BooleanQueryOperationNot(BooleanQuery element)
        {
            Element = element;
        }
    }
}
