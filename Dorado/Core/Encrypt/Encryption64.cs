using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dorado.Core.Encrypt
{
    public class Encryption64
    {
        public static string encryptQueryString(string strQueryString)
        {
            return Encryption64.Encrypt(strQueryString, "!#$a54?3");
        }

        public static string decryptQueryString(string strQueryString)
        {
            return Encryption64.Decrypt(strQueryString, "!#$a54?3");
        }

        public static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            stringToDecrypt = stringToDecrypt.Replace("NBAM", "+");
            byte[] key = new byte[0];
            byte[] IV = new byte[]
			{
				10,
				20,
				30,
				40,
				50,
				60,
				70,
				80
			};
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            string @string;
            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                @string = encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return @string;
        }

        public static string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {
            byte[] key = new byte[0];
            byte[] IV = new byte[]
			{
				10,
				20,
				30,
				40,
				50,
				60,
				70,
				80
			};
            string result;
            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                ms.ToArray();
                string strValue = Convert.ToBase64String(ms.ToArray()).Replace("+", "NBAM");
                result = strValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}