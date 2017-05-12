#region using

using System.Collections.Generic;
using System.Data;
using System.Linq;

#endregion using

namespace Dorado.VWS.Model
{
    public abstract class EntityBase<T> where T : new()
    {
        /// <summary>
        ///     将指定的DataTable转换成指定实体
        /// </summary>
        /// <param name = "dataTable">数据表</param>
        /// <returns>实体信息</returns>
        public static T ConvertToEntity(DataTable dataTable)
        {
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return ((IConvertToEntity<T>)new T()).ConvertToEntity(row);
            }
            return ((IConvertToEntity<T>)new T()).ConvertToEntity(null);
        }

        /// <summary>
        ///     将指定的DataTable转换成指定实体列表
        /// </summary>
        /// <param name = "dataTable">数据表</param>
        /// <returns>实体信息列表</returns>
        public static IList<T> ConvertToEntityList(DataTable dataTable)
        {
            var t = (IConvertToEntity<T>)new T();
            return (from DataRow row in dataTable.Rows select t.ConvertToEntity(row)).ToList();
        }

        public static int ConvertToInt(string domainType)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(domainType) && int.TryParse(domainType, out result))
            {
            }
            return result;
        }
    }

    internal interface IConvertToEntity<out T>
    {
        T ConvertToEntity(DataRow row);
    }
}