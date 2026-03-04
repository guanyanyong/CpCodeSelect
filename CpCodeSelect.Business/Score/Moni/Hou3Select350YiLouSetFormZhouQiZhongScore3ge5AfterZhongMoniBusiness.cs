using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Business.Score.Moni
{
    /// <summary>
    /// 模拟执行4个2轮的确认点买入
    /// </summary>
    public class Hou3Select350YiLouSetFormZhouQiZhongScore3ge5AfterZhongMoniBusiness
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        private List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private bool WaitingForNextRound = false;//等待中出后开启下一轮
        public Hou3Select350YiLouSetFormZhouQiZhongScore3ge5AfterZhongMoniBusiness(LogDelegate logMethod, List<Hou3Select350_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;

            for (int i = 0; i <= 12; i++)
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
        private decimal[,] LunAmountMatrix = {
                    { 5.95M, 15.05M,29.4M,51.8M,86.8M },
                    { 141.75M, 227.5M,361.9M,571.9M,900.55M },
                    { 1414.7M, 2219M,3477.6M,5446.7M,8527.4M }
                };
        private decimal[,] ZhongJiangAmountMatrix = {
                    { 16.49M, 41.71M,81.48M,143.56M,240.56M },
                    { 392.85M, 630.5M,1002.98M,1584.98M,2495.81M },
                    { 3920.74M, 6149.8M,9637.92M,15095.14M,23633.08M }
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
        public int TotalLun { get; set; } = 3;

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
            List<PositionNumber> list = new List<PositionNumber>();
            if (this.model350List == null || model350List.Count == 0)
            {
                model350List = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List;
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
                    var housanStr = code.GetHou3String();
                    if (!current350List.Contains(housanStr))
                    {
                        //等待下一轮,没有中出,继续等待
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-挂了{CurrentLun-1}轮5期 等待中奖后进入下一轮");
                        return;
                    }
                    else
                    {
                        WaitingForNextRound = false;
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-中奖了 开启下一轮");
                        return;
                    }
                }
                //继续当前轮次
                if (IsRunning)
                {
                    //当前执行中 当前是中
                    var housanStr = code.GetHou3String();
                    if (current350List.Contains(housanStr))
                    {
                        //执行中,中出
                        TotalZhong++;
                        var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, GuaCount - 1];
                        int zhongjiangqi = (CurrentLun - 1) * 4 + GuaCount;
                        yilouStatisticList[zhongjiangqi - 1].TotalCount = yilouStatisticList[zhongjiangqi - 1].TotalCount + 1;
                        TotalResult = TotalResult + zhongjiangAmount;
                        //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{GuaCount}期已中出,中奖金额:{zhongjiangAmount}，总额【{TotalResult}】。");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}");

                        LunInit();
                        before350List = current350List;
                        Select350AndStartCalc(code);

                    }
                    else
                    {
                        //执行中，未中出
                        GuaCount++;
                        if (GuaCount <= 4)
                        {
                            //挂5说明挂了4次
                            IsRunning = true;
                            CurrentaQi = GuaCount;
                            StartCalc(code);
                        }
                        else if (GuaCount == 5)
                        {
                            //挂5说明挂了2次,当前轮结束,开始下一轮
                            IsRunning = false;
                            CurrentLun++;
                            CurrentaQi = 1;
                            GuaCount = 1;

                            if (CurrentLun > TotalLun)
                            {
                                //超过总轮次，结束
                                TotalGua++;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已超过总轮次{TotalLun}轮，号码为:{string.Join(" ", current350List)}");
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}");
                                LunInit();
                                before350List = current350List;
                                Select350AndStartCalc(code);
                                yilouStatisticList[12].TotalCount = yilouStatisticList[12].TotalCount + 1;
                                return;
                            }
                            else
                            {
                                //当前轮结束,等待下一轮
                                before350List = current350List;
                                WaitingForNextRound = true;
                            }

                            //Select350AndGoonCalc(code);
                        }
                    }
                }
                else
                {
                    // 如果不是执行中，说明上一轮中出后结束，开始下一轮
                    before350List = current350List;
                    Select350AndGoonCalc(code);
                }
            }
        }

        /// <summary>
        /// 查找满足条件的号码并开始执行
        /// </summary>
        public void Select350AndStartCalc(Code code)
        {
            //list = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.
            if (CurrentLun == 0) CurrentLun = 1;
            Hou3Select350_ZhouQiZhongScore getEnoughRecord = null;
            if (Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.Count >= 170)
            {
                foreach (var currentRecord in Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List)
                {
                    var lastScoreDate = currentRecord.ScoreDateList.LastOrDefault();
                    if (lastScoreDate != null)
                    {
                        if (lastScoreDate.Score >= 150)
                        {
                            getEnoughRecord = currentRecord;
                            break;
                        }
                    }
                }
            }
            if (getEnoughRecord == null) return;

            //最多查找5次,如果5次没有找到合适的记录就不投注
            if (getEnoughRecord.Number350.Count > 0)
            {
                IsRunning = true;
                current350List = getEnoughRecord.Number350;
                //初始状态
                //第一期投注
                CurrentLun = 1;
                CurrentaQi = 1;
                GuaCount = 1;
                CurrentAmount = LunAmountMatrix[CurrentLun - 1, 0];
                TotalResult = TotalResult - CurrentAmount;
                TotalLiuShui += CurrentAmount;
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
            TotalResult = TotalResult - CurrentAmount;
            TotalLiuShui += CurrentAmount;
            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
            //CurrentaQi++;
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
