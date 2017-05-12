/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:44:33
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
    public class RoleEntity : EntityBase<RoleEntity>, IConvertToEntity<RoleEntity>
    {
        public int Id { get; set; }

        /// <summary>
        ///     环境
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        ///     域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        ///     角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        ///     域名ID
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public int UserId { get; set; }

        #region IConvertToEntity<RoleEntity> Members

        public RoleEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var roleEntity = new RoleEntity
                                     {
                                         Environment = row["Environment"].ToString(),
                                         Domain = row["DomainName"].ToString(),
                                         Id = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                                         RoleName = row["RoleName"].ToString(),
                                         DomainId = Convert.ToInt32(row["DomainID"])
                                     };
                return roleEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<RoleEntity> Members
    }
}