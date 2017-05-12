/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：han
 * 联系方式：shanfeng.han@dorado.com 
 * 创建时间： 2014/6/16 15:30:39
 * 版本号：v1.0
 * 本类主要用途描述：文件变更记录表数据访问操作类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    public class FilesWatcherDao : DBbase<FilesWatcherEntity>
    {
        /// <summary>
        /// 获取文件变更数据
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public IList<FilesWatcherEntity> GetFilesWatcherById(int id)
        {
            const string sql = @"SELECT [Id]
                                  ,[OperateType]
                                  ,[ServerId]
                                  ,[Root]
                                  ,[AddFiles]
                                  ,[UpdateFiles]
                                  ,[DelFiles]
                                  ,[UserName]
                                  ,[OperatorIp]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[LogInfo]
                                  ,[Description]
                                  ,[DeleteFlag]
                                  ,[Remark]
                              FROM [Zsync_FilesWatcher](NOLOCK) WHERE Id=@Id";
            SqlParameter[] pars = new SqlParameter[]{
                                                       new SqlParameter("@ServerId",id)
                                                   };
            return GetEntities(CommandType.Text, sql, pars);
        }

        /// <summary>
        /// 获取文件变更数据
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public IList<FilesWatcherEntity> GetFilesWatcherByServerId(int serverId)
        {
            const string sql = @"SELECT s.[Root] AS serverRoot,fw.* FROM dbo.Zsync_FilesWatcher fw (NOLOCK)
                                INNER JOIN
                                dbo.zsync_server s (NOLOCK)
                                ON s.Id=fw.ServerId
                                WHERE fw.ServerId=@ServerId";
            SqlParameter[] pars = new SqlParameter[]{
                                                       new SqlParameter("@ServerId",serverId)
                                                   };
            return GetEntities(CommandType.Text, sql, pars);
        }

        /// <summary>
        /// 添加文件变更数据
        /// </summary>
        /// <param name="fwentity"></param>
        /// <returns></returns>
        public int Insert(FilesWatcherEntity fwentity)
        {
            #region sql

            const string sql = @"INSERT INTO [Zsync_FilesWatcher]
                                       ([OperateType]
                                       ,[ServerId]
                                       ,[Root]
                                       ,[AddFiles]
                                       ,[UpdateFiles]
                                       ,[DelFiles]
                                       ,[UserName]
                                       ,[OperatorIp]
                                       ,[CreateTime]
                                       ,[UpdateTime]
                                       ,[LogInfo]
                                       ,[Description]
                                       ,[DeleteFlag]
                                       ,[Remark])
                                 VALUES
                                       (@OperateType
                                       ,@ServerId
                                       ,@Root
                                       ,@AddFiles
                                       ,@UpdateFiles
                                       ,@DelFiles
                                       ,@UserName
                                       ,@OperatorIp
                                       ,@CreateTime
                                       ,@UpdateTime
                                       ,@LogInfo
                                       ,@Description
                                       ,@DeleteFlag
                                       ,@Remark)";

            #endregion sql

            #region SqlParameter

            SqlParameter[] pars = new SqlParameter[]{
                new SqlParameter("@OperateType",fwentity.OperateType),
                new SqlParameter("@ServerId",fwentity.ServerId),
                new SqlParameter("@Root",fwentity.Root),
                new SqlParameter("@AddFiles",fwentity.AddFiles),
                new SqlParameter("@UpdateFiles",fwentity.UpdateFiles),
                new SqlParameter("@DelFiles",fwentity.DelFiles),
                new SqlParameter("@UserName",fwentity.UserName),
                new SqlParameter("@OperatorIp",fwentity.OperatorIp),
                new SqlParameter("@CreateTime",fwentity.CreateTime),
                new SqlParameter("@UpdateTime",fwentity.UpdateTime),
                new SqlParameter("@LogInfo",fwentity.LogInfo),
                new SqlParameter("@Description",fwentity.Description),
                new SqlParameter("@DeleteFlag",fwentity.DeleteFlag),
                new SqlParameter("@Remark",fwentity.Remark),
                                                  };

            #endregion SqlParameter

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, pars));
        }

        /// <summary>
        /// 删除文件变更数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(IList<int> domainIdList)
        {
            const string sql = @"UPDATE [Zsync_FilesWatcher]
                               SET [DeleteFlag] = 1
                               WHERE Id ={0}";
            var domainStrBuilder = new StringBuilder();
            if (domainIdList.Count > 0)
            {
                foreach (var domainId in domainIdList)
                {
                    domainStrBuilder.Append(string.Format("{0},", domainId));
                }
                domainStrBuilder.Remove(domainStrBuilder.Length - 1, 1);

                int result = ExecuteNonQuery(CommandType.Text, string.Format(sql, domainStrBuilder));

                return result >= 1;
            }
            return true;
        }
    }
}