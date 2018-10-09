using Dorado.Core.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dorado.Tool
{
    public partial class FTool : Form
    {
        public FTool()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMsmqAddress.Text))
                {
                    MessageBox.Show("队列地址不能为空");
                    txtMsmqAddress.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMsmqBody.Text))
                {
                    MessageBox.Show("队列内容不能为空");
                    txtMsmqBody.Focus();
                    return;
                }

                using (IQueue<string> queue = new MsmqQueue<string>(txtMsmqAddress.Text, chkTrans.Checked))
                {
                    string[] msmqList = txtMsmqBody.Text.Split(new char[] { '\r', '\n' });
                    foreach (string msmq in msmqList)
                    {
                        queue.Push(msmq);
                    }
                }
                MessageBox.Show("ok");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出错了", MessageBoxButtons.OK);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "文本文件(*.txt;*.log;*.csv;)|*.txt;*.log;*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = textBox1.Text;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("请选择要去重的文件");
                return;
            }

            TextReader reader = File.OpenText(filePath);

            int i = 0;
            SortedSet<string> previousLines = new SortedSet<string>();

            string currentLine;
            while (!string.IsNullOrWhiteSpace(currentLine = reader.ReadLine()))
            {
                currentLine = currentLine.TrimEnd();
                if (!previousLines.Add(currentLine))
                {
                    i++;
                }
            }
            reader.Close();
            reader.Dispose();

            TextWriter writer = File.CreateText(filePath + ".result");

            foreach (string s in previousLines)
            {
                writer.WriteLine(s);
            }

            writer.Close();
            writer.Dispose();

            MessageBox.Show(string.Format("去重完毕，重复记录数->{0}个", i));
        }
    }
}