/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：共通帮助类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;

#endregion using

namespace Dorado.VWS.Utils
{
    public class CommonHelper
    {
        /// <summary>
        ///     计算文件的 MD5 值
        /// </summary>
        /// <param name = "fileName">要计算 MD5 值的文件名和路径</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string MD5File(string fileName)
        {
            return HashFile(fileName, "md5");
        }

        public static string MD5Buffer(byte[] buffer)
        {
            return HashBuffer(buffer, "md5");
        }

        /// <summary>
        ///     获取指定服务的状态
        /// </summary>
        /// <param name = "serviceName">服务名</param>
        /// <returns>0：不存在此服务；1：运行中； 2：其它状态</returns>
        public static int ServiceStatus(string serviceName)
        {
            ServiceController[] scs = ServiceController.GetServices();
            ServiceController sc =
                scs.Where(x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (sc == null) return 0;
            if (ServiceControllerStatus.Running.Equals(sc.Status))
            {
                return 1;
            }
            return 2;
        }

        /// <summary>
        ///     将string类型转换为IList
        /// </summary>
        /// <param name = "data"></param>
        /// <returns></returns>
        public static IList<string> ConvertByComma(string data)
        {
            return data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///     计算文件的哈希值
        /// </summary>
        /// <param name = "fileName">要计算哈希值的文件名和路径</param>
        /// <param name = "algName">算法:sha1,md5</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(string fileName, string algName)
        {
            if (!File.Exists(fileName))
                return string.Empty;

            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] hashBytes = HashData(fs, algName);
            fs.Close();
            return ByteArrayToHexString(hashBytes);
        }

        private static string HashBuffer(byte[] buffer, string algName)
        {
            byte[] hashBytes = HashData(buffer, algName);
            return ByteArrayToHexString(hashBytes);
        }

        /// <summary>
        ///     计算哈希值
        /// </summary>
        /// <param name = "stream">要计算哈希值的 Stream</param>
        /// <param name = "algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(Stream stream, string algName)
        {
            HashAlgorithm algorithm = GetAlgorithm(algName);
            return algorithm.ComputeHash(stream);
        }

        /// <summary>
        ///     计算哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希值的 buffer</param>
        /// <param name = "algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(byte[] buffer, string algName)
        {
            HashAlgorithm algorithm = GetAlgorithm(algName);
            return algorithm.ComputeHash(buffer);
        }

        private static HashAlgorithm GetAlgorithm(string algName)
        {
            HashAlgorithm algorithm;
            if (algName == null)
            {
                throw new ArgumentNullException("algName");
            }
            if (string.Compare(algName, "sha1", true) == 0)
            {
                algorithm = SHA1.Create();
            }
            else
            {
                if (string.Compare(algName, "md5", true) != 0)
                {
                    throw new Exception("algName 只能使用 sha1 或 md5");
                }
                algorithm = MD5.Create();
            }
            return algorithm;
        }

        /// <summary>
        ///     字节数组转换为16进制表示的字符串
        /// </summary>
        private static string ByteArrayToHexString(byte[] buf)
        {
            var ss = new StringBuilder();
            for (int i = 0; i < buf.Length; i++)
            {
                ss.Append(buf[i].ToString("X2"));
            }

            return ss.ToString();
        }
    }

    ///   <summary>
    ///   生成随机字符串的密封类，不能被继承
    ///   </summary>
    public class RandomStr
    {
        private static readonly int defaultLength = 8;

        private static int GetNewSeed()
        {
            byte[] rndBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }

        private static string BuildRndCodeAll(int strLen)
        {
            System.Random RandomObj = new System.Random(GetNewSeed());
            string buildRndCodeReturn = null;
            for (int i = 0; i < strLen; i++)
            {
                buildRndCodeReturn += (char)RandomObj.Next(33, 125);
            }
            return buildRndCodeReturn;
        }

        #region 输出随机字符串

        ///   <summary>
        ///   输出长度为8的随机字符串
        ///   </summary>
        ///   <returns>长度为8的随机字符串</returns>
        public static string GetRndStrOfAll()
        {
            return BuildRndCodeAll(defaultLength);
        }

        ///   <summary>
        ///   输出指定长度的随机字符串
        ///   </summary>
        ///   <param   name="LenOf">长度</param>
        ///   <returns>指定长度的随机字符串</returns>
        public static string GetRndStrOfAll(int LenOf)
        {
            return BuildRndCodeAll(LenOf);
        }

        #endregion 输出随机字符串

        private static string sCharLow = "abcdefghijklmnopqrstuvwxyz";
        private static string sCharUpp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string sNumber = "0123456789";

        private static string BuildRndCodeOnly(string StrOf, int strLen)
        {
            System.Random RandomObj = new System.Random(GetNewSeed());
            string buildRndCodeReturn = null;
            for (int i = 0; i < strLen; i++)
            {
                buildRndCodeReturn += StrOf.Substring(RandomObj.Next(0, StrOf.Length - 1), 1);
            }
            return buildRndCodeReturn;
        }

        #region 输出指定范围随机字符串

        ///   <summary>
        ///   输出长度为8的小写字母加数字的字符串
        ///   </summary>
        ///   <returns>长度为8的小写字母加数字的字符串</returns>
        public static string GetRndStrOnlyFor()
        {
            return BuildRndCodeOnly(sCharLow + sNumber, defaultLength);
        }

        ///   <summary>
        ///   输出指定长度的小写字母加数字的字符串
        ///   </summary>
        ///   <param   name="LenOf">长度</param>
        ///   <returns>指定长度的小写字母加数字的字符串</returns>
        public static string GetRndStrOnlyFor(int LenOf)
        {
            return BuildRndCodeOnly(sCharLow + sNumber, LenOf);
        }

        ///   <summary>
        ///   输出长度为8的指定字符串
        ///   </summary>
        ///   <param   name="bUseUpper">是否含有大写字母</param>
        ///   <param   name="bUseNumber">是否含有数字</param>
        ///   <returns>长度为8的指定字符串</returns>
        public static string GetRndStrOnlyFor(bool bUseUpper, bool bUseNumber)
        {
            string strTmp = sCharLow;
            if (bUseUpper) strTmp += sCharUpp;
            if (bUseNumber) strTmp += sNumber;

            return BuildRndCodeOnly(strTmp, defaultLength);
        }

        ///   <summary>
        ///   输出指定长度的指定字符串
        ///   </summary>
        ///   <param   name="LenOf">长度</param>
        ///   <param   name="bUseUpper">是否含有大写字母</param>
        ///   <param   name="bUseNumber">是否含有数字</param>
        ///   <returns>指定长度的指定字符串</returns>
        public static string GetRndStrOnlyFor(int LenOf, bool bUseUpper, bool bUseNumber)
        {
            string strTmp = sCharLow;
            if (bUseUpper) strTmp += sCharUpp;
            if (bUseNumber) strTmp += sNumber;

            return BuildRndCodeOnly(strTmp, LenOf);
        }

        #endregion 输出指定范围随机字符串
    }
}