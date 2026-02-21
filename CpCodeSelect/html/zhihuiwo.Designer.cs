namespace CpCodeSelect.html
{
    partial class zhihuiwo
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtInitAmount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCurrentAmount = new System.Windows.Forms.TextBox();
            this.btnCalc = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblClickCount = new System.Windows.Forms.Label();
            this.lblSplitStage = new System.Windows.Forms.Label();
            this.lblCurrentProfitLoss = new System.Windows.Forms.Label();
            this.lblSplitAmount = new System.Windows.Forms.Label();
            this.lblBetAccountBalance = new System.Windows.Forms.Label();
            this.lblCurrentBetAmount = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbZhong = new System.Windows.Forms.RadioButton();
            this.rbNotZhong = new System.Windows.Forms.RadioButton();
            this.btnResult = new System.Windows.Forms.Button();
            this.lblZhong = new System.Windows.Forms.Label();
            this.lblGua = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label1.Location = new System.Drawing.Point(57, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "初始本金:";
            // 
            // txtInitAmount
            // 
            this.txtInitAmount.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtInitAmount.Location = new System.Drawing.Point(156, 108);
            this.txtInitAmount.Name = "txtInitAmount";
            this.txtInitAmount.Size = new System.Drawing.Size(224, 32);
            this.txtInitAmount.TabIndex = 1;
            this.txtInitAmount.Text = "200";
            this.txtInitAmount.Leave += new System.EventHandler(this.txtInitAmount_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label2.Location = new System.Drawing.Point(57, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "当前本金:";
            // 
            // txtCurrentAmount
            // 
            this.txtCurrentAmount.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtCurrentAmount.Location = new System.Drawing.Point(156, 145);
            this.txtCurrentAmount.Name = "txtCurrentAmount";
            this.txtCurrentAmount.Size = new System.Drawing.Size(224, 32);
            this.txtCurrentAmount.TabIndex = 1;
            this.txtCurrentAmount.Text = "200";
            this.txtCurrentAmount.Leave += new System.EventHandler(this.txtCurrentAmount_Leave);
            // 
            // btnCalc
            // 
            this.btnCalc.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnCalc.Location = new System.Drawing.Point(62, 322);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(115, 56);
            this.btnCalc.TabIndex = 2;
            this.btnCalc.Text = "投注";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label3.Location = new System.Drawing.Point(426, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "点击次数:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label4.Location = new System.Drawing.Point(426, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "每阶段点击次数:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label5.Location = new System.Drawing.Point(426, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 25);
            this.label5.TabIndex = 4;
            this.label5.Text = "当前拆分阶段:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label6.Location = new System.Drawing.Point(426, 217);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 25);
            this.label6.TabIndex = 3;
            this.label6.Text = "当前盈亏:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label7.Location = new System.Drawing.Point(426, 250);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 25);
            this.label7.TabIndex = 4;
            this.label7.Text = "拆分金额:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label8.Location = new System.Drawing.Point(426, 287);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 25);
            this.label8.TabIndex = 3;
            this.label8.Text = "投资账户余额:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label9.Location = new System.Drawing.Point(426, 322);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 25);
            this.label9.TabIndex = 3;
            this.label9.Text = "投资倍数:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label10.Location = new System.Drawing.Point(602, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(23, 25);
            this.label10.TabIndex = 5;
            this.label10.Text = "5";
            // 
            // lblClickCount
            // 
            this.lblClickCount.AutoSize = true;
            this.lblClickCount.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblClickCount.Location = new System.Drawing.Point(602, 152);
            this.lblClickCount.Name = "lblClickCount";
            this.lblClickCount.Size = new System.Drawing.Size(23, 25);
            this.lblClickCount.TabIndex = 5;
            this.lblClickCount.Text = "0";
            // 
            // lblSplitStage
            // 
            this.lblSplitStage.AutoSize = true;
            this.lblSplitStage.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblSplitStage.Location = new System.Drawing.Point(602, 180);
            this.lblSplitStage.Name = "lblSplitStage";
            this.lblSplitStage.Size = new System.Drawing.Size(23, 25);
            this.lblSplitStage.TabIndex = 5;
            this.lblSplitStage.Text = "1";
            // 
            // lblCurrentProfitLoss
            // 
            this.lblCurrentProfitLoss.AutoSize = true;
            this.lblCurrentProfitLoss.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblCurrentProfitLoss.Location = new System.Drawing.Point(602, 217);
            this.lblCurrentProfitLoss.Name = "lblCurrentProfitLoss";
            this.lblCurrentProfitLoss.Size = new System.Drawing.Size(23, 25);
            this.lblCurrentProfitLoss.TabIndex = 5;
            this.lblCurrentProfitLoss.Text = "0";
            // 
            // lblSplitAmount
            // 
            this.lblSplitAmount.AutoSize = true;
            this.lblSplitAmount.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblSplitAmount.Location = new System.Drawing.Point(602, 248);
            this.lblSplitAmount.Name = "lblSplitAmount";
            this.lblSplitAmount.Size = new System.Drawing.Size(23, 25);
            this.lblSplitAmount.TabIndex = 5;
            this.lblSplitAmount.Text = "0";
            // 
            // lblBetAccountBalance
            // 
            this.lblBetAccountBalance.AutoSize = true;
            this.lblBetAccountBalance.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblBetAccountBalance.Location = new System.Drawing.Point(602, 287);
            this.lblBetAccountBalance.Name = "lblBetAccountBalance";
            this.lblBetAccountBalance.Size = new System.Drawing.Size(23, 25);
            this.lblBetAccountBalance.TabIndex = 5;
            this.lblBetAccountBalance.Text = "0";
            // 
            // lblCurrentBetAmount
            // 
            this.lblCurrentBetAmount.AutoSize = true;
            this.lblCurrentBetAmount.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblCurrentBetAmount.Location = new System.Drawing.Point(602, 322);
            this.lblCurrentBetAmount.Name = "lblCurrentBetAmount";
            this.lblCurrentBetAmount.Size = new System.Drawing.Size(23, 25);
            this.lblCurrentBetAmount.TabIndex = 5;
            this.lblCurrentBetAmount.Text = "0";
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnReset.Location = new System.Drawing.Point(62, 410);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(115, 56);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbNotZhong);
            this.groupBox1.Controls.Add(this.rbZhong);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.groupBox1.Location = new System.Drawing.Point(62, 198);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 75);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "是否总奖";
            // 
            // rbZhong
            // 
            this.rbZhong.AutoSize = true;
            this.rbZhong.Location = new System.Drawing.Point(31, 40);
            this.rbZhong.Name = "rbZhong";
            this.rbZhong.Size = new System.Drawing.Size(68, 29);
            this.rbZhong.TabIndex = 0;
            this.rbZhong.Text = "中奖";
            this.rbZhong.UseVisualStyleBackColor = true;
            // 
            // rbNotZhong
            // 
            this.rbNotZhong.AutoSize = true;
            this.rbNotZhong.Checked = true;
            this.rbNotZhong.Location = new System.Drawing.Point(123, 40);
            this.rbNotZhong.Name = "rbNotZhong";
            this.rbNotZhong.Size = new System.Drawing.Size(87, 29);
            this.rbNotZhong.TabIndex = 0;
            this.rbNotZhong.TabStop = true;
            this.rbNotZhong.Text = "未中奖";
            this.rbNotZhong.UseVisualStyleBackColor = true;
            // 
            // btnResult
            // 
            this.btnResult.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnResult.Location = new System.Drawing.Point(216, 322);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(115, 56);
            this.btnResult.TabIndex = 2;
            this.btnResult.Text = "开奖";
            this.btnResult.UseVisualStyleBackColor = true;
            this.btnResult.Click += new System.EventHandler(this.btnResult_Click);
            // 
            // lblZhong
            // 
            this.lblZhong.AutoSize = true;
            this.lblZhong.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblZhong.Location = new System.Drawing.Point(211, 286);
            this.lblZhong.Name = "lblZhong";
            this.lblZhong.Size = new System.Drawing.Size(61, 25);
            this.lblZhong.TabIndex = 7;
            this.lblZhong.Text = "中0个";
            // 
            // lblGua
            // 
            this.lblGua.AutoSize = true;
            this.lblGua.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblGua.Location = new System.Drawing.Point(319, 286);
            this.lblGua.Name = "lblGua";
            this.lblGua.Size = new System.Drawing.Size(61, 25);
            this.lblGua.TabIndex = 7;
            this.lblGua.Text = "挂0个";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblTotal.Location = new System.Drawing.Point(57, 286);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(61, 25);
            this.lblTotal.TabIndex = 8;
            this.lblTotal.Text = "总0个";
            // 
            // zhihuiwo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 523);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblGua);
            this.Controls.Add(this.lblZhong);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblCurrentBetAmount);
            this.Controls.Add(this.lblBetAccountBalance);
            this.Controls.Add(this.lblSplitAmount);
            this.Controls.Add(this.lblCurrentProfitLoss);
            this.Controls.Add(this.lblSplitStage);
            this.Controls.Add(this.lblClickCount);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnResult);
            this.Controls.Add(this.btnCalc);
            this.Controls.Add(this.txtCurrentAmount);
            this.Controls.Add(this.txtInitAmount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "zhihuiwo";
            this.Text = "zhihuiwo";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInitAmount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCurrentAmount;
        private System.Windows.Forms.Button btnCalc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblClickCount;
        private System.Windows.Forms.Label lblSplitStage;
        private System.Windows.Forms.Label lblCurrentProfitLoss;
        private System.Windows.Forms.Label lblSplitAmount;
        private System.Windows.Forms.Label lblBetAccountBalance;
        private System.Windows.Forms.Label lblCurrentBetAmount;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbNotZhong;
        private System.Windows.Forms.RadioButton rbZhong;
        private System.Windows.Forms.Button btnResult;
        private System.Windows.Forms.Label lblZhong;
        private System.Windows.Forms.Label lblGua;
        private System.Windows.Forms.Label lblTotal;
    }
}