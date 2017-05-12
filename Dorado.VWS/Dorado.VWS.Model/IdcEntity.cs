/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/5 11:26:47
 * 版本号：v1.0
 * 本类主要用途描述：IDC实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     IDC实体
    /// </summary>
    [Serializable]
    public class IdcEntity : EntityBase<IdcEntity>, IConvertToEntity<IdcEntity>
    {
        /// <summary>
        ///     idcId
        /// </summary>
        public int IdcId { get; set; }

        /// <summary>
        ///     idc名称
        /// </summary>
        public string IdcName { get; set; }

        /// <summary>
        ///     idc描述
        /// </summary>
        public string Description { get; set; }

        #region IConvertToEntity<IdcEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public IdcEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var idcEntity = new IdcEntity
                                    {
                                        IdcId = Convert.ToInt32(row["Id"]),
                                        IdcName = row["IdcName"].ToString(),
                                        Description = row["Description"].ToString()
                                    };

                return idcEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<IdcEntity> Members
    }
}