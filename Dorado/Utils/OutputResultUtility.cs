using System;
using System.Linq;

namespace Dorado.Utils
{
    public static class OutputResultUtility
    {
        public static void AssertValuesNotNull<T>(params T[] values) where T : class
        {
            if (values.Any(value => value == null))
            {
                throw new InvalidOperationException("Some or all values are null. Type: " + typeof(T).FullName);
            }
        }

        public static void AssertValuesNotNull<T>(string paramName, params T[] values) where T : class
        {
            if (values.Any(value => value == null))
            {
                throw new InvalidOperationException("Some or all values are null. Name: " + paramName + "Type: " + typeof(T).FullName);
            }
        }

        public static void AssertNotNull<T>(T value, string paramName) where T : class
        {
            AssertValuesNotNull<T>(paramName, new T[]
			{
				value
			});
        }

        public static void AssertInRange(string paramName, int value, int rangeBegin, int rangeEnd)
        {
            if (value < rangeBegin || value > rangeEnd)
            {
                throw new InvalidOperationException(string.Format("Value is null: Parameter '{0}' must be within the range {1} - {2}.  The value given was {3}.", new object[]
				{
					paramName,
					rangeBegin,
					rangeEnd,
					value
				}));
            }
        }
    }
}