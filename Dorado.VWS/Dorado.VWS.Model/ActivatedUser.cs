using System;

namespace Dorado.VWS.Model
{
    /// <summary>
    /// domain user
    /// </summary>
    public class ActivatedUser : EntityBase<ActivatedUser>, IConvertToEntity<ActivatedUser>
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime CreateTime { get; set; }

        public bool DeleteFlag { get; set; }

        public bool UserMasterFlag { get; set; }

        public ActivatedUser ConvertToEntity(System.Data.DataRow row)
        {
            if (row != null)
            {
                var user = new ActivatedUser
                {
                    UserName = row["UserName"].ToString(),
                    Email = row["Email"].ToString(),
                    CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                    DeleteFlag = bool.Parse(row["DeleteFlag"].ToString()),
                    UserMasterFlag = bool.Parse(row["UserMasterFlag"].ToString()),
                };

                return user;
            }
            return null;
        }
    }
}