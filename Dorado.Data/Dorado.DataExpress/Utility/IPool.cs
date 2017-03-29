namespace Dorado.DataExpress.Utility
{
    public interface IPool<T>
    {
        int IdleCount
        {
            get;
        }

        int ActiveCount
        {
            get;
        }

        int MaxActive
        {
            get;
        }

        int MaxIdle
        {
            get;
        }

        int MaxWait
        {
            get;
        }

        T Obtain();

        void Return(T poolItem);

        void Invalidate(T obj);

        void Clear();

        void Close();

        T CreateInstance();

        void DestroyInstance(T obj);
    }
}