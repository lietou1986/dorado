using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;

namespace Dorado.ActivityEngine.ServiceImp
{
    internal static class Util
    {
        public static List<int> ParseInts(string str)
        {
            List<int> result = new List<int>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] array = str.Split(new char[]
				{
					','
				});
                for (int i = 0; i < array.Length; i++)
                {
                    string v = array[i];
                    result.Add(int.Parse(v));
                }
            }
            return result;
        }

        public static void ExecuteWithCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Util", ex);
            }
        }
    }
}