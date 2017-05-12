using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

namespace Dorado.VWS.Services
{
    /// <summary>
    /// 激活用户类
    /// </summary>
    public class ActivateUserProvider
    {
        private readonly ActivateUserDao _activateDao = new ActivateUserDao();

        public IList<ActivatedUser> GetAllUsers()
        {
            return _activateDao.GetAllUsers();
        }

        public ActivatedUser GetActivatedUser(string userName)
        {
            return _activateDao.GetUser(userName);
        }

        public bool HasExist(string userName)
        {
            return _activateDao.HasExist(userName);
        }

        public bool Activate(string username, string email, string password, bool isEmployee)
        {
            return _activateDao.Activate(username, email, password, isEmployee);
        }
    }
}