/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/11 14:01:25
 * 版本号：v1.0
 * 本类主要用途描述：版本文件数据访问操作类
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
    ///     版本文件数据访问操作类
    /// </summary>
    public class VersionFileDao : DBbase<VersionFileEntity>
    {
        /// <summary>
        ///     获取版本文件信息
        /// </summary>
        /// <param name = "versionFileId">版本文件Id</param>
        /// <returns>版本文件实体</returns>
        public VersionFileEntity GetVersionFile(int versionFileId)
        {
            const string sql = @"SELECT * FROM zsync_version_file(NOLOCK) WHERE Id = @VersionFileId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@VersionFileId", versionFileId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取版本文件列表
        /// </summary>
        /// <param name = "domainId"></param>
        /// <param name = "filePath">文件路径（包括文件名）</param>
        /// <returns>版本文件实体列表</returns>
        public IList<VersionFileEntity> GetVersionFiles(int domainId, string filePath)
        {
            const string sql =
                @"SELECT * FROM zsync_version_file(NOLOCK) WHERE FilePath = @FilePath and DomainId=@DomainId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@FilePath", filePath)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取版本文件列表
        /// </summary>
        /// <param name = "domainId"></param>
        /// <param name = "filePath">文件路径（包括文件名）</param>
        /// <param name = "begin"></param>
        /// <param name = "end"></param>
        /// <returns>版本文件实体列表</returns>
        public IList<VersionFileEntity> GetVersionFiles(int domainId, string filePath, int begin, int end)
        {
            var sql =
                string.Format(
                    @"SELECT * FROM
(SELECT *,ROW_NUMBER() OVER (order by CreateTime desc)as RowNumber
FROM zsync_version_file(NOLOCK) WHERE FilePath = @FilePath and DomainId=@DomainId and DeleteFlag = 0) tab
WHERE RowNumber between {0} AND {1} ORDER BY CreateTime DESC",
                    begin, end);

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@FilePath", filePath)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     计算总共有多少个版本
        /// </summary>
        /// <returns></returns>
        public int GetVersionCount(int domainId, string filePath)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_version_file(NOLOCK) WHERE DomainId=@DomainId and FilePath=@FilePath";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@FilePath", filePath)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     获取版本文件列表
        /// </summary>
        /// <param name = "syncTaskId">同步任务Id</param>
        /// <returns>版本文件实体列表</returns>
        public IList<VersionFileEntity> GetVersionFiles(int syncTaskId)
        {
            const string sql =
                @"select * from zsync_version_file(NOLOCK)
                            where SyncTaskId = @SyncTaskId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@SyncTaskId", syncTaskId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<VersionFileEntity> GetPreVersionFile(int syncTaskId)
        {
            string sqlGetDomainID = "SELECT DomainId FROM zsync_synctask WHERE TaskId=@SyncTaskId";
            SqlParameter[] paras ={
                                      new SqlParameter("@SyncTaskId", syncTaskId)
                                 };
            object obj = ExecuteScalar(CommandType.Text, sqlGetDomainID, paras);

            int domainID = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out domainID);
            }

            const string sql =
                @"SELECT * FROM zsync_version_file(NOLOCK) WHERE DomainId=@DomainId AND Id IN(
SELECT MAX(Id) FROM zsync_version_file (NOLOCK) WHERE SyncTaskId<@SyncTaskId AND FilePath IN (SELECT FilePath FROM zsync_version_file(NOLOCK) WHERE SyncTaskId=@SyncTaskId) GROUP BY FilePath )";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainID),
                                            new SqlParameter("@SyncTaskId", syncTaskId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     插入版本文件信息
        /// </summary>
        /// <param name = "versionFile">版本文件实体</param>
        /// <returns>记录Id</returns>
        public int Insert(VersionFileEntity versionFile)
        {
            const string sql =
                @"INSERT INTO zsync_version_file(SyncTaskId,FilePath,VersionPath ,DomainId,Creator, Description)
                           VALUES (@SyncTaskId,@FilePath, @VersionPath,@DomainId,@Creator,@Description)
                           select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@SyncTaskId", versionFile.SyncTaskId),
                                            new SqlParameter("@FilePath", versionFile.FilePath),
                                            new SqlParameter("@VersionPath", versionFile.VersionPath),
                                            new SqlParameter("@DomainId", versionFile.DomainId),
                                            new SqlParameter("@Creator", versionFile.Creator),
                                            new SqlParameter("@Description", versionFile.Description)
                                        };
            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     删除版本文件信息
        /// </summary>
        /// <param name = "versionFileId">版本文件实体Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(int versionFileId)
        {
            const string sql = @"UPDATE zsync_version_file SET DeleteFlag = 1 where Id = @VersionFileId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@VersionFileId", versionFileId)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }
    }
}