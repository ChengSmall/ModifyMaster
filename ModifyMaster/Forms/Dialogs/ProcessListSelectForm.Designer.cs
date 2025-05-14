
namespace Cheng.ModifyMaster
{
    partial class ProcessListSelectForm
    {
        /// <summary>
        /// 必需的设计器变量
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源
        /// </summary>
        /// <param name="disposing">如果应处置托管资源，则为true；否则为false</param>
        protected override void Dispose(bool disposing)
        {
            f_disposeing(disposing);
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.col_listView = new System.Windows.Forms.ListView();
            this.col_columnHeader_pro = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_columnHeader_proID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_button_ok = new System.Windows.Forms.Button();
            this.col_button_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // col_listView
            // 
            this.col_listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.col_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_columnHeader_pro,
            this.col_columnHeader_proID});
            this.col_listView.HideSelection = false;
            this.col_listView.Location = new System.Drawing.Point(12, 12);
            this.col_listView.MultiSelect = false;
            this.col_listView.Name = "col_listView";
            this.col_listView.Size = new System.Drawing.Size(438, 467);
            this.col_listView.TabIndex = 0;
            this.col_listView.UseCompatibleStateImageBehavior = false;
            this.col_listView.View = System.Windows.Forms.View.Details;
            // 
            // col_columnHeader_pro
            // 
            this.col_columnHeader_pro.Text = "进程";
            this.col_columnHeader_pro.Width = 150;
            // 
            // col_columnHeader_proID
            // 
            this.col_columnHeader_proID.Text = "ID";
            this.col_columnHeader_proID.Width = 150;
            // 
            // col_button_ok
            // 
            this.col_button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.col_button_ok.Location = new System.Drawing.Point(92, 506);
            this.col_button_ok.Name = "col_button_ok";
            this.col_button_ok.Size = new System.Drawing.Size(100, 35);
            this.col_button_ok.TabIndex = 1;
            this.col_button_ok.Text = "确定";
            this.col_button_ok.UseVisualStyleBackColor = true;
            // 
            // col_button_cancel
            // 
            this.col_button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.col_button_cancel.Location = new System.Drawing.Point(262, 506);
            this.col_button_cancel.Name = "col_button_cancel";
            this.col_button_cancel.Size = new System.Drawing.Size(100, 35);
            this.col_button_cancel.TabIndex = 2;
            this.col_button_cancel.Text = "取消";
            this.col_button_cancel.UseVisualStyleBackColor = true;
            // 
            // ProcessListSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 553);
            this.Controls.Add(this.col_button_cancel);
            this.Controls.Add(this.col_button_ok);
            this.Controls.Add(this.col_listView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 480);
            this.Name = "ProcessListSelectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择进程";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView col_listView;
        private System.Windows.Forms.ColumnHeader col_columnHeader_pro;
        private System.Windows.Forms.ColumnHeader col_columnHeader_proID;
        private System.Windows.Forms.Button col_button_ok;
        private System.Windows.Forms.Button col_button_cancel;
    }
}