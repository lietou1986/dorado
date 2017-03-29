using System;
using System.Security.Cryptography;
using System.Text;

namespace Dorado.Core.Encrypt
{
    public class HmacMD5
    {
        public static string HMAC(string text, string key)
        {
            string ipad = string.Empty;
            string opad = string.Empty;
            string iResult = string.Empty;
            string oResult = string.Empty;
            for (int i = 0; i < 64; i++)
            {
                ipad += "6";
                opad += "\\";
            }
            int KLen = key.Length;
            for (int j = 0; j < 64; j++)
            {
                if (j < KLen)
                {
                    iResult += Convert.ToChar((int)(ipad[j] ^ key[j]));
                }
                else
                {
                    iResult += Convert.ToChar(ipad[j]);
                }
            }
            iResult += text;
            iResult = HmacMD5.fun_MD5(iResult);
            byte[] Test = HmacMD5.Hexstr2Array(iResult);
            iResult = string.Empty;
            char[] b = Encoding.GetEncoding(1252).GetChars(Test);
            for (int k = 0; k < b.Length; k++)
            {
                iResult += b[k];
            }
            for (int l = 0; l < 64; l++)
            {
                if (l < KLen)
                {
                    oResult += Convert.ToChar((int)(opad[l] ^ key[l]));
                }
                else
                {
                    oResult += Convert.ToChar(opad[l]);
                }
            }
            oResult += iResult;
            return HmacMD5.fun_MD5(oResult);
        }

        private static string fun_MD5(string str)
        {
            byte[] byteArray = Encoding.GetEncoding(1252).GetBytes(str);
            byteArray = new MD5CryptoServiceProvider().ComputeHash(byteArray);
            string ret = "";
            for (int i = 0; i < byteArray.Length; i++)
            {
                ret += byteArray[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        private static byte[] Hexstr2Array(string HexStr)
        {
            string HEX = "0123456789ABCDEF";
            string str = HexStr.ToUpper();
            int len = str.Length;
            byte[] retByte = new byte[len / 2];
            for (int i = 0; i < len / 2; i++)
            {
                int numHigh = HEX.IndexOf(str[i * 2]);
                int numLow = HEX.IndexOf(str[i * 2 + 1]);
                retByte[i] = Convert.ToByte(numHigh * 16 + numLow);
            }
            return retByte;
        }
    }
}