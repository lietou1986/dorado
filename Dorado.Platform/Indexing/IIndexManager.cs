namespace Dorado.Platform.Indexing
{
    public interface IIndexManager
    {
        bool HasIndexProvider();

        IIndexProvider GetSearchIndexProvider();
    }
}