using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dorado.PerformanceCounter
{
    public class PerfCounterFactory
    {
        private static IList<PerfCounter> _counters = new List<PerfCounter>();
        private static object _countsLocker = new object();

        public static T GetCounters<T>()
            where T : IPerfCounterProvider, new()
        {
            T t = new T();
            t.CreateCounters();
            CreateCounters(t.PerformanceObjectName, true);
            t.CountersCreated();
            return t;
        }

        public static System.Diagnostics.PerformanceCounter Find(string performanceObjectName, string perfCountName)
        {
            lock (_countsLocker)
            {
                PerfCounter perfCounter = _counters.FirstOrDefault(n => n.PerformanceObjectName == performanceObjectName && n.Name == perfCountName);
                return perfCounter.Counter;
            }
        }

        public static void AddPerfCounter(PerfCounter counter)
        {
            lock (_countsLocker)
            {
                if (!_counters.Any(n => n.PerformanceObjectName == counter.PerformanceObjectName && n.Name == counter.Name))
                    _counters.Add(counter);
            }
        }

        private static void CreateCounters(string PerformanceObjectName, bool force)
        {
            // does the object exist?
            bool countersExist = PerformanceCounterCategory.Exists(PerformanceObjectName);

            // delete the category?
            if (countersExist == true && force == true)
            {
                PerformanceCounterCategory.Delete(PerformanceObjectName);
                countersExist = false;
            }

            // do we need to create it?
            if (countersExist == false)
            {
                // create a collection...
                CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();

                lock (_countsLocker)
                {
                    // go through each counter...
                    foreach (PerfCounter counter in _counters)
                    {
                        counterCreationDataCollection.Add(new CounterCreationData
                        {
                            CounterName = counter.Name,
                            CounterHelp = counter.HelpText,
                            CounterType = counter.Type
                        });
                    }
                }

                // create the category and all of the counters...
                PerformanceCounterCategory.Create(PerformanceObjectName, string.Empty, PerformanceCounterCategoryType.SingleInstance, counterCreationDataCollection);
            }

            lock (_countsLocker)
            {
                // now, go back through the counters and create instances...
                foreach (PerfCounter counter in _counters)
                {
                    // create an instance and store it...
                    counter.Counter = new System.Diagnostics.PerformanceCounter(PerformanceObjectName, counter.Name, string.Empty, false);

                    // reset the value...
                    counter.Counter.RawValue = 0L;
                }
            }
        }
    }
}