using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CpCodeSelect.html
{
    public partial class zhihuiwo : Form
    {
        public int ClickCount = 0;
        public int SplitStage = 1;  // 当前拆分阶段 
        public int BaseClicks = 5;  // 每阶段需要点击的次数
        public int TotalZhong = 0;  // 中的次数
        public int TotalGua = 0;    // 挂的次数
        public int TotalTime = 0;   // 总的点击次数
        /// <summary>
        /// 初始本金，默认为200元，可以根据需要调整
        /// </summary>
        public decimal InitialPrincipal = 200M;
        /// <summary>
        /// 当前余额，初始为200元，每次点击后根据盈亏情况更新
        /// </summary>
        public decimal CurrentPrincipal = 200M;
        public decimal currentBetAmount = 0M; // 当前投注倍数
        public decimal MaxPrincipal = 210M; // 当前最大本金，初始为200元，每次超过5%后更新
        public zhihuiwo()
        {
            InitializeComponent();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            // 增加点击次数
            ClickCount++;
            lblClickCount.Text = ClickCount.ToString();

            // 更新拆分阶段和金额
            decimal currentSplitAmount;
            if (SplitStage <= 10)
            {
                currentSplitAmount = (InitialPrincipal * SplitStage) / 200;
                if (ClickCount % BaseClicks == 0) SplitStage++;
            }
            else if (SplitStage <= 30)
            {
                currentSplitAmount = (InitialPrincipal * SplitStage) / 200;
                if (ClickCount % BaseClicks == 0) SplitStage += 2;
            }
            else if (SplitStage <= 60)
            {
                currentSplitAmount = (InitialPrincipal * SplitStage) / 200;
                if (ClickCount % BaseClicks == 0) SplitStage += 3;
            }
            else
            {
                MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                return;
            }
            lblSplitAmount.Text = currentSplitAmount.ToString("F2");
            lblSplitStage.Text = SplitStage.ToString();

            // 计算当前盈亏
            var currentProfitLoss = CurrentPrincipal - InitialPrincipal;
            lblCurrentProfitLoss.Text = currentProfitLoss.ToString("F2");

            // 计算投注账户余额
            var betAccountBalance = currentProfitLoss / 8.0M + currentSplitAmount;
            lblBetAccountBalance.Text = betAccountBalance.ToString("F2");

            // 计算当期投注倍数
            currentBetAmount = Math.Round(betAccountBalance / 0.35M, 0);
            lblCurrentBetAmount.Text = currentBetAmount.ToString();


        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            InitialPrincipal = 200M;
            CurrentPrincipal = 200M;
            ClickCount = 0;
            SplitStage = 1; // 当前拆分阶段 
            BaseClicks = 5; // 每阶段需要点击的次数

            lblClickCount.Text = "0";
            lblSplitAmount.Text = "0";
            lblSplitStage.Text = "1";
            lblCurrentProfitLoss.Text = "0";
            lblBetAccountBalance.Text = "0";

            // 计算当期投注倍数
            lblCurrentBetAmount.Text = "0";

            txtInitAmount.Text = "200";
            txtCurrentAmount.Text = "200";
            MaxPrincipal = InitialPrincipal*1.05M; // 重置最大本金为初始本金的105%

            TotalZhong = 0;  // 中的次数
            TotalGua = 0;   // 挂的次数
            TotalTime = 0;   // 总的点击次数
            lblTotal.Text = "总计0个";
            lblZhong.Text = "中0个";
            lblGua.Text = "挂0个";
        }

        private void MaxChangeInit()
        {

            ClickCount = 0;
            SplitStage = 1; // 当前拆分阶段 
            BaseClicks = 5; // 每阶段需要点击的次数

            lblClickCount.Text = "0";
            lblSplitAmount.Text = "0";
            lblSplitStage.Text = "1";
            lblCurrentProfitLoss.Text = "0";
            lblBetAccountBalance.Text = "0";

            // 计算当期投注倍数
            lblCurrentBetAmount.Text = "0";

            txtInitAmount.Text = InitialPrincipal.ToString();
            txtCurrentAmount.Text = CurrentPrincipal.ToString();
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            if (!rbZhong.Checked)
            {
                CurrentPrincipal = CurrentPrincipal - 0.35M * currentBetAmount;
                txtCurrentAmount.Text = CurrentPrincipal.ToString("F2");
                TotalGua++;
                lblGua.Text = string.Format($"挂{TotalGua}");
            }
            else
            {
                TotalZhong++;
                lblZhong.Text = string.Format($"中{TotalZhong}");
                CurrentPrincipal = CurrentPrincipal + 0.98M * currentBetAmount;
                txtCurrentAmount.Text = CurrentPrincipal.ToString("F2");
                if (CurrentPrincipal >= MaxPrincipal)
                {
                    InitialPrincipal = CurrentPrincipal;
                    MaxPrincipal = Math.Round(CurrentPrincipal * 1.05M);
                    MaxChangeInit();
                }
            }

            TotalTime = TotalGua + TotalZhong;
            lblTotal.Text = string.Format($"总计{TotalTime}");

            decimal currentSplitAmount;
            if (SplitStage <= 10)
            {
                currentSplitAmount = (InitialPrincipal * SplitStage) / 200;
            }
            else if (SplitStage <= 30)
            {
                currentSplitAmount = (InitialPrincipal * SplitStage) / 200;
            }
            else if (SplitStage <= 60)
            {
                currentSplitAmount = (InitialPrincipal * SplitStage) / 200;
            }
            else
            {
                MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                return;
            }
            lblSplitAmount.Text = currentSplitAmount.ToString("F2");
            lblSplitStage.Text = SplitStage.ToString();

            // 计算当前盈亏
            var currentProfitLoss = CurrentPrincipal - InitialPrincipal;
            lblCurrentProfitLoss.Text = currentProfitLoss.ToString("F2");

            // 计算投注账户余额
            var betAccountBalance = currentProfitLoss / 8.0M + currentSplitAmount;
            lblBetAccountBalance.Text = betAccountBalance.ToString("F2");

            // 计算当期投注倍数
            currentBetAmount = Math.Abs(Math.Round(betAccountBalance / 0.35M, 0));
            if (currentBetAmount == 0) currentBetAmount = 1;
            lblCurrentBetAmount.Text = currentBetAmount.ToString();
        }

        private void txtInitAmount_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtInitAmount.Text, out InitialPrincipal))
            {
                MessageBox.Show("请输入有效的初始本金金额。");
                txtInitAmount.Focus();
            }
        }

        private void txtCurrentAmount_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtCurrentAmount.Text, out CurrentPrincipal))
            {
                MessageBox.Show("请输入有效的当前本金金额。");
                txtCurrentAmount.Focus();
            }
        }
    }
}
