using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Dorado.Core.Encrypt
{
    public class Cryption
    {
        private const string strKey = "8635yn2G2s|l6t$fj29s92l4%5o|Ao475";

        public static string Base64Encryption(string str, string key = strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(key));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string Base64Decode(string base64Text, string key = strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(key));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string MD5(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            }
            return str;
        }

        public static string MD5(string str, int length)
        {
            string temp = MD5(str);
            if (length == 16) //16位MD5加密（取32位加密的9~25字符）
                return temp.Substring(8, 16);
            return temp;
        }
    }
}