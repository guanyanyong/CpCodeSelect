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

namespace CpCodeSelect.Business.Hou2Selct27Three18.Moni
{
    /// <summary>
    /// 模拟执行3个3轮的确认点买入
    /// </summary>
    public class MoniRun270ZhouQiZhong3ge6Business
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        private List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        public MoniRun270ZhouQiZhong3ge6Business(LogDelegate logMethod, List<Hou3Select350_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;

            for (int i = 0; i <= 18; i++)
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
                    { 4.05M, 9.72M,17.55M ,28.35M, 43.2M,63.72M},
                    {  92.34M,132.03M, 186.84M,262.98M, 368.28M,514.35M },
                    { 716.85M, 997.38M,1386.18M ,1924.83M, 2671.38M,3705.75M}
                };
        private decimal[,] ZhongJiangAmountMatrix = {
                    { 14.55M, 34.92M ,63.05M,101.85M, 155.2M ,228.92M},
                    { 331.74M ,474.33M, 671.24M,944.78M, 1323.08M ,1847.85M},
                    { 2575.35M, 3583.18M ,4979.98M,6915.13M, 9597.18M ,13313.25M }
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
                model350List = Hou2Selct27Three18SetFormBusiness.model350List;
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
                        int zhongjiangqi = (CurrentLun - 1) * 6 + GuaCount;
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
                        if (GuaCount <= 6)
                        {
                            //挂2说明挂了1次
                            IsRunning = true;
                            CurrentaQi = GuaCount;
                            StartCalc(code);
                        }
                        else if (GuaCount == 7)
                        {
                            //挂7说明挂了6次,当前轮结束,开始下一轮
                            IsRunning = false;
                            CurrentLun++;
                            CurrentaQi = 1;
                            GuaCount = 1;

                            if (CurrentLun > TotalLun)
                            {
                                //超过总轮次，结束
                                TotalGua++;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已超过总轮次{TotalLun}轮，结束本次执行。350注号码是{string.Join(" ", current350List)}");
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】。总挂次数{TotalGua}");
                                LunInit();
                                before350List = current350List;
                                Select350AndStartCalc(code);
                                yilouStatisticList[18].TotalCount = yilouStatisticList[18].TotalCount + 1;
                                return;
                            }
                            else
                            {
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，轮次{TotalLun - 2}轮已结束，准备下一轮。350注号码是{string.Join(" ", current350List)}");
                            }
                            before350List = current350List;
                            Select350AndGoonCalc(code);
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
            //list = Hou2Selct27Three18SetFormBusiness.model350List.
            //var list = model350List.Where(p => p.NeedZhong == false && p.ZhouQiZhongHouGua == 0 && p.GuaCount == 1 && p.IsZhouQiZhongHou).ToList();
            var list = model350List.ToList();
            Hou3Select350_ZhouQiZhongScore record = null;

            if (list.Count == 0) return;
            //最多查找5次,如果5次没有找到合适的记录就不投注
            int calcTime = 5;
            var totalCount = list.Count;
            if (list.Count > 5) calcTime = list.Count;
            bool foundRecord = false;
            Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();
            for (int i = 0; keyValuePairs.Count < calcTime; i++)
            {
                if (i >= 10) break;
                int index = GetThreadSafeSeed(totalCount - 1);
                if (keyValuePairs.ContainsKey(index))
                {
                    keyValuePairs[index] = keyValuePairs[index] + 1;
                    continue;
                }
                else keyValuePairs.Add(index, 1);
                var zhouQiZhongRecord = list[index];
                foundRecord = true;
                record = zhouQiZhongRecord;
                break;

                //var klinLIst = zhouQiZhongRecord.KLineList;
                //var result = KLine350Calc.KLineIsEnough(klinLIst);
                //if (result.Result)
                //{
                //    foundRecord = true;
                //    record = zhouQiZhongRecord;
                //    break;
                //}
            }
            // 没有找到合适的记录,本期不投注
            if (!foundRecord) return;
            if (record != null && record.Number350.Count > 0)
            {
                IsRunning = true;
                current350List = record.Number350;
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
            if (CurrentLun == 0) CurrentLun = 1;
            var list = model350List.ToList();
            Hou3Select350_ZhouQiZhongScore record = null;
            if (list.Count == 0) return;
            //最多查找5次,如果5次没有找到合适的记录就不投注
            int calcTime = 5;
            var totalCount = list.Count;
            if (list.Count > 5) calcTime = list.Count;
            bool foundRecord = false;
            Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();

            for (int i = 0; keyValuePairs.Count < calcTime; i++)
            {
                if (i >= 10) break;
                int index = GetThreadSafeSeed(totalCount - 1);
                if (keyValuePairs.ContainsKey(index))
                {
                    keyValuePairs[index] = keyValuePairs[index] + 1;
                    continue;
                }
                else keyValuePairs.Add(index, 1);
                var zhouQiZhongRecord = list[index];
                foundRecord = true;
                record = zhouQiZhongRecord;
                break;
            }
            // 没有找到合适的记录,本期不投注
            if (!foundRecord) return;
            if (record != null && record.Number350.Count > 0)
            {
                current350List = record.Number350;

                IsRunning = true;
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
