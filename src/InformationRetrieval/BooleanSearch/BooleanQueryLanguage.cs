using System;
using System.Linq;
using Sprache;

namespace InformationRetrieval.BooleanSearch
{
    public static class BooleanQueryLanguage
    {
        public static BooleanQuery ParseQuery(string statement)
        {
            return Expression.Parse(statement);
        }

        private enum Operator
        {
            And,
            Or,
            Not
        };

        private static readonly Parser<BooleanQuery> Term =
            from word in Parse.LetterOrDigit.AtLeastOnce().Text().Token()
            select BooleanQuery.CreateTerm(word);

        private static readonly Parser<Operator> OperatorIndicator =
            Parse.String("AND").Return(Operator.And)
                 .Or(Parse.String("OR").Return(Operator.Or))
                 .Or(Parse.String("NOT").Return(Operator.Not));

        private static readonly Parser<BooleanQuery> Operation =
            from open in Parse.Char('(')
            from op in OperatorIndicator.Token()
            from operands in Parse.Ref(() => Expression).AtLeastOnce()
            from close in Parse.Char(')')
            select CreateOperator(op, operands.ToArray());

        private static readonly Parser<BooleanQuery> Expression = Operation.Or(Term).Token();

        private static BooleanQuery CreateOperator(Operator op, BooleanQuery[] operands)
        {
            switch (op)
            {
                case Operator.And:
                    return BooleanQuery.CreateAnd(operands);
                case Operator.Or:
                    return BooleanQuery.CreateOr(operands);
                case Operator.Not:
                    if (operands.Length != 1)
                    {
                        throw new InvalidOperationException($"Operator NOT expects exctly 1 argument {operands.Length} were given");
                    }
                    return BooleanQuery.CreateNot(operands.First());
                default:
                    throw new InvalidOperationException($"Unknown operation: {op}");
            }
        }
    }
}
