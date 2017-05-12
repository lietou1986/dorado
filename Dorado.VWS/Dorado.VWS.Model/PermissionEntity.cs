/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:42:33
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
    public class PermissionEntity : EntityBase<PermissionEntity>, IConvertToEntity<PermissionEntity>
    {
        public int roleId { get; set; }

        /// <summary>
        ///     路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     文件类型(0文件夹1文件)
        /// </summary>
        public int Type { get; set; }

        #region IConvertToEntity<PermissionEntity> Members

        public PermissionEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var permissionEntity = new PermissionEntity
                                           {
                                               roleId = Convert.ToInt32(row["roleId"]),
                                               Path = row["Path"].ToString(),
                                               Type = Convert.ToInt32(row["Type"])
                                           };

                return permissionEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<PermissionEntity> Members
    }
}