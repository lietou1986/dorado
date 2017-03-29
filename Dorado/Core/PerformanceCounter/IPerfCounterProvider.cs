namespace Dorado.PerformanceCounter
{
    public interface IPerfCounterProvider
    {
        void CreateCounters();

        void CountersCreated();

        string PerformanceObjectName { get; }
    }
}