namespace Dorado.Core.Queue
{
    public interface IQueueFactory
    {
        IQueue<T> CreateQueue<T>();
    }
}