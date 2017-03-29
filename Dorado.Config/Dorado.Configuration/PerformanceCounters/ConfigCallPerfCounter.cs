using Dorado.PerformanceCounter;
using System.Diagnostics;

namespace Dorado.Configuration.PerformanceCounters
{
    public class ConfigCallPerfCounter : IPerfCounterProvider
    {
        private static readonly ConfigCallPerfCounter _instance =
            PerfCounterFactory.GetCounters<ConfigCallPerfCounter>();

        public static ConfigCallPerfCounter Instance
        {
            get
            {
                return _instance;
            }
        }

        public System.Diagnostics.PerformanceCounter TotalOfRequest;
        public System.Diagnostics.PerformanceCounter RateOfRequest;
        public System.Diagnostics.PerformanceCounter CountOfCurrentRequest;

        private const string TotalOfRequestStr = "total # req";
        private const string RateOfRequestStr = "req/sec";
        private const string CountOfCurrentRequestStr = "current # req";

        #region IPerfCounterProvider 成员

        public void CreateCounters()
        {
            PerfCounterFactory.AddPerfCounter(new PerfCounter(PerformanceObjectName, TotalOfRequestStr, "请求总数",
                PerformanceCounterType.NumberOfItems64));
            PerfCounterFactory.AddPerfCounter(new PerfCounter(PerformanceObjectName, RateOfRequestStr, "每秒请求的数量",
                PerformanceCounterType.RateOfCountsPerSecond64));
            PerfCounterFactory.AddPerfCounter(new PerfCounter(PerformanceObjectName, CountOfCurrentRequestStr, "当前并发请求量",
                PerformanceCounterType.RateOfCountsPerSecond64));
        }

        public void CountersCreated()
        {
            TotalOfRequest = PerfCounterFactory.Find(PerformanceObjectName, TotalOfRequestStr);
            RateOfRequest = PerfCounterFactory.Find(PerformanceObjectName, RateOfRequestStr);
            CountOfCurrentRequest = PerfCounterFactory.Find(PerformanceObjectName, CountOfCurrentRequestStr);
        }

        public string PerformanceObjectName
        {
            get { return "ConfigCallPerfCounter"; }
        }

        #endregion IPerfCounterProvider 成员
    }
}