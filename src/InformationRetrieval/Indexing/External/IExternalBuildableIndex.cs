namespace InformationRetrieval.Indexing.External
{
    public interface IExternalBuildableIndex<T> : IBuildableIndex<T>
    {
        ExternalIndex<T> BuildExternalIndex();
    }
}
