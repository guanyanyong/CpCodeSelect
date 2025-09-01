namespace CpCodeSelect
{
    partial class LianKaiForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.listBoxHistory = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetDataFile = new System.Windows.Forms.Button();
            this.txtDataFIlePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button1.Location = new System.Drawing.Point(1223, 424);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 66);
            this.button1.TabIndex = 15;
            this.button1.Text = "开始执行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // listBoxHistory
            // 
            this.listBoxHistory.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.listBoxHistory.FormattingEnabled = true;
            this.listBoxHistory.ItemHeight = 25;
            this.listBoxHistory.Items.AddRange(new object[] {
            " "});
            this.listBoxHistory.Location = new System.Drawing.Point(259, 312);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(841, 304);
            this.listBoxHistory.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(118, 389);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 25);
            this.label3.TabIndex = 13;
            this.label3.Text = "当前操作历史";
            // 
            // btnSetDataFile
            // 
            this.btnSetDataFile.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnSetDataFile.Location = new System.Drawing.Point(1016, 81);
            this.btnSetDataFile.Name = "btnSetDataFile";
            this.btnSetDataFile.Size = new System.Drawing.Size(161, 31);
            this.btnSetDataFile.TabIndex = 12;
            this.btnSetDataFile.Text = "设置数据文档";
            this.btnSetDataFile.UseVisualStyleBackColor = true;
            // 
            // txtDataFIlePath
            // 
            this.txtDataFIlePath.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtDataFIlePath.Location = new System.Drawing.Point(259, 81);
            this.txtDataFIlePath.Name = "txtDataFIlePath";
            this.txtDataFIlePath.Size = new System.Drawing.Size(727, 32);
            this.txtDataFIlePath.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(118, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "文档路径";
            // 
            // LianKaiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1593, 1070);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBoxHistory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSetDataFile);
            this.Controls.Add(this.txtDataFIlePath);
            this.Controls.Add(this.label1);
            this.Name = "LianKaiForm";
            this.Text = "LianKaiForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBoxHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSetDataFile;
        private System.Windows.Forms.TextBox txtDataFIlePath;
        private System.Windows.Forms.Label label1;
    }
}