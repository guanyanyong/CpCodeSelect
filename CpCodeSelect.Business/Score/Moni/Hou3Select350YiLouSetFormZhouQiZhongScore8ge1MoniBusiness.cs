using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Scorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Business.Score.Moni
{
    public class RunAndStop
    {
        // 最大盈利数值和最大亏损数值,当盈利超过最大盈利数值或者亏损超过最大亏损数值时停止
        public decimal WinMaxValue = 3500;
        public decimal LoseMaxValue = -3500;

        public decimal TotalResult = 0;//当前总金额

        //每轮盈利超过12次 停止30分钟
        //最多盈利3轮后 
        public bool IsTotalStop = false;//当前是否总停止

        public int WinTimesStopNum = 12;//单轮盈利超多少次停止
        public int CurrentWinTime = 0;//当前轮盈利次数
        public int StopLeftTime = 30;//停止多少次后继续
        public int CurrentStopLeftTime = 0; //当前剩余停止次数

        public bool IsWinStop = false;//是否盈利停止中

        public int CurrentLun = 0;//当前轮次

        /// <summary>
        /// 判断数额是否达到停止条件
        /// </summary>
        /// <returns></returns>
        public bool CheckAmountIsStop()
        {
            if (TotalResult >= WinMaxValue)
            {
                return true;
            }
            else if (TotalResult <= LoseMaxValue)
            {
                if (CurrentLun <= 3)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 判断是否可以继续执行
        /// </summary>
        /// <returns></returns>
        public bool CanGoOn()
        {
            if (IsTotalStop) return false;
            if (IsWinStop) return false;
            return true;
        }
        
        /// <summary>
        /// 通过中奖与否断是否停止
        /// </summary>
        /// <param name="isWin"></param>
        /// <returns></returns>
        public bool CheckIsStopByResult(bool isWin)
        {
            var result = CheckAmountIsStop();
            if (result) IsTotalStop = true;
            return true;

            if (isWin)
            {
                //中后,盈利次数+1 如果盈利次数超过设定值,则停止
                CurrentWinTime++;
                if (CurrentWinTime >= WinTimesStopNum)
                {
                    IsTotalStop = false;
                    IsWinStop = true;
                    CurrentStopLeftTime = StopLeftTime;
                }
            }
            else
            {
                if (IsWinStop)
                {
                    CurrentStopLeftTime--;
                    if (CurrentStopLeftTime <= 0)
                    {
                        IsTotalStop = false;
                        IsWinStop = false;
                        CurrentStopLeftTime = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 模拟执行8个1轮的确认点买入
    /// </summary>
    public class Hou3Select350YiLouSetFormZhouQiZhongScore8ge1MoniBusiness
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        private List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        public decimal MaxResult = 0;
        public decimal MinResult = 0;

        public RunAndStop RunAndStop = new RunAndStop();

        /// <summary>
        /// 上次出手,本次需要检查是否中奖
        /// </summary>
        private bool beforeChuShouNeedCheckZhongJiang = false;
        public Hou3Select350YiLouSetFormZhouQiZhongScore8ge1MoniBusiness(LogDelegate logMethod, List<Hou3Select350_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;

            for (int i = 0; i <= 9; i++)
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
                    { 56.7M },
                    { 88.55M },
                    { 138.6M},
                    { 217M },
                    { 339.5M },
                    { 530.95M },
                    { 830.9M },
                    { 1299.9M }
                };
        private decimal[,] ZhongJiangAmountMatrix = {
                    { 157.14M },
                    {  245.41M },
                    { 384.12M },
                    { 601.4M },
                    { 940.9M },
                    { 1471.49M },
                    { 2302.78M },
                    { 3602.58M }
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
        public int TotalLun { get; set; } = 8;

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
            return CurrentLun == 1 && CurrentaQi == 1 && !IsRunning;
        }
        /// <summary>
        /// 添加统计信息到dic中
        /// </summary>
        /// <param name="number"></param>
        /// <param name="model"></param>
        public void CalcCode(Code code)
        {
            if(RunAndStop.IsTotalStop) return;
            if(RunAndStop.IsWinStop) return;
            
            List<PositionNumber> list = new List<PositionNumber>();
            if (this.model350List == null || model350List.Count == 0)
            {
                model350List = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List;
            }
            if (IsOriginBeginStatus())
            {
                //如果是初始状态,肯定不是执行中 开始执行

                Select350AndStartCalc(code);
            }
            else
            {
                //如果之前出手了,需要先验证是否中奖
                if (beforeChuShouNeedCheckZhongJiang)
                {
                    //当前执行中 当前是中
                    var housanStr = code.GetHou3String();
                    if (current350List.Contains(housanStr))
                    {
                        beforeChuShouNeedCheckZhongJiang = false;
                        //执行中,中出
                        TotalZhong++;
                        var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 2, 0];
                        int zhongjiangqi = (CurrentLun - 1) * 1;
                        yilouStatisticList[CurrentLun - 1].TotalCount = yilouStatisticList[CurrentLun - 1].TotalCount + 1;
                        TotalResult = TotalResult + zhongjiangAmount;
                        //记录最大金额
                        if (TotalResult > MaxResult)
                        {
                            MaxResult = TotalResult;
                        }

                        //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun - 1}轮已中出,中奖金额:{zhongjiangAmount}，总额【{TotalResult}】。");
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】，总流水【{TotalLiuShui}】。总挂次数{TotalGua}");

                        LunInit();
                        before350List = current350List;
                        CurrentLun = 1;
                        Select350AndStartCalc(code);
                        IsRunning = true;
                    }
                    else
                    {
                        //执行中，未中出
                        GuaCount++;
                        IsRunning = true;
                        CurrentaQi = 1;
                        GuaCount = 1;
                        beforeChuShouNeedCheckZhongJiang = false;
                        if (CurrentLun > TotalLun)
                        {
                            //超过总轮次，结束
                            TotalGua++;
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，已超过总轮次{TotalLun}轮，结束本次执行。");
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-总中奖次数{TotalZhong}，总额【{TotalResult}】，【流水总计{TotalLiuShui}】。总挂次数{TotalGua}");
                            LunInit();
                            before350List = current350List;
                            Select350AndStartCalc(code);
                            yilouStatisticList[9].TotalCount = yilouStatisticList[9].TotalCount + 1;
                            return;
                        }
                        before350List = current350List;
                        Select350AndGoonCalc(code);
                        IsRunning = true;
                    }
                }
                else
                {
                    //如果之前没出手,查找并执行

                    Select350AndGoonCalc(code);
                }
                //else
                //{
                //    // 如果不是执行中，说明上一轮中出后结束，开始下一轮
                //    before350List = current350List;
                //    Select350AndGoonCalc(code);
                //}
            }
        }

        /// <summary>
        /// 查找满足条件的号码并开始执行
        /// </summary>
        public void Select350AndStartCalc(Code code)
        {
            //list = Hou3Select350YiLouSetFormZhouQiZhongBusiness.model350List.
            if (CurrentLun == 0) CurrentLun = 1;

            //if (Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.Count >= 350)
            //{

            //    foreach (var currentRecord in Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List)
            //    {
            //        var lastScoreDate = currentRecord.ScoreDateList.LastOrDefault();
            //        if (lastScoreDate != null)
            //        {
            //            if (lastScoreDate.Score >= 80)
            //            {
            //                getEnoughRecordList.Add(getEnoughRecord);
            //            }
            //        }
            //    }
            //}
            //if (getEnoughRecord == null) return;
            List<Hou3Select350_ZhouQiZhongScore> getEnoughRecordList = new List<Hou3Select350_ZhouQiZhongScore>();
            getEnoughRecordList = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.
                Where(p => p.IsChuShou && p.Score >= 70 && p.ShouNumber == 1).ToList();
            if (getEnoughRecordList.Count <= 0) return;
            var kLineIsEnoughList = new List<Hou3Select350_ZhouQiZhongScore>();
            foreach (var record in getEnoughRecordList)
            {

                var result = KLine350ScoreCalc.KLineIsEnough(record.KLineList);
                if (result.Result)
                {
                    kLineIsEnoughList.Add(record);
                }
            }
            if (kLineIsEnoughList.Count <= 0) return;
            Random random = new Random(GetThreadSafeSeed());
            int num = kLineIsEnoughList.Count - 1;
            if (num < 0) num = 0;

            var index = random.Next(0, num);
            var getEnoughRecord = kLineIsEnoughList[index];
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
                //设置已经出手需要验证
                beforeChuShouNeedCheckZhongJiang = true;
                TotalResult = TotalResult - CurrentAmount;
                //记录最小金额
                if (TotalResult < MinResult)
                {
                    MinResult = TotalResult;
                }
                TotalLiuShui += CurrentAmount;
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
                CurrentLun = 2;
            }
        }
        /// <summary>
        /// 开始下一轮 从第一期开始
        /// </summary>
        /// <param name="code"></param>
        public void Select350AndGoonCalc(Code code)
        {
            if (CurrentLun == 0 || CurrentLun >= 9) CurrentLun = 1;

            List<Hou3Select350_ZhouQiZhongScore> getEnoughRecordList = new List<Hou3Select350_ZhouQiZhongScore>();
            var shouNumber = CurrentLun;
            //shouNumber = shouNumber % 4;
            //if (shouNumber == 0) shouNumber = 4;
            getEnoughRecordList = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.
                Where(p => p.IsChuShou && p.Score >= 70 && p.ShouNumber == shouNumber).ToList();
            if (getEnoughRecordList.Count <= 0) return;
            var kLineIsEnoughList = new List<Hou3Select350_ZhouQiZhongScore>();
            foreach (var record in getEnoughRecordList)
            {

                //var result = KLine350ScoreCalc.KLineIsEnough(record.KLineList);
                //if (result.Result)
                //{
                //    kLineIsEnoughList.Add(record);
                //}

                kLineIsEnoughList.Add(record);
            }
            if (kLineIsEnoughList.Count <= 0) return;
            Random random = new Random(GetThreadSafeSeed());
            int num = kLineIsEnoughList.Count - 1;
            if (num < 0) num = 0;

            var index = random.Next(0, num);
            var getEnoughRecord = kLineIsEnoughList[index];
            if (getEnoughRecord.Number350.Count > 0)
            {
                current350List = getEnoughRecord.Number350;

                IsRunning = true;
                CurrentaQi = 1;
                GuaCount = 1;
                CurrentAmount = LunAmountMatrix[CurrentLun - 1, 0];
                TotalResult = TotalResult - CurrentAmount;
                //记录最小金额
                if (TotalResult < MinResult)
                {
                    MinResult = TotalResult;
                }
                TotalLiuShui += CurrentAmount;
                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
                CurrentaQi = 2;

                beforeChuShouNeedCheckZhongJiang = true;
                CurrentLun++;
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
            //记录最小金额
            if (TotalResult < MinResult)
            {
                MinResult = TotalResult;
            }
            TotalLiuShui += CurrentAmount;
            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期,下注金额【{CurrentAmount}】,投注后总额【{TotalResult}】");
            //CurrentaQi++;
        }


        /// <summary>
        /// 获取线程安全的随机数种子
        /// </summary>
        private static int GetThreadSafeSeed()
        {
            lock (_threadLocalRandom)
            {
                return _threadLocalRandom.Value.Next();
            }
        }
    }
}
