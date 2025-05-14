
namespace Cheng.ModifyMaster
{
    partial class MainForm
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
            f_disposing(disposing);

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.col_listView = new System.Windows.Forms.ListView();
            this.col_columnHeader_title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_columnHeader_toggle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_columnHeader_type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_columnHeader_value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_columnHeader_cond = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_menuStrip = new System.Windows.Forms.MenuStrip();
            this.col_menuItem_File = new System.Windows.Forms.ToolStripMenuItem();
            this.col_menuItem_File_openSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.col_menuItem_File_openProcess = new System.Windows.Forms.ToolStripMenuItem();
            this.col_menuItem_File_openProcessByID = new System.Windows.Forms.ToolStripMenuItem();
            this.col_Separator_File_1 = new System.Windows.Forms.ToolStripSeparator();
            this.col_menuItem_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.col_menuItem_setup = new System.Windows.Forms.ToolStripMenuItem();
            this.col_menuItem_setup_setting = new System.Windows.Forms.ToolStripMenuItem();
            this.col_timer = new System.Windows.Forms.Timer(this.components);
            this.col_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.col_label_title = new System.Windows.Forms.Label();
            this.col_label_ifOpenProcessText = new System.Windows.Forms.Label();
            this.col_menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // col_listView
            // 
            this.col_listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.col_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_columnHeader_title,
            this.col_columnHeader_toggle,
            this.col_columnHeader_type,
            this.col_columnHeader_value,
            this.col_columnHeader_cond});
            this.col_listView.HideSelection = false;
            this.col_listView.Location = new System.Drawing.Point(0, 95);
            this.col_listView.MultiSelect = false;
            this.col_listView.Name = "col_listView";
            this.col_listView.Size = new System.Drawing.Size(622, 326);
            this.col_listView.TabIndex = 1;
            this.col_listView.UseCompatibleStateImageBehavior = false;
            this.col_listView.View = System.Windows.Forms.View.Details;
            // 
            // col_columnHeader_title
            // 
            this.col_columnHeader_title.Text = "修改项";
            this.col_columnHeader_title.Width = 120;
            // 
            // col_columnHeader_toggle
            // 
            this.col_columnHeader_toggle.Text = "开关";
            // 
            // col_columnHeader_type
            // 
            this.col_columnHeader_type.Text = "修改类型";
            this.col_columnHeader_type.Width = 120;
            // 
            // col_columnHeader_value
            // 
            this.col_columnHeader_value.Text = "值";
            this.col_columnHeader_value.Width = 120;
            // 
            // col_columnHeader_cond
            // 
            this.col_columnHeader_cond.Text = "条件";
            this.col_columnHeader_cond.Width = 120;
            // 
            // col_menuStrip
            // 
            this.col_menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.col_menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_menuItem_File,
            this.col_menuItem_setup});
            this.col_menuStrip.Location = new System.Drawing.Point(0, 0);
            this.col_menuStrip.Name = "col_menuStrip";
            this.col_menuStrip.Size = new System.Drawing.Size(622, 28);
            this.col_menuStrip.TabIndex = 2;
            this.col_menuStrip.Text = "menuStrip";
            // 
            // col_menuItem_File
            // 
            this.col_menuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_menuItem_File_openSetup,
            this.col_menuItem_File_openProcess,
            this.col_menuItem_File_openProcessByID,
            this.col_Separator_File_1,
            this.col_menuItem_File_Exit});
            this.col_menuItem_File.Name = "col_menuItem_File";
            this.col_menuItem_File.Size = new System.Drawing.Size(71, 24);
            this.col_menuItem_File.Text = "文件(&F)";
            // 
            // col_menuItem_File_openSetup
            // 
            this.col_menuItem_File_openSetup.Name = "col_menuItem_File_openSetup";
            this.col_menuItem_File_openSetup.Size = new System.Drawing.Size(167, 26);
            this.col_menuItem_File_openSetup.Text = "打开(&O)";
            // 
            // col_menuItem_File_openProcess
            // 
            this.col_menuItem_File_openProcess.Name = "col_menuItem_File_openProcess";
            this.col_menuItem_File_openProcess.Size = new System.Drawing.Size(167, 26);
            this.col_menuItem_File_openProcess.Text = "选择进程";
            // 
            // col_menuItem_File_openProcessByID
            // 
            this.col_menuItem_File_openProcessByID.Name = "col_menuItem_File_openProcessByID";
            this.col_menuItem_File_openProcessByID.Size = new System.Drawing.Size(167, 26);
            this.col_menuItem_File_openProcessByID.Text = "打开进程ID";
            // 
            // col_Separator_File_1
            // 
            this.col_Separator_File_1.Name = "col_Separator_File_1";
            this.col_Separator_File_1.Size = new System.Drawing.Size(164, 6);
            // 
            // col_menuItem_File_Exit
            // 
            this.col_menuItem_File_Exit.Name = "col_menuItem_File_Exit";
            this.col_menuItem_File_Exit.Size = new System.Drawing.Size(167, 26);
            this.col_menuItem_File_Exit.Text = "退出(&X)";
            // 
            // col_menuItem_setup
            // 
            this.col_menuItem_setup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_menuItem_setup_setting});
            this.col_menuItem_setup.Enabled = false;
            this.col_menuItem_setup.Name = "col_menuItem_setup";
            this.col_menuItem_setup.Size = new System.Drawing.Size(72, 24);
            this.col_menuItem_setup.Text = "选项(&T)";
            this.col_menuItem_setup.Visible = false;
            // 
            // col_menuItem_setup_setting
            // 
            this.col_menuItem_setup_setting.Name = "col_menuItem_setup_setting";
            this.col_menuItem_setup_setting.Size = new System.Drawing.Size(122, 26);
            this.col_menuItem_setup_setting.Text = "设置";
            // 
            // col_label_title
            // 
            this.col_label_title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.col_label_title.Location = new System.Drawing.Point(12, 28);
            this.col_label_title.Name = "col_label_title";
            this.col_label_title.Size = new System.Drawing.Size(376, 64);
            this.col_label_title.TabIndex = 3;
            this.col_label_title.Text = "title";
            this.col_label_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // col_label_ifOpenProcessText
            // 
            this.col_label_ifOpenProcessText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.col_label_ifOpenProcessText.Location = new System.Drawing.Point(394, 28);
            this.col_label_ifOpenProcessText.Name = "col_label_ifOpenProcessText";
            this.col_label_ifOpenProcessText.Size = new System.Drawing.Size(216, 64);
            this.col_label_ifOpenProcessText.TabIndex = 4;
            this.col_label_ifOpenProcessText.Text = "未打开进程";
            this.col_label_ifOpenProcessText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 433);
            this.Controls.Add(this.col_label_ifOpenProcessText);
            this.Controls.Add(this.col_label_title);
            this.Controls.Add(this.col_listView);
            this.Controls.Add(this.col_menuStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.col_menuStrip;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "万能改";
            this.col_menuStrip.ResumeLayout(false);
            this.col_menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView col_listView;
        private System.Windows.Forms.ColumnHeader col_columnHeader_title;
        private System.Windows.Forms.ColumnHeader col_columnHeader_type;
        private System.Windows.Forms.ColumnHeader col_columnHeader_value;
        private System.Windows.Forms.ColumnHeader col_columnHeader_toggle;
        private System.Windows.Forms.ColumnHeader col_columnHeader_cond;
        private System.Windows.Forms.MenuStrip col_menuStrip;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_File;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_setup;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_File_openSetup;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_File_openProcess;
        private System.Windows.Forms.ToolStripSeparator col_Separator_File_1;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_File_Exit;
        private System.Windows.Forms.Timer col_timer;
        private System.Windows.Forms.OpenFileDialog col_openFileDialog;
        private System.Windows.Forms.Label col_label_title;
        private System.Windows.Forms.Label col_label_ifOpenProcessText;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_setup_setting;
        private System.Windows.Forms.ToolStripMenuItem col_menuItem_File_openProcessByID;
    }
}

