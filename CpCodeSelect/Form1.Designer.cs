namespace CpCodeSelect
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFIlePath = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxHistory = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGetLast10record = new System.Windows.Forms.Button();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.listBoxTuiJian = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button1.Location = new System.Drawing.Point(1123, 732);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 66);
            this.button1.TabIndex = 0;
            this.button1.Text = "开始执行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(43, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "文档路径";
            // 
            // txtFIlePath
            // 
            this.txtFIlePath.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtFIlePath.Location = new System.Drawing.Point(184, 63);
            this.txtFIlePath.Name = "txtFIlePath";
            this.txtFIlePath.Size = new System.Drawing.Size(853, 32);
            this.txtFIlePath.TabIndex = 2;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnSelectFile.Location = new System.Drawing.Point(1083, 64);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(116, 31);
            this.btnSelectFile.TabIndex = 3;
            this.btnSelectFile.Text = "设置文档";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(370, 20);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(126, 25);
            this.labelError.TabIndex = 1;
            this.labelError.Text = "显示错误信息";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(43, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "推荐号码";
            // 
            // listBoxHistory
            // 
            this.listBoxHistory.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.listBoxHistory.FormattingEnabled = true;
            this.listBoxHistory.ItemHeight = 25;
            this.listBoxHistory.Items.AddRange(new object[] {
            " "});
            this.listBoxHistory.Location = new System.Drawing.Point(184, 573);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(841, 304);
            this.listBoxHistory.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(43, 650);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "当前操作历史";
            // 
            // btnGetLast10record
            // 
            this.btnGetLast10record.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnGetLast10record.Location = new System.Drawing.Point(1116, 573);
            this.btnGetLast10record.Name = "btnGetLast10record";
            this.btnGetLast10record.Size = new System.Drawing.Size(168, 31);
            this.btnGetLast10record.TabIndex = 3;
            this.btnGetLast10record.Text = "只留10条记录";
            this.btnGetLast10record.UseVisualStyleBackColor = true;
            this.btnGetLast10record.Click += new System.EventHandler(this.btnGetLast10record_Click);
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnClearHistory.Location = new System.Drawing.Point(1116, 668);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(168, 31);
            this.btnClearHistory.TabIndex = 3;
            this.btnClearHistory.Text = "清空历史记录";
            this.btnClearHistory.UseVisualStyleBackColor = true;
            this.btnClearHistory.Click += new System.EventHandler(this.btnClearHistory_Click);
            // 
            // listBoxTuiJian
            // 
            this.listBoxTuiJian.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.listBoxTuiJian.FormattingEnabled = true;
            this.listBoxTuiJian.ItemHeight = 25;
            this.listBoxTuiJian.Items.AddRange(new object[] {
            " "});
            this.listBoxTuiJian.Location = new System.Drawing.Point(184, 115);
            this.listBoxTuiJian.Name = "listBoxTuiJian";
            this.listBoxTuiJian.Size = new System.Drawing.Size(841, 404);
            this.listBoxTuiJian.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button2.Location = new System.Drawing.Point(1123, 832);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(161, 45);
            this.button2.TabIndex = 7;
            this.button2.Text = "执行文本文档";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button3.Location = new System.Drawing.Point(1124, 899);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 47);
            this.button3.TabIndex = 8;
            this.button3.Text = "执行连开计划";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1441, 1009);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBoxTuiJian);
            this.Controls.Add(this.listBoxHistory);
            this.Controls.Add(this.btnClearHistory);
            this.Controls.Add(this.btnGetLast10record);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtFIlePath);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "号码推荐";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFIlePath;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGetLast10record;
        private System.Windows.Forms.Button btnClearHistory;
        private System.Windows.Forms.ListBox listBoxTuiJian;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

