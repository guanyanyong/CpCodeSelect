using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpCodeSelect.html
{
    public class CalcTouZhu
    {
        public CalcTouZhu()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialPrincipal">初始本金</param>
        /// <param name="CurrentPrincipal">当前本金</param>
        public CalcTouZhu(decimal initialPrincipal, decimal currentPrincipal)
        {
            this.InitialPrincipal = initialPrincipal;
            this.CurrentPrincipal = currentPrincipal;
        }
        public int ClickCount = 0;
        public int SplitStage = 1;  // 当前拆分阶段 
        public int BaseClicks = 5;  // 每阶段需要点击的次数
        public int TotalZhong = 0;  // 中的次数
        public int TotalGua = 0;    // 挂的次数
        public int TotalTime = 0;   // 总的点击次数
        /// <summary>
        /// 初始本金，默认为200元，可以根据需要调整
        /// </summary>
        public decimal InitialPrincipal = 2000M;
        /// <summary>
        /// 当前余额，初始为200元，每次点击后根据盈亏情况更新
        /// </summary>
        public decimal CurrentPrincipal = 2000M;
        public decimal CurrentBetAmount = 0M; // 当前投注倍数
        public decimal MaxPrincipal = 210M; // 当前最大本金，初始为200元，每次超过5%后更新
        public decimal CurrentSplitAmount = 0; // 当前拆分金额
        public decimal CurrentProfitLoss = 0; // 当前盈亏
        public decimal BetAccountBalance = 0; // 投注账户余额
        public void TouZhu()
        {
            // 增加点击次数
            ClickCount++;

            // 更新拆分阶段和金额
            if (SplitStage <= 10)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 200;
                if (ClickCount % BaseClicks == 0) SplitStage++;
            }
            else if (SplitStage <= 30)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 200;
                if (ClickCount % BaseClicks == 0) SplitStage += 2;
            }
            else if (SplitStage <= 60)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 200;
                if (ClickCount % BaseClicks == 0) SplitStage += 3;
            }
            else
            {
                //todo 这里可以考虑重置或者其他处理方式，目前先提示无法继续拆分
                MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                return;
            }

            // 计算当前盈亏
            CurrentProfitLoss = CurrentPrincipal - InitialPrincipal;

            // 计算投注账户余额
            BetAccountBalance = CurrentProfitLoss / 8.0M + CurrentSplitAmount;

            // 计算当期投注倍数
            CurrentBetAmount = Math.Round(BetAccountBalance / 0.35M, 0);

        }
        /// <summary>
        /// 返回是否是最大本金变化的情况
        /// </summary>
        /// <param name="isZhong">判断是否中奖</param>
        /// <returns></returns>
        public bool KaiJiang(bool isZhong)
        {
            var maxChange = false;
            if (!isZhong)
            {
                CurrentPrincipal = CurrentPrincipal - 0.35M * CurrentBetAmount;
                TotalGua++;
            }
            else
            {
                TotalZhong++;
                CurrentPrincipal = CurrentPrincipal + 0.98M * CurrentBetAmount;
                if (CurrentPrincipal >= MaxPrincipal)
                {
                    InitialPrincipal = CurrentPrincipal;
                    MaxPrincipal = Math.Round(CurrentPrincipal * 1.05M);
                    MaxChangeInit();
                    maxChange = true;
                }
            }

            TotalTime = TotalGua + TotalZhong;

            decimal CurrentSplitAmount;
            if (SplitStage <= 10)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 200;
            }
            else if (SplitStage <= 30)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 200;
            }
            else if (SplitStage <= 60)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 200;
            }
            else
            {
                MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                return maxChange;
            }

            // 计算当前盈亏
            CurrentProfitLoss = CurrentPrincipal - InitialPrincipal;

            // 计算投注账户余额
            BetAccountBalance = CurrentProfitLoss / 8.0M + CurrentSplitAmount;

            // 计算当期投注倍数
            CurrentBetAmount = Math.Abs(Math.Round(BetAccountBalance / 0.35M, 0));
            if (CurrentBetAmount == 0) CurrentBetAmount = 1;

            return maxChange;
        }

        public void Reset()
        {
            InitialPrincipal = 2000M;
            CurrentPrincipal = 2000M;
            ClickCount = 0;
            SplitStage = 1; // 当前拆分阶段 
            BaseClicks = 5; // 每阶段需要点击的次数


            MaxPrincipal = InitialPrincipal * 1.05M; // 重置最大本金为初始本金的105%

            TotalZhong = 0;  // 中的次数
            TotalGua = 0;   // 挂的次数
            TotalTime = 0;   // 总的点击次数
        }


        private void MaxChangeInit()
        {

            ClickCount = 0;
            SplitStage = 1; // 当前拆分阶段 
            BaseClicks = 5; // 每阶段需要点击的次数


        }
    }
}
