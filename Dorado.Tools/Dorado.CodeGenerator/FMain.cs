using Dorado.Configuration;
using Dorado.Core.Data;
using Dorado.Tool.SqlSettings;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dorado.Tool
{
    public partial class FMain : Form
    {
        private bool _isGenTableOver;
        private bool _isGenProcOver;
        private bool _isGenSqlOver;

        private static SqlSettingsConfiguration Config
        {
            get { return SqlSettingsConfigurationManager.Config; }
        }

        public FMain()
        {
            InitializeComponent();
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            foreach (var conn in ConnectionStringCollection.Instance.Entries)
            {
                cbConn.Items.Add(conn.Name);
            }
            cbConn.SelectedIndex = 0;
        }

        private void btnGenerator_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutputDir.Text))
            {
                MessageBox.Show("请输入或选择代码生成目录");
                btnOutputDir_Click(null, null);
                return;
            }
            if (string.IsNullOrEmpty(txtSpace.Text))
            {
                MessageBox.Show("请输入命名空间");
                txtSpace.Focus();
                return;
            }

            _isGenProcOver = false;
            _isGenSqlOver = false;

            backWork.RunWorkerAsync();

            Task.Factory.StartNew(GenProcCode);
            Task.Factory.StartNew(GenSqlCode);

            btnGenerator.Enabled = false;
            btnGenerator.Text = "正在生成...";
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

        private void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
                gvSql.Rows.Clear();
                gvProc.Rows.Clear();
                using (Conn conn = new Conn(ConnectionStringProvider.Get(cbConn.Text)))
                {
                    string sql = "select id,name,xtype,crdate from sysobjects where xtype='P' and name not like 'dt_%' order by name";
                    DataArray data = conn.Select(sql);

                    while (data.Read())
                    {
                        gvProc.Rows.Add(data["id"].ToString(), data["name"].ToString(), data["crdate"].ToString());
                    }
                }
                Config.Sqls.SqlList.ForEach(n =>
                    {
                        gvSql.Rows.Add(n.Name, DateTime.Now.ToString());
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gvProc_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex <= -1) return;
            if (e.Button == MouseButtons.Right)
                this.procContextMenu.Show(MousePosition.X, MousePosition.Y);
            else
                gvProc.Rows[e.RowIndex].Selected = !gvProc.Rows[e.RowIndex].Selected;
        }

        private void gvSql_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex <= -1) return;
            if (e.Button == MouseButtons.Right)
                this.sqlContextMenu.Show(MousePosition.X, MousePosition.Y);
            else
                gvSql.Rows[e.RowIndex].Selected = !gvSql.Rows[e.RowIndex].Selected;
        }

        private void procSelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gvProc.SelectAll();
        }

        private void sqlSelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gvSql.SelectAll();
        }

        private void sqlGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gvSql.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择数据");
                return;
            }
            foreach (DataGridViewRow row in gvSql.SelectedRows)
            {
                string id = row.Cells[0].Value.ToString();
                string code = GenSqlClass.Gen(cbConn.Text, id, txtSpace.Text);
                string codeDir = Path.Combine(txtOutputDir.Text, "Sql");
                if (!Directory.Exists(codeDir))
                    Directory.CreateDirectory(codeDir);
                string codeFile = Path.Combine(codeDir, row.Cells[0].Value.ToString() + ".cs");
                File.Delete(codeFile);
                File.AppendAllText(codeFile, code, Encoding.UTF8);
            }
            MessageBox.Show("生成完毕");
            OpenOutputDir();
        }

        private void procGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gvProc.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择数据");
                return;
            }
            foreach (DataGridViewRow row in gvProc.SelectedRows)
            {
                string id = row.Cells[0].Value.ToString();
                string code = GenProcClass.Gen(cbConn.Text, id, txtSpace.Text);
                string codeDir = Path.Combine(txtOutputDir.Text, "Proc");
                if (!Directory.Exists(codeDir))
                    Directory.CreateDirectory(codeDir);
                string codeFile = Path.Combine(codeDir, row.Cells[1].Value.ToString() + ".cs");
                File.Delete(codeFile);
                File.AppendAllText(codeFile, code, Encoding.UTF8);
            }
            MessageBox.Show("生成完毕");
            OpenOutputDir();
        }

        private void OpenOutputDir()
        {
            Process.Start(txtOutputDir.Text);
        }

        private void backWork_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!(_isGenTableOver && _isGenProcOver && _isGenSqlOver))
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

        /// <summary>
        /// 生成存储过程操作代码
        /// </summary>
        private void GenProcCode()
        {
            try
            {
                using (Conn conn = new Conn(ConnectionStringProvider.Get(cbConn.Text)))
                {
                    string sql = "select id,name,xtype,crdate from sysobjects where xtype='P' and name not like 'dt_%' order by name";
                    DataArray data = conn.Select(sql);
                    while (data.Read())
                    {
                        string code = GenProcClass.Gen(cbConn.Text, data["id"].ToString(), txtSpace.Text);
                        string codeDir = Path.Combine(txtOutputDir.Text, "Proc");
                        if (!Directory.Exists(codeDir))
                            Directory.CreateDirectory(codeDir);
                        string codeFile = Path.Combine(codeDir, data["name"].ToString() + ".cs");
                        File.Delete(codeFile);
                        File.AppendAllText(codeFile, code, Encoding.UTF8);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            _isGenProcOver = true;
        }

        /// <summary>
        /// 生成自定义sql操作代码
        /// </summary>
        private void GenSqlCode()
        {
            _isGenSqlOver = true;
        }

        private void dBToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FDbTool form = new FDbTool();
            form.Show();
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTool form = new FTool();
            form.Show();
        }
    }
}