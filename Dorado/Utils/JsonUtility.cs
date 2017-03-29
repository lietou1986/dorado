using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dorado.Utils
{
    public class JsonUtility
    {
        /// <summary>
        /// DataTable转成Json
        /// </summary>
        /// <param name="jsonName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToJson(string jsonName, DataTable dt)
        {
            StringBuilder json = new StringBuilder();
            json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            json.Append(",");
                        }
                    }
                    json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        json.Append(",");
                    }
                }
            }
            json.Append("]}");
            return json.ToString();
        }

        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            json.Append(",");
                        }
                    }
                    json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        json.Append(",");
                    }
                }
            }
            json.Append("]");
            return json.ToString();
        }

        //List转成json
        public static string ObjectToJson<T>(string jsonName, IList<T> il)
        {
            StringBuilder json = new StringBuilder();
            json.Append("{\"" + jsonName + "\":[");
            if (il.Count > 0)
            {
                for (int i = 0; i < il.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    Type type = obj.GetType();
                    PropertyInfo[] pis = type.GetProperties();
                    json.Append("{");
                    for (int j = 0; j < pis.Length; j++)
                    {
                        json.Append("\"" + pis[j].Name.ToString() + "\":\"" + pis[j].GetValue(il[i], null) + "\"");
                        if (j < pis.Length - 1)
                        {
                            json.Append(",");
                        }
                    }
                    json.Append("}");
                    if (i < il.Count - 1)
                    {
                        json.Append(",");
                    }
                }
            }
            json.Append("]}");
            return json.ToString();
        }

        public static string ObjectToJson<T>(IList<T> il)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");
            if (il.Count > 0)
            {
                for (int i = 0; i < il.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    Type type = obj.GetType();
                    PropertyInfo[] pis = type.GetProperties();
                    json.Append("{");
                    for (int j = 0; j < pis.Length; j++)
                    {
                        json.Append("\"" + pis[j].Name.ToString() + "\":\"" + pis[j].GetValue(il[i], null) + "\"");
                        if (j < pis.Length - 1)
                        {
                            json.Append(",");
                        }
                    }
                    json.Append("}");
                    if (i < il.Count - 1)
                    {
                        json.Append(",");
                    }
                }
            }
            json.Append("]");
            return json.ToString();
        }

        /// <summary>
        /// 对象转换为Json字符串
        /// </summary>
        /// <param name="jsonObject">对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(object jsonObject)
        {
            string jsonString = "{";
            PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
            foreach (PropertyInfo t in propertyInfo)
            {
                object objectValue = t.GetGetMethod().Invoke(jsonObject, null);
                string value = string.Empty;
                if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value = "'" + objectValue.ToString() + "'";
                }
                else if (objectValue is string)
                {
                    value = "'" + ToJson(objectValue.ToString()) + "'";
                }
                else if (objectValue is IEnumerable)
                {
                    value = ToJson((IEnumerable)objectValue);
                }
                else
                {
                    value = ToJson(objectValue.ToString());
                }
                jsonString += "\"" + ToJson(t.Name) + "\":" + value + ",";
            }
            return DeleteLast(jsonString) + "}";
        }

        /// <summary>
        /// 对象集合转换Json
        /// </summary>
        /// <param name="array">集合对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(IEnumerable array)
        {
            string jsonString = array.Cast<object>().Aggregate("[", (current, item) => current + (ToJson(item) + ","));
            return DeleteLast(jsonString) + "]";
        }

        /// <summary>
        /// 普通集合转换Json
        /// </summary>
        /// <param name="array">集合对象</param>
        /// <returns>Json字符串</returns>
        public static string ToArrayString(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString = ToJson(item.ToString()) + ",";
            }
            return DeleteLast(jsonString) + "]";
        }

        /// <summary>
        /// 删除结尾字符
        /// </summary>
        /// <param name="str">需要删除的字符</param>
        /// <returns>完成后的字符串</returns>
        private static string DeleteLast(string str)
        {
            return str.Length > 1 ? str.Substring(0, str.Length - 1) : str;
        }

        /// <summary>
        /// Datatable转换为Json
        /// </summary>
        /// <param name="table">Datatable对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DataTable table)
        {
            string jsonString = "[";
            DataRowCollection drc = table.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString += "{";
                foreach (DataColumn column in table.Columns)
                {
                    jsonString += "\"" + ToJson(column.ColumnName) + "\":";
                    if (column.DataType == typeof(DateTime) || column.DataType == typeof(string))
                    {
                        jsonString += "\"" + ToJson(drc[i][column.ColumnName].ToString()) + "\",";
                    }
                    else
                    {
                        jsonString += ToJson(drc[i][column.ColumnName].ToString()) + ",";
                    }
                }
                jsonString = DeleteLast(jsonString) + "},";
            }
            return DeleteLast(jsonString) + "]";
        }

        /// <summary>
        /// DataReader转换为Json
        /// </summary>
        /// <param name="dataReader">DataReader对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DbDataReader dataReader)
        {
            string jsonString = "[";
            while (dataReader.Read())
            {
                jsonString += "{";

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    jsonString += "\"" + ToJson(dataReader.GetName(i)) + "\":";
                    if (dataReader.GetFieldType(i) == typeof(DateTime) || dataReader.GetFieldType(i) == typeof(string))
                    {
                        jsonString += "\"" + ToJson(dataReader[i].ToString()) + "\",";
                    }
                    else
                    {
                        jsonString += ToJson(dataReader[i].ToString()) + ",";
                    }
                }
                jsonString = DeleteLast(jsonString) + "}";
            }
            dataReader.Close();
            return DeleteLast(jsonString) + "]";
        }

        /// <summary>
        /// DataSet转换为Json
        /// </summary>
        /// <param name="dataSet">DataSet对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = dataSet.Tables.Cast<DataTable>().Aggregate("{", (current, table) => current + ("\"" + ToJson(table.TableName) + "\":" + ToJson(table) + ","));
            return DeleteLast(jsonString) + "}";
        }

        /// <summary>
        /// String转换为Json
        /// </summary>
        /// <param name="value">String对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var temstr = value;
            temstr = temstr.Replace("{", "｛").Replace("}", "｝").Replace(":", "：").Replace(",", "，").Replace("[", "【").Replace("]", "】").Replace(";", "；").Replace("\n", "<br/>").Replace("\r", "");

            temstr = temstr.Replace("\t", "   ");
            temstr = temstr.Replace("'", "\'");
            temstr = temstr.Replace(@"\", @"\\");
            temstr = temstr.Replace("\"", "\"\"");
            return temstr;
        }
    }
}