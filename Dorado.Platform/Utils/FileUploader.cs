/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/8/31
 * 时间: 16:55
 *
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Dorado.Platform.Utils
{
    /// <summary>
    /// 文件上传(用于移动端等非web上传)
    /// </summary>
    public class FileUploader
    {
        private readonly string _address;
        private readonly int _timeout;

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="address">上传地址</param>
        /// <param name="timeout">上传超时时间</param>
        public FileUploader(string address, int timeout = 300)
        {
            _address = address;
            _timeout = timeout;
        }

        public string UploadByWebClient(string filePath, Action<string> callBack)
        {
            Guard.ArgumentNotNullOrEmpty(_address);
            Guard.ArgumentNotNullOrEmpty(filePath);

            WebClient myWebClient = new WebClient();
            byte[] responseArray = myWebClient.UploadFile(_address, "POST", filePath);
            string result = Encoding.GetEncoding("UTF-8").GetString(responseArray);
            return result;
        }

        public string Upload(string filePath, Action<string> callBack)
        {
            return Upload(filePath, new FileInfo(filePath).Name, callBack);
        }

        public string Upload(string filePath, string saveName, Action<string> callBack)
        {
            Guard.ArgumentNotNullOrEmpty(_address);
            Guard.ArgumentNotNullOrEmpty(filePath);
            Guard.ArgumentNotNullOrEmpty(saveName);

            // 要上传的文件
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            //时间戳
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

            //请求头部信息
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(saveName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            // 根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(_address));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存
            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（300秒）
            httpReq.Timeout = _timeout * 1000;
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            httpReq.ContentLength = length;
            try
            {
                //每次上传4k
                const int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                if (s != null)
                {
                    StreamReader sr = new StreamReader(s);
                    //读取服务器端返回的消息
                    string result = sr.ReadToEnd();
                    s.Close();
                    sr.Close();
                    if (callBack != null) callBack(result);
                    return result;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                fs.Close();
                r.Close();
            }
        }
    }
}