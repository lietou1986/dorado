using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;

namespace Dorado.VWS.Services.Persistence
{
    public class ActivateUserDao : DBbase<ActivatedUser>
    {
        public IList<ActivatedUser> GetAllUsers()
        {
            const string sql = @"SELECT * FROM zsync_activateduser WHERE  DeleteFlag = 0";
            //var parameter = new SqlParameter("@RoleID", roleID);
            SqlParameter[] parameters = { };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public ActivatedUser GetUser(string userName)
        {
            const string sql = @"SELECT * FROM zsync_activateduser WHERE  DeleteFlag = 0 and UserName=@UserName";
            var parameter = new SqlParameter("@UserName", userName);
            return GetEntity(CommandType.Text, sql, parameter);
        }

        public bool HasExist(string userName)
        {
            const string sql = @"SELECT * FROM zsync_activateduser WHERE   UserName=@UserName";
            SqlParameter[] parameters = { new SqlParameter("@UserName", userName) };
            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);

            return result == 1;
        }

        public bool Activate(string username, string email, string password, bool isEmployee)
        {
            const string sql = @"proc_ActivateUser";
            SqlParameter outPut = new SqlParameter("@Result", SqlDbType.Int);
            outPut.Direction = ParameterDirection.Output;
            SqlParameter[] parameters ={
              new SqlParameter("@UserName",username),
              new SqlParameter("@Email",email),
              new SqlParameter("@IsEmployee",isEmployee),
              new SqlParameter("@Password",password),
              outPut};
            ExecuteNonQuery(CommandType.StoredProcedure, sql, parameters);
            return (int)outPut.Value == 1;
        }
    }
}