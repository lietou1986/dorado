/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:43:38
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;

#endregion using

namespace Dorado.VWS.Model
{
    public class ResourceEntity : EntityBase<ResourceEntity>, IConvertToEntity<ResourceEntity>
    {
        public int ResourceId { get; set; }

        public string ResourceValue { get; set; }

        public string ResourceDescription { get; set; }

        #region IConvertToEntity<ResourceEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public ResourceEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var resourceEntity = new ResourceEntity
                                         {
                                             ResourceId = Convert.ToInt32(row["Id"]),
                                             ResourceValue = row["ResourceValue"].ToString(),
                                             ResourceDescription = row["Description"].ToString()
                                         };
                return resourceEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<ResourceEntity> Members
    }
}