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
        //public decimal LargeTotalPrincipal = 26300M;
        public decimal LargeTotalPrincipal = 17400M;
        /// <summary>
        /// 总轮次-Large-总共进行的轮次 共8轮 
        /// </summary>
        public decimal LargeTotalLun = 8;
        /// <summary>
        /// 总轮次-Large-当前轮次，初始为1轮，当中间轮次的8轮完后增加，说明爆掉一次
        /// </summary>
        public decimal LargeCurrentLun = 1;
        /// <summary>
        /// 总轮次-Large-每轮的资金矩阵,金额，跟随当前轮次改变，第一轮200，第二轮300，第三轮500，第四轮900，第五轮1700，第六轮3300，第七轮6500，第八轮12900
        /// </summary>
        //public decimal[] LargeLunAmount = { 200, 300, 500, 900, 1700, 3300, 6500, 12900 };
        public decimal[] LargeLunAmount = { 200, 200, 400, 600, 1000, 1600, 2600, 10800 };
        /// <summary>
        /// 总轮次-Large-总轮次的流水
        /// </summary>
        public decimal LargeTotalLiuShui = 0;
        /// <summary>
        /// 总轮次-Large-最大的中间轮次
        /// </summary>
        public decimal LargeMaxMiddleLunCount = 1;
        /// <summary>
        /// 总轮次-Large-总点击次数
        /// </summary>
        public decimal LargeTotalClickCount = 0;
        #endregion

        #region 中间轮次-Middle-当前循环轮次-相关字段
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-总轮次本金，初始为26300元， 分为8轮
        /// 1轮-200,2轮-300，3轮500，4轮900
        /// 5轮1700，6轮3300，7轮6500，8轮12900
        /// </summary>
        //public decimal MiddleTotalPrincipal = 26300M;
        public decimal MiddleTotalPrincipal = 17400M;

        /// <summary>
        /// 中间轮次-Middle-当前剩余本金,不包含Small，初始为26300元，每轮结束后根据盈亏情况更新
        /// </summary>

        //public decimal MiddleCurrentPrincipalExcludSmall = 26300M;
        public decimal MiddleCurrentPrincipalExcludSmall = 17400M;
        /// <summary>
        /// 中间轮次-Middle-总的初始值 常量保持不变,用于判断当前轮是否盈利，固定为26300元
        /// </summary>
        //public const decimal MiddleTotalPrincipalInit = 26300M;
        public const decimal MiddleTotalPrincipalInit = 17400M;
        

        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-总轮次 共8轮 
        /// </summary>
        public decimal MiddleTotalLun = 8;
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-当前轮次，初始为1轮，每轮点击达到设定次数后增加，超过8轮后提示无法继续拆分
        /// </summary>
        public int MiddleCurrentLun = 1;
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-每轮的资金矩阵,金额，跟随当前轮次改变，第一轮200，第二轮300，第三轮500，第四轮900，第五轮1700，第六轮3300，第七轮6500，第八轮12900
        /// </summary>
        //public decimal[] MiddleLunAmount = { 200, 300, 500, 900, 1700, 3300, 6500, 12900 };
        public decimal[] MiddleLunAmount = { 200, 200, 400, 600, 1000, 1600, 2600, 10800 };
        /// <summary>
        /// 中间轮次-Middle-当前循环轮次-总轮次的流水
        /// </summary>
        public decimal MiddleTotalLiuShui = 0;

        /// <summary>
        /// 中间轮每轮判断是否回退的盈离率
        /// </summary>
        public const decimal MiddleLunEnoughProfitLossRate = 1.9m;
        #endregion

        #region 当前执行轮次-Small-相关字段
        /// <summary>
        /// 用于第一次执行判断
        /// </summary>
        public bool IsRunning = false;
        /// <summary>
        /// 当前执行轮次-Small-点击次数
        /// </summary>
        public int SmallClickCount = -1;
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
        /// 当前执行轮次-Small-初始本金，默认为200元，可以根据需要调整
        /// </summary>
        public decimal SmallInitialPrincipal = 200M;
        /// <summary>
        /// 每轮盈利30% 就回收
        /// </summary>
        public decimal SmallLunEnoughPrincipal = 200M * MiddleLunEnoughProfitLossRate;
        /// <summary>
        /// 当前执行轮次-Small-当前余额，初始为200元，每次点击后根据盈亏情况更新
        /// </summary>
        public decimal SmallCurrentPrincipal = 200M;
        /// <summary>
        /// 当前执行轮次-Small-当前投注倍数
        /// </summary>
        public decimal SmallCurrentBetAmount = 0M;
        /// <summary>
        /// 当前执行轮次-Small-每倍投注金额
        /// </summary>
        public const decimal SmallPerBetAmount = 0.156M;
        public const decimal SmallPerBetAmountZhong = 0.834M;
        /// <summary>
        /// 当前执行轮次-Small-当前轮最大本金，初始为200元，每次超过5%后更新
        /// </summary>
        public decimal SmallMaxPrincipal = 210M;
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
        public decimal SmallCurrentMaxAmount = 200M;

        /// <summary>
        /// 当前执行轮次-Small-当前轮最小余额数值
        /// </summary>
        public decimal SmallCurrentMinAmount = 200M;
        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 初始化执行 用于从Middle中下拨钱
        /// </summary>
        private void InitRunning()
        {
            if(MiddleCurrentLun == 1)
            {
                //中间轮次的第一次
                //MiddleCurrentPrincipalExcludSmall -= MiddleLunAmount[0];
            }
        }
        /// <summary>
        /// 当前执行轮次-Small-投注方法
        /// 根据Smll轮的相关信息进行投注，
        /// 更新点击次数、拆分阶段、当前拆分金额、当前盈亏、
        /// 计划金额、投注账户余额和当期投注倍数等信息
        /// </summary>
        /// <returns></returns>
        public TouZhuResult SmallTouZhu(bool needInit=false,List<string> messageList=null)
        {
            if (!IsRunning)
            {
                InitRunning();
                IsRunning = true;
            }
            var touZhuResult = new TouZhuResult();
            touZhuResult.NeedInit= needInit;
            touZhuResult.Success = true;
            // 增加点击次数
            SmallClickCount++;
            SmallAllTotalTime++;
            LargeTotalClickCount++;
            // 更新拆分阶段和金额
            if (SmallSplitStage <= 10)
            {
                //SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
                if (SmallClickCount !=0 && SmallClickCount % SmallBaseClicks == 0) SmallSplitStage++;
                //SmallCurrentProfitLossBei = 8M;
            }
            else if (SmallSplitStage <= 20)
            {
                //SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
                if (SmallClickCount % SmallBaseClicks == 0) SmallSplitStage += 1;
                //SmallCurrentProfitLossBei = 8M;
            }
            else if (SmallSplitStage < 30)
            {
                //SmallCurrentSplitAmount = (SmallInitialPrincipal * SmallSplitStage) / 300;
                if (SmallClickCount % SmallBaseClicks == 0) SmallSplitStage += 1;
                //SmallCurrentProfitLossBei = 8M;
            }
            if(SmallSplitStage >= 30)
            {
                
                #endregion
                //先获取当前轮次  和当前余额
                var currentLun = MiddleCurrentLun;
                var currentYuE = SmallCurrentPrincipal;

                //根据当前轮次和余额进行判断
                if (currentYuE >= MiddleLunAmount[currentLun - 1])
                {
                    //当前轮余额大于初始,说明赚钱 
                    var currentLunAmount = MiddleLunAmount[currentLun - 1];
                    var yingLi = currentYuE - currentLunAmount;
                    //把盈利的钱加到中间轮剩余本金中
                    MiddleCurrentPrincipalExcludSmall += yingLi;

                    if (MiddleCurrentPrincipalExcludSmall + MiddleLunAmount[currentLun - 1]
                        > MiddleTotalPrincipalInit)
                    {
                        //中间轮盈利，说明中间轮次赚钱,需要更新Large轮次的金额，
                        //同时重新从第1轮开始,重置当前执行轮次-Small的相关字段，继续执行
                        LargeTotalPrincipal += MiddleCurrentPrincipalExcludSmall + MiddleLunAmount[currentLun - 1] - MiddleTotalPrincipalInit;
                        //中间轮次重置 恢复值为初始值
                        SmallLunOrigianInit();
                    }
                    else
                    {
                        //Small轮次盈利,但中间轮次不赚钱,需要继续当前轮次
                        //重新设置当前轮次的相关数值
                        SmallInitialPrincipal = MiddleLunAmount[currentLun - 1];
                        SmallCurrentPrincipal = SmallInitialPrincipal;
                        SmallLunInit();
                    }
                }
                else
                {
                    //当前轮余额小于初始,说明亏钱,需要跳转到下一轮
                    //重新设置当前执行轮次-Small的相关字段，继续执行

                    //先把余额添加到中间轮剩余本金中
                    MiddleCurrentPrincipalExcludSmall += currentYuE;
                    //再把当前轮次加1
                    MiddleCurrentLun++;

                    //如果中奖轮的轮次超过了之前的最大轮次,说明当前轮次是新的最大轮次,更新最大轮次
                    if(MiddleCurrentLun> LargeMaxMiddleLunCount)
                    {
                        LargeMaxMiddleLunCount = MiddleCurrentLun;
                    }
                    if (MiddleCurrentLun > MiddleTotalLun)
                    {
                        //如果超过总轮次,说明没有下一轮,更新最大轮的钱,重置中间轮
                        LargeCurrentLun++;

                        if (messageList == null)
                        {
                            messageList = new List<string>();
                        }
                        messageList.Add("**********************超过最大轮,重置********************");
                        messageList.Add(string.Format($"**********************重置前最大轮当前值{LargeTotalPrincipal}中间轮当前值{MiddleCurrentPrincipalExcludSmall}最小轮当前值{SmallCurrentPrincipal}********************"));
                        messageList.Add(string.Format($"需要减掉的数值是{MiddleTotalPrincipalInit}"));
                        //最大的钱加上中间轮剩余本金和当前轮的剩余本金
                        LargeTotalPrincipal += MiddleCurrentPrincipalExcludSmall;
                        LargeTotalPrincipal += SmallCurrentPrincipal;

                        //扣除掉中奖轮的初始本金,因为中奖轮的初始本金是从总轮次-Large-总轮次本金中扣除的
                        LargeTotalPrincipal -= MiddleTotalPrincipalInit;
                        messageList.Add(string.Format($"减掉后剩余的最大轮当前值为{LargeTotalPrincipal}"));
                        MiddleLunOrigianInit();
                        return SmallTouZhu(true,messageList);
                    }

                    //设置当前轮次的初始本金 
                    SmallInitialPrincipal = MiddleLunAmount[MiddleCurrentLun - 1];
                    //中间轮次-Middle-当前剩余本金 扣除掉 初始本金
                    if (MiddleCurrentLun > 1)
                    {
                        // 第一轮的钱已经扣除掉了,从第二轮开始才需要扣除掉当前轮次的初始本金
                        MiddleCurrentPrincipalExcludSmall -= SmallInitialPrincipal;
                    }
                    SmallLunInit();
                }

                // 进入到这里说明当前轮次已经重置完成,继续执行投注逻辑
                return SmallTouZhu(true,messageList);
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
            else if (SmallSplitStage < 30)
            {
                SmallJiHuaJin = Math.Round(
                    SmallInitialPrincipal / 50 + SmallInitialPrincipal / 60 * 9
                    + SmallInitialPrincipal / 60 * 10 * 1.404m
                    + SmallInitialPrincipal / 60 * (SmallSplitStage - 20) * 1.404m * 2m
                    , 1);
            }
            SmallCurrentProfitLoss = SmallCurrentPrincipal - SmallInitialPrincipal;

            // 计算投注账户余额
            SmallBetAccountBalance = SmallCurrentProfitLoss + SmallJiHuaJin;

            // 计算当期投注倍数
            SmallCurrentBetAmount = Math.Abs(Math.Round(SmallBetAccountBalance / 4.0m / SmallPerBetAmount, 0));
            if (SmallCurrentBetAmount == 0) SmallCurrentBetAmount = 1;

            var liuShuiAmount = SmallCurrentBetAmount * SmallPerBetAmount;
            SmallTotalLiuShui += liuShuiAmount;
            LargeTotalLiuShui += liuShuiAmount;

            if(messageList!=null && messageList.Count > 0)
            {
                if (touZhuResult.MessageList == null)
                {
                    touZhuResult.MessageList = new List<string>();
                }
                touZhuResult.MessageList.AddRange(messageList);
            }
            return touZhuResult;
        }
        /// <summary>
        /// 当前执行轮次-Small-开奖方法
        /// 根据上期是否中奖进行本金的更新，统计中的次数和挂的次数，更新总的点击次数
        /// 返回是否是最大本金变化的情况
        /// </summary>
        /// <param name="isZhong">是否中奖</param>
        /// <returns></returns>
        public KaiJiangResult SmallKaiJiang(bool isZhong)
        {
            var kaiJiangResult = new KaiJiangResult();
            kaiJiangResult.Success = true;
            kaiJiangResult.MaxChange = false;
            kaiJiangResult.Success = true;
            kaiJiangResult.Message = "";
            if (!isZhong)
            {
                //没有中奖,更新Small轮的当前金额
                SmallCurrentPrincipal = SmallCurrentPrincipal - SmallPerBetAmount * SmallCurrentBetAmount;
                SmallTotalGua++;
                SmallAllTotalGua++;
            }
            else
            {
                //中奖,更新Small轮的当前金额
                SmallTotalZhong++;
                SmallAllTotalZhong++;
                SmallCurrentPrincipal = SmallCurrentPrincipal + SmallPerBetAmountZhong * SmallCurrentBetAmount;
                //先判断是否超过指定的盈利 ,如果超过指定的盈利,说明当前轮次盈利了,
                //这里百分比根据参数MiddleLunEnoughProfitLossRate来进行计算
                if (SmallCurrentPrincipal >= SmallLunEnoughPrincipal)
                {
                    kaiJiangResult.MaxChange = true;

                    var currentLun = MiddleCurrentLun;
                    var currentYuE = SmallCurrentPrincipal;

                    var currentLunAmount = MiddleLunAmount[currentLun - 1];
                    var yingLi = currentYuE - currentLunAmount;
                    //把盈利的钱加到中间轮剩余本金中
                    MiddleCurrentPrincipalExcludSmall += yingLi;

                    if (MiddleCurrentPrincipalExcludSmall + MiddleLunAmount[currentLun - 1]
                        > MiddleTotalPrincipalInit)
                    {
                        //中间轮盈利，说明中间轮次赚钱,需要更新Large轮次的金额，
                        //同时重新从第1轮开始,重置当前执行轮次-Small的相关字段，继续执行
                        if (kaiJiangResult.MessageList == null)
                        {
                            kaiJiangResult.MessageList = new List<string>();
                        }
                        kaiJiangResult.MessageList.Add(string.Format($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:重置前中间轮盈利含最小轮{MiddleCurrentPrincipalExcludSmall + SmallCurrentPrincipal - 200 - MiddleTotalPrincipalInit}**********"));
                        kaiJiangResult.MessageList.Add(string.Format($"【中奖】目前资金:{SmallCurrentPrincipal},当前Middle轮:【{MiddleCurrentLun}】,当前Middle资金不包含Samll{MiddleCurrentPrincipalExcludSmall},当前Large资金{LargeTotalPrincipal}"));

                        LargeTotalPrincipal += MiddleCurrentPrincipalExcludSmall + MiddleLunAmount[currentLun - 1] - MiddleTotalPrincipalInit;
                        
                        //中间轮次重置 恢复值为初始值
                        MiddleLunOrigianInit();
                        //SmallLunOrigianInit();
                        kaiJiangResult.MessageList.Add(string.Format($"*******当前轮盈利超过{(MiddleLunEnoughProfitLossRate-1)*100}%,中间轮盈利,退回到第一轮。总盈利:{LargeTotalPrincipal - MiddleTotalPrincipalInit}**********"));
                    }
                    else
                    {
                        //Small轮次盈利,但中间轮次不赚钱,需要继续当前轮次
                        //重新设置当前轮次的相关数值
                        if (kaiJiangResult.MessageList == null)
                        {
                            kaiJiangResult.MessageList = new List<string>();
                        }
                        kaiJiangResult.MessageList.Add(string.Format($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:重置前中间轮盈利含最小轮{MiddleCurrentPrincipalExcludSmall + SmallCurrentPrincipal - 200 - MiddleTotalPrincipalInit}**********"));
                        kaiJiangResult.MessageList.Add(string.Format($"【中奖】目前资金:{SmallCurrentPrincipal},当前Middle轮:【{MiddleCurrentLun}】,当前Middle资金不包含Samll{MiddleCurrentPrincipalExcludSmall},当前Large资金{LargeTotalPrincipal}"));

                        SmallInitialPrincipal = MiddleLunAmount[currentLun - 1];
                        SmallCurrentPrincipal = SmallInitialPrincipal;
                        kaiJiangResult.MessageList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:当前轮盈利超过{(MiddleLunEnoughProfitLossRate - 1) * 100}%,但是中间轮未盈利,继续当前第{MiddleCurrentLun}轮。中间轮金额:{MiddleCurrentPrincipalExcludSmall+ currentLunAmount}**********");
                        SmallLunInit();
                        
                        /*
                        LargeTotalPrincipal += MiddleCurrentPrincipalExcludSmall + MiddleLunAmount[currentLun - 1] - MiddleTotalPrincipalInit;
                        //中间轮次重置 恢复值为初始值
                        MiddleLunOrigianInit();
                        //SmallLunOrigianInit();
                        kaiJiangResult.Message = string.Format($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:当前轮盈利超过{(MiddleLunEnoughProfitLossRate - 1) * 100}%,中间轮并未盈利,退回到第一轮。总盈利:{LargeTotalPrincipal - MiddleTotalPrincipalInit}**********");
                        */

                    }


                    //返回新的开奖结果，继续全新下一轮的投注
                    return kaiJiangResult;
                }

                if (SmallCurrentPrincipal >= SmallMaxPrincipal)
                {
                    SmallInitialPrincipal = SmallCurrentPrincipal;
                    SmallMaxPrincipal = Math.Round(SmallCurrentPrincipal * 1.05M);
                    SmallMaxChangeInit();
                    kaiJiangResult.MaxChange = true;
                }

                SmallTotalTime = SmallTotalGua + SmallTotalZhong;

                // 计算当前盈亏
                SmallCurrentProfitLoss = SmallCurrentPrincipal - SmallInitialPrincipal;

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
                else if (SmallSplitStage < 30)
                {
                    SmallJiHuaJin = Math.Round(
                        SmallInitialPrincipal / 50 + SmallInitialPrincipal / 60 * 9
                        + SmallInitialPrincipal / 60 * 10 * 1.404m
                        + SmallInitialPrincipal / 60 * (SmallSplitStage - 20) * 1.404m * 2m
                        , 1);
                }
                // 计算投注账户余额
                SmallBetAccountBalance = SmallCurrentProfitLoss + SmallJiHuaJin;

                // 计算投注倍数
                SmallCurrentBetAmount = Math.Abs(Math.Round(SmallBetAccountBalance / 4m / SmallPerBetAmount, 0));
                if (SmallCurrentBetAmount == 0) SmallCurrentBetAmount = 1;

            }
            return kaiJiangResult;
        }
        /// <summary>
        /// 当前执行轮次-Small-重置方法
        /// 重置当前执行轮次的相关字段到初始状态，方便下一轮的执行
        /// </summary>
        public void SmallReset()
        {
            //
            //目前先重置点击次数、拆分阶段和每阶段需要点击的次数，其他字段根据需要调整
            //后续需要根据当前轮次-Small的具体轮次信息进行设置
            SmallInitialPrincipal = 200M;
            SmallCurrentPrincipal = 200M;
            SmallClickCount = -1;
            SmallSplitStage = 1; // 当前拆分阶段 
            SmallBaseClicks = 5; // 每阶段需要点击的次数
            SmallMaxPrincipal = SmallInitialPrincipal * 1.05M; // 重置最大本金为初始本金的105%
            SmallLunEnoughPrincipal = SmallInitialPrincipal * MiddleLunEnoughProfitLossRate;

            SmallTotalZhong = 0;  // 中的次数
            SmallTotalGua = 0;   // 挂的次数
            SmallTotalTime = 0;   // 总的点击次数
            SmallTotalLiuShui = 0; //总流水
            SmallJiHuaJin = 0;//计划金
        }

        /// <summary>
        /// 当前执行轮次-Small-最大值改变初始化方法
        /// 主要是在开奖方法中调用，当达到最大本金变化的情况时，重置当前执行轮次的相关字段到初始状态，方便下一轮的执行
        /// </summary>
        private void SmallMaxChangeInit()
        {
            SmallClickCount = -1;
            SmallSplitStage = 1; // 当前拆分阶段 
            SmallBaseClicks = 5; // 每阶段需要点击的次数
        }
        /// <summary>
        /// 当前轮初始化
        /// </summary>
        private void SmallLunInit()
        {
            SmallClickCount = -1;
            SmallSplitStage = 1; // 当前拆分阶段 
            SmallBaseClicks = 5; // 每阶段需要点击的次数
            SmallTotalZhong = 0;  // 中的次数
            SmallTotalGua = 0;   // 挂的次数
            SmallTotalTime = 0;   // 总的点击次数
            SmallCurrentBetAmount = 0; //当前投注倍数
            SmallCurrentSplitAmount = 0; //当前拆分金额
            SmallCurrentProfitLoss = 0; //当前盈亏
            SmallCurrentProfitLossBei = 0; //当前盈亏倍数
            SmallBetAccountBalance = 0; //计算投注账户余额
            SmallTotalLiuShui = 0;//总流水
            SmallJiHuaJin = 0;//计划金
            SmallCurrentMaxAmount = SmallInitialPrincipal;//当前轮最大余额数值
            SmallCurrentMinAmount = SmallInitialPrincipal;//当前轮最小余额数值
            SmallCurrentPrincipal = SmallInitialPrincipal;
            SmallMaxPrincipal = Math.Round(SmallCurrentPrincipal * 1.05M);
            SmallLunEnoughPrincipal = SmallCurrentPrincipal * MiddleLunEnoughProfitLossRate;
        }
        /// <summary>
        /// 原始轮初始化
        /// </summary>
        public void SmallLunOrigianInit()
        {
            MiddleCurrentLun = 1;
            //设置本金
            SmallInitialPrincipal = MiddleLunAmount[0];
            //把本金从中间轮次的剩余本金中扣除
            MiddleCurrentPrincipalExcludSmall = MiddleTotalPrincipalInit - MiddleLunAmount[0];

            SmallLunInit();
        }

        public void MiddleLunOrigianInit()
        {
            IsRunning = false;
            MiddleTotalPrincipal = 17400M;
            MiddleCurrentPrincipalExcludSmall = 17400M;
            MiddleCurrentLun = 1;
            MiddleTotalLiuShui = 0;
            SmallLunOrigianInit();
        }

        public void LargeLunOrigianInit()
        {
            LargeTotalPrincipal = 17400M;
            LargeCurrentLun = 1;
            LargeTotalLiuShui = 0;
            LargeTotalClickCount = 0;
            MiddleLunOrigianInit();
        }
    }
}
