using System.Configuration;

namespace Dorado.Core.Encrypt
{
    public class StrEncryptor
    {
        private static string privatekey = ConfigurationManager.AppSettings["enckey"];

        public static string EncryptStr(string strText)
        {
            return WebFrameworkSymEncrypt.SymEncryptor.Encrypt(strText);
        }

        public static string DecryptStr(string strText)
        {
            return WebFrameworkSymEncrypt.SymEncryptor.Decrypt(strText);
        }
    }
}