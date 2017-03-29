﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Dorado.Extensions
{
    public static class ListExtensions
    {
        public static string ToSeparatedString<T>(this IList<T> value)
        {
            return ToSeparatedString(value, ",");
        }

        public static string ToSeparatedString<T>(this IList<T> value, string separator)
        {
            if (value.Count == 0)
            {
                return String.Empty;
            }
            if (value.Count == 1)
            {
                if (value[0] != null)
                {
                    return value[0].ToString();
                }
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            bool flag = true;
            bool flag2 = false;
            foreach (object obj2 in value)
            {
                if (!flag)
                {
                    builder.Append(separator);
                }
                if (obj2 != null)
                {
                    builder.Append(obj2.ToString().TrimEnd(new char[0]));
                    flag2 = true;
                }
                flag = false;
            }
            if (!flag2)
            {
                return string.Empty;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Makes a slice of the specified list in between the start and end indexes.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>A slice of the list.</returns>
        public static IList<T> Slice<T>(this IList<T> list, int? start, int? end)
        {
            return list.Slice(start, end, null);
        }

        /// <summary>
        /// Makes a slice of the specified list in between the start and end indexes,
        /// getting every so many items based upon the step.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="step">The step.</param>
        /// <returns>A slice of the list.</returns>
        public static IList<T> Slice<T>(this IList<T> list, int? start, int? end, int? step)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (step == 0)
                throw Error.Argument("step", "Step cannot be zero.");

            List<T> slicedList = new List<T>();

            // nothing to slice
            if (list.Count == 0)
                return slicedList;

            // set defaults for null arguments
            int s = step ?? 1;
            int startIndex = start ?? 0;
            int endIndex = end ?? list.Count;

            // start from the end of the list if start is negative
            startIndex = (startIndex < 0) ? list.Count + startIndex : startIndex;

            // end from the start of the list if end is negative
            endIndex = (endIndex < 0) ? list.Count + endIndex : endIndex;

            // ensure indexes keep within collection bounds
            startIndex = Math.Max(startIndex, 0);
            endIndex = Math.Min(endIndex, list.Count - 1);

            // loop between start and end indexes, incrementing by the step
            for (int i = startIndex; i < endIndex; i += s)
            {
                slicedList.Add(list[i]);
            }

            return slicedList;
        }

        /// <summary>
        /// 集合转换为DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        private static DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = prop.PropertyType.GetCoreType();
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }
    }
}