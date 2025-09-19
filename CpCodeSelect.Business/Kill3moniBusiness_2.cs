using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Business
{
    public class Kill3moniBusiness_2
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        public Kill3moniBusiness_2(LogDelegate logMethod)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
        }
        public void SetLogMethod(LogDelegate logMethod)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
        }

        public int YiGuaCount { get; set; }
        private void LogInfo(string message) => _logMethod?.Invoke(message);
        /// <summary>
        /// 每轮上挂的次数
        /// </summary>
        private int NeedGuaTime = 2;
        /// <summary>
        /// 每轮的投注矩阵,倍数,不是金额
        /// </summary>
        private int[,] LunAmountMatrix = {
                    { 5,0},
                    { 15,19 },
                    { 43,55 },
                    { 116,148 },
                    { 307,390 },
                    { 803,1023 },
                    { 2096,2670 },
                    { 5464,6962 }
                };
        private decimal[,] ZhongJiangAmountMatrix = {
                    { 4.682M, 4.682M,},
                    { 14.046M, 17.7916M},
                    { 40.2625M, 51.502M},
                    { 108.6224M, 138.5872M},
                    { 287.4748M, 365.196M},
                    { 751.9292M, 957.9372M},
                    { 1962.6944M, 2500.188M},
                    { 5116.4896M, 6519.2168M}
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
        public int CurrentLun { get; set; } = 0;
        /// <summary>
        /// 当前期数
        /// </summary>
        public int CurrentaQi { get; set; } = 0;
        /// <summary>
        /// 当前倍数
        /// </summary>
        public int CurrentBei { get; set; }
        /// <summary>
        /// 总轮次
        /// </summary>
        public int TotalLun { get; set; } = 7;

        /// <summary>
        /// 当前上号的位置
        /// </summary>
        public Kill3Position Kill3Position { get; set; }
        /// <summary>
        /// 当前轮中奖次数
        /// </summary>
        public int CurrentLunZhongJiangCiShu { get; set; } = 0;
        /// <summary>
        /// 当前上号的位置
        /// </summary>
        public string CurrentPositionName { get; set; }
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
            CurrentLun = 0;
            CurrentaQi = 0;
            CurrentLunZhongJiangCiShu = 0;
            CurrentBei = 0;
        }
        /// <summary>
        /// 当前是否是原始开始状态
        /// </summary>
        /// <returns></returns>
        public bool IsOriginBeginStatus()
        {
            return CurrentLun == 0 && CurrentaQi == 0;
        }
        /// <summary>
        /// 是否是轮次开始状态
        /// </summary>
        /// <returns></returns>
        public bool IsLunBeginStatus()
        {
            return CurrentLun > 0 && CurrentaQi == 0;
        }
        /// <summary>
        /// 添加统计信息到dic中
        /// </summary>
        /// <param name="number"></param>
        /// <param name="model"></param>
        public void CalcCode(Code code)
        {
            if (IsOriginBeginStatus())
            {
                //如果是初始状态,则查找第一个挂3个的位置
                var positionNumber = code.Kill3ModelList.Where(p => p.GuaCount == NeedGuaTime).FirstOrDefault();
                if (positionNumber != null)
                {
                    //初始状态找到了挂相应*个的位置
                    CurrentPositionName = positionNumber.Name;
                    CurrentLun = 1;
                    CurrentaQi = 1;
                    CurrentBei = LunAmountMatrix[CurrentLun - 1, 0];
                    var CurrentTouru = CurrentBei * 0.729M;
                    TotalResult = TotalResult - CurrentTouru;
                    TotalLiuShui += CurrentTouru;
                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期的【{CurrentPositionName}】,倍数是【{CurrentBei}】投入是【{CurrentTouru}】,总额【{TotalResult}】");
                }
            }
            else
            {
                if (CurrentPositionName != null)
                {
                    var position = code.Kill3ModelList.Where(p => p.Name == CurrentPositionName && p.IsLianGua && p.GuaCount >= NeedGuaTime).FirstOrDefault();
                    if (position != null)
                    {
                        if (position.GuaHouZhong == 0)
                        {
                            //如果是轮次开始状态,则查找当前轮次的挂位置
                            CurrentLun++;
                            //var positionNumber = list.Where(p => p.GuaCount == 3 - 1 + CurrentLun).FirstOrDefault();
                            if (CurrentLun >= 8)
                            {
                                YiGuaCount++;
                                CurrentPositionName = string.Empty;
                                LunInit();
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期的【{CurrentPositionName}】,倍数是【{CurrentBei}】已挂,***************************************************");
                                return;
                            }
                            CurrentaQi = 1;
                            CurrentBei = LunAmountMatrix[CurrentLun - 1, 0];
                            var CurrentTouru = CurrentBei * 0.729M;
                            TotalResult = TotalResult - CurrentTouru;
                            TotalLiuShui += CurrentTouru;
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期的【{CurrentPositionName}】,倍数是【{CurrentBei}】投入是【{CurrentTouru}】,总额【{TotalResult}】");
                        }
                        else if (position.GuaHouZhong == 1)
                        {
                            if (CurrentLun == 1)
                            {
                                //第一轮中出,则继续投资第一轮
                                TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期的【{CurrentPositionName}】中出,中奖金额:{zhongjiangAmount},总额【{TotalResult}】");
                                //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");


                                CurrentBei = LunAmountMatrix[CurrentLun - 1, 0];
                                var CurrentTouru = CurrentBei * 0.729M;
                                TotalResult = TotalResult - CurrentTouru;
                                TotalLiuShui += CurrentTouru;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi+1}期的【{CurrentPositionName}】,倍数是【{CurrentBei}】投入是【{CurrentTouru}】,总额【{TotalResult}】");

                                //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-第一轮中出等待下一次");

                            }
                            else
                            {
                                //除了第2-7轮中出 当前中出一次
                                //只中一轮,则进入下一轮
                                var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期的【{CurrentPositionName}】中出,中奖金额:{zhongjiangAmount},目前中出1次,本轮还需要再中一次,总额【{TotalResult}】");
                                //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");

                                CurrentBei = LunAmountMatrix[CurrentLun - 1, 1];
                                var CurrentTouru = CurrentBei * 0.729M;
                                TotalResult = TotalResult - CurrentTouru;
                                TotalLiuShui += CurrentTouru;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi+1}期的【{CurrentPositionName}】,倍数是【{CurrentBei}】投入是【{CurrentTouru}】,总额【{TotalResult}】");
                            }
                            CurrentaQi++;
                        }
                        else if (position.GuaHouZhong == 2)
                        {
                            //如果当前的连中期数大于等于设置的2,则表示已中出
                            CurrentaQi=2;
                            var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                            TotalResult = TotalResult + zhongjiangAmount;
                            LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentaQi}期的【{CurrentPositionName}】已中出，中奖金额:{zhongjiangAmount},总额【{TotalResult}】,等待下一次机会☺");
                            CurrentPositionName = string.Empty;
                            LunInit();
                        }
                    }
                }
            }
        }
    }
}
