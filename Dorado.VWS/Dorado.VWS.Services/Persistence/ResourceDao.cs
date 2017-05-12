/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/9/1 15:29:04
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：
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
    public class ResourceDao : DBbase<ResourceEntity>
    {
        /// <summary>
        ///     获取资源信息
        /// </summary>
        /// <param name = "resourceId">资源Id</param>
        /// <returns>资源实体</returns>
        public ResourceEntity GetResourceById(int resourceId)
        {
            const string sql = @"SELECT * FROM zsync_resource(NOLOCK) WHERE Id = @ResourceId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@ResourceId", resourceId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取所有可用资源
        /// </summary>
        /// <returns></returns>
        public IList<ResourceEntity> GetResourceList()
        {
            const string sql = @"SELECT * FROM zsync_resource(NOLOCK) WHERE DeleteFlag = 0 order by Description";

            return GetEntities(CommandType.Text, sql);
        }

        /// <summary>
        ///     插入资源数据
        /// </summary>
        /// <param name = "resoureEntity">资源实体信息</param>
        /// <returns>记录Id</returns>
        public int Insert(ResourceEntity resoureEntity)
        {
            const string sql =
                @"INSERT INTO zsync_resource( ResourceValue, Description ) VALUES  (@ResourceValue, @Description)
                          select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@ResourceValue", resoureEntity.ResourceValue),
                                            new SqlParameter("@Description", resoureEntity.ResourceDescription)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改资源数据
        /// </summary>
        /// <param name = "resoureEntity">资源实体信息</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(ResourceEntity resoureEntity)
        {
            const string sql =
                @"UPDATE zsync_resource SET ResourceValue = @ResourceValue, Description = @Description where Id = @ResourceId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@ResourceId", resoureEntity.ResourceId),
                                            new SqlParameter("@ResourceValue", resoureEntity.ResourceValue),
                                            new SqlParameter("@Description", resoureEntity.ResourceDescription)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除资源数据
        /// </summary>
        /// <param name = "resourceId">资源Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(int resourceId)
        {
            const string sql = @"UPDATE zsync_resource SET DeleteFlag = 1 where Id = @ResourceId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@ResourceId", resourceId)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     验证资源值是否存在
        /// </summary>
        /// <param name = "resourceEntity">资源实体</param>
        /// <returns>已经存在返回true，反之返回false</returns>
        public bool ExistsResource(ResourceEntity resourceEntity)
        {
            var sql =
                @"SELECT count(Id) FROM zsync_resource
                          WHERE ResourceValue = @ResourceValue and DeleteFlag = 0  ";

            var parameters = new List<SqlParameter> { new SqlParameter("@ResourceValue", resourceEntity.ResourceValue) };

            if (resourceEntity.ResourceId >= 1)
            {
                sql += "and ID <> @ResourceId";
                parameters.Add(new SqlParameter("@ResourceId", resourceEntity.ResourceId));
            }

            var result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));
            return result >= 1;
        }
    }
}