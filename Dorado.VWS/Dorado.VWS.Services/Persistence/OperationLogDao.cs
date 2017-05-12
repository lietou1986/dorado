/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/8 14:18:28
 * 版本号：v1.0
 * 本类主要用途描述：操作日志数据访问类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     操作日志数据访问类
    /// </summary>
    public class OperationLogDao : DBbase<OperationLogEntity>
    {
        /// <summary>
        ///     获取操作日志
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <param name = "operateType">操作类型</param>
        /// <param name = "userName">用户名</param>
        /// <param name = "startDate">开始时间</param>
        /// <param name = "endDate">结束时间</param>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>操作日志实体列表</returns>
        public IList<OperationLogEntity> GetOperationLog(string domainName, string operateType, string userName,
                                                         DateTime startDate, DateTime endDate, int beginIndex,
                                                         int endIndex)
        {
            const string sql =
                @"SELECT * FROM
                          (SELECT *,ROW_NUMBER() OVER (order by Id desc)as RowNumber FROM zsync_operation_log(NOLOCK)
                           WHERE DomainName = ISNULL(@DomainName,DomainName) AND OperateType = ISNULL(@OperateType,OperateType)
                           AND UserName = ISNULL(@UserName,UserName) AND CONVERT(varchar(10) ,Createtime, 120) between isnull(@StartDate,Createtime)
                           AND isnull(@EndDate,Createtime)
                           AND DeleteFlag = 0) tab
                           WHERE RowNumber between @BeginIndex AND @EndIndex ORDER BY Id DESC";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainName", domainName),
                                            new SqlParameter("@OperateType", operateType),
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@StartDate", startDate),
                                            new SqlParameter("@EndDate", endDate),
                                            new SqlParameter("@BeginIndex", beginIndex),
                                            new SqlParameter("@EndIndex", endIndex)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     计算操作日志数量
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <param name = "operateType">操作类型</param>
        /// <param name = "userName">用户名</param>
        /// <param name = "startDate">开始时间</param>
        /// <param name = "endDate">结束时间</param>
        /// <returns></returns>
        public int GetOperationLogCout(string domainName, string operateType, string userName, DateTime startDate,
                                       DateTime endDate)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_operation_log(NOLOCK) WHERE DomainName = ISNULL(@DomainName,DomainName)
                           AND OperateType = ISNULL(@OperateType,OperateType) AND UserName = ISNULL(@UserName,UserName) AND
                           CONVERT(varchar(10) ,Createtime,120) between isnull(@StartDate,Createtime) AND isnull(@EndDate,Createtime) AND DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainName", domainName),
                                            new SqlParameter("@OperateType", operateType),
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@StartDate", startDate),
                                            new SqlParameter("@EndDate", endDate)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     插入操作日志信息
        /// </summary>
        /// <param name = "operationLog">操作日志实体</param>
        /// <returns>记录Id</returns>
        public int Insert(OperationLogEntity operationLog)
        {
            const string sql =
                @"INSERT INTO zsync_operation_log(UserName,DomainName,OperateType,Log,Result) VALUES (@UserName,@DomainName,@OperateType,@Log,@Result)
                          select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", operationLog.UserName),
                                            new SqlParameter("@DomainName", operationLog.DomainName),
                                            new SqlParameter("@OperateType", operationLog.OperateType),
                                            new SqlParameter("@Log", operationLog.Log),
                                            new SqlParameter("@Result", operationLog.Result)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }
    }
}