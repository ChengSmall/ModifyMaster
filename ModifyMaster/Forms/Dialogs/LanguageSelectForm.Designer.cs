
namespace Cheng.ModifyMaster
{
    partial class LanguageSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            f_disposed(disposing);

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
            this.col_columnHeader_lanDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_columnHeader_fileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // col_listView
            // 
            this.col_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_columnHeader_lanDisplayName,
            this.col_columnHeader_fileName});
            this.col_listView.Location = new System.Drawing.Point(12, 12);
            this.col_listView.Name = "col_listView";
            this.col_listView.Size = new System.Drawing.Size(438, 390);
            this.col_listView.TabIndex = 0;
            this.col_listView.UseCompatibleStateImageBehavior = false;
            this.col_listView.View = System.Windows.Forms.View.Details;
            // 
            // col_columnHeader_lanDisplayName
            // 
            this.col_columnHeader_lanDisplayName.Text = "";
            this.col_columnHeader_lanDisplayName.Width = 200;
            // 
            // col_columnHeader_fileName
            // 
            this.col_columnHeader_fileName.Text = " ";
            this.col_columnHeader_fileName.Width = 200;
            // 
            // LanguageSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 493);
            this.Controls.Add(this.col_listView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 450);
            this.Name = "LanguageSelectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Language";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView col_listView;
        private System.Windows.Forms.ColumnHeader col_columnHeader_lanDisplayName;
        private System.Windows.Forms.ColumnHeader col_columnHeader_fileName;
    }
}