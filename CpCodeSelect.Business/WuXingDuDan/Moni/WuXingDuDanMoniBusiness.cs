using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Config;
using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.ZiJinFangAn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Business.WuXingDuDan.Moni
{
    /// <summary>
    /// 模拟执行8个1轮的确认点买入
    /// </summary>
    public class WuXingDuDanMoniBusiness
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        private List<Hou3Select156_ZhouQiZhongScore> model350List = new List<Hou3Select156_ZhouQiZhongScore>();
        public List<string> current350List = new List<string>();
        public List<string> before350List = new List<string>();
        public List<YilouStatistic> yilouStatisticList = new List<YilouStatistic>();
        public List<YilouStatistic> chuShouWeiZhongList = new List<YilouStatistic>();
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        private List<Hou3Select156_ZhouQiZhongScore> CurrentExecuteList = new List<Hou3Select156_ZhouQiZhongScore>();
        public Hou3Select156_ZhouQiZhongScore currentExecute = null;
        public decimal MaxResult = 0;
        public decimal MinResult = 0;
        /// <summary>
        /// 资金方案,包含初始本金,当前本金,点击次数,拆分阶段等信息
        /// </summary>
        public ZiJinFangAnV40951 ZiJinFangAn { get; set; }
        /// <summary>
        /// 是否投注中,是的话需要判断是否中奖
        /// </summary>
        public bool IsTouZhuing { get; set; } = false;
        public void Reset()
        {
            ZiJinFangAn = new ZiJinFangAnV40951();
            beforeChuShouNeedCheckZhongJiang = false;
            currentExecute = null;
            yilouStatisticList.Clear();
            chuShouWeiZhongList.Clear();
            CurrentExecuteList.Clear();
            MaxResult = 0;
            MinResult = 0;
        }
        /// <summary>
        /// 上次出手,本次需要检查是否中奖
        /// </summary>
        private bool beforeChuShouNeedCheckZhongJiang = false;
        public WuXingDuDanMoniBusiness(LogDelegate logMethod, List<Hou3Select156_ZhouQiZhongScore> model350List)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
            this.model350List = model350List;
            ZiJinFangAn = new ZiJinFangAnV40951();
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
        private decimal[] LunAmount = { 200, 200, 400, 600, 1000, 1600, 2600, 10800 };
        /// <summary>
        /// 总金额
        /// </summary>
        public Decimal TotalResult { get; set; } = 17400;
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

        public int MaxLianGua = 0;
        public int CurrentLianGua = 0;
        public string MaxLianGuaCurrentQiHao;
        /// <summary>
        /// 是否需要统计超过NeedCalcMaxGuaCount的挂数的数值,默认开启
        /// </summary>
        public int NeedCalcMaxGuaCount = AppConfig.Current.LunSettings.NeedCalcMaxGuaCount;
        /// <summary>
        /// 设置的需要统计超过就计算的数值,默认35
        /// </summary>
        public int CalcMaxGuaCount = AppConfig.Current.LunSettings.MaxGuaCount;
        /// <summary>
        /// 上一次进行记录最大挂数的期号
        /// </summary>
        public string LastCalcMaxGuaCountQiHao = string.Empty;
        /// <summary>
        /// 当前统计的超过最大挂的次数
        /// </summary>
        public int AllCalcMaxGuaSumCount = 0;
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
        public void LunAdd()
        {
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
                model350List = WuXingDuDanBusiness.model350List;
            }
            Select156AndStartCalc(code, zhongHouDelete);

        }

        /// <summary>
        /// 查找满足条件的号码并开始执行 同时计算是否中奖
        /// </summary>
        public void Select156AndStartCalc(Code code, bool zhongHouDelete = false)
        {
            if (IsTouZhuing && currentExecute != null && !string.IsNullOrEmpty(currentExecute.CodeQiHao))
            {
                //如果是投注中的状态,说明需要先计算是否中奖,再进行下一轮的投注
                var isZhong = currentExecute.Number156.Contains(code.CodeNumber);

                int zhongCount = 0;
                if (isZhong)
                {

                    if (code.Wan.Number.ToString() == currentExecute.DanNumber) zhongCount++;
                    if (code.Qian.Number.ToString() == currentExecute.DanNumber) zhongCount++;
                    if (code.Bai.Number.ToString() == currentExecute.DanNumber) zhongCount++;
                    if (code.Shi.Number.ToString() == currentExecute.DanNumber) zhongCount++;
                    if (code.Ge.Number.ToString() == currentExecute.DanNumber) zhongCount++;
                }

                var kaiJiangResult = ZiJinFangAn.SmallKaiJiang(isZhong, zhongCount);
                if (isZhong)
                {
                    CurrentLianGua = 0;
                    if (kaiJiangResult.MaxChange)
                    {
                        LogInfo($"###########################中奖后达到重置要求,重置###########################");
                    }
                    if (!string.IsNullOrEmpty(kaiJiangResult.Message))
                    {
                        LogInfo($"###########################{kaiJiangResult.Message}###########################");
                    }
                    if (kaiJiangResult.MessageList != null && kaiJiangResult.MessageList.Count > 0)
                    {
                        foreach (var message in kaiJiangResult.MessageList)
                        {
                            LogInfo($"###########################{message}###########################");
                        }
                    }
                    LogInfo($"【中奖】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖后本轮总金额:{ZiJinFangAn.SmallCurrentPrincipal},本轮重置需要金额{ZiJinFangAn.SmallMaxPrincipal}");

                    LogInfo($"【中奖】点击次数:{ZiJinFangAn.SmallClickCount},当前拆分阶段{ZiJinFangAn.SmallSplitStage},总点击次数{ZiJinFangAn.SmallTotalTime}");
                    LogInfo($"【中奖】当前Middle轮:【{ZiJinFangAn.MiddleCurrentLun}】,当前Middle资金{ZiJinFangAn.MiddleCurrentPrincipalExcludSmall},当前Large资金{ZiJinFangAn.LargeTotalPrincipal}");
                }
                else
                {

                    CurrentLianGua++;
                    LogInfo($"【未中奖】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},未中奖后总金额:{ZiJinFangAn.SmallCurrentPrincipal}");
                    LogInfo($"【未中奖】点击次数:{ZiJinFangAn.SmallClickCount},当前拆分阶段{ZiJinFangAn.SmallSplitStage},总点击次数{ZiJinFangAn.LargeTotalClickCount}");
                    LogInfo($"【未中奖】当前Middle轮:【{ZiJinFangAn.MiddleCurrentLun}】,当前Middle资金不含Small{ZiJinFangAn.MiddleCurrentPrincipalExcludSmall},当前Large资金{ZiJinFangAn.LargeTotalPrincipal}");
                }
                //设置后,把当前执行的记录置空,等待下一轮重新赋值
                IsTouZhuing = false;
                if (CurrentLianGua > MaxLianGua)
                {
                    MaxLianGua = CurrentLianGua;
                    MaxLianGuaCurrentQiHao = code.CodeQiHao;
                }
                if (NeedCalcMaxGuaCount == 1)
                {
                    if (CurrentLianGua >= CalcMaxGuaCount)
                    {

                        var codeDecimal = Convert.ToDecimal(code.CodeQiHao);

                        //如果上一期的统计为空号,或者当前期和上一期的差值超过最大连挂认为是新的一轮最大挂
                        if (!string.IsNullOrEmpty(LastCalcMaxGuaCountQiHao))
                        {
                            var lastCodeDecimal = Convert.ToDecimal(LastCalcMaxGuaCountQiHao);

                            if (Math.Abs(codeDecimal - lastCodeDecimal) > CalcMaxGuaCount)
                            {
                                AllCalcMaxGuaSumCount++;
                                LastCalcMaxGuaCountQiHao = code.CodeQiHao;
                            }
                            LogInfo($"**************************【未中奖】连挂超过{CalcMaxGuaCount}期,当前挂的数量{AllCalcMaxGuaSumCount}，当前期号:{code.CodeQiHao}，当前开奖号:{code.CodeNumber}********************************************************");

                        }
                        else
                        {
                            //如果上一期的统计为空号
                            AllCalcMaxGuaSumCount++;
                            LastCalcMaxGuaCountQiHao = code.CodeQiHao;
                            LogInfo($"【未中奖】当前Middle轮:【{ZiJinFangAn.MiddleCurrentLun}】,当前Middle资金不含Small{ZiJinFangAn.MiddleCurrentPrincipalExcludSmall},当前Large资金{ZiJinFangAn.LargeTotalPrincipal}");

                            LogInfo($"**************************【未中奖】连挂超过{CalcMaxGuaCount}期,当前挂的数量{AllCalcMaxGuaSumCount}，当前期号:{code.CodeQiHao}，当前开奖号:{code.CodeNumber}********************************************************");
                        }
                    }
                }
            }
            List<Hou3Select156_ZhouQiZhongScore> getEnoughRecordList = new List<Hou3Select156_ZhouQiZhongScore>();

            foreach (var model in WuXingDuDanBusiness.model350List)
            {
                if (model.KLineList.Count > 0) {
                    var kline = model.KLineList[model.KLineList.Count - 1];
                    if (kline.IsOverMiddle) 
                    {
                        getEnoughRecordList.Add(model);
                    }
                }
            }

            //getEnoughRecordList = WuXingDuDanBusiness.model350List.
            //    Where(p => p.IsChuShou && p.Score >= 80).ToList();
            if (getEnoughRecordList.Count <= 0)
            {
                return;
            }
            var getHighEnouthRecordList= new List<Hou3Select156_ZhouQiZhongScore>();
            foreach (var model in getEnoughRecordList)
            {
                var kLineCount = model.KLineList.Count;
                var lastData = model.KLineList[kLineCount - 1];
                var prev1 = model.KLineList[kLineCount - 2];
                var prev2 = model.KLineList[kLineCount - 3];

                // 判断上轨、中轨、下轨是否都在上升
                bool upperUp = lastData.Bolling.BollUpperValue > prev1.Bolling.BollUpperValue &&
                              prev1.Bolling.BollUpperValue > prev2.Bolling.BollUpperValue;

                bool middleUp = lastData.Bolling.MiddleValue > prev1.Bolling.MiddleValue &&
                               prev1.Bolling.MiddleValue > prev2.Bolling.MiddleValue;

                bool lowerUp = lastData.Bolling.BollLowerValue > prev1.Bolling.BollLowerValue &&
                              prev1.Bolling.BollLowerValue > prev2.Bolling.BollLowerValue;

                if (upperUp && middleUp && lowerUp)
                {
                    getHighEnouthRecordList.Add(model);
                }
            }
            var getMiddleEnouthRecordList = new List<Hou3Select156_ZhouQiZhongScore>();
            foreach (var model in getEnoughRecordList)
            {
                var kLineCount = model.KLineList.Count;
                var lastData = model.KLineList[kLineCount - 1];
                var prev1 = model.KLineList[kLineCount - 2];
                var prev2 = model.KLineList[kLineCount - 3];

                // 判断上轨、中轨、下轨是否都在上升
                bool upperUp = lastData.Bolling.BollUpperValue > prev1.Bolling.BollUpperValue &&
                              prev1.Bolling.BollUpperValue > prev2.Bolling.BollUpperValue;

                bool middleUp = lastData.Bolling.MiddleValue > prev1.Bolling.MiddleValue &&
                               prev1.Bolling.MiddleValue > prev2.Bolling.MiddleValue;

                bool lowerUp = lastData.Bolling.BollLowerValue > prev1.Bolling.BollLowerValue &&
                              prev1.Bolling.BollLowerValue > prev2.Bolling.BollLowerValue;

                if (upperUp && middleUp )
                {
                    getMiddleEnouthRecordList.Add(model);
                }
            }
            int randomIndex = -1;
            if (getHighEnouthRecordList.Count > 0)
            {
                randomIndex = _threadLocalRandom.Value.Next(0, getHighEnouthRecordList.Count);
                randomIndex--;
                if (randomIndex < 0) randomIndex = 0;
                currentExecute = getHighEnouthRecordList[randomIndex];
            }
            else if (getMiddleEnouthRecordList.Count > 0)
            {
                randomIndex = _threadLocalRandom.Value.Next(0, getMiddleEnouthRecordList.Count);
                randomIndex--;
                if (randomIndex < 0) randomIndex = 0;
                currentExecute = getMiddleEnouthRecordList[randomIndex];
            }
            else if (getEnoughRecordList.Count > 0)
            {
                randomIndex = _threadLocalRandom.Value.Next(0, getEnoughRecordList.Count);
                randomIndex--;
                if (randomIndex < 0) randomIndex = 0;
                currentExecute = getEnoughRecordList[randomIndex];
            }
            else
            {
                return;
            }


            //添加出手记录
            var touZhuResult = ZiJinFangAn.SmallTouZhu();
            if (touZhuResult.Success)
            {
                IsTouZhuing = true;
                var currentTouzhu = ZiJinFangAn.SmallCurrentBetAmount * ZiJinFangAnV40951.SmallPerBetAmount;
                //投注了对应期的话,把对应未中的统计数加上1
                LogInfo($"【投注】[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},投注胆码:{currentExecute.DanNumber},投注金额{currentTouzhu},投注后总金额:{ZiJinFangAn.SmallCurrentPrincipal - currentTouzhu},总流水{ZiJinFangAn.LargeTotalLiuShui}");
                LogInfo($"【投注】当前Middle轮:{ZiJinFangAn.MiddleCurrentLun},当前Middle资金{ZiJinFangAn.MiddleCurrentPrincipalExcludSmall},当前Large资金{ZiJinFangAn.LargeTotalPrincipal}");
                if (touZhuResult.MessageList != null && touZhuResult.MessageList.Count > 0)
                {
                    foreach (var message in touZhuResult.MessageList)
                    {
                        LogInfo($"【投注后返回结果信息】**************{message}******************************");
                    }
                }
            }
            else
            {
                LogInfo("投注失败,进入下一轮");

            }
        }
    }
}
