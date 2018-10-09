using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Data;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Dorado.Tool
{
    public partial class FDbTool : Form
    {
        public FDbTool()
        {
            InitializeComponent();
        }

        private void FDbTool_Load(object sender, EventArgs e)
        {
            foreach (var conn in ConnectionStringCollection.Instance.Entries)
            {
                cbConn.Items.Add(conn.Name);
            }
            cbConn.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("您确定要初始化主键吗？", "所有表初始化主键", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    button1.Enabled = false;
                    button1.Text = "正在添加...";
                    using (Conn conn = new Conn(ConnectionStringProvider.Get(cbConn.Text)))
                    {
                        string sql = "select id,name,xtype,crdate from sysobjects where xtype='U' and name not like 'dt_%' order by name";
                        DataArray data = conn.Select(sql);
                        while (data.Read())
                        {
                            try
                            {
                                string tableName = data["name"].ToString();

                                sql = string.Format("SELECT CONSTRAINT_NAME pkname FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{0}' ", tableName);
                                DataArray pkData = conn.Select(sql);
                                if (!pkData.IsEmpty)
                                {
                                    //先删除主键
                                    sql = string.Format("alter table {0} drop constraint {1};", tableName, pkData["pkname"].ToString());
                                    conn.ExecuteNonQuery(sql);
                                }

                                sql = string.Format("Select name from syscolumns Where Id = OBJECT_ID('{0}') and name = 'id'", tableName);
                                DataArray columnData = conn.Select(sql);
                                if (columnData.IsEmpty)
                                {
                                    sql = string.Format("ALTER TABLE {0} ADD AutoId bigint IdENTITY(1,1) NOT NULL;alter table {0} add Id bigint not null;alter table {0} add constraint PK_{0}_Id primary key (Id);", tableName);
                                    conn.ExecuteNonQuery(sql);
                                }
                                else
                                {
                                    sql = string.Format("exec sp_rename '{0}.Id','AutoId';alter table {0} alter column AutoId bigint not null;alter table {0} add Id bigint null;", tableName);
                                    conn.ExecuteNonQuery(sql);
                                    sql = string.Format("update {0} set Id=AutoId;", tableName);
                                    conn.ExecuteNonQuery(sql);
                                    sql = string.Format("alter table {0} alter column Id bigint not null;", tableName);
                                    conn.ExecuteNonQuery(sql);
                                    sql = string.Format("alter table {0} add constraint PK_{0}_Id primary key (Id);", tableName);
                                    conn.ExecuteNonQuery(sql);
                                }
                            }
                            catch (Exception ex) { LoggerWrapper.Logger.Error("初始化主键错误", ex); }
                        }
                    }

                    MessageBox.Show("操作完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    button1.Enabled = false;
                    button1.Text = "初始化主键";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("您确定要清洗数据吗？", "清洗数据", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    button2.Enabled = false;
                    button2.Text = "正在清洗数据...";
                    using (Conn conn = new Conn(ConnectionStringProvider.Get(cbConn.Text)))
                    {
                        string sql = "select id,name,xtype,crdate from sysobjects where xtype='U' and name not like 'dt_%' order by name";
                        DataArray data = conn.Select(sql);
                        while (data.Read())
                        {
                            try
                            {
                                string tableName = data["name"].ToString();

                                sql = string.Format("SELECT CONSTRAINT_NAME pkname FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{0}' ", tableName);
                                DataArray pkData = conn.Select(sql);
                                if (!pkData.IsEmpty)
                                {
                                    //先删除主键
                                    sql = string.Format("alter table {0} drop constraint {1};", tableName, pkData["pkname"].ToString());
                                    conn.ExecuteNonQuery(sql);
                                }

                                sql = string.Format("Select name from syscolumns Where Id = OBJECT_ID('{0}') and name = 'id'", tableName);
                                DataArray columnData = conn.Select(sql);
                                if (columnData.IsEmpty)
                                {
                                    sql = string.Format("ALTER TABLE {0} ADD AutoId bigint IdENTITY(1,1) NOT NULL;alter table {0} add Id bigint not null;alter table {0} add constraint PK_{0}_Id primary key (Id);", tableName);
                                    conn.ExecuteNonQuery(sql);
                                }
                                else
                                {
                                    sql = string.Format("exec sp_rename '{0}.Id','AutoId';alter table {0} alter column AutoId bigint not null;alter table {0} add Id bigint null;", tableName);
                                    conn.ExecuteNonQuery(sql);
                                    sql = string.Format("update {0} set Id=AutoId;", tableName);
                                    conn.ExecuteNonQuery(sql);
                                    sql = string.Format("alter table {0} alter column Id bigint not null;", tableName);
                                    conn.ExecuteNonQuery(sql);
                                    sql = string.Format("alter table {0} add constraint PK_{0}_Id primary key (Id);", tableName);
                                    conn.ExecuteNonQuery(sql);
                                }
                            }
                            catch (Exception ex) { LoggerWrapper.Logger.Error("清洗数据错误", ex); }
                        }
                    }

                    MessageBox.Show("操作完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    button2.Enabled = false;
                    button2.Text = "清洗数据";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("您确定要创建索引吗？", "索引创建", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    button3.Enabled = false;
                    button3.Text = "正在创建索引...";
                    using (Conn conn = new Conn(ConnectionStringProvider.Get(cbConn.Text)))
                    {
                        string sql = "select id,name,xtype,crdate from sysobjects where xtype='U' and name not like 'dt_%' order by name";
                        DataArray data = conn.Select(sql);
                        while (data.Read())
                        {
                            try
                            {
                                string tableName = data["name"].ToString();

                                List<string> columns = new List<string>();
                                //columns.Add("AutoId");
                                columns.Add("UserType");
                                columns.Add("ParentId");
                                columns.Add("City");
                                columns.Add("Region");
                                columns.Add("Province");
                                columns.ForEach(n =>
                                {
                                    sql = string.Format("Select name from syscolumns Where Id = OBJECT_ID('{0}') and name = '{1}'", tableName, n);
                                    DataArray columnData = conn.Select(sql);
                                    if (!columnData.IsEmpty)
                                    {
                                        string indexName = string.Format("IX_{0}_{1}", tableName, n);
                                        sql = string.Format("drop Index {2} on {0};create Index {2} on {0}({1} ASC);", tableName, n, indexName);
                                        conn.ExecuteNonQuery(sql);
                                    }
                                });
                            }
                            catch (Exception ex) { LoggerWrapper.Logger.Error("索引创建错误", ex); }
                        }
                    }

                    MessageBox.Show("操作完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    button3.Enabled = false;
                    button3.Text = "索引创建";
                }
            }
        }
    }
}