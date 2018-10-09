namespace Dorado.Tool
{
    partial class FMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.lblConn = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnOutputDir = new System.Windows.Forms.Button();
            this.btnGenerator = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.tableContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableGenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.procContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.procGenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.procSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sqlGenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblSpace = new System.Windows.Forms.Label();
            this.txtSpace = new System.Windows.Forms.TextBox();
            this.backWork = new System.ComponentModel.BackgroundWorker();
            this.cbConn = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dBToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbPageSql = new System.Windows.Forms.TabPage();
            this.gvSql = new System.Windows.Forms.DataGridView();
            this.clName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbPageProc = new System.Windows.Forms.TabPage();
            this.gvProc = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabDBObject = new System.Windows.Forms.TabControl();
            this.procContextMenu.SuspendLayout();
            this.sqlContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tbPageSql.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvSql)).BeginInit();
            this.tbPageProc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvProc)).BeginInit();
            this.tabDBObject.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblConn
            // 
            this.lblConn.AutoSize = true;
            this.lblConn.Location = new System.Drawing.Point(13, 43);
            this.lblConn.Name = "lblConn";
            this.lblConn.Size = new System.Drawing.Size(77, 12);
            this.lblConn.TabIndex = 1;
            this.lblConn.Text = "数据库实例：";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOutputDir.Location = new System.Drawing.Point(15, 469);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(438, 21);
            this.txtOutputDir.TabIndex = 3;
            this.txtOutputDir.Text = "c:\\\\DoradoCode";
            // 
            // btnOutputDir
            // 
            this.btnOutputDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOutputDir.Location = new System.Drawing.Point(466, 468);
            this.btnOutputDir.Name = "btnOutputDir";
            this.btnOutputDir.Size = new System.Drawing.Size(75, 23);
            this.btnOutputDir.TabIndex = 4;
            this.btnOutputDir.Text = "输出目录";
            this.btnOutputDir.UseVisualStyleBackColor = true;
            this.btnOutputDir.Click += new System.EventHandler(this.btnOutputDir_Click);
            // 
            // btnGenerator
            // 
            this.btnGenerator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerator.Location = new System.Drawing.Point(234, 500);
            this.btnGenerator.Name = "btnGenerator";
            this.btnGenerator.Size = new System.Drawing.Size(96, 44);
            this.btnGenerator.TabIndex = 5;
            this.btnGenerator.Text = "开始生成";
            this.btnGenerator.UseVisualStyleBackColor = true;
            this.btnGenerator.Click += new System.EventHandler(this.btnGenerator_Click);
            // 
            // btnInit
            // 
            this.btnInit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInit.Location = new System.Drawing.Point(436, 30);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(99, 78);
            this.btnInit.TabIndex = 6;
            this.btnInit.Text = "加载对象";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // tableContextMenu
            // 
            this.tableContextMenu.Name = "tableContextMenu";
            this.tableContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // tableGenToolStripMenuItem
            // 
            this.tableGenToolStripMenuItem.Name = "tableGenToolStripMenuItem";
            this.tableGenToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // tableSelectAllToolStripMenuItem
            // 
            this.tableSelectAllToolStripMenuItem.Name = "tableSelectAllToolStripMenuItem";
            this.tableSelectAllToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // procContextMenu
            // 
            this.procContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.procGenToolStripMenuItem,
            this.procSelectAllToolStripMenuItem});
            this.procContextMenu.Name = "contextMenuStrip1";
            this.procContextMenu.Size = new System.Drawing.Size(125, 48);
            // 
            // procGenToolStripMenuItem
            // 
            this.procGenToolStripMenuItem.Name = "procGenToolStripMenuItem";
            this.procGenToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.procGenToolStripMenuItem.Text = "生成代码";
            this.procGenToolStripMenuItem.Click += new System.EventHandler(this.procGenToolStripMenuItem_Click);
            // 
            // procSelectAllToolStripMenuItem
            // 
            this.procSelectAllToolStripMenuItem.Name = "procSelectAllToolStripMenuItem";
            this.procSelectAllToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.procSelectAllToolStripMenuItem.Text = "选择全部";
            this.procSelectAllToolStripMenuItem.Click += new System.EventHandler(this.procSelectAllToolStripMenuItem_Click);
            // 
            // sqlContextMenu
            // 
            this.sqlContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sqlGenToolStripMenuItem,
            this.sqlSelectAllToolStripMenuItem});
            this.sqlContextMenu.Name = "contextMenuStrip1";
            this.sqlContextMenu.Size = new System.Drawing.Size(125, 48);
            // 
            // sqlGenToolStripMenuItem
            // 
            this.sqlGenToolStripMenuItem.Name = "sqlGenToolStripMenuItem";
            this.sqlGenToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.sqlGenToolStripMenuItem.Text = "生成代码";
            this.sqlGenToolStripMenuItem.Click += new System.EventHandler(this.sqlGenToolStripMenuItem_Click);
            // 
            // sqlSelectAllToolStripMenuItem
            // 
            this.sqlSelectAllToolStripMenuItem.Name = "sqlSelectAllToolStripMenuItem";
            this.sqlSelectAllToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.sqlSelectAllToolStripMenuItem.Text = "选择全部";
            this.sqlSelectAllToolStripMenuItem.Click += new System.EventHandler(this.sqlSelectAllToolStripMenuItem_Click);
            // 
            // lblSpace
            // 
            this.lblSpace.AutoSize = true;
            this.lblSpace.Location = new System.Drawing.Point(16, 81);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(65, 12);
            this.lblSpace.TabIndex = 7;
            this.lblSpace.Text = "命名空间：";
            // 
            // txtSpace
            // 
            this.txtSpace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSpace.Location = new System.Drawing.Point(108, 77);
            this.txtSpace.Name = "txtSpace";
            this.txtSpace.Size = new System.Drawing.Size(322, 21);
            this.txtSpace.TabIndex = 8;
            this.txtSpace.Text = "Dorado.Platform.Data";
            // 
            // backWork
            // 
            this.backWork.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backWork_DoWork);
            this.backWork.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backWork_RunWorkerCompleted);
            // 
            // cbConn
            // 
            this.cbConn.FormattingEnabled = true;
            this.cbConn.Location = new System.Drawing.Point(108, 40);
            this.cbConn.Name = "cbConn";
            this.cbConn.Size = new System.Drawing.Size(322, 20);
            this.cbConn.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dBToolsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(553, 25);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dBToolsToolStripMenuItem
            // 
            this.dBToolsToolStripMenuItem.Name = "dBToolsToolStripMenuItem";
            this.dBToolsToolStripMenuItem.Size = new System.Drawing.Size(76, 21);
            this.dBToolsToolStripMenuItem.Text = "DBHelper";
            this.dBToolsToolStripMenuItem.Click += new System.EventHandler(this.dBToolsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // tbPageSql
            // 
            this.tbPageSql.Controls.Add(this.gvSql);
            this.tbPageSql.Location = new System.Drawing.Point(4, 22);
            this.tbPageSql.Name = "tbPageSql";
            this.tbPageSql.Size = new System.Drawing.Size(518, 308);
            this.tbPageSql.TabIndex = 2;
            this.tbPageSql.Text = "自定义Sql";
            this.tbPageSql.UseVisualStyleBackColor = true;
            // 
            // gvSql
            // 
            this.gvSql.AllowUserToAddRows = false;
            this.gvSql.AllowUserToDeleteRows = false;
            this.gvSql.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSql.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clName,
            this.clDate});
            this.gvSql.Location = new System.Drawing.Point(7, 6);
            this.gvSql.Name = "gvSql";
            this.gvSql.ReadOnly = true;
            this.gvSql.RowTemplate.Height = 23;
            this.gvSql.Size = new System.Drawing.Size(505, 332);
            this.gvSql.TabIndex = 1;
            this.gvSql.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gvSql_CellMouseDown);
            // 
            // clName
            // 
            this.clName.HeaderText = "名称";
            this.clName.Name = "clName";
            this.clName.ReadOnly = true;
            this.clName.Width = 200;
            // 
            // clDate
            // 
            this.clDate.HeaderText = "时间";
            this.clDate.Name = "clDate";
            this.clDate.ReadOnly = true;
            this.clDate.Width = 240;
            // 
            // tbPageProc
            // 
            this.tbPageProc.Controls.Add(this.gvProc);
            this.tbPageProc.Location = new System.Drawing.Point(4, 22);
            this.tbPageProc.Name = "tbPageProc";
            this.tbPageProc.Padding = new System.Windows.Forms.Padding(3);
            this.tbPageProc.Size = new System.Drawing.Size(518, 308);
            this.tbPageProc.TabIndex = 1;
            this.tbPageProc.Text = "存储过程";
            this.tbPageProc.UseVisualStyleBackColor = true;
            // 
            // gvProc
            // 
            this.gvProc.AllowUserToAddRows = false;
            this.gvProc.AllowUserToDeleteRows = false;
            this.gvProc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvProc.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.gvProc.Location = new System.Drawing.Point(7, 6);
            this.gvProc.Name = "gvProc";
            this.gvProc.ReadOnly = true;
            this.gvProc.RowTemplate.Height = 23;
            this.gvProc.Size = new System.Drawing.Size(505, 332);
            this.gvProc.TabIndex = 2;
            this.gvProc.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gvProc_CellMouseDown);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "ID";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "名称";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 200;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "时间";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 240;
            // 
            // tabDBObject
            // 
            this.tabDBObject.Controls.Add(this.tbPageProc);
            this.tabDBObject.Controls.Add(this.tbPageSql);
            this.tabDBObject.Location = new System.Drawing.Point(15, 115);
            this.tabDBObject.Name = "tabDBObject";
            this.tabDBObject.SelectedIndex = 0;
            this.tabDBObject.Size = new System.Drawing.Size(526, 334);
            this.tabDBObject.TabIndex = 2;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 556);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.cbConn);
            this.Controls.Add(this.txtSpace);
            this.Controls.Add(this.lblSpace);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.btnGenerator);
            this.Controls.Add(this.btnOutputDir);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.tabDBObject);
            this.Controls.Add(this.lblConn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FMain";
            this.Text = "Dorado Tool";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.procContextMenu.ResumeLayout(false);
            this.sqlContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tbPageSql.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvSql)).EndInit();
            this.tbPageProc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvProc)).EndInit();
            this.tabDBObject.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConn;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnOutputDir;
        private System.Windows.Forms.Button btnGenerator;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.ContextMenuStrip tableContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tableGenToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip procContextMenu;
        private System.Windows.Forms.ToolStripMenuItem procGenToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip sqlContextMenu;
        private System.Windows.Forms.ToolStripMenuItem sqlGenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tableSelectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem procSelectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sqlSelectAllToolStripMenuItem;
        private System.Windows.Forms.Label lblSpace;
        private System.Windows.Forms.TextBox txtSpace;
        private System.ComponentModel.BackgroundWorker backWork;
        private System.Windows.Forms.ComboBox cbConn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dBToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabPage tbPageSql;
        private System.Windows.Forms.DataGridView gvSql;
        private System.Windows.Forms.DataGridViewTextBoxColumn clName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDate;
        private System.Windows.Forms.TabPage tbPageProc;
        private System.Windows.Forms.DataGridView gvProc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.TabControl tabDBObject;
    }
}

