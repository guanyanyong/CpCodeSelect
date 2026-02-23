using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.ZiJinFangAn
{
    public class ZiJinFangAn
    {
        #region 构造方法
        public ZiJinFangAn()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialPrincipal">初始本金</param>
        /// <param name="CurrentPrincipal">当前本金</param>
        public ZiJinFangAn(decimal initialPrincipal, decimal currentPrincipal)
        {
            this.InitialPrincipal = initialPrincipal;
            this.CurrentPrincipal = currentPrincipal;
        }
        #endregion

        #region 属性字段
        /// <summary>
        /// 点击次数
        /// </summary>
        public int ClickCount = 0;
        /// <summary>
        /// 当前拆分阶段
        /// </summary>
        public int SplitStage = 1;
        /// <summary>
        /// 每阶段需要点击的次数
        /// </summary>
        public int BaseClicks = 5;
        /// <summary>
        /// 中的次数
        /// </summary>
        public int TotalZhong = 0;
        /// <summary>
        /// 挂的次数
        /// </summary>
        public int TotalGua = 0;
        /// <summary>
        /// 总的点击次数
        /// </summary>
        public int TotalTime = 0;
        /// <summary>
        /// 所有点击次数，包含重置后的，方便统计总的点击次数
        /// </summary>
        public int AllTotalTime = 0;
        /// <summary>
        /// 所有中的次数，包含重置后的，方便统计总的中的次数
        /// </summary>
        public int AllTotalZhong = 0;
        /// <summary>
        /// 所有挂的次数，包含重置后的，方便统计总的挂的次数
        /// </summary>
        public int AllTotalGua = 0;
        /// <summary>
        /// 初始本金，默认为2000元，可以根据需要调整
        /// </summary>
        public decimal InitialPrincipal = 2000M;
        /// <summary>
        /// 当前余额，初始为2000元，每次点击后根据盈亏情况更新
        /// </summary>
        public decimal CurrentPrincipal = 2000M;
        /// <summary>
        /// 当前投注倍数
        /// </summary>
        public decimal CurrentBetAmount = 0M;
        /// <summary>
        /// 每倍投注金额
        /// </summary>
        public const decimal PerBetAmount = 0.35M; 
        /// <summary>
        /// 当前轮最大本金，初始为200元，每次超过5%后更新
        /// </summary>
        public decimal MaxPrincipal = 2100M;
        /// <summary>
        /// 当前拆分金额
        /// </summary>
        public decimal CurrentSplitAmount = 0;
        /// <summary>
        /// 当前盈亏
        /// </summary>
        public decimal CurrentProfitLoss = 0;
        /// <summary>
        /// 当前盈亏倍数 跟随拆分阶段改变，
        /// 第一大阶段点击0-45为8倍，
        /// 第二大阶段点击46-90为12倍，
        /// 第三大阶段点击91-145为18倍
        /// /// </summary>

        private decimal currentProfitLossBei = 0;
        /// <summary>
        /// 投注账户余额
        /// </summary>
        public decimal BetAccountBalance = 0;
        public decimal TotalLiuShui = 0;
        #endregion

        #region 方法

        public TouZhuResult TouZhu()
        {
            var touZhuResult = new TouZhuResult();
            touZhuResult.Success = true;
            // 增加点击次数
            ClickCount++;
            AllTotalTime++;
            // 更新拆分阶段和金额
            #region 注释之前的方案
            if (SplitStage <= 10)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                if (ClickCount % BaseClicks == 0) SplitStage++;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 20)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                if (ClickCount % BaseClicks == 0) SplitStage+=1;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 30)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                if (ClickCount % BaseClicks == 0) SplitStage+=1;
                currentProfitLossBei = 8M;
            }
            else
            {
                //todo 这里可以考虑重置或者其他处理方式，目前先提示无法继续拆分
                //MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                touZhuResult.Success = false;
                touZhuResult.Message = "已达到最大拆分阶段，无法继续拆分。";
                return touZhuResult;
            }
            /*
            if (SplitStage <= 10)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                if (ClickCount % BaseClicks == 0) SplitStage++;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 30)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 400;
                if (ClickCount % BaseClicks == 0) SplitStage += 2;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 60)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 700;
                if (ClickCount % BaseClicks == 0) SplitStage += 3;
                currentProfitLossBei = 11.95M;
            }
            else
            {
                //todo 这里可以考虑重置或者其他处理方式，目前先提示无法继续拆分
                //MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                touZhuResult.Success = false;
                touZhuResult.Message = "已达到最大拆分阶段，无法继续拆分。";
                return touZhuResult;
            }
            */
            #endregion
            // 计算当前盈亏
            CurrentProfitLoss = CurrentPrincipal - InitialPrincipal;

            // 计算投注账户余额
            BetAccountBalance = CurrentProfitLoss / currentProfitLossBei + CurrentSplitAmount;

            // 计算当期投注倍数
            CurrentBetAmount = Math.Abs(Math.Round(BetAccountBalance / PerBetAmount, 0));
            TotalLiuShui += CurrentBetAmount * PerBetAmount;
            return touZhuResult;
        }
        /// <summary>
        /// 返回是否是最大本金变化的情况
        /// </summary>
        /// <param name="isZhong">判断是否中奖</param>
        /// <returns></returns>
        public KaiJiangResult KaiJiang(bool isZhong)
        {
            var kaiJiangResult = new KaiJiangResult();
            kaiJiangResult.MaxChange = false;
            kaiJiangResult.Success = true;
            kaiJiangResult.Message = "";
            if (!isZhong)
            {
                CurrentPrincipal = CurrentPrincipal - PerBetAmount * CurrentBetAmount;
                TotalGua++;
                AllTotalGua++;
            }
            else
            {
                TotalZhong++;
                AllTotalZhong++;
                CurrentPrincipal = CurrentPrincipal + 0.62M * CurrentBetAmount;
                if (CurrentPrincipal >= MaxPrincipal)
                {
                    InitialPrincipal = CurrentPrincipal;
                    MaxPrincipal = Math.Round(CurrentPrincipal * 1.05M);
                    MaxChangeInit();
                    kaiJiangResult.MaxChange = true;
                }
            }
            TotalTime = TotalGua + TotalZhong;

            decimal CurrentSplitAmount;
            #region 注释之前的方案

            if (SplitStage <= 10)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 20)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 30)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                currentProfitLossBei = 16M;
            }
            else
            {
                kaiJiangResult.Success = false;
                kaiJiangResult.Message = "已达到最大拆分阶段，无法继续拆分。";
                return kaiJiangResult;
            }
            /*
            if (SplitStage <= 10)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 30)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 300;
                currentProfitLossBei = 8M;
            }
            else if (SplitStage <= 60)
            {
                CurrentSplitAmount = (InitialPrincipal * SplitStage) / 600;
                currentProfitLossBei = 16M;
            }
            else
            {
                kaiJiangResult.Success = false;
                kaiJiangResult.Message = "已达到最大拆分阶段，无法继续拆分。";
                return kaiJiangResult;
            }
            */
            #endregion

            // 计算当前盈亏
            CurrentProfitLoss = CurrentPrincipal - InitialPrincipal;

            // 计算投注账户余额
            BetAccountBalance = CurrentProfitLoss / currentProfitLossBei + CurrentSplitAmount;

            // 计算当期投注倍数
            CurrentBetAmount = Math.Abs(Math.Round(BetAccountBalance / PerBetAmount, 0));
            if (CurrentBetAmount == 0) CurrentBetAmount = 1;

            return kaiJiangResult;
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
        #endregion
    }
}
