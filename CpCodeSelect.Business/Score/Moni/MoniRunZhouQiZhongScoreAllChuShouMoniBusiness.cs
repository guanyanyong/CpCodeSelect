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

namespace CpCodeSelect.Business.Score.Moni
{
    /// <summary>
    /// 模拟执行8个1轮的确认点买入
    /// </summary>
    public class MoniRunZhouQiZhongScoreAllChuShouMoniBusiness
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        private List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        public List<YilouStatistic> chuShouWeiZhongList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private List<Hou3Select350_ZhouQiZhongScore> CurrentExecuteList = new List<Hou3Select350_ZhouQiZhongScore>();
        public decimal MaxResult = 0;
        public decimal MinResult = 0;

        /// <summary>
        /// 上次出手,本次需要检查是否中奖
        /// </summary>
        private bool beforeChuShouNeedCheckZhongJiang = false;
        public MoniRunZhouQiZhongScoreAllChuShouMoniBusiness(LogDelegate logMethod, List<Hou3Select350_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;

            for (int i = 0; i <= 8; i++)
            {
                YilouStatistic entity = new YilouStatistic();
                entity.YilouCount = i;
                entity.TotalCount = 0;
                yilouStatisticList.Add(entity);
                YilouStatistic chuShouWeiZhong = new YilouStatistic();
                chuShouWeiZhong.YilouCount = i;
                chuShouWeiZhong.TotalCount = 0;
                chuShouWeiZhongList.Add(chuShouWeiZhong);
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
        /// <param name="code"></param>
        /// <param name="zhongHouDelete"></param>
        public void CalcCode(Code code, bool zhongHouDelete = false)
        {
            List<PositionNumber> list = new List<PositionNumber>();
            if (this.model350List == null || model350List.Count == 0)
            {
                model350List = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List;
            }
            Select350AndStartCalc(code, zhongHouDelete);
            
        }

        /// <summary>
        /// 查找满足条件的号码并开始执行 同时计算是否中奖
        /// </summary>
        public void Select350AndStartCalc(Code code, bool zhongHouDelete = false)
        {
            List<Hou3Select350_ZhouQiZhongScore> getEnoughRecordList = new List<Hou3Select350_ZhouQiZhongScore>();
            getEnoughRecordList = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.
                Where(p => p.IsChuShou && p.Score >= 70).ToList();
            if (getEnoughRecordList.Count <= 0) return;
            if (CurrentExecuteList.Count > 0)
            {
                //如果之前存在记录,说明已经执行过了,需要验证之前的记录是否中奖
                var housanStr = code.GetHou3String();
                foreach(var record in CurrentExecuteList)
                {
                    if (record.Number350.Contains(housanStr))
                    {
                        //如果中奖了,计算中奖金额
                        var count = record.ShouNumber - 1;
                        var zhongjiangAmount = ZhongJiangAmountMatrix[count, 0];
                        yilouStatisticList[count].TotalCount = yilouStatisticList[count].TotalCount + 1;
                        //中奖了的话,把未中的统计数减去1
                        //如果第六期中奖了 需要把5-0的都减去1
                        for (var i= 0;i <= count; i++){
                            chuShouWeiZhongList[i].TotalCount = chuShouWeiZhongList[i].TotalCount - 1;
                            if (chuShouWeiZhongList[i].TotalCount < 0)
                            {
                                chuShouWeiZhongList[i].TotalCount = 0;
                            }
                            LogInfo($"【中奖更新】{i+1}手未中总计{chuShouWeiZhongList[i].TotalCount}");
                        }
                        for(var i= record.ShouNumber+1; i <= 8; i++)
                        {
                            LogInfo($"【中奖更新】{i}手未中总计{chuShouWeiZhongList[i-1].TotalCount}");
                        }
                        TotalResult = TotalResult + zhongjiangAmount;
                        //记录最大金额
                        if(TotalResult > MaxResult)
                        {
                            MaxResult = TotalResult;
                        }
                        TotalZhong++;
                        LogInfo($"【中奖】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额第{record.ShouNumber}手:{zhongjiangAmount},中奖后金额:{TotalResult},{record.ShouNumber}手未中总计{chuShouWeiZhongList[count].TotalCount}");
                        if(zhongHouDelete){
                            //如果是中后删除,把中了的记录都删除掉
                            var foundRecord = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.Where(p => p.CodeNumber == record.CodeNumber && p.CodeQiHao == record.CodeQiHao && p.Number350 == record.Number350).FirstOrDefault();
                            Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.Remove(foundRecord);
                        }
                    }
                    else
                    {
                        if (record.ShouNumber == 8){
                            //如果已经是第8手了,还没有中奖,说明这个号码周期结束了,记录挂的次数
                            TotalGua++;
                            yilouStatisticList[8].TotalCount = yilouStatisticList[8].TotalCount + 1;

                            //爆了的话,把未中奖的统计数减去1
                            for (var i = 0; i <= 7; i++)
                            {
                                chuShouWeiZhongList[i].TotalCount = chuShouWeiZhongList[i].TotalCount - 1;

                            }

                            if (zhongHouDelete)
                            {
                                //如果是中后删除,把爆了的记录从总行情中删除掉
                                var foundRecord = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.Where(p => p.CodeNumber == record.CodeNumber && p.CodeQiHao == record.CodeQiHao && p.Number350 == record.Number350).FirstOrDefault();
                                Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.Remove(foundRecord);
                            }
                        }
                    }
                }
            }
            //计算完成后清空旧记录,添加新的记录
            CurrentExecuteList.Clear();
            //CurrentExecuteList.AddRange(getEnoughRecordList);
            foreach (var record in getEnoughRecordList)
            {

                Hou3Select350_ZhouQiZhongScore score = new Hou3Select350_ZhouQiZhongScore();
                score.CodeQiHao = record.CodeQiHao;
                score.IsChuShou = record.IsChuShou;
                score.ShouNumber = record.ShouNumber;
                score.CodeNumber = record.CodeNumber;
                score.Number350 = record.Number350;
                score.Score = record.Score;
                CurrentExecuteList.Add(score);
            }

            //添加投注金额和流水
            foreach(var record in CurrentExecuteList)
            {
                var count = record.ShouNumber - 1;
                var touzhuAmount = LunAmountMatrix[count, 0];
                TotalResult = TotalResult - touzhuAmount;
                //记录最小金额
                if (TotalResult < MinResult)
                {
                    MinResult = TotalResult;
                }
                TotalLiuShui += touzhuAmount;
                //投注了对应期的话,把未中的统计数加上1
                chuShouWeiZhongList[count].TotalCount = chuShouWeiZhongList[count].TotalCount + 1;
                LogInfo($"【投注】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},投注第{record.ShouNumber}手:{touzhuAmount},投注后总金额:{TotalResult},{record.ShouNumber}手未中总计{chuShouWeiZhongList[count].TotalCount}");
            }
        }
    }
}
