using System.Collections.Generic;
using System.Linq;

namespace NaturalLanguageTools.Indexing
{
    public abstract class BooleanQuery<T>
    {
        public static BooleanQuery<T> CreateTerm(T word)
        {
            return new BooleanQueryTerm<T>(word);
        }

        public static BooleanQuery<T> CreateAnd(params T[] words)
        {
            return new BooleanQueryOperationAnd<T>(words.Select(CreateTerm).ToList());
        }
    }

    public class BooleanQueryTerm<T> : BooleanQuery<T>
    {
        public T Word { get; }

        public BooleanQueryTerm(T word)
        {
            Word = word;
        }
    }

    public class BooleanQueryOperationAnd<T> : BooleanQuery<T>
    {
        public IList<BooleanQuery<T>> Elements { get; }

        public BooleanQueryOperationAnd(IList<BooleanQuery<T>> elements)
        {
            Elements = elements;
        }
    }
}
