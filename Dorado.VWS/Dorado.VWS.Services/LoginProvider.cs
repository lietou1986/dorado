using System;
using System.IO;
//using log4net;

using System.Security.Cryptography;
using System.Text;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Utils;

namespace Dorado.VWS.Services
{
    public class LoginProvider
    {
        public bool Login(string loginname, string password, ref bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(loginname) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var isEmployee = false;
            EmployeeResult result = GetEmployee(loginname, password);

            if (result != null && !result.status.Equals("0"))
            {
                isEmployee = true;
            }
            ActivateUserProvider _activateProvider = new ActivateUserProvider();
            SysytemRoleProvider _systemRoleProvider = new SysytemRoleProvider();
            var isLogin = _activateProvider.Activate(loginname, isEmployee ? result.employee.email : "", MD5Encrypt(password), isEmployee);
            var sysrole = _systemRoleProvider.GetSysytemRoleByUser(loginname);
            if (isLogin)
                isAdmin = sysrole.Count > 0 && (sysrole[0].ID == (int)SysytemRoleEnumType.SuperAdmin);
            return isLogin;
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="loginname">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public EmployeeResult GetEmployee(string loginname, string password)
        {
            string code = string.Format("{0}{1}martin_got_an_ipad", loginname, password);
            string MD5Code = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(code, "MD5").ToLower();

            string erpUrl = "http://nw.Dorado.com/cas/login_interface.jsp?username={0}&password={1}&code={2}";
            
            erpUrl = string.Format(erpUrl, loginname, password, MD5Code);
            LoggerWrapper.Logger.Info("VWS.Site", "http://nw.Dorado.com/cas/login_interface.jsp " + loginname);
            string result = WebHttpHelper.GetWebContent(erpUrl);

            LoggerWrapper.Logger.Info("VWS.Site", result);
            EmployeeResult emplyer = XmlSerializerHelper.XmlDeserialize<EmployeeResult>(result);
            return emplyer;
        }

        public string MD5Encrypt(string inStr)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] InBytes = Encoding.GetEncoding("GB2312").GetBytes(inStr);
            byte[] OutBytes = md5.ComputeHash(InBytes);
            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString;
        }

        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="sDecrKey"></param>
        /// <returns></returns>
        public static string DESDecrypt(string inputText, string sDecrKey)
        {
            Byte[] byKey = { };
            Byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            Byte[] inputByteArray = new byte[inputText.Length];
            try
            {
                byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(inputText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}