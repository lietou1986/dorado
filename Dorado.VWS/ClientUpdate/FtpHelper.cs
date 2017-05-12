using System;
using System.IO;
using System.Net;
using System.Text;

namespace ClientUpdate
{
    public class FtpHelper
    {
        private string ftpUserID;
        private string ftpPassword;

        public FtpHelper(string ftpUserID, string ftpPassword)
        {
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }

        /// <summary>
        /// 单个文件下载方法
        /// </summary>
        /// <param name="adss">保存文件的本地路径</param>
        /// <param name="ftpadss">下载文件的FTP路径</param>
        public void download(string adss, string ftpadss)
        {
            //FileMode常数确定如何打开或创建文件,指定操作系统应创建新文件。
            //FileMode.Create如果文件已存在，它将被改写
            FileStream outputStream = new FileStream(adss, FileMode.Create);
            FtpWebRequest downRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpadss));
            downRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            //设置要发送到 FTP 服务器的命令
            downRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse response = (FtpWebResponse)downRequest.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            long cl = response.ContentLength;
            int bufferSize = 2048;
            int readCount;
            byte[] buffer = new byte[bufferSize];
            readCount = ftpStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                outputStream.Write(buffer, 0, readCount);
                readCount = ftpStream.Read(buffer, 0, bufferSize);
            }
            ftpStream.Close();
            outputStream.Close();
            response.Close();
        }

        /// </summary>
        /// <param name="ftpads">FTP地址路径</param>
        /// <param name="name">我们所选择的文件或者文件夹名字</param>
        /// <param name="type">要发送到FTP服务器的命令</param>
        /// <returns></returns>
        public string[] ftp(string ftpads, string name, string type)
        {
            WebResponse webresp = null;
            StreamReader ftpFileListReader = null;
            FtpWebRequest ftpRequest = null;
            try
            {
                Uri uri = new Uri(ftpads + name);
                ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
                ftpRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftpRequest.Method = type;
                webresp = ftpRequest.GetResponse();
                ftpFileListReader = new StreamReader(webresp.GetResponseStream(), Encoding.Default);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            StringBuilder str = new StringBuilder();
            string line = ftpFileListReader.ReadLine();
            while (line != null)
            {
                str.Append(line);
                str.Append("\n");
                line = ftpFileListReader.ReadLine();
            }
            string[] fen = str.ToString().Split('\n');
            return fen;
        }

        /// <summary>
        /// 下载方法KO
        /// </summary>
        /// <param name="ftpads">FTP路径</param>
        /// <param name="name">需要下载文件路径</param>
        /// <param name="Myads">保存的本地路径</param>
        public void downftp(string ftpads, string name, string Myads)
        {
            string downloadDir = Myads + name;
            string ftpdir = ftpads + name;
            string[] fullname = ftp(ftpads, name, WebRequestMethods.Ftp.ListDirectoryDetails);
            //判断是否为单个文件
            if (fullname.Length <= 2)
            {
                if (fullname[fullname.Length - 1] == "")
                {
                    download(downloadDir + "/" + name, ftpads + name + "/" + name);
                }
            }
            else
            {
                string[] onlyname = ftp(ftpads, name, WebRequestMethods.Ftp.ListDirectory);
                if (!Directory.Exists(downloadDir))
                {
                    Directory.CreateDirectory(downloadDir);
                }
                foreach (string names in fullname)
                {
                    //判断是否具有文件夹标识<DIR>
                    if (names.Contains("<DIR>"))
                    {
                        string olname = names.Split(new string[] { "<DIR>" },
                        StringSplitOptions.None)[1].Trim();
                        downftp(ftpdir, "//" + olname, downloadDir);
                    }
                    else
                    {
                        foreach (string onlynames in onlyname)
                        {
                            if (onlynames == "" || onlynames == " " || names == "")
                            {
                                break;
                            }
                            else
                            {
                                if (names.Contains(" " + onlynames))
                                {
                                    download(downloadDir + "/" + onlynames, ftpads + name + "/" +
                                    onlynames);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}