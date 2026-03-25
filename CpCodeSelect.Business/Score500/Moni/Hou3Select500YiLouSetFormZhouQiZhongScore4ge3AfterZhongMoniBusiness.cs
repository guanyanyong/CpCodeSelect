using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Business.Score500.Moni
{
    /// <summary>
    /// 模拟执行6个6轮的确认点买入
    /// </summary>
    public class Hou3Select500YiLouSetFormZhouQiZhongScore4ge3AfterZhongMoniBusiness
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        private List<Hou3Select500_ZhouQiZhongScore> model350List = new List<Hou3Select500_ZhouQiZhongScore>();
        //public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private bool WaitingForNextRound = false;//等待中出后开启下一轮
        private const int TotalQiCount = 12;
        public Hou3Select500_ZhouQiZhongScore currentSelect = null;
        public Hou3Select500YiLouSetFormZhouQiZhongScore4ge3AfterZhongMoniBusiness(LogDelegate logMethod, List<Hou3Select500_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;

            for (int i = 0; i <= TotalQiCount; i++)
            {
                YilouStatistic entity = new YilouStatistic();
                entity.YilouCount = i;
                entity.TotalCount = 0;
                yilouStatisticList.Add(entity);
            }
        }
        public void SetLogMethod(LogDelegate logMethod)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
        }
        private void LogInfo(string message) => _logMethod?.Invoke(message);
        /// <summary>
        /// 每轮上挂的次数
        /// </summary>
        private int[] LunGuaTime = { 2, 3, 4 };
        /// <summary>
        /// 每轮的投注矩阵,金额
        /// </summary>
        private int[,] LunBeiAmountMatrix = {
                        
           
                        { 3,      9 ,21},
                        {    46 ,98,205  },
                        {426,882  , 1823},
                        { 3765,7773,16045    },
          
             /*
                        { 3,      9 ,
                          21,46 ,
                          98,205 ,
                        426,882  ,
                          1823,3765  }
              */
                };
        /// <summary>
        /// 每轮的投注矩阵,金额
        /// </summary>
        private decimal[,] LunAmountMatrix =
                    {
           
                {1.5M, 4.5M,10.5M, },
                {  23M,49M,102M,},
                { 213M,441M,911,},
                {1882,3886.5M,8022.5M},
            /*

            {
                1.5M, 4.5M,
                 10.5M,  23M,
                49M,102M,
                 213M,441M,
                911,1882,
                } 
             */
            };
        private decimal[,] ZhongJiangAmountMatrix =

                    {
           
                {2.91M, 8.73M,20.37M,},
                { 44.62M,95.06M,198.85M},
                {413.22M,855.54M,1768.31M,},
                {3652.05M,7539.81M,15563.62M},
            
                 /*
            
                {2.91M, 8.73M,
                20.37M, 44.62M,
                95.06M,198.85M,
                413.22M,855.54M,
                1768.31M,3652.05M,
                } 
        */
        };
        /// <summary>
        /// 总金额
        /// </summary>
        public Decimal TotalResult { get; set; }
        /// <summary>
        /// 总流水
        /// </summary>
        public Decimal TotalLiuShui { get; set; }
        /// <summary>
        /// 当前轮次
        /// </summary>
        public int CurrentLun { get; set; } = 1;
        /// <summary>
        /// 当前期数
        /// </summary>
        public int CurrentaQi { get; set; } = 1;
        /// <summary>
        /// 当前倍数
        /// </summary>
        public int CurrentBei { get; set; }
        /// <summary>
        /// 当前投注金额
        /// </summary>
        public decimal CurrentAmount { get; set; }
        /// <summary>
        /// 总轮次
        /// </summary>
        public int TotalLun { get; set; } = 4;

        /// <summary>
        /// 当前上号的位置
        /// </summary>
        public PositionType CurrentPositionType { get; set; }
        /// <summary>
        /// 是否大小
        /// </summary>
        public bool IsDaXiao { get; set; } = true;
        /// <summary>
        /// 当前的大小单双字符串
        /// </summary>
        public string CurrentStr { get; set; }
        /// <summary>
        /// 当前轮中奖次数
        /// </summary>
        public int CurrentLunZhongJiangCiShu { get; set; } = 0;
        public bool IsRunning { get; set; } = false;
        public int GuaCount { get; set; } = 1;
        /// <summary>
        /// 总中奖次数
        /// </summary>
        public int TotalZhong { get; set; } = 0;
        public int TotalGua { get; set; } = 0;
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            TotalResult = 0;
            LunInit();
        }
        /// <summary>
        /// 轮次初始化
        /// </summary>
        public void LunInit()
        {
            CurrentLun = 1;
            CurrentaQi = 1;
            CurrentLunZhongJiangCiShu = 0;
            CurrentAmount = 0;
            IsRunning = false;
            WaitingForNextRound = false;
        }

        public void Reset()
        {
            yilouStatisticList.Clear();
            TotalResult = 0;
            TotalLiuShui = 0;
            TotalZhong = 0;
            TotalGua = 0;

            for (int i = 0; i <= TotalQiCount; i++)
            {
                YilouStatistic entity = new YilouStatistic();
                entity.YilouCount = i;
                entity.TotalCount = 0;
                yilouStatisticList.Add(entity);
            }
            LunInit();
        }

        /// <summary>
        /// 当前是否是原始开始状态
        /// </summary>
        /// <returns></returns>
        public bool IsOriginBeginStatus()
        {
            return CurrentLun == 1 && CurrentaQi == 1;
        }
        /// <summary>
        /// 添加统计信息到dic中
        /// </summary>
        /// <param name="number"></param>
        /// <param name="model"></param>
        public void CalcCode(Code code)
        {
            if (TotalGua >= 1)
            {
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已挂,不再投注。");
                CurrentBei = 0;
                return;
            }

            List<PositionNumber> list = new List<PositionNumber>();
            if (this.model350List == null || model350List.Count == 0)
            {
                model350List = Hou3Select500YiLouSetFormScoreAndChuShouBusiness.model350List;
            }
            if (IsOriginBeginStatus())
            {
                //如果是初始状态,肯定不是执行中 开始执行
                if (!IsRunning)
                {
                    Select350AndStartCalc(code);
                }
            }
            else
            {
                if (WaitingForNextRound)
                {
                    //当前是等待下一轮状态,说明上一轮已经结束,等待中奖后开启下一轮
                    var model = currentSelect;
                    if (!(
                    model.PositionType == PositionType.万 && model.Number500.Contains(code.Wan.Number.ToString())
                    || model.PositionType == PositionType.千 && model.Number500.Contains(code.Qian.Number.ToString())
                    || model.PositionType == PositionType.百 && model.Number500.Contains(code.Bai.Number.ToString())
                    || model.PositionType == PositionType.十 && model.Number500.Contains(code.Shi.Number.ToString())
                    || model.PositionType == PositionType.个 && model.Number500.Contains(code.Ge.Number.ToString())
                    ))

                    {
                        //等待下一轮,没有中出,继续等待
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，-挂了{CurrentLun - 1}轮2期 选择的位置是:{currentSelect.PositionType},选择的号码是{string.Join(" ", currentSelect.Number500)}，开奖号是:{code.CodeNumber} 等待中奖后进入下一轮");
                        CurrentBei = 0;
                        return;
                    }
                    else
                    {
                        WaitingForNextRound = false;
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，-中奖了 开启下一轮");
                        //如果当前中了,则是开启开一轮
                        //return;
                    }
                }
                //继续当前轮次
                //继续当前轮次
                if (IsRunning)
                {
                    var model = currentSelect;
                    //当前执行中 当前是中
                    var housanStr = code.GetHou3String();
                    if (
                    model.PositionType == PositionType.万 && model.Number500.Contains(code.Wan.Number.ToString())
                    || model.PositionType == PositionType.千 && model.Number500.Contains(code.Qian.Number.ToString())
                    || model.PositionType == PositionType.百 && model.Number500.Contains(code.Bai.Number.ToString())
                    || model.PositionType == PositionType.十 && model.Number500.Contains(code.Shi.Number.ToString())
                    || model.PositionType == PositionType.个 && model.Number500.Contains(code.Ge.Number.ToString())
                    )

                    {
                        //执行中,中出
                        TotalZhong++;
                        //中了以后 设置当前号码为未选中
                        currentSelect.IsSelect = false;
                        var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, GuaCount - 1];
                        int zhongjiangqi = (CurrentLun - 1) * 3 + GuaCount;
                        yilouStatisticList[zhongjiangqi - 1].TotalCount = yilouStatisticList[zhongjiangqi - 1].TotalCount + 1;
                        TotalResult = TotalResult + zhongjiangAmount;
                        //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{GuaCount}期已中出,中奖金额:{zhongjiangAmount}，总额【{TotalResult}】。");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}");

                        LunInit();
                        before350List = currentSelect.Number500;
                        Select350AndStartCalc(code);

                    }
                    else
                    {
                        //执行中，未中出
                        GuaCount++;
                        if (GuaCount <= 3)
                        {
                            //挂3说明挂了2次
                            IsRunning = true;
                            CurrentaQi = GuaCount;
                            StartCalc(code);
                        }
                        else if (GuaCount == 4)
                        {
                            //挂3说明挂了2次,当前轮结束,开始下一轮
                            IsRunning = false;
                            CurrentBei = 0;
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，当前第{CurrentLun}轮已挂");

                            CurrentLun++;
                            CurrentaQi = 1;
                            GuaCount = 1;
                            
                            if (CurrentLun > TotalLun)
                            {
                                //超过总轮次，结束

                                //挂了以后 设置当前号码为未选中
                                currentSelect.IsSelect = false;
                                TotalGua++;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已超过总轮次{TotalLun}轮，位置为{currentSelect.PositionType}，号码为:{string.Join(" ", currentSelect.Number500)}");
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}");
                                LunInit();
                                before350List = currentSelect.Number500;
                                Select350AndStartCalc(code);
                                yilouStatisticList[TotalQiCount].TotalCount = yilouStatisticList[TotalQiCount].TotalCount + 1;
                                return;
                            }
                            else
                            {
                                //当前轮结束,等待下一轮
                                before350List = currentSelect.Number500;
                                WaitingForNextRound = true;
                            }

                            //Select350AndGoonCalc(code);
                        }
                    }
                }
                else
                {
                    // 如果不是执行中，说明上一轮中出后结束，开始下一轮
                    before350List = currentSelect.Number500;
                    Select350AndGoonCalc(code);
                }
            }
        }

        /// <summary>
        /// 查找满足条件的号码并开始执行
        /// </summary>
        public void Select350AndStartCalc(Code code)
        {
            if (TotalGua >= 1)
            {
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已挂,不再投注。");
                CurrentBei = 0;
                return;
            }
            //list = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.
            if (CurrentLun == 0) CurrentLun = 1;
            Hou3Select500_ZhouQiZhongScore getEnoughRecord = null;
            var getEnoughRecordList = new List<Hou3Select500_ZhouQiZhongScore>();
            if (Hou3Select500YiLouSetFormScoreAndChuShouBusiness.model350List.Count >= 170)
            {
                /*
                foreach (var record in Hou3Select500YiLouSetFormScoreAndChuShouBusiness.model350List)
                {
                    var kLine500 = record.YiLouKline500;
                    var boll = record.KLineList;
                    if (kLine500.Count > 4)
                    {
                        var yilouk1 = kLine500[kLine500.Count - 1];
                        var yilouk2 = kLine500[kLine500.Count - 2];
                        var yilouk3 = kLine500[kLine500.Count - 3];
                        var yilouk4 = kLine500[kLine500.Count - 4];
                        var kLin1 = boll[boll.Count - 1];
                        //if (yilouk1.YiLouGuaCount == 0 && yilouk2.YiLouGuaCount == 1 && yilouk3.YiLouGuaCount >= 3)
                        //要求当前的K值要比布林上轨值小1.5 比下轨大1.5
                        if (kLin1.KValue <= kLin1.Bolling.BollUpperValue - 1.5 && kLin1.KValue >= kLin1.Bolling.MiddleValue + 0.3)
                        {
                            //注释4交
                            //if (yilouk1.YiLouGuaCount == 1 && yilouk2.YiLouGuaCount == 1 && yilouk3.YiLouGuaCount == 0 && yilouk4.YiLouGuaCount == 0)
                            //下面是3交
                            if (yilouk1.YiLouGuaCount == 1 && yilouk2.YiLouGuaCount == 0 && yilouk3.YiLouGuaCount == 0 )
                            {
                                getEnoughRecordList.Add(record);
                            }
                        }
                    }
                }
                *
                */

                foreach (var record in Hou3Select500YiLouSetFormScoreAndChuShouBusiness.model350List.Where(p=>!p.IsSelect))
                {
                    var kLine500 = record.YiLouKline500;
                    var boll = record.KLineList;
                    if (kLine500.Count > 5)
                    {
                        var kLin1 = boll[boll.Count - 1];
                        var kLin2 = boll[boll.Count - 2];
                        var kLin3 = boll[boll.Count - 3];
                        var kLin4 = boll[boll.Count - 4];
                        var kLin5 = boll[boll.Count - 5];


                        var gap1 = kLin1.Bolling.BollUpperValue - kLin2.Bolling.BollUpperValue;
                        var gap2 = kLin2.Bolling.BollUpperValue - kLin3.Bolling.BollUpperValue;
                        var gap3 = kLin3.Bolling.BollUpperValue - kLin4.Bolling.BollUpperValue;

                        if (gap1 > 0.5 && gap2 > 0.5 && gap3 > 0.5)
                        {
                            //不要有轨超压
                            if (kLin1.Bolling.BollUpperValue <= kLin1.KValue + 0.3)
                            {
                                //轨距是增加
                                if (kLin1.Bolling.BollUpperValue - kLin1.Bolling.BollLowerValue > kLin2.Bolling.BollUpperValue - kLin2.Bolling.BollLowerValue
                                 && kLin2.Bolling.BollUpperValue - kLin2.Bolling.BollLowerValue > kLin3.Bolling.BollUpperValue - kLin3.Bolling.BollLowerValue
                                 && kLin3.Bolling.BollUpperValue - kLin3.Bolling.BollLowerValue > kLin4.Bolling.BollUpperValue - kLin4.Bolling.BollLowerValue
                                 && kLin4.Bolling.BollUpperValue - kLin4.Bolling.BollLowerValue > kLin5.Bolling.BollUpperValue - kLin5.Bolling.BollLowerValue
                                    )
                                {
                                    //下轨没有出现轨沟向上的
                                    if (kLin1.Bolling.BollLowerValue < kLin2.Bolling.BollLowerValue
                                     && kLin2.Bolling.BollLowerValue < kLin3.Bolling.BollLowerValue
                                     && kLin3.Bolling.BollLowerValue < kLin4.Bolling.BollLowerValue
                                     && kLin4.Bolling.BollLowerValue < kLin5.Bolling.BollLowerValue
                                        )
                                        getEnoughRecordList.Add(record);
                                }
                            }
                        }

                    }
                }
            }

            if (getEnoughRecordList.Count <= 0) return;
            //CurrentExecuteList.AddRange(getEnoughRecordList);
            var randomIndex = _threadLocalRandom.Value.Next(0, getEnoughRecordList.Count);
            randomIndex--;
            if (randomIndex < 0) randomIndex = 0;
            getEnoughRecord = getEnoughRecordList[randomIndex];
            //currentExecute = record;



            if (getEnoughRecord == null) return;

            //最多查找5次,如果5次没有找到合适的记录就不投注
            if (getEnoughRecord.Number500.Count > 0)
            {
                getEnoughRecord.IsSelect = true;
                IsRunning = true;
                //current350List = GetRemainingDigits(getEnoughRecord.Number500);
                currentSelect = getEnoughRecord;
                //初始状态
                //第一期投注
                CurrentLun = 1;
                CurrentaQi = 1;
                GuaCount = 1;
                CurrentAmount = LunAmountMatrix[CurrentLun - 1, 0];
                CurrentBei=LunBeiAmountMatrix[CurrentLun - 1, 0];
                TotalResult = TotalResult - CurrentAmount;
                TotalLiuShui += CurrentAmount;
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:选择新的号码,新号位置是:{currentSelect.PositionType}，号码是:{string.Join(",",currentSelect.Number500)}。");
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
                CurrentaQi = 2;
            }
        }
        /// <summary>
        /// 开始下一轮 从第一期开始
        /// </summary>
        /// <param name="code"></param>
        public void Select350AndGoonCalc(Code code)
        {
            //这里不需要再获取新号码 原来的号码继续用就行了
            if (CurrentLun == 0) CurrentLun = 1;

            IsRunning = true;
            CurrentaQi = 1;
            GuaCount = 1;
            CurrentAmount = LunAmountMatrix[CurrentLun - 1, 0];
            CurrentBei=LunBeiAmountMatrix[CurrentLun - 1, 0];
            TotalResult = TotalResult - CurrentAmount;
            TotalLiuShui += CurrentAmount;
            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
            CurrentaQi = 2;
        }

        /// <summary>
        /// 进行第二期计算
        /// </summary>
        /// <param name="code"></param>
        public void StartCalc(Code code)
        {
            CurrentAmount = LunAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
            CurrentBei=LunBeiAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
            TotalResult = TotalResult - CurrentAmount;
            TotalLiuShui += CurrentAmount;
            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
            //CurrentaQi++;
        }

        /// <summary>
        /// 获取剩余的数字（从0-9中排除已选的5个数字）
        /// </summary>
        /// <param name="selectedDigits">已选择的5个数字字符串列表</param>
        /// <returns>剩余的5个数字字符串列表</returns>
        public static List<string> GetRemainingDigits(List<string> selectedDigits)
        {
            string allDigits = "0123456789";
            var remaining = new List<string>();

            foreach (char digit in allDigits)
            {
                if (!selectedDigits.Contains(digit.ToString()))
                {
                    remaining.Add(digit.ToString());
                }
            }

            return remaining;
        }


        /// <summary>
        /// 获取线程安全的随机数种子
        /// </summary>
        private static int GetThreadSafeSeed(int number)
        {
            if (number <= 0) return 0;
            lock (_threadLocalRandom)
            {
                return _threadLocalRandom.Value.Next(0, number);
            }
        }
    }
}
