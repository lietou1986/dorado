/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 9:50:33
 * 版本号：v1.0
 * 本类主要用途描述：数据操作基类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.Configuration;
using Dorado.VWS.Model;
using Microsoft.ApplicationBlocks.Data;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     数据操作基类
    /// </summary>
    public class DBbase<T> where T : new()
    {
        /// <summary>
        ///     数据库连接字符串
        /// </summary>
        public static readonly string ZsyncConnectionString = ConnectionStringProvider.Get("Zsync");

        /// <summary>
        ///     事务
        /// </summary>
        public SqlTransaction Transation { get; set; }

        /// <summary>
        ///     获取事务
        /// </summary>
        /// <returns></returns>
        public static SqlTransaction GetNewTransation()
        {
            var sqlConnection = new SqlConnection(ZsyncConnectionString);
            sqlConnection.Open();
            return sqlConnection.BeginTransaction();
        }

        #region 取得实例

        public T GetEntity(CommandType commandType, string commandText,
                           params SqlParameter[] commandParameters)
        {
            var dt = commandParameters.Length > 0
                         ? ExecuteDataset(commandType, commandText, commandParameters).Tables[0]
                         : ExecuteDataset(commandType, commandText).Tables[0];
            return EntityBase<T>.ConvertToEntity(dt);
        }

        public IList<T> GetEntities(CommandType commandType, string commandText,
                                    params SqlParameter[] commandParameters)
        {
            var dt = commandParameters.Length > 0
                         ? ExecuteDataset(commandType, commandText, commandParameters).Tables[0]
                         : ExecuteDataset(commandType, commandText).Tables[0];
            //if (dt.Rows.Count > 0 && dt.Rows[0]["UserName"].ToString() == "google.snoop")
            //{
            //    DataTable rt = new DataTable();
            //    rt.Columns.Add("ID", System.Type.GetType("System.Int32"));
            //    rt.Columns.Add("DomainID", System.Type.GetType("System.Int32"));
            //    rt.Columns.Add("UserName", System.Type.GetType("System.String"));
            //    rt.Columns.Add("PermissionType", System.Type.GetType("System.Int32"));
            //    rt.Columns.Add("DeleteFlag", System.Type.GetType("System.Boolean"));
            //    rt.Columns.Add("AddTime", System.Type.GetType("System.DateTime"));
            //    rt.Columns.Add("AddUserName", System.Type.GetType("System.String"));
            //    rt.Columns.Add("UpdateTime", System.Type.GetType("System.DateTime"));
            //    rt.Columns.Add("UpdateUserName", System.Type.GetType("System.String"));
            //    DataRow drow = null;
            //    foreach (DataRow item in dt.Rows)
            //    {
            //        //PermissionType 1,2,3,4,5
            //        for (int i = 1; i <= 5; i++ ) {
            //            drow = rt.NewRow();
            //            drow["ID"] = item["ID"];
            //            drow["DomainID"] = item["DomainID"];
            //            drow["UserName"] = item["UserName"];
            //            drow["PermissionType"] = i;
            //            drow["DeleteFlag"] = item["DeleteFlag"];
            //            drow["AddTime"] = item["AddTime"];
            //            drow["AddUserName"] = item["AddUserName"];
            //            drow["UpdateTime"] = item["UpdateTime"];
            //            drow["UpdateUserName"] = item["UpdateUserName"];
            //            rt.Rows.Add(drow);
            //        }
            //    }
            //    dt = rt;
            //}
            return EntityBase<T>.ConvertToEntityList(dt);
        }

        #endregion 取得实例

        #region SqlHelp 改造

        #region ExecuteDataset

        public DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteDataset(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteDataset(Transation, commandType, commandText);
        }

        public DataSet ExecuteDataset(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteDataset(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteDataset(Transation, spName, parameterValues);
        }

        public DataSet ExecuteDataset(CommandType commandType, string commandText,
                                      params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteDataset(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteDataset(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteDataset

        #region ExecuteNonQuery

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteNonQuery(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteNonQuery(Transation, commandType, commandText);
        }

        public int ExecuteNonQuery(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteNonQuery(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteNonQuery(Transation, spName, parameterValues);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteNonQuery(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteNonQuery(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteNonQuery

        #region ExecuteReader

        public SqlDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteReader(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteReader(Transation, commandType, commandText);
        }

        public SqlDataReader ExecuteReader(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteReader(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteReader(Transation, spName, parameterValues);
        }

        public SqlDataReader ExecuteReader(CommandType commandType, string commandText,
                                           params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteReader(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteReader(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteScalar(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteScalar(Transation, commandType, commandText);
        }

        public object ExecuteScalar(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteScalar(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteScalar(Transation, spName, parameterValues);
        }

        public object ExecuteScalar(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteScalar(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteScalar(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteScalar

        #region FillDataset

        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (Transation == null)
            {
                SqlHelper.FillDataset(ZsyncConnectionString, commandType, commandText, dataSet, tableNames);
            }
            else
            {
                SqlHelper.FillDataset(Transation, commandType, commandText, dataSet, tableNames);
            }
        }

        public void FillDataset(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (Transation == null)
            {
                SqlHelper.FillDataset(ZsyncConnectionString, spName, dataSet, tableNames, parameterValues);
            }
            else
            {
                SqlHelper.FillDataset(Transation, spName, dataSet, tableNames, parameterValues);
            }
        }

        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames,
                                params SqlParameter[] commandParameters)
        {
            if (Transation == null)
            {
                SqlHelper.FillDataset(ZsyncConnectionString, commandType, commandText, dataSet, tableNames,
                                      commandParameters);
            }
            else
            {
                SqlHelper.FillDataset(Transation, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        public void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand,
                                  DataSet dataSet, string tableName)
        {
            SqlHelper.UpdateDataset(insertCommand, deleteCommand, updateCommand, dataSet, tableName);
        }

        #endregion FillDataset

        #endregion SqlHelp 改造
    }

    /// <summary>
    ///     数据操作基类
    /// </summary>
    public class DBbase
    {
        /// <summary>
        ///     数据库连接字符串
        /// </summary>
        public static readonly string ZsyncConnectionString =
             ConnectionStringProvider.Get("Zsync");

        /// <summary>
        ///     事务
        /// </summary>
        public SqlTransaction Transation { get; set; }

        /// <summary>
        ///     获取事务
        /// </summary>
        /// <returns></returns>
        public static SqlTransaction GetNewTransation()
        {
            var sqlConnection = new SqlConnection(ZsyncConnectionString);
            sqlConnection.Open();
            return sqlConnection.BeginTransaction();
        }

        #region SqlHelp 改造

        #region ExecuteDataset

        public DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteDataset(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteDataset(Transation, commandType, commandText);
        }

        public DataSet ExecuteDataset(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteDataset(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteDataset(Transation, spName, parameterValues);
        }

        public DataSet ExecuteDataset(CommandType commandType, string commandText,
                                      params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteDataset(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteDataset(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteDataset

        #region ExecuteNonQuery

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteNonQuery(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteNonQuery(Transation, commandType, commandText);
        }

        public int ExecuteNonQuery(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteNonQuery(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteNonQuery(Transation, spName, parameterValues);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteNonQuery(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteNonQuery(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteNonQuery

        #region ExecuteReader

        public SqlDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteReader(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteReader(Transation, commandType, commandText);
        }

        public SqlDataReader ExecuteReader(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteReader(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteReader(Transation, spName, parameterValues);
        }

        public SqlDataReader ExecuteReader(CommandType commandType, string commandText,
                                           params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteReader(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteReader(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return Transation == null
                       ? SqlHelper.ExecuteScalar(ZsyncConnectionString, commandType, commandText)
                       : SqlHelper.ExecuteScalar(Transation, commandType, commandText);
        }

        public object ExecuteScalar(string spName, params object[] parameterValues)
        {
            return Transation == null
                       ? SqlHelper.ExecuteScalar(ZsyncConnectionString, spName, parameterValues)
                       : SqlHelper.ExecuteScalar(Transation, spName, parameterValues);
        }

        public object ExecuteScalar(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return Transation == null
                       ? SqlHelper.ExecuteScalar(ZsyncConnectionString, commandType, commandText, commandParameters)
                       : SqlHelper.ExecuteScalar(Transation, commandType, commandText, commandParameters);
        }

        #endregion ExecuteScalar

        #region FillDataset

        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (Transation == null)
            {
                SqlHelper.FillDataset(ZsyncConnectionString, commandType, commandText, dataSet, tableNames);
            }
            else
            {
                SqlHelper.FillDataset(Transation, commandType, commandText, dataSet, tableNames);
            }
        }

        public void FillDataset(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (Transation == null)
            {
                SqlHelper.FillDataset(ZsyncConnectionString, spName, dataSet, tableNames, parameterValues);
            }
            else
            {
                SqlHelper.FillDataset(Transation, spName, dataSet, tableNames, parameterValues);
            }
        }

        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames,
                                params SqlParameter[] commandParameters)
        {
            if (Transation == null)
            {
                SqlHelper.FillDataset(ZsyncConnectionString, commandType, commandText, dataSet, tableNames,
                                      commandParameters);
            }
            else
            {
                SqlHelper.FillDataset(Transation, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        public void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand,
                                  DataSet dataSet, string tableName)
        {
            SqlHelper.UpdateDataset(insertCommand, deleteCommand, updateCommand, dataSet, tableName);
        }

        #endregion FillDataset

        #endregion SqlHelp 改造
    }
}