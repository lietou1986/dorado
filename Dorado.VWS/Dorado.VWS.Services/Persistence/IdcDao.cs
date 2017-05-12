/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/5 15:16:04
 * 版本号：v1.0
 * 本类主要用途描述：Idc数据访问操作类
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
    ///     Idc数据访问操作类
    /// </summary>
    public class IdcDao : DBbase<IdcEntity>
    {
        /// <summary>
        ///     获取idc信息
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>Idc实体</returns>
        public IdcEntity GetIdcById(int idcId)
        {
            const string sql = @"SELECT * FROM zsync_idc(NOLOCK) WHERE Id = @idcId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@idcId", idcId)
                                        };
            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取idc信息
        /// </summary>
        /// <param name = "idcName">idc名称</param>
        /// <returns>Idc实体</returns>
        public IdcEntity GetIdcByName(string idcName)
        {
            const string sql = @"SELECT * FROM zsync_idc(NOLOCK) WHERE IdcName = @IdcName and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcName", idcName)
                                        };
            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取所有idc列表
        /// </summary>
        /// <returns>Idc实体列表</returns>
        public IList<IdcEntity> GetAllIdcs()
        {
            const string sql = @"SELECT * FROM zsync_idc(NOLOCK) WHERE DeleteFlag = 0 order by IdcName ";

            return GetEntities(CommandType.Text, sql);
        }

        /// <summary>
        ///     插入Idc数据
        /// </summary>
        /// <param name = "idcEntity">Idc实体信息</param>
        /// <returns>记录Id</returns>
        public int Insert(IdcEntity idcEntity)
        {
            const string sql =
                @"INSERT INTO zsync_idc( IdcName, Description ) VALUES  (@IdcName, @Description)
                          select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcName", idcEntity.IdcName),
                                            new SqlParameter("@Description", idcEntity.Description)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改Idc数据
        /// </summary>
        /// <param name = "idcEntity">Idc实体信息</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(IdcEntity idcEntity)
        {
            const string sql = @"UPDATE zsync_idc SET IdcName = @IdcName, Description = @Description where Id = @IdcId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcId", idcEntity.IdcId),
                                            new SqlParameter("@IdcName", idcEntity.IdcName),
                                            new SqlParameter("@Description", idcEntity.Description)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除Idc数据
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(int idcId)
        {
            const string sql =
                @"UPDATE zsync_idc SET DeleteFlag = 1 where Id = @IdcId
                           UPDATE zsync_domain SET DeleteFlag = 1 where IdcId = @IdcId
                           UPDATE zsync_server SET DeleteFlag = 1 where DomainId in(select Id from zsync_domain where IdcId = @IdcId) ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcId", idcId)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }
    }
}