namespace InformationRetrieval.Indexing.External
{
    public interface IExternalBuildableIndex<T> : IBuildableIndex<T> where T : notnull
    {
        ExternalIndex<T> BuildExternalIndex();
    }
}
