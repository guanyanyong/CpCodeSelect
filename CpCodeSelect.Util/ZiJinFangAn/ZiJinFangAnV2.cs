using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.ZiJinFangAn
{
    public class ZiJinFangAnV2
    {
        #region 构造方法
        public ZiJinFangAnV2()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialPrincipal">初始本金</param>
        /// <param name="CurrentPrincipal">当前本金</param>
        public ZiJinFangAnV2(decimal initialPrincipal, decimal currentPrincipal)
        {
            this.SmallInitialPrincipal = initialPrincipal;
            this.SmallCurrentPrincipal = currentPrincipal;
        }
        #endregion


        #region 属性字段

        #region 总轮次-Large-相关字段
        /// <summary>
        /// 总轮次-Large本金，初始为26300元， 分为8轮
        /// 1轮-200,2轮-300，3轮500，4轮900
        /// 5轮1700，6轮3300，7轮6500，8轮12900
        /// </summary>
        public decimal LargeTotalPrincipal= 26300M;
        /// <summary>
        /// 总轮次-Large-总共进行的轮次 共8轮 
        /// </summary>
        public decimal LargeTotalLun = 8;
        /// <summary>
        /// 总轮次-Large-当前轮次，初始为1轮，每轮点击达到设定次数后增加，超过8轮后提示无法继续拆分
        /// </summary>
        public decimal LargeCurrentLun = 1;
        /// <summary>
        /// 总轮次-Large-每轮的资金矩阵,金额，跟随当前轮次改变，第一轮200，第二轮300，第三轮500，第四轮900，第五轮1700，第六轮3300，第七轮6500，第八轮12900
        /// </summary>
        public decimal[] LargeLunAmount = { 200, 300, 500, 900, 1700, 3300, 6500, 12900 };
        /// <summary>
        /// 总轮次-Large-总轮次的流水
        /// </summary>
        public decimal LargeTotalLiuShui = 0;
        #endregion

        #region 中间轮次-Middle-当前循环轮次-相关字段
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-总轮次本金，初始为26300元， 分为8轮
        /// 1轮-200,2轮-300，3轮500，4轮900
        /// 5轮1700，6轮3300，7轮6500，8轮12900
        /// </summary>
        public decimal MiddleTotalPrincipal = 26300M;
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-总轮次 共8轮 
        /// </summary>
        public decimal MiddleTotalLun = 8;
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-当前轮次，初始为1轮，每轮点击达到设定次数后增加，超过8轮后提示无法继续拆分
        /// </summary>
        public decimal MiddleCurrentLun = 1;
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-每轮的资金矩阵,金额，跟随当前轮次改变，第一轮200，第二轮300，第三轮500，第四轮900，第五轮1700，第六轮3300，第七轮6500，第八轮12900
        /// </summary>
        public decimal[] MiddleLunAmount = { 200, 300, 500, 900, 1700, 3300, 6500, 12900 };
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-总轮次的流水
        /// </summary>
        public decimal MiddleTotalLiuShui = 0;
        #endregion

        #region 当前执行轮次-Small-相关字段
        /// <summary>
        /// 当前执行轮次-Small-点击次数
        /// </summary>
        public int SmallClickCount = 0;
        /// <summary>
        /// 当前执行轮次-Small-当前拆分阶段
        /// </summary>
        public int SmallSplitStage = 1;
        /// <summary>
        /// 当前执行轮次-Small-每阶段需要点击的次数
        /// </summary>
        public int SmallBaseClicks = 5;
        /// <summary>
        /// 当前执行轮次-Small-中的次数
        /// </summary>
        public int SmallTotalZhong = 0;
        /// <summary>
        /// 当前执行轮次-Small-挂的次数
        /// </summary>
        public int SmallTotalGua = 0;
        /// <summary>
        /// 当前执行轮次-Small-总的点击次数
        /// </summary>
        public int SmallTotalTime = 0;
        /// <summary>
        /// 当前执行轮次-Small-所有点击次数，包含重置后的，方便统计总的点击次数
        /// </summary>
        public int SmallAllTotalTime = 0;
        /// <summary>
        /// 当前执行轮次-Small-所有中的次数，包含重置后的，方便统计总的中的次数
        /// </summary>
        public int SmallAllTotalZhong = 0;
        /// <summary>
        /// 当前执行轮次-Small-所有挂的次数，包含重置后的，方便统计总的挂的次数
        /// </summary>
        public int SmallAllTotalGua = 0;
        /// <summary>
        /// 当前执行轮次-Small-初始本金，默认为2000元，可以根据需要调整
        /// </summary>
        public decimal SmallInitialPrincipal = 2000M;
        /// <summary>
        /// 当前执行轮次-Small-当前余额，初始为2000元，每次点击后根据盈亏情况更新
        /// </summary>
        public decimal SmallCurrentPrincipal = 2000M;
        /// <summary>
        /// 当前执行轮次-Small-当前投注倍数
        /// </summary>
        public decimal SmallCurrentBetAmount = 0M;
        /// <summary>
        /// 当前执行轮次-Small-每倍投注金额
        /// </summary>
        public const decimal SmallPerBetAmount = 0.35M;
        /// <summary>
        /// 当前执行轮次-Small-当前轮最大本金，初始为200元，每次超过5%后更新
        /// </summary>
        public decimal SmallMaxPrincipal = 2100M;
        /// <summary>
        /// 当前执行轮次-Small-当前拆分金额
        /// </summary>
        public decimal SmallCurrentSplitAmount = 0;
        /// <summary>
        /// 当前执行轮次-Small-当前盈亏
        /// </summary>
        public decimal SmallCurrentProfitLoss = 0;
        /// <summary>
        /// 当前执行轮次-Small-当前盈亏倍数 跟随拆分阶段改变，
        /// 第一大阶段点击0-45为8倍，
        /// 第二大阶段点击46-90为12倍，
        /// 第三大阶段点击91-145为18倍
        /// /// </summary>

        private decimal SmallCurrentProfitLossBei = 0;
        /// <summary>
        /// 当前执行轮次-Small-计算投注账户余额
        /// </summary>
        public decimal SmallBetAccountBalance = 0;
        /// <summary>
        /// 当前执行轮次-Small-总流水
        /// </summary>
        public decimal SmallTotalLiuShui = 0;
        /// <summary>
        /// 当前执行轮次-Small-计划金，根据当前本金和当前计划阶段进行计算
        /// 用于计算投注账户余额
        /// </summary>
        /// </summary>
        public decimal SmallJiHuaJin = 0;

        /// <summary>
        /// 当前执行轮次-Small-当前轮最大余额数值
        /// </summary>
        public decimal SmallCurrentMaxAmount = 2000M;

        /// <summary>
        /// 当前执行轮次-Small-当前轮最小余额数值
        /// </summary>
        public decimal SmallCurrentMinAmount = 2000M;
        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 当前执行轮次-Small-投注方法
        /// 根据Smll轮的相关信息进行投注，
        /// 更新点击次数、拆分阶段、当前拆分金额、当前盈亏、
        /// 计划金额、投注账户余额和当期投注倍数等信息
        /// </summary>
        /// <returns></returns>
        public TouZhuResult TouZhu()
        {
            var touZhuResult = new TouZhuResult();
            touZhuResult.Success = true;
            // 增加点击次数
            SmallClickCount++;
            SmallAllTotalTime++;
            // 更新拆分阶段和金额
            if (SmallSplitStage <= 10)
            {
                SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
                if (SmallClickCount % SmallBaseClicks == 0) SmallSplitStage++;
                SmallCurrentProfitLossBei = 8M;
            }
            else if (SmallSplitStage <= 20)
            {
                SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
                if (SmallClickCount % SmallBaseClicks == 0) SmallSplitStage+=1;
                SmallCurrentProfitLossBei = 8M;
            }
            else if (SmallSplitStage <= 30)
            {
                SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
                if (SmallClickCount % SmallBaseClicks == 0) SmallSplitStage+=1;
                SmallCurrentProfitLossBei = 8M;
            }
            else
            {
                //todo 这里可以考虑重置或者其他处理方式，目前先提示无法继续拆分
                //MessageBox.Show("已达到最大拆分阶段，无法继续拆分。");
                touZhuResult.Success = false;
                touZhuResult.Message = "已达到最大拆分阶段，无法继续拆分。";
                return touZhuResult;
            }
            //计算计划金，根据当前本金和当前计划阶段进行计算
            if (SmallSplitStage <= 10)
            {
                SmallJiHuaJin = Math.Round(SmallInitialPrincipal / 50 + SmallInitialPrincipal / 60 * (SmallSplitStage - 1), 1);
            }
            else if (SmallSplitStage <= 20)
            {
                SmallJiHuaJin = Math.Round(
                    SmallInitialPrincipal / 50 + SmallInitialPrincipal / 60 * 9
                    + SmallInitialPrincipal / 60 * (SmallSplitStage - 10) * 1.404m
                    , 1);
            }
            else if (SmallSplitStage <= 30)
            {
                SmallJiHuaJin = Math.Round(
                    SmallInitialPrincipal / 50 + SmallInitialPrincipal / 60 * 9
                    + SmallInitialPrincipal / 60 * 10 * 1.404m
                    + SmallInitialPrincipal / 60 * (SmallSplitStage - 20) * 1.404m * 2m
                    , 1);
            }
            // 计算投注账户余额
            SmallBetAccountBalance = SmallCurrentProfitLoss + SmallJiHuaJin;

            // 计算当期投注倍数
            SmallCurrentBetAmount = Math.Abs(Math.Round(SmallBetAccountBalance / 4 / SmallPerBetAmount, 0));
            if (SmallCurrentBetAmount == 0) SmallCurrentBetAmount = 1;

            SmallTotalLiuShui += SmallCurrentBetAmount * SmallPerBetAmount;
            return touZhuResult;
        }
        /// <summary>
        /// 当前执行轮次-Small-开奖方法
        /// 根据上期是否中奖进行本金的更新，统计中的次数和挂的次数，更新总的点击次数
        /// 返回是否是最大本金变化的情况
        /// </summary>
        /// <param name="isZhong">是否中奖</param>
        /// <returns></returns>
        public KaiJiangResult KaiJiang(bool isZhong)
        {
            var kaiJiangResult = new KaiJiangResult();
            kaiJiangResult.Success = true;
            kaiJiangResult.MaxChange = false;
            kaiJiangResult.Success = true;
            kaiJiangResult.Message = "";
            if (!isZhong)
            {
                SmallCurrentPrincipal = SmallCurrentPrincipal - SmallPerBetAmount * SmallCurrentBetAmount;
                SmallTotalGua++;
                SmallAllTotalGua++;
            }
            else
            {
                SmallTotalZhong++;
                SmallAllTotalZhong++;
                SmallCurrentPrincipal = SmallCurrentPrincipal + 0.62M * SmallCurrentBetAmount;
                if (SmallCurrentPrincipal >= SmallMaxPrincipal)
                {
                    SmallInitialPrincipal = SmallCurrentPrincipal;
                    SmallMaxPrincipal = Math.Round(SmallCurrentPrincipal * 1.05M);
                    MaxChangeInit();
                    kaiJiangResult.MaxChange = true;
                }
            }
            SmallTotalTime = SmallTotalGua + SmallTotalZhong;

            if (SmallSplitStage <= 10)
            {
                SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
            }
            else if (SmallSplitStage <= 20)
            {
                SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
            }
            else if (SmallSplitStage <= 30)
            {
                SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
            }
            else
            {
                kaiJiangResult.Success = false;
                kaiJiangResult.Message = "已达到最大拆分阶段，无法继续拆分。";
                return kaiJiangResult;
            }

            #region 注释之前的方案
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
            SmallCurrentProfitLoss = SmallCurrentPrincipal - SmallInitialPrincipal;

            //计算计划金，根据当前本金和当前计划阶段进行计算
            if (SmallSplitStage <= 10)
            {
                SmallJiHuaJin = Math.Round(SmallInitialPrincipal/50+ SmallInitialPrincipal/60*(SmallSplitStage-1), 1);
            }else if (SmallSplitStage <= 20)
            {
                SmallJiHuaJin= Math.Round(
                    SmallInitialPrincipal/50+ SmallInitialPrincipal/60*9
                    + SmallInitialPrincipal/60*(SmallSplitStage-10)*1.404m
                    , 1);
            }else if(SmallSplitStage <= 30)
            {
                SmallJiHuaJin = Math.Round(
                    SmallInitialPrincipal/50+ SmallInitialPrincipal/60*9
                    + SmallInitialPrincipal/60*10*1.404m
                    + SmallInitialPrincipal/60*(SmallSplitStage-20)*1.404m*2m
                    , 1);
            }
            // 计算投注账户余额
            SmallBetAccountBalance = SmallCurrentProfitLoss  + SmallJiHuaJin;

            // 计算当期投注倍数
            SmallCurrentBetAmount = Math.Abs(Math.Round(SmallBetAccountBalance/4m / SmallPerBetAmount, 0));
            if (SmallCurrentBetAmount == 0) SmallCurrentBetAmount = 1;

            return kaiJiangResult;
        }
        /// <summary>
        /// 当前执行轮次-Small-重置方法
        /// 重置当前执行轮次的相关字段到初始状态，方便下一轮的执行
        /// </summary>
        public void Reset()
        {
            //todo 重置需要根据实际情况调整，
            //目前先重置点击次数、拆分阶段和每阶段需要点击的次数，其他字段根据需要调整
            //后续需要根据当前轮次-Small的具体轮次信息进行设置
            SmallInitialPrincipal = 2000M;
            SmallCurrentPrincipal = 2000M;
            SmallClickCount = 0;
            SmallSplitStage = 1; // 当前拆分阶段 
            SmallBaseClicks = 5; // 每阶段需要点击的次数


            SmallMaxPrincipal = SmallInitialPrincipal * 1.05M; // 重置最大本金为初始本金的105%

            SmallTotalZhong = 0;  // 中的次数
            SmallTotalGua = 0;   // 挂的次数
            SmallTotalTime = 0;   // 总的点击次数
        }

        /// <summary>
        /// 当前执行轮次-Small-最大值改变初始化方法
        /// 主要是在开奖方法中调用，当达到最大本金变化的情况时，重置当前执行轮次的相关字段到初始状态，方便下一轮的执行
        /// </summary>
        private void MaxChangeInit()
        {
            SmallClickCount = 0;
            SmallSplitStage = 1; // 当前拆分阶段 
            SmallBaseClicks = 5; // 每阶段需要点击的次数
        }

        #endregion
    }
}
