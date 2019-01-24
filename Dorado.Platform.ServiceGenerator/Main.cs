using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dorado.Platform.ServicesGenerator
{
    public partial class Main : Form
    {
        private bool _isGenOver;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ddlStartType.SelectedIndex = 0;
        }

        private void btnOutputDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = txtOutputDir.Text
            };
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtOutputDir.Text = folderDlg.SelectedPath;
            }
        }

        private void btnGenerator_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServiceName.Text))
            {
                MessageBox.Show("请填写服务名称");
                txtServiceName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtDisplayName.Text))
            {
                MessageBox.Show("请填写服务显示名称");
                txtDisplayName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("请填写服务描述");
                txtDescription.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtOutputDir.Text))
            {
                MessageBox.Show("请填写或选择代码生成目录");
                btnOutputDir_Click(null, null);
                return;
            }

            _isGenOver = false;

            backWork.RunWorkerAsync();

            System.Threading.Tasks.Task.Factory.StartNew(this.GenService);

            btnGenerator.Enabled = false;
            btnGenerator.Text = "正在生成...";
        }

        private void backWork_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!(_isGenOver))
            {
                Thread.Sleep(100);
            }
        }

        private void backWork_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            btnGenerator.Enabled = true;
            btnGenerator.Text = "开始生成";
            MessageBox.Show("生成完毕");
            OpenOutputDir();
        }

        private void GenService()
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            _isGenOver = true;
        }

        private void OpenOutputDir()
        {
            Process.Start(txtOutputDir.Text);
        }
    }
}