namespace CpCodeSelect
{
    partial class Hou2Select50AutoForm
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFIlePath = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRestart = new System.Windows.Forms.Button();
            this.listBoxHistory = new System.Windows.Forms.ListBox();
            this.chkRefersh = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txt50Number = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.CodeQiHao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodeNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GuaCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZhongGount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Numer50 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblTotalNumber = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblMaxGua = new System.Windows.Forms.Label();
            this.autoClkTimer = new System.Windows.Forms.Timer(this.components);
            this.btnStartAuto = new System.Windows.Forms.Button();
            this.btnStopAuto = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownAutoClick = new System.Windows.Forms.NumericUpDown();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoClick)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.button1.Location = new System.Drawing.Point(1220, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "开始执行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(43, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "文档路径";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtFIlePath
            // 
            this.txtFIlePath.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtFIlePath.Location = new System.Drawing.Point(184, 22);
            this.txtFIlePath.Name = "txtFIlePath";
            this.txtFIlePath.Size = new System.Drawing.Size(853, 32);
            this.txtFIlePath.TabIndex = 2;
            this.txtFIlePath.TextChanged += new System.EventHandler(this.txtFIlePath_TextChanged);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnSelectFile.Location = new System.Drawing.Point(1083, 23);
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
            this.labelError.Location = new System.Drawing.Point(1572, 25);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(126, 25);
            this.labelError.TabIndex = 1;
            this.labelError.Text = "显示错误信息";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(6, 356);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "当前操作历史";
            // 
            // btnRestart
            // 
            this.btnRestart.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnRestart.Location = new System.Drawing.Point(1387, 23);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(161, 31);
            this.btnRestart.TabIndex = 0;
            this.btnRestart.Text = "重新开始";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // listBoxHistory
            // 
            this.listBoxHistory.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.listBoxHistory.FormattingEnabled = true;
            this.listBoxHistory.ItemHeight = 25;
            this.listBoxHistory.Items.AddRange(new object[] {
            " "});
            this.listBoxHistory.Location = new System.Drawing.Point(166, 291);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(1139, 454);
            this.listBoxHistory.TabIndex = 5;
            // 
            // chkRefersh
            // 
            this.chkRefersh.AutoSize = true;
            this.chkRefersh.Location = new System.Drawing.Point(1086, 73);
            this.chkRefersh.Name = "chkRefersh";
            this.chkRefersh.Size = new System.Drawing.Size(120, 16);
            this.chkRefersh.TabIndex = 7;
            this.chkRefersh.Text = "开出新号是否刷新";
            this.chkRefersh.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.tabControl1.Location = new System.Drawing.Point(58, 118);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1478, 849);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblMaxGua);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.lblTotalNumber);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.listBoxHistory);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 37);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1470, 808);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "总体信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txt50Number);
            this.tabPage2.Controls.Add(this.btnClear);
            this.tabPage2.Controls.Add(this.btnCopy);
            this.tabPage2.Controls.Add(this.btnStopAuto);
            this.tabPage2.Controls.Add(this.btnStartAuto);
            this.tabPage2.Controls.Add(this.btnSelect);
            this.tabPage2.Controls.Add(this.numericUpDownAutoClick);
            this.tabPage2.Controls.Add(this.numericUpDown1);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 37);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1470, 808);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "显示信息";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txt50Number
            // 
            this.txt50Number.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.txt50Number.Location = new System.Drawing.Point(975, 153);
            this.txt50Number.Multiline = true;
            this.txt50Number.Name = "txt50Number";
            this.txt50Number.Size = new System.Drawing.Size(406, 631);
            this.txt50Number.TabIndex = 4;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.btnClear.Location = new System.Drawing.Point(791, 600);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(158, 184);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.btnCopy.Location = new System.Drawing.Point(791, 153);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(158, 381);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "拷贝数值";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.btnSelect.Location = new System.Drawing.Point(343, 63);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(123, 39);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "搜索";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.numericUpDown1.Location = new System.Drawing.Point(157, 63);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 36);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.label2.Location = new System.Drawing.Point(50, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "连挂次数";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CodeQiHao,
            this.CodeNumber,
            this.GuaCount,
            this.ZhongGount,
            this.Numer50});
            this.dataGridView1.Location = new System.Drawing.Point(45, 141);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(700, 661);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // CodeQiHao
            // 
            this.CodeQiHao.DataPropertyName = "CodeQiHao";
            this.CodeQiHao.HeaderText = "期号";
            this.CodeQiHao.Name = "CodeQiHao";
            // 
            // CodeNumber
            // 
            this.CodeNumber.DataPropertyName = "CodeNumber";
            this.CodeNumber.HeaderText = "期号开奖号";
            this.CodeNumber.Name = "CodeNumber";
            // 
            // GuaCount
            // 
            this.GuaCount.DataPropertyName = "GuaCount";
            this.GuaCount.HeaderText = "连挂次数";
            this.GuaCount.Name = "GuaCount";
            // 
            // ZhongGount
            // 
            this.ZhongGount.DataPropertyName = "ZhongGount";
            this.ZhongGount.HeaderText = "连中次数";
            this.ZhongGount.Name = "ZhongGount";
            // 
            // Numer50
            // 
            this.Numer50.DataPropertyName = "Numer50";
            this.Numer50.HeaderText = "50号码";
            this.Numer50.Name = "Numer50";
            this.Numer50.Text = "查看50个号码";
            this.Numer50.UseColumnTextForButtonValue = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(56, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 30);
            this.label4.TabIndex = 6;
            this.label4.Text = "当前走势总数量:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // lblTotalNumber
            // 
            this.lblTotalNumber.AutoSize = true;
            this.lblTotalNumber.Location = new System.Drawing.Point(234, 54);
            this.lblTotalNumber.Name = "lblTotalNumber";
            this.lblTotalNumber.Size = new System.Drawing.Size(0, 30);
            this.lblTotalNumber.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(527, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(150, 30);
            this.label6.TabIndex = 6;
            this.label6.Text = "当前最大遗漏:";
            // 
            // lblMaxGua
            // 
            this.lblMaxGua.AutoSize = true;
            this.lblMaxGua.Location = new System.Drawing.Point(700, 54);
            this.lblMaxGua.Name = "lblMaxGua";
            this.lblMaxGua.Size = new System.Drawing.Size(0, 30);
            this.lblMaxGua.TabIndex = 6;
            // 
            // btnStartAuto
            // 
            this.btnStartAuto.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.btnStartAuto.Location = new System.Drawing.Point(499, 63);
            this.btnStartAuto.Name = "btnStartAuto";
            this.btnStartAuto.Size = new System.Drawing.Size(123, 39);
            this.btnStartAuto.TabIndex = 3;
            this.btnStartAuto.Text = "开始自动点击";
            this.btnStartAuto.UseVisualStyleBackColor = true;
            this.btnStartAuto.Click += new System.EventHandler(this.btnStartAuto_Click);
            // 
            // btnStopAuto
            // 
            this.btnStopAuto.Enabled = false;
            this.btnStopAuto.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.btnStopAuto.Location = new System.Drawing.Point(659, 63);
            this.btnStopAuto.Name = "btnStopAuto";
            this.btnStopAuto.Size = new System.Drawing.Size(123, 39);
            this.btnStopAuto.TabIndex = 3;
            this.btnStopAuto.Text = "结束自动点击";
            this.btnStopAuto.UseVisualStyleBackColor = true;
            this.btnStopAuto.Click += new System.EventHandler(this.btnStopAuto_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.label5.Location = new System.Drawing.Point(813, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(255, 30);
            this.label5.TabIndex = 1;
            this.label5.Text = "自动点击间隔时间单位秒";
            // 
            // numericUpDownAutoClick
            // 
            this.numericUpDownAutoClick.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.numericUpDownAutoClick.Location = new System.Drawing.Point(1093, 68);
            this.numericUpDownAutoClick.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.numericUpDownAutoClick.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownAutoClick.Name = "numericUpDownAutoClick";
            this.numericUpDownAutoClick.Size = new System.Drawing.Size(120, 36);
            this.numericUpDownAutoClick.TabIndex = 2;
            this.numericUpDownAutoClick.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // Hou2Select50AutoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1782, 1023);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkRefersh);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtFIlePath);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.button1);
            this.Name = "Hou2Select50AutoForm";
            this.Text = "组六杀1码推荐";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoClick)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFIlePath;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.ListBox listBoxHistory;
        private System.Windows.Forms.CheckBox chkRefersh;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.TextBox txt50Number;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeQiHao;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn GuaCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZhongGount;
        private System.Windows.Forms.DataGridViewButtonColumn Numer50;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTotalNumber;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label lblMaxGua;
        private System.Windows.Forms.Timer autoClkTimer;
        private System.Windows.Forms.Button btnStopAuto;
        private System.Windows.Forms.Button btnStartAuto;
        private System.Windows.Forms.NumericUpDown numericUpDownAutoClick;
        private System.Windows.Forms.Label label5;
    }
}

