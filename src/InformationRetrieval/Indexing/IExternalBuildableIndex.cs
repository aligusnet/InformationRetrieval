using System;
using System.Collections.Generic;
using System.Text;

namespace InformationRetrieval.Indexing
{
    public interface IExternalBuildableIndex<T> : IBuildableIndex<T>
    {
        ExternalIndex<T> BuildExternalIndex();
    }
}
