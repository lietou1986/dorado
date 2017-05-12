/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:47:14
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
    public class UserResoureEntity : EntityBase<UserResoureEntity>, IConvertToEntity<UserResoureEntity>
    {
        public int UserroleId { get; set; }

        public int ResourceId { get; set; }

        public string ResourceValue { get; set; }

        public string ResourceDescription { get; set; }

        #region IConvertToEntity<UserResoureEntity> Members

        public UserResoureEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var userResoureEntity = new UserResoureEntity
                                            {
                                                UserroleId = Convert.ToInt32(row["UserroleId"]),
                                                ResourceId = Convert.ToInt32(row["ResourceID"]),
                                                ResourceValue = row["ResourceValue"].ToString(),
                                                ResourceDescription = row["Description"].ToString()
                                            };
                return userResoureEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<UserResoureEntity> Members
    }
}