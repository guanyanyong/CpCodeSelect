namespace CpCodeSelect
{
    partial class Zu6Kill1Form
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
            this.chkLianGua = new System.Windows.Forms.CheckBox();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnmoni = new System.Windows.Forms.Button();
            this.btnMoni2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnStatistic = new System.Windows.Forms.Button();
            this.chkLianZhong = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button1.Location = new System.Drawing.Point(1377, 486);
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
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtFIlePath
            // 
            this.txtFIlePath.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtFIlePath.Location = new System.Drawing.Point(184, 63);
            this.txtFIlePath.Name = "txtFIlePath";
            this.txtFIlePath.Size = new System.Drawing.Size(853, 32);
            this.txtFIlePath.TabIndex = 2;
            this.txtFIlePath.TextChanged += new System.EventHandler(this.txtFIlePath_TextChanged);
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
            this.listBoxHistory.Location = new System.Drawing.Point(184, 486);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(1140, 254);
            this.listBoxHistory.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(43, 563);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "当前操作历史";
            // 
            // btnGetLast10record
            // 
            this.btnGetLast10record.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnGetLast10record.Location = new System.Drawing.Point(1370, 269);
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
            this.btnClearHistory.Location = new System.Drawing.Point(1370, 329);
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
            this.listBoxTuiJian.Size = new System.Drawing.Size(1140, 329);
            this.listBoxTuiJian.TabIndex = 6;
            // 
            // chkLianGua
            // 
            this.chkLianGua.AutoSize = true;
            this.chkLianGua.Checked = true;
            this.chkLianGua.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLianGua.Location = new System.Drawing.Point(1370, 130);
            this.chkLianGua.Name = "chkLianGua";
            this.chkLianGua.Size = new System.Drawing.Size(48, 16);
            this.chkLianGua.TabIndex = 9;
            this.chkLianGua.Text = "连挂";
            this.chkLianGua.UseVisualStyleBackColor = true;
            // 
            // btnRestart
            // 
            this.btnRestart.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnRestart.Location = new System.Drawing.Point(1544, 486);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(161, 66);
            this.btnRestart.TabIndex = 0;
            this.btnRestart.Text = "重新开始";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnReset.Location = new System.Drawing.Point(1377, 591);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(161, 66);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnmoni
            // 
            this.btnmoni.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnmoni.Location = new System.Drawing.Point(184, 762);
            this.btnmoni.Name = "btnmoni";
            this.btnmoni.Size = new System.Drawing.Size(197, 45);
            this.btnmoni.TabIndex = 8;
            this.btnmoni.Text = "显示模拟执行信息";
            this.btnmoni.UseVisualStyleBackColor = true;
            this.btnmoni.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnMoni2
            // 
            this.btnMoni2.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnMoni2.Location = new System.Drawing.Point(387, 762);
            this.btnMoni2.Name = "btnMoni2";
            this.btnMoni2.Size = new System.Drawing.Size(197, 45);
            this.btnMoni2.TabIndex = 8;
            this.btnMoni2.Text = "显示模拟2执行信息";
            this.btnMoni2.UseVisualStyleBackColor = true;
            this.btnMoni2.Click += new System.EventHandler(this.btnMoni2_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button4.Location = new System.Drawing.Point(601, 762);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(197, 45);
            this.button4.TabIndex = 8;
            this.button4.Text = "显示模拟3执行信息";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnStatistic
            // 
            this.btnStatistic.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnStatistic.Location = new System.Drawing.Point(1544, 591);
            this.btnStatistic.Name = "btnStatistic";
            this.btnStatistic.Size = new System.Drawing.Size(161, 66);
            this.btnStatistic.TabIndex = 0;
            this.btnStatistic.Text = "显示统计信息";
            this.btnStatistic.UseVisualStyleBackColor = true;
            this.btnStatistic.Click += new System.EventHandler(this.btnStatistic_Click);
            // 
            // chkLianZhong
            // 
            this.chkLianZhong.AutoSize = true;
            this.chkLianZhong.Checked = true;
            this.chkLianZhong.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLianZhong.Location = new System.Drawing.Point(1370, 162);
            this.chkLianZhong.Name = "chkLianZhong";
            this.chkLianZhong.Size = new System.Drawing.Size(48, 16);
            this.chkLianZhong.TabIndex = 9;
            this.chkLianZhong.Text = "连中";
            this.chkLianZhong.UseVisualStyleBackColor = true;
            // 
            // Zu6Kill1Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1741, 843);
            this.Controls.Add(this.chkLianZhong);
            this.Controls.Add(this.chkLianGua);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnMoni2);
            this.Controls.Add(this.btnmoni);
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
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnStatistic);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.button1);
            this.Name = "Zu6Kill1Form";
            this.Text = "组六杀1码推荐";
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
        private System.Windows.Forms.CheckBox chkLianGua;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnmoni;
        private System.Windows.Forms.Button btnMoni2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnStatistic;
        private System.Windows.Forms.CheckBox chkLianZhong;
    }
}

