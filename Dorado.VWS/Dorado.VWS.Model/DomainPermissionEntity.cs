/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2011/11/24 10:19:52
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0    2011/11/24 10:19:52               创建
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Data;

namespace Dorado.VWS.Model
{
    public class DomainPermissionEntity : EntityBase<DomainPermissionEntity>, IConvertToEntity<DomainPermissionEntity>
    {
        /*字段
         [ID]
      ,[DomainID]
      ,[UserName]
      ,[PermissionType]
      ,[DeleteFlag]
      ,[AddTime]
      ,[AddUserName]
      ,[UpdateTime]
      ,[UpdateUserName]
         */

        public int ID { get; set; }

        /// <summary>
        /// 域名id
        /// </summary>
        public int DomainID { get; set; }

        /// <summary>
        /// 被授权的域账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 权限类型 Dorado.VWS.Model.Enum. EnumManageType
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public bool DeleteFlag { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 进行授权的用户
        /// </summary>
        public string AddUserName { get; set; }

        /// <summary>
        /// 更新授权的时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 跟新授权的用户
        /// </summary>
        public string UpdateUserName { get; set; }

        #region IConvertToEntity<DomainPermissionEntity> Members

        public DomainPermissionEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var domainPermissionEntity = new DomainPermissionEntity
                {
                    ID = Convert.ToInt32(row["ID"]),
                    DomainID = Convert.ToInt32(row["DomainID"]),
                    UserName = row["UserName"].ToString(),
                    PermissionType = Convert.ToInt32(row["PermissionType"]),
                    DeleteFlag = Convert.ToBoolean(row["DeleteFlag"]),
                    AddTime = Convert.ToDateTime(row["AddTime"]),
                    AddUserName = row["AddUserName"].ToString(),
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                    UpdateUserName = row["UpdateUserName"].ToString(),
                };

                return domainPermissionEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<DomainPermissionEntity> Members
    }
}