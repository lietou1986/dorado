/*-------------------------------------------------------------------------
 * 版权所有：凡客诚品（北京）科技有限公司
 * 版本：v1.0
 * 时间： 2011/7/29 11:44:03
 * 作者：蔡昌艳（Bruce Tscai）
 * 联系方式：caichangyan@vancl.cn
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.ServiceProcess;

namespace Vancl.IC.VWS.ClientService
{
    public class Common
    {
        private static ServiceController _iisService = new ServiceController("iisadmin");
        /// <summary>
        /// 计算文件的 MD5 值
        /// </summary>
        /// <param name="fileName">要计算 MD5 值的文件名和路径</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string MD5File(string fileName)
        {
            return HashFile(fileName, "md5");
        }

        /// <summary>
        /// 获取IIS状态
        /// </summary>
        /// <returns>1：运行中。 2：其它状态</returns>
        public static int IISStatus()
        {
            if (ServiceControllerStatus.Running.Equals(_iisService.Status))
            {
                return 1;
            }
            return 2;
        }

        /// <summary>
        /// 将string类型转换为IList
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IList<string> ConvertByComma(string data)
        {
            return data.Split(new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #region private

        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="fileName">要计算哈希值的文件名和路径</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(string fileName, string algName)
        {
            if (!System.IO.File.Exists(fileName))
                return string.Empty;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] hashBytes = HashData(fs, algName);
            fs.Close();
            return ByteArrayToHexString(hashBytes);
        }


        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(Stream stream, string algName)
        {
            HashAlgorithm algorithm;
            if (algName == null)
            {
                throw new ArgumentNullException("algName 不能为 null");
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
            return algorithm.ComputeHash(stream);
        }

        /// <summary>

        /// 字节数组转换为16进制表示的字符串

        /// </summary>

        private static string ByteArrayToHexString(byte[] buf)
        {
            //int iLen = 0;

            //// 通过反射获取 MachineKeySection 中的 ByteArrayToHexString 方法，该方法用于将字节数组转换为16进制表示的字符串。
            //Type type = typeof(System.Web.Configuration.MachineKeySection);

            //MethodInfo byteArrayToHexString = type.GetMethod("ByteArrayToHexString", BindingFlags.Static | BindingFlags.NonPublic);

            //// 字节数组转换为16进制表示的字符串
            //return (string)byteArrayToHexString.Invoke(null, new object[] { buf, iLen });

            StringBuilder ss = new StringBuilder();
            for (int i = 0; i < buf.Length; i++)
            {
                ss.Append(buf[i].ToString("X2"));
            }

            return ss.ToString();
        }

        #endregion

    }
}
