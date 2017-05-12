using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dorado.VWS.Admin
{
    public partial class ReadLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                tbDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindHours();
            }
        }

        /// <summary>
        /// 绑定小时
        /// </summary>
        private void BindHours()
        {
            ddlBegin.Items.Clear();
            ddlEnd.Items.Clear();

            ddlEnd.Items.Add(new ListItem("现在", ""));
            for (int i = 0; i < 24; i++)
            {
                ddlBegin.Items.Add(new ListItem(i + ":00", i.ToString()));
                ddlEnd.Items.Add(new ListItem(i + ":00", i.ToString()));
            }

            ddlBegin.SelectedIndex = Math.Max(0, DateTime.Now.Hour);
            ddlEnd.SelectedIndex = 0;
        }

        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRead_Click(object sender, EventArgs e)
        {
            tbResult.Text = string.Empty;
            ltInfo.Text = DateTime.Now.ToString();

            DateTime date = DateTime.Today;
            if (!DateTime.TryParse(tbDate.Text, out date))
            {
                tbResult.Text = "日期格式错误！";
                return;
            }

            DateTime begin = date.AddHours(Convert.ToByte(ddlBegin.SelectedValue));
            DateTime end = date.AddDays(1);
            if (ddlEnd.SelectedIndex != 0)
            {
                end = date.AddHours(Convert.ToByte(ddlEnd.SelectedValue) + 1);
            }
            bool isBegin = false;

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"log\" + date.ToString("yyyy-MM-dd") + ".log";
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    ltInfo.Text = "文件最后更新于" + fi.LastWriteTime.ToString() + " 文件大小" + (fi.Length / 1024) + "KB";

                    StringBuilder sb = new StringBuilder();
                    using (FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GB2312"));
                        string line = string.Empty;
                        DateTime logTime;
                        bool containsTime = false;
                        while (sr.Peek() > 0)
                        {
                            line = sr.ReadLine();
                            containsTime = GetTimeFromLog(line, out logTime);
                            if (isBegin)
                            {
                                if (containsTime && logTime > end)
                                {
                                    break;
                                }
                                sb.AppendLine(line);
                            }
                            else if (containsTime && logTime >= begin)
                            {
                                isBegin = true;
                                sb.AppendLine(line);
                            }
                        }
                        fs.Close();
                    }

                    tbResult.Text = sb.ToString();
                }
                else
                {
                    tbResult.Text = "文件[" + path + "]不存在！";
                }
            }
            catch (Exception ex)
            {
                tbResult.Text = "读取出错\r\n" + ex.ToString();
            }
        }

        /// <summary>
        /// 获取日志中的时间
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool GetTimeFromLog(string log, out DateTime time)
        {
            time = DateTime.MinValue;

            if (!string.IsNullOrEmpty(log) && log.Length > 19)
            {
                return DateTime.TryParse(log.Substring(0, 19), out time);
            }

            return false;
        }
    }
}