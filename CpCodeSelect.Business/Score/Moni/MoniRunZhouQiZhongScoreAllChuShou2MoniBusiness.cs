using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.ZiJinFangAn;
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
    public class MoniRunZhouQiZhongScoreAllChuShou2MoniBusiness
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
        private Hou3Select350_ZhouQiZhongScore currentExecute = null;
        public decimal MaxResult = 0;
        public decimal MinResult = 0;
        /// <summary>
        /// 资金方案,包含初始本金,当前本金,点击次数,拆分阶段等信息
        /// </summary>
        public ZiJinFangAn ZiJinFangAn { get; set; }
        /// <summary>
        /// 是否投注中,是的话需要判断是否中奖
        /// </summary>
        public bool IsTouZhuing { get; set; } = false;

        /// <summary>
        /// 上次出手,本次需要检查是否中奖
        /// </summary>
        private bool beforeChuShouNeedCheckZhongJiang = false;
        public MoniRunZhouQiZhongScoreAllChuShou2MoniBusiness(LogDelegate logMethod, List<Hou3Select350_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;
            ZiJinFangAn = new ZiJinFangAn(2000, 2000);
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
        private decimal[] LunAmount = {200, 300, 500, 900, 1700, 3300, 6500, 12900 };
        /// <summary>
        /// 总金额
        /// </summary>
        public Decimal TotalResult { get; set; } = 26300;
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
        /// 当前轮中奖次数
        /// </summary>
        public int CurrentLunZhongJiangCiShu { get; set; } = 0;
        public bool IsRunning { get; set; } = false;
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
        public void LunAdd() {
            CurrentLun++;

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
            if (IsTouZhuing && currentExecute != null && !string.IsNullOrEmpty(currentExecute.CodeQiHao))
            {
                //如果是投注中的状态,说明需要先计算是否中奖,再进行下一轮的投注
                var housanStr = code.GetHou3String();
                var isZhong = currentExecute.Number350.Contains(housanStr);
                var kaiJiangResult = ZiJinFangAn.KaiJiang(isZhong);
                if (isZhong)
                {
                    if (kaiJiangResult.MaxChange)
                    {
                        LogInfo($"###########################中奖后达到重置要求,重置###########################");
                    }
                    LogInfo($"【中奖】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖后总金额:{ZiJinFangAn.CurrentPrincipal},本轮重置需要金额{ZiJinFangAn.MaxPrincipal}");

                    LogInfo($"点击次数:{ZiJinFangAn.ClickCount},当前拆分阶段{ZiJinFangAn.SplitStage},总点击次数{ZiJinFangAn.AllTotalTime}");
                }
                else
                {
                    LogInfo($"【未中奖】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},未中奖后总金额:{ZiJinFangAn.CurrentPrincipal}");
                    LogInfo($"点击次数:{ZiJinFangAn.ClickCount},当前拆分阶段{ZiJinFangAn.SplitStage},总点击次数{ZiJinFangAn.AllTotalTime}");
                }
                //设置后,把当前执行的记录置空,等待下一轮重新赋值
                IsTouZhuing = false;
            }
            List<Hou3Select350_ZhouQiZhongScore> getEnoughRecordList = new List<Hou3Select350_ZhouQiZhongScore>();
            getEnoughRecordList = Hou3Select350YiLouSetFormScoreAndChuShouBusiness.model350List.
                Where(p => p.IsChuShou && p.Score >= 150).ToList();
            if (getEnoughRecordList.Count <= 0) return;
            //CurrentExecuteList.AddRange(getEnoughRecordList);
            var randomIndex = _threadLocalRandom.Value.Next(0, getEnoughRecordList.Count);
            randomIndex--;
            if (randomIndex < 0) randomIndex = 0;
            var record = getEnoughRecordList[randomIndex];
            currentExecute = record;

            //添加出手记录
            var touZhuResult = ZiJinFangAn.TouZhu();
            if (touZhuResult.Success)
            {
                IsTouZhuing = true;
                var currentTouzhu = ZiJinFangAn.CurrentBetAmount * ZiJinFangAn.PerBetAmount;
                //投注了对应期的话,把对应未中的统计数加上1
                LogInfo($"【投注】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},投注金额{currentTouzhu},投注后总金额:{ZiJinFangAn.CurrentPrincipal - currentTouzhu},总流水{ZiJinFangAn.TotalLiuShui}");
            }
            else
            {
                LogInfo("投注失败,进入下一轮");

            }
        }
        private void InitChuShouWeiZhong()
        {
            for (var i = 0; i <= 7; i++)
            {
                if (chuShouWeiZhongList[i].TotalCount < 0)
                    chuShouWeiZhongList[i].TotalCount = 0;
            }
        }
    }
}
