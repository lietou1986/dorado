/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:50:00
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
    public class UserRoleEntity : EntityBase<UserRoleEntity>, IConvertToEntity<UserRoleEntity>
    {
        public int ID { get; set; }

        /// <summary>
        ///     角色ID
        /// </summary>
        public int roleId { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        public string UserName { get; set; }

        #region IConvertToEntity<UserRoleEntity> Members

        public UserRoleEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var userRoleEntity = new UserRoleEntity
                                         {
                                             ID = Convert.ToInt32(row["Id"]),
                                             roleId = Convert.ToInt32(row["roleId"]),
                                             UserName = row["UserName"].ToString()
                                         };
                return userRoleEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<UserRoleEntity> Members
    }
}