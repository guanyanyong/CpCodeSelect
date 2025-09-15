using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Business
{
    /// <summary>
    /// 模拟执行3个3轮的大小单双单双
    /// </summary>
    public class DaXiaoDanShuangMoniBusiness
    {
        public delegate void LogDelegate(string message);
        private LogDelegate _logMethod;
        public DaXiaoDanShuangMoniBusiness(LogDelegate logMethod)
        {
            _logMethod = logMethod ?? throw new ArgumentNullException(nameof(logMethod));
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
        /// 每轮的投注矩阵,倍数,不是金额
        /// </summary>
        private int[,] LunAmountMatrix = {
                    { 114, 243,519 },
                    { 568, 1214, 2593 },
                    { 3182, 6798, 14523 }
                };
        private decimal[,] ZhongJiangAmountMatrix = {
                    { 106.76M, 227.56M,486M },
                    { 531.93M, 1141.71M, 2428.32M},
                    { 2979.91M, 6366.26M, 13600.66M}
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
        public void CalcCode(Code code)
        {
            List<PositionNumber> list = new List<PositionNumber>();
            list.Add(code.Wan);
            list.Add(code.Qian);
            list.Add(code.Bai);
            list.Add(code.Shi);
            list.Add(code.Ge);
            if (IsOriginBeginStatus())
            {
                //如果是初始状态,则查找第一个挂2个的位置
                var positionNumber = list.Where(p => p.DanShuangLianKaiGuaCount == LunGuaTime[0] || p.DaXiaoLianKaiGuaCount == LunGuaTime[0]).FirstOrDefault();
                if (positionNumber != null)
                {
                    //初始状态找到了挂相应个的位置
                    CurrentPositionType = positionNumber.PositionType;
                    CurrentLun = 1;
                    CurrentaQi = 1;
                    CurrentBei = LunAmountMatrix[CurrentLun - 1, 0];
                    if (positionNumber.DanShuangLianKaiGuaCount == LunGuaTime[0])
                    {
                        //如果是单双连开挂
                        IsDaXiao = false;
                        CurrentStr = positionNumber.DanShuangTuijianNumber; 
                        TotalResult = TotalResult - CurrentBei / 2;
                        TotalLiuShui += CurrentBei / 2;
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的单双,当前推荐【{positionNumber.DanShuangLianGuaTuiJianNumber}】,倍数是【{CurrentBei}】,总额【{TotalResult}】");
                        
                    }
                    else
                    {
                        //如果是大小连开挂
                        IsDaXiao = true;
                        CurrentStr = positionNumber.DaXiaoTuijianNumber;
                        TotalResult = TotalResult - CurrentBei / 2;
                        TotalLiuShui += CurrentBei / 2;
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的大小,当前推荐【{positionNumber.DaXiaoLianGuaTuiJianNumber}】,倍数是【{CurrentBei}】,总额【{TotalResult}】");
                        
                    }
                }
            }
            else if (IsLunBeginStatus())
            {
                //如果是轮次开始状态,则查找当前轮次的挂位置
                var positionNumber = list.Where(p => p.DanShuangLianKaiGuaCount == LunGuaTime[CurrentLun - 1] || p.DaXiaoLianKaiGuaCount == LunGuaTime[CurrentLun - 1]).FirstOrDefault();
                if (positionNumber != null)
                {
                    //轮次的开始状态找到了对应的挂了几次,即出现机会
                    CurrentPositionType = positionNumber.PositionType;
                    CurrentaQi = 1;
                    CurrentBei = LunAmountMatrix[CurrentLun - 1, 0];

                    if (positionNumber.DanShuangLianKaiGuaCount == LunGuaTime[CurrentLun - 1])
                    {
                        //如果是单双连开挂
                        IsDaXiao = false;
                        CurrentStr = positionNumber.DanShuangTuijianNumber;
                        TotalResult = TotalResult - CurrentBei / 2;
                        TotalLiuShui += CurrentBei / 2;
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的单双当前推荐【{positionNumber.DanShuangLianGuaTuiJianNumber}】,倍数是【{CurrentBei}】,总额【{TotalResult}】");
                    }
                    else
                    {
                        //如果是大小连开挂
                        IsDaXiao = true;
                        CurrentStr = positionNumber.DaXiaoTuijianNumber; 
                        TotalResult = TotalResult - CurrentBei / 2;
                        TotalLiuShui += CurrentBei / 2;
                        LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的大小当前推荐【{positionNumber.DaXiaoLianGuaTuiJianNumber}】,倍数是【{CurrentBei}】,总额【{TotalResult}】");
                        
                    }
                }
            }
            else
            {
                //继续当前轮次
                var position = list.Where(p => p.PositionType == CurrentPositionType).FirstOrDefault();
                if (position != null)
                {
                    if (IsDaXiao)
                    {
                        //上一期是大小的逻辑

                        //先获取当前的大小连挂期数,再做判断
                        var lianGuaCount = position.DaXiaoLianKaiGuaCount;
                        if (lianGuaCount > LunGuaTime[CurrentLun - 1])
                        {
                            //如果当前的连挂期数大于设置的当前轮的初始连挂期数,则表示上一期未中奖,继续挂
                            CurrentaQi++;
                            if (CurrentaQi > 3)
                            {
                                //如果当前期数大于3,则表示当前轮次结束,进入下一轮
                                CurrentLun++;
                                if (CurrentLun > TotalLun)
                                {
                                    //如果当前轮次大于总轮次,则表示结束
                                    //Todo 添加结束逻辑
                                    //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-模拟结束,总金额是{TotalResult}");
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi - 1}期的【{CurrentPositionType.ToString()}】位的大小已挂,总额【{TotalResult}】,当前9期计划已挂********************");
                                    LunInit();
                                    return;
                                }
                                else
                                {
                                    //进入下一轮,重新设置当前的连挂期数和倍数
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun - 1}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi - 1}期的【{CurrentPositionType.ToString()}】位的大小已挂,总额【{TotalResult}】,等待机会进入下一轮##########");
                                    CurrentLunZhongJiangCiShu = 0;
                                    CurrentBei = 0;
                                }
                                CurrentaQi = 0;
                            }
                            else
                            {
                                //当前期数未大于3,则继续当前轮次,倍数变为下一期
                                CurrentBei = LunAmountMatrix[CurrentLun - 1, CurrentaQi - 1]; 
                                TotalResult = TotalResult - CurrentBei / 2;
                                TotalLiuShui += CurrentBei / 2;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi - 1}期的【{CurrentPositionType.ToString()}】位的大小已挂当前推荐【{position.DaXiaoLianGuaTuiJianNumber}】,进入下一期倍数是【{CurrentBei}】,总额【{TotalResult}】+++++");
                                
                            }
                        }
                        else if (lianGuaCount == 0)
                        {
                            //说明中出
                            if (CurrentLun == 1)
                            {
                                //第一轮中出,则直接重新初始化
                                TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的大小已中出,总额【{TotalResult}】,机会寻找中...");
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                                LunInit();
                                //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-第一轮中出等待下一次");

                            }
                            else
                            {
                                //第2轮第3轮中出 如果当前轮次大于1,则表示当前轮次结束,进入下一轮

                                CurrentLunZhongJiangCiShu++;
                                if (CurrentLunZhongJiangCiShu > 2) CurrentLunZhongJiangCiShu = 2;
                                if (CurrentLunZhongJiangCiShu == 2)
                                {
                                    var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的大小已中出,已中出2次,总额【{TotalResult}】,机会寻找中...");
                                    //中出2轮,则重新初始化
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                                    LunInit();
                                }
                                else
                                {
                                    //只中一轮,则进入下一轮
                                    var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的大小已中出,目前中出1次,本轮还需要再中一次,总额【{TotalResult}】,机会寻找中...");
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");

                                    CurrentaQi = 0;
                                    CurrentBei = 0;
                                }

                            }

                        }
                    }
                    else
                    {
                        //上一期是单双的逻辑
                        //先获取当前的大小连挂期数,再做判断
                        var lianGuaCount = position.DanShuangLianKaiGuaCount;
                        if (lianGuaCount > LunGuaTime[CurrentLun - 1])
                        {
                            //如果当前的连挂期数大于设置的当前轮的初始连挂期数,则表示上一期未中奖,继续挂
                            CurrentaQi++;
                            if (CurrentaQi > 3)
                            {
                                //如果当前期数大于3,则表示当前轮次结束,进入下一轮
                                CurrentLun++;
                                if (CurrentLun > TotalLun)
                                {
                                    CurrentLun = TotalLun;
                                    //如果当前轮次大于总轮次,则表示结束
                                    //Todo 添加结束逻辑
                                    //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-模拟结束,总金额是{TotalResult}");
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi - 1}期的【{CurrentPositionType.ToString()}】位的单双已挂,总额【{TotalResult}】,当前9期计划已挂********************");
                                    LunInit();
                                    return;
                                }
                                else
                                {
                                    //进入下一轮,重新设置当前的连挂期数和倍数
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun - 1}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi - 1}期的【{CurrentPositionType.ToString()}】位的单双已挂,总额【{TotalResult}】,等待机会进入下一轮##########");
                                    CurrentLunZhongJiangCiShu = 0;
                                    CurrentBei = 0;
                                }
                                CurrentaQi = 0;
                            }
                            else
                            {
                                //当前期数未大于3,则继续当前轮次,倍数变为下一期
                                CurrentBei = LunAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                TotalResult = TotalResult - CurrentBei / 2;
                                TotalLiuShui += CurrentBei / 2;
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi - 1}期的【{CurrentPositionType.ToString()}】位的单双已挂当前推荐【{position.DanShuangLianGuaTuiJianNumber}】,进入下一期倍数是【{CurrentBei}】,总额【{TotalResult}】+++++");
                                
                            }
                        }
                        else if (lianGuaCount == 0)
                        {
                            //说明中出
                            if (CurrentLun == 1)
                            {
                                //第一轮中出,则直接重新初始化
                                var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu + 1}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的单双已中出,总额【{TotalResult}】,机会寻找中...");
                                LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                                LunInit();
                                //LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-第一轮中出等待下一次");

                            }
                            else
                            {
                                //第2轮第3轮中出 如果当前轮次大于1,则表示当前轮次结束,进入下一轮

                                CurrentLunZhongJiangCiShu++;
                                if (CurrentLunZhongJiangCiShu > 2) CurrentLunZhongJiangCiShu = 2;
                                if (CurrentLunZhongJiangCiShu == 2)
                                {
                                    var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的单双已中出,已中出2次,总额【{TotalResult}】,机会寻找中...");
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");
                                    //中出2轮,则重新初始化
                                    LunInit();
                                }
                                else
                                {
                                    //只中一轮,则进入下一轮
                                    var zhongjiangAmount = ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    TotalResult = TotalResult + ZhongJiangAmountMatrix[CurrentLun - 1, CurrentaQi - 1];
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber}，第{CurrentLun}轮第{CurrentLunZhongJiangCiShu}次第{CurrentaQi}期的【{CurrentPositionType.ToString()}】位的单双已中出,目前中出1次,本轮还需要再中一次,总额【{TotalResult}】,机会寻找中...");
                                    LogInfo($"[{DateTime.Now:HH:mm:ss.fff}]-期号:{code.CodeQiHao},号码：{code.CodeNumber},中奖金额:{zhongjiangAmount}");

                                    CurrentaQi = 0;
                                    CurrentBei = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
