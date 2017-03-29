using System.Diagnostics;

namespace Dorado.PerformanceCounter
{
    public class PerfCounter
    {
        public string Name = string.Empty;
        public string HelpText = string.Empty;
        public PerformanceCounterType Type;
        public System.Diagnostics.PerformanceCounter Counter;
        public string PerformanceObjectName = string.Empty;

        private PerfCounter()
        {
        }

        public PerfCounter(string performanceObjectName, string name, string helpText = "", PerformanceCounterType type = PerformanceCounterType.RateOfCountsPerSecond64)
        {
            PerformanceObjectName = performanceObjectName;
            Name = name;
            HelpText = helpText;
            Type = type;
        }
    }
}