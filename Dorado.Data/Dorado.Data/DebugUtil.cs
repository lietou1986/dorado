using System;
using System.Data.SqlClient;
using System.Linq;

namespace Dorado.Data
{
    public static class DebugUtil
    {
        public static string GetParameterString(object[] parameters)
        {
            string ret = string.Empty;
            if (parameters == null)
            {
                return ret;
            }
            string result;
            try
            {
                ret = parameters.Aggregate(ret, (current, obj) => current + "{" + DebugUtil.GetDebugParamValue(obj) + "}");
            }
            catch (Exception)
            {
                result = "!error!";
                return result;
            }
            return ret;
        }

        public static string GetParameterString(int[] parameters)
        {
            string ret = string.Empty;
            if (parameters == null)
            {
                return ret;
            }
            try
            {
                ret = parameters.Aggregate(ret, (current, param) => current + "{" + param.ToString() + "}");
            }
            catch (Exception)
            {
                return "!error!";
            }
            return ret;
        }

        public static string GetDebugParamValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            string result;
            try
            {
                if (value is string)
                {
                    string strVal = (string)value;

                    if (strVal.Length > 20)
                    {
                        result = strVal.Substring(0, 20) + "...";
                    }
                    else
                    {
                        result = strVal;
                    }

                }
                else
                {
                    if (value.GetType().IsPrimitive || value.GetType().IsValueType || value.GetType().IsEnum)
                    {
                        result = value.ToString();
                    }
                    else
                    {
                        result = "object:" + value.GetType().Name;
                    }
                }
            }
            catch (Exception)
            {
                result = "!error!";
            }
            return result;
        }

        public static string GetParameterString(SqlCommand c)
        {
            string ret = string.Empty;
            if (c == null)
            {
                return ret;
            }
            try
            {
                foreach (SqlParameter p in c.Parameters)
                {
                    string text = ret;
                    ret = string.Concat(new string[]
					{
						text,
						"{@",
						p.ParameterName,
						"}={",
						DebugUtil.GetDebugParamValue(p.Value),
						"}"
					});
                }
            }
            catch (Exception)
            {
                return "!error!"; ;
            }
            return ret;
        }
    }
}