using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CpCodeSelect.Util.ZiJinFangAn;

namespace CpCodeSelect.html
{
    public partial class zhihuiwoV2 : Form
    {
        /*public int ClickCount = 0;
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
        */
        //private ziJinFangAn ziJinFangAn;
        private ZiJinFangAnV2 ziJinFangAn;
        public zhihuiwoV2()
        {
            //ziJinFangAn = new ziJinFangAn(200, 200);
            ziJinFangAn = new ZiJinFangAnV2(200, 200);
            InitializeComponent();
            InitForm();
        }
        public void InitForm()
        {
            
            lblClickCount.Text = "0";
            lblSplitAmount.Text = "0";
            lblSplitStage.Text = "1";
            lblCurrentProfitLoss.Text = "0";
            lblBetAccountBalance.Text = "0";

            lblTotal.Text = "总计0个";
            lblZhong.Text = "中0个";
            lblGua.Text = "挂0个";
            txtInitAmount.Text = ziJinFangAn.SmallInitialPrincipal.ToString();
            txtCurrentAmount.Text = ziJinFangAn.SmallCurrentPrincipal.ToString();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            var result = ziJinFangAn.SmallTouZhu();
            if (result.NeedInit)
            {
                //需要初始化
                txtInitAmount.Text=ziJinFangAn.SmallInitialPrincipal.ToString();
                lblTouZhuNeedInit.Text = result.NeedInit.ToString();
            }
            if (!string.IsNullOrEmpty(result.Message))
            {
                txtTouzhuMessage.Text = result.Message;
            }

            lblTouZhuNeedInit.Text = result.NeedInit.ToString();
            lblTouZhuResult.Text = result.Success.ToString();

            ShowForm();
            //当前期投注倍数
            lblCurrentBetAmount.Text = ziJinFangAn.SmallCurrentBetAmount.ToString();
            // 计算下一期投注倍数
            lblNextBetAmount.Text = ziJinFangAn.SmallCurrentBetAmount.ToString();
            /*
            lblClickCount.Text = ziJinFangAn.SmallClickCount.ToString();

            lblSplitAmount.Text = ziJinFangAn.SmallJiHuaJin.ToString("F2");
            lblSplitStage.Text = ziJinFangAn.SmallSplitStage.ToString();

            // 计算当前盈亏
            var currentProfitLoss = ziJinFangAn.SmallCurrentPrincipal - ziJinFangAn.SmallInitialPrincipal;
            lblCurrentProfitLoss.Text = ziJinFangAn.SmallCurrentProfitLoss.ToString("F2");

            // 计算投注账户余额
            lblBetAccountBalance.Text = (ziJinFangAn.SmallBetAccountBalance/4).ToString("F2");

            // 计算当期投注倍数
            lblCurrentBetAmount.Text = ziJinFangAn.SmallCurrentBetAmount.ToString();

            // 计算下一期投注倍数 投注时和当前期投注倍数一样
            lblNextBetAmount.Text= ziJinFangAn.SmallCurrentBetAmount.ToString();
            */

        }


        private void MaxChangeInit()
        {


            lblClickCount.Text = "0";
            lblSplitAmount.Text = "0";
            lblSplitStage.Text = "1";
            lblCurrentProfitLoss.Text = "0";
            lblBetAccountBalance.Text = "0";

            // 计算当期投注倍数
            lblCurrentBetAmount.Text = "0";

            txtInitAmount.Text = ziJinFangAn.SmallInitialPrincipal.ToString();
            txtCurrentAmount.Text = ziJinFangAn.SmallCurrentPrincipal.ToString();
        }

        private void btnResult_Click(object sender, EventArgs e)
        {

            //当前期投注倍数
            lblCurrentBetAmount.Text = ziJinFangAn.SmallCurrentBetAmount.ToString();

            var isZhong = rbZhong.Checked;

            Random ran = new Random();
            var number = ran.Next(0, 99);
            
            isZhong = number < 35;

            var kaiJiangResult = ziJinFangAn.SmallKaiJiang(isZhong);
            if (kaiJiangResult.MaxChange)
            {
                MaxChangeInit();
            }
            if (!string.IsNullOrEmpty(kaiJiangResult.Message))
            {
                txtKaiJiangMessage.Text = kaiJiangResult.Message;
            }            
            lblKaiJiangResult.Text = kaiJiangResult.Success.ToString() ;

            ShowForm();

            // 计算下一期投注倍数
            lblNextBetAmount.Text = ziJinFangAn.SmallCurrentBetAmount.ToString();
            /*

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
            */
        }

        private void ShowForm()
        {
            lblClickCount.Text = ziJinFangAn.SmallClickCount.ToString();
            txtCurrentAmount.Text = ziJinFangAn.SmallCurrentPrincipal.ToString("F2");
            lblGua.Text = string.Format($"挂{ziJinFangAn.SmallTotalGua}");
            lblZhong.Text = string.Format($"中{ziJinFangAn.SmallTotalZhong}");

            // 计算当期投注倍数

            lblTotal.Text = string.Format($"总计{ziJinFangAn.SmallTotalTime}");

            lblSplitAmount.Text = ziJinFangAn.SmallJiHuaJin.ToString("F2");
            lblSplitStage.Text = ziJinFangAn.SmallSplitStage.ToString();

            // 计算当前盈亏
            lblCurrentProfitLoss.Text = ziJinFangAn.SmallCurrentProfitLoss.ToString("F2");

            // 计算投注账户余额
            lblBetAccountBalance.Text = (ziJinFangAn.SmallBetAccountBalance / 4).ToString("F2");


            #region Large 设置

            lblLargeTotal.Text = ziJinFangAn.LargeTotalPrincipal.ToString();
            lblLargeCurrentLun.Text = ziJinFangAn.LargeCurrentLun.ToString();
            lblLargeTotalLiushui.Text = ziJinFangAn.LargeTotalLiuShui.ToString();

            #endregion

            #region Middle 设置
            lblMiddleCurrent.Text = (ziJinFangAn.MiddleCurrentPrincipalExcludSmall+ziJinFangAn.SmallCurrentPrincipal).ToString();
            lblMiddleLun.Text = ziJinFangAn.MiddleCurrentLun.ToString();

            #endregion
        }

        private void txtInitAmount_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtInitAmount.Text, out ziJinFangAn.SmallInitialPrincipal))
            {
                MessageBox.Show("请输入有效的初始本金金额。");
                txtInitAmount.Focus();
            }
            else
            {
                ziJinFangAn.SmallMaxPrincipal = ziJinFangAn.SmallInitialPrincipal * 1.05M;
                ziJinFangAn.SmallLunEnoughPrincipal = ziJinFangAn.SmallInitialPrincipal * 1.3M;
            }
        }

        private void txtCurrentAmount_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtCurrentAmount.Text, out ziJinFangAn.SmallCurrentPrincipal))
            {
                MessageBox.Show("请输入有效的当前本金金额。");
                txtCurrentAmount.Focus();
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var testTime = numTestTime.Value;
            for (var i = 0; i < testTime; i++)
            {
                //btnCalc_Click(sender, EventArgs.Empty);
                //btnReset_Click(sender, EventArgs.Empty);
                btnCalc.PerformClick();
                //Thread.Sleep(500);
                btnResult.PerformClick();
                //Thread.Sleep(500);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetMiddle();
        }

        private void ResetMiddle()
        {
            ziJinFangAn.MiddleLunOrigianInit();

            lblClickCount.Text = "0";
            lblSplitAmount.Text = "0";
            lblSplitStage.Text = "1";
            lblCurrentProfitLoss.Text = "0";
            lblBetAccountBalance.Text = "0";
            lblCurrentBetAmount.Text = "0";

            lblTotal.Text = "总计0个";
            lblZhong.Text = "中0个";
            lblGua.Text = "挂0个";

            txtInitAmount.Text = ziJinFangAn.SmallInitialPrincipal.ToString();
            txtCurrentAmount.Text = ziJinFangAn.SmallCurrentPrincipal.ToString();

            lblMiddleCurrent.Text = "0";
            lblMiddleLun.Text = "0";
        }


        private void ResetAll()
        {
            ziJinFangAn.LargeLunOrigianInit();
            ResetMiddle();

            lblClickCount.Text = "0";
            lblSplitAmount.Text = "0";
            lblSplitStage.Text = "1";
            lblCurrentProfitLoss.Text = "0";
            lblBetAccountBalance.Text = "0";
            lblCurrentBetAmount.Text = "0";

            lblTotal.Text = "总计0个";
            lblZhong.Text = "中0个";
            lblGua.Text = "挂0个";

            txtInitAmount.Text = ziJinFangAn.SmallInitialPrincipal.ToString();
            txtCurrentAmount.Text = ziJinFangAn.SmallCurrentPrincipal.ToString();

            lblLargeTotal.Text = "0";
            lblLargeCurrentLun.Text = "0";
            lblLargeTotalLiushui.Text = "0";

        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            ResetAll();

        }
    }
}
