using CpCodeSelect.Business.Score;
using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Business.MACDIndex.Moni
{
    /// <summary>
    /// 模拟执行1个8的确认点买入
    /// </summary>
    public class Hou3Select350YiLouSetFormMACDLianXu6MoniBusiness
    {
        public delegate void LogDelegate(string message);
        public delegate void LogDelegate2(string message,bool needPlay);
        private LogDelegate2 _logMethod;
        private List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private Hou3Select350YiLouSetFormZhongParentBusiness business = null;
        public Hou3Select350YiLouSetFormMACDLianXu6MoniBusiness(LogDelegate2 logMethod, List<Hou3Select350_ZhouQiZhongScore> model350List, Hou3Select350YiLouSetFormZhongParentBusiness business)
        {
            this.business = business;
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;


            for (int i = 0; i <= 6; i++)
            {
                YilouStatistic entity = new YilouStatistic();
                entity.YilouCount = i;
                entity.TotalCount = 0;
                yilouStatisticList.Add(entity);
            }
        }
        public void SetLogMethod(LogDelegate2 logMethod)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
        }

        private void LogInfo(string message,bool needPlay) => _logMethod?.Invoke(message,needPlay);
        /// <summary>
        /// 每轮上挂的次数
        /// </summary>
        private int[] LunGuaTime = { 2, 3, 4 };
        /// <summary>
        /// 每轮的投注矩阵,金额
        /// </summary>
        private decimal[,] LunAmountMatrix = {
                    { 56.7M, 88.55M,138.6M, 217M,339.5M, 530.95M,830.9M, 1299.9M},
                };
        private decimal[,] ZhongJiangAmountMatrix = {
                    { 157.14M, 245.41M ,384.12M, 601.4M,940.9M, 1471.49M ,2302.78M, 3602.58M}
                };
        //private decimal[,] LunAmountMatrix = {
        //            { 56.7M, 88.55M,138.6M, 217M,0M, 0M,0M, 0M},
        //        };
        //private decimal[,] ZhongJiangAmountMatrix = {
        //            { 157.14M, 245.41M ,384.12M, 601.4M,0M, 0M,0M, 0M}
        //        };
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

        public void AllDataInit()
        {
            InitData();
            ListInit();
        }
        public void ListInit()
        {
            model350List = new List<Hou3Select350_ZhouQiZhongScore>();
            current350List = new List<string>();
            before350List = new List<string>();
            yilouStatisticList = new List<YilouStatistic>();
        }

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
                this.model350List = Hou3Select350YiLouSetFormMACDIndexBusiness.model350List;
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

                        int zhongjiangqi = (CurrentLun - 1) * 2 + GuaCount;
                        yilouStatisticList[zhongjiangqi - 1].TotalCount = yilouStatisticList[zhongjiangqi - 1].TotalCount + 1;
                        TotalResult = TotalResult + zhongjiangAmount;
                        //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{GuaCount}期已中出,中奖金额:{zhongjiangAmount}，总额【{TotalResult}】。",false);
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}",false);

                        LunInit();
                        before350List = current350List;
                        Select350AndStartCalc(code);

                    }
                    else
                    {
                        //执行中，未中出
                        GuaCount++;
                        if (GuaCount <= 6)
                        {
                            //挂6以内说明挂了1次
                            IsRunning = true;
                            CurrentaQi = GuaCount;
                            StartCalc(code);
                        }
                        else if (GuaCount == 7)
                        {
                            //挂7说明挂了6次,结束
                            IsRunning = false;
                            CurrentaQi = 1;
                            GuaCount = 1;

                            //超过总轮次，结束
                            TotalGua++;
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已超过总轮次轮，结束本次执行。号码为:{string.Join(" ", current350List)}",false);
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}",false);
                            LunInit();
                            before350List = current350List;
                            Select350AndStartCalc(code);

                            before350List = current350List;

                            yilouStatisticList[6].TotalCount = yilouStatisticList[6].TotalCount + 1;
                        }
                    }
                }
                else
                {
                    // 如果不是执行中，说明上一轮中出后结束，开始下一轮
                    before350List = current350List;
                    Select350AndStartCalc(code);
                }
            }
        }

        /// <summary>
        /// 查找满足条件的号码并开始执行
        /// </summary>
        public void Select350AndStartCalc(Code code)
        {
            var getEnoughRecordList = new List<Hou3Select350_ZhouQiZhongScore>();
            if (Hou3Select350YiLouSetFormMACDIndexBusiness.model350List.Count >= 350)
            {
                foreach (var record in Hou3Select350YiLouSetFormMACDIndexBusiness.model350List)
                {
                    var lastScoreDate = record.ScoreDateList.LastOrDefault();
                    if (lastScoreDate != null)
                    {
                        if (lastScoreDate.Score > 0)
                        {
                            var macdResult = KLine350ScoreCalc.MACDIsEnough(record.KLineList);
                            var adxResult = KLine350ScoreCalc.ADXIsEnough(record.KLineList);
                            if (macdResult.Result && adxResult.Result)
                            {
                                record.Score = lastScoreDate.Score;
                                getEnoughRecordList.Add(record);
                            }
                        }
                    }
                }
            }

            getEnoughRecordList = getEnoughRecordList.Where(p => p.Score >= 80 && p.IsChuShou).OrderByDescending(p => p.ShouNumber).ThenByDescending(p => p.Score).ToList();

            //2026-02-04 修改设置的逻辑为获取形成趋势段的号码进行投注
            bool foundRecord = false; 
            Hou3Select350_ZhouQiZhongScore select350 = null;

            if (getEnoughRecordList.Count > 0)
            {
                foundRecord = true;
                int index = GetThreadSafeSeed(getEnoughRecordList.Count);
                select350 = getEnoughRecordList[index];
            }


            // 没有找到合适的记录,本期不投注
            if (!foundRecord) return;

            if (select350 != null && select350.Number350.Count > 0)
            {
                IsRunning = true;
                current350List = select350.Number350;
                //初始状态
                //第一期投注
                CurrentLun = 1;
                CurrentaQi = 1;
                GuaCount = 1;
                CurrentAmount = LunAmountMatrix[CurrentLun - 1, 0];
                TotalResult = TotalResult - CurrentAmount;
                TotalLiuShui += CurrentAmount;
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】", false);
                CurrentaQi = 2;
            }
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
            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】", false);
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
