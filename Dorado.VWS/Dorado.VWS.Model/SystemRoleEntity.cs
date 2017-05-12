using System;
using System.Data;

namespace Dorado.VWS.Model
{
    public class SystemRoleEntity : EntityBase<SystemRoleEntity>, IConvertToEntity<SystemRoleEntity>
    {
        public int ID { get; set; }

        /// <summary>
        ///     权限名称
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     权限名称
        /// </summary>
        public bool DeleteFlag { get; set; }

        #region IConvertToEntity<SystemRoleEntity> Members

        public SystemRoleEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var userRoleEntity = new SystemRoleEntity
                {
                    ID = Convert.ToInt32(row["Id"]),
                    DeleteFlag = Convert.ToBoolean(row["DeleteFlag"]),
                    Description = row["Description"].ToString()
                };
                return userRoleEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<SystemRoleEntity> Members
    }
}