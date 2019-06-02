namespace NaturalLanguageTools.Transformers
{
    public interface ITransformer<TIn, TOut>
    {
        TOut Transform(TIn source);
    }
}
