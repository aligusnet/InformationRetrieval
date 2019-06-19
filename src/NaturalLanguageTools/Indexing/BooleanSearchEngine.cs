using System;
using System.Collections.Generic;

using DocumentStorage;
using NaturalLanguageTools.Utility;

namespace NaturalLanguageTools.Indexing
{
    public class BooleanSearchEngine<T>
    {
        private readonly ISearchableIndex<T> index;

        public BooleanSearchEngine(ISearchableIndex<T> index)
        {
            this.index = index;
        }

        public IEnumerable<DocumentId> ExecuteQuery(BooleanQuery<T> query)
        {
            switch(query)
            {
                case BooleanQueryTerm<T> term:
                    return index.Search(term.Word);
                case BooleanQueryOperationAnd<T> opAnd:
                    return ExecuteAnd(opAnd);
                case BooleanQueryOperationOr<T> opOr:
                    return ExecuteOr(opOr);
                default:
                    throw new InvalidOperationException($"Got unexpected Boolean query: {query?.GetType()}");
            }
        }

        private IEnumerable<DocumentId> ExecuteAnd(BooleanQueryOperationAnd<T> opAnd)
        {
            var minHeapComparer = Comparer<IEnumerator<DocumentId>>.Create((x, y) 
                => y.Current.CompareTo(x.Current));

            var queue = new PriorityQueue<IEnumerator<DocumentId>>(
                opAnd.Elements.Count,
                minHeapComparer);

            foreach (var q in opAnd.Elements)
            {
                var enumerator = ExecuteQuery(q).GetEnumerator();
                
                if (enumerator.MoveNext())
                {
                    queue.Push(enumerator);
                }
                else
                {
                    yield break;
                }
            }

            DocumentId prev = new DocumentId(0);
            int counter = 0;

            while (queue.Count > 0)
            {
                var enumerator = queue.Pop();

                if (prev.CompareTo(enumerator.Current) == 0)
                {
                    counter++;
                    if (counter == opAnd.Elements.Count)
                    {
                        yield return prev;
                    }
                }
                else if (queue.Count < opAnd.Elements.Count - 1)
                {
                    yield break;
                }
                else
                {
                    prev = enumerator.Current;
                    counter = 1;
                }

                if (enumerator.MoveNext())
                {
                    queue.Push(enumerator);
                }
            }
        }

        private IEnumerable<DocumentId> ExecuteOr(BooleanQueryOperationOr<T> opOr)
        {
            var minHeapComparer = Comparer<IEnumerator<DocumentId>>.Create((x, y)
                => y.Current.CompareTo(x.Current));

            var queue = new PriorityQueue<IEnumerator<DocumentId>>(
                opOr.Elements.Count,
                minHeapComparer);

            foreach (var q in opOr.Elements)
            {
                var enumerator = ExecuteQuery(q).GetEnumerator();

                if (enumerator.MoveNext())
                {
                    queue.Push(enumerator);
                }
            }

            DocumentId? prev = null;

            while (queue.Count > 0)
            {
                var enumerator = queue.Pop();

                DocumentId current = enumerator.Current;

                if (enumerator.MoveNext())
                {
                    queue.Push(enumerator);
                }

                if ((prev?.CompareTo(current) ?? -1) != 0)
                {
                    yield return current;
                }

                prev = current;
            }
        }
    }
}
