using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CpCodeSelect.Util
{
    public class KLineCalc
    {
        public static void CalcKlineCurrent(Hou3Select350_ZhouQiZhong model, Code code)
        {
            var kline = new KLine();
            var hou3Str = code.GetHou3String();
            if (model.KLineList.Count == 0)
            {
                //判断是否中奖
                if (model.Number350.Contains(hou3Str))
                {
                    //中了 K值加1.857
                    kline.KValue = 1.857;
                }
                else
                {
                    //没中 K值减1.0
                    kline.KValue = -1.0;
                }
            }
            else
            {
                //判断是否中奖
                if (model.Number350.Contains(hou3Str))
                {
                    //中了 K值加1.857
                    kline.KValue = model.KLineList[model.KLineList.Count - 1].KValue + 1.857;
                }
                else
                {
                    //没中 K值减1.0
                    kline.KValue = model.KLineList[model.KLineList.Count - 1].KValue - 1.0;
                }
            }

            if (model.KLineList.Count >= 20)
            {
                //超过20期开始计算布林带
                var result = BollingerBandsSimple.CalculateBollingerBands(
                    prices: model.KLineList.Select(p => p.KValue).ToArray(),
                    period: 20,
                    stdDevMultiplier: 2.0);
                kline.Bolling = new Bolling
                {
                    MiddleValue = result.middle,
                    BollUpperValue = result.upper,
                    BollLowerValue = result.lower,
                };
            }
            kline.CurrentGuaCount = model.GuaCount;
            kline.currentZhongCount = model.ZhongGount;

            kline.Code350Code = model.Number350;
            kline.CodeQiHao = code.CodeQiHao;
            kline.CodeNumber = code.CodeNumber;
            //把当前期号和号码保存到K线中
            model.KLineList.Add(kline);
        }

        /// <summary>
        /// 计算K线列表 根据所有的开奖号以及需要计算的期数
        /// </summary>
        /// <param name="model">需要计算的对象</param>
        /// <param name="AllCode">所有的开奖号</param>
        /// <param name="number">需要往前计算的期数</param>
        public static void CalcKLineHistoryList(Hou3Select350_ZhouQiZhong model, List<Code> AllCode, int number = 100)
        {
            if (number < 100) number = 100;
            KLine beforeKLine = null;
            var runCount = number;
            for (runCount = 100; runCount > 0; runCount--)
            {
                var kline = new KLine();
                var code = AllCode[runCount - 1];
                var hou3Str = code.GetHou3String();

                if (beforeKLine == null)
                {
                    //第一次执行
                    //判断是否中奖
                    if (model.Number350.Contains(hou3Str))
                    {
                        //中了 K值加1.857
                        kline.KValue = 1.857;
                        model.GuaCount = 0;
                        model.ZhongGount = 1;
                    }
                    else
                    {
                        //没中 K值减1.0
                        kline.KValue = -1.0;
                        model.GuaCount = 1;
                        model.ZhongGount = 0;
                    }
                }
                else
                {
                    //判断是否中奖
                    if (model.Number350.Contains(hou3Str))
                    {
                        //中了 K值加1.857
                        kline.KValue = beforeKLine.KValue + 1.857;


                        model.GuaCount = 0;
                        model.ZhongGount++;
                    }
                    else
                    {
                        //没中 K值减1.0
                        //注意 因为是倒退 所以没中要加1.0
                        kline.KValue = beforeKLine.KValue - 1.0;

                        model.GuaCount++;
                        model.ZhongGount = 0;
                    }
                }
                if (Math.Abs(runCount - number) > 20)
                {
                    //先把最新记录加入然后再进行计算
                    model.KLineList.Add(kline);
                    //超过20期开始计算布林带
                    var result = BollingerBandsSimple.CalculateBollingerBands(
                        prices: model.KLineList.Select(p => p.KValue).ToArray(),
                        period: 20,
                        stdDevMultiplier: 2.0);
                    kline.Bolling = new Bolling
                    {
                        MiddleValue = result.middle,
                        BollUpperValue = result.upper,
                        BollLowerValue = result.lower,
                    };
                }
                else
                {
                    //把当前期号和号码保存到K线中
                    model.KLineList.Add(kline);
                }


                kline.Code350Code = model.Number350;
                kline.CodeQiHao = code.CodeQiHao;
                kline.CodeNumber = code.CodeNumber;
                kline.CurrentGuaCount = model.GuaCount;
                kline.currentZhongCount = model.ZhongGount;

                beforeKLine = kline;
            }
        }
        /// <summary>
        /// 判断K线数据是否满足条件
        /// </summary>
        /// <param name="kLineList"></param>
        /// <returns></returns>
        public static CheckResult KLineIsEnough(List<KLine> kLineList)
        {
            var checkResult = new CheckResult();
            var count = kLineList.Count;
            var blowMiddleCount = 0;
            //1 最近20期内是否有10个在中轨下
            for (var i = count; i > count - 20; i--)
            {
                var kline = kLineList[i - 1];
                if (kline.Bolling != null)
                {
                    if (!kline.IsOverMiddle)
                    {
                        blowMiddleCount++;
                    }
                }
            }
            if (blowMiddleCount >= 10)
            {
                checkResult.Result = false;
                checkResult.Message = "最近20期有10个在中轨下";
                return checkResult;
            }
            //2 最近30期内是否有超过7挂含7挂
            for (var i = count; i > count - 30; i--)
            {
                var kline = kLineList[i - 1];
                if (kline.CurrentGuaCount >= 7)
                {
                    checkResult.Result = false;
                    checkResult.Message = "最近30期有超过7挂";
                    return checkResult;
                }
            }
            //3 中轨10个有7个下降
            var middleDownCount = 0;
            for (var i = count; i > count - 10; i--)
            {
                var kline = kLineList[i - 1];
                var klinBefore = kLineList[i - 2];
                if (kline.Bolling.MiddleValue < klinBefore.Bolling.MiddleValue)
                {
                    middleDownCount++;
                }
            }
            if (middleDownCount >= 7)
            {
                checkResult.Result = false;
                checkResult.Message = "连续10个有7个下降";
                return checkResult;
            }
            // 30期内是否有在理论周期内开出8个以上的。
            for (var i = count; i > count - 30; i--)
            {
                int lianKaiCount = 0;
                if (kLineList[i - 1].CurrentGuaCount <= 1)
                {
                    lianKaiCount = 1;
                    //如果当前的挂的次数小于等于1 则表示在理论周期内开出
                    //继续向后查找
                    for (int j = i - 1; j > count - 30; j--)
                    {
                        if (kLineList[j - 1].CurrentGuaCount <= 1)
                        {
                            //如果当前的挂的次数小于等于1 则表示在理论周期内开出
                            if (kLineList[j - 1].CurrentGuaCount == 0)
                            {
                                lianKaiCount++;
                                if (lianKaiCount >= 8)
                                {
                                    checkResult.Result = false;
                                    checkResult.Message = "30期内存在理论周期内开出8个以上";
                                    return checkResult;
                                }
                            }
                        }
                        else
                        {
                            //否则跳出循环
                            break;
                        }
                    }
                }
            }


            checkResult.Result = true;
            return checkResult;

        }
    }
}
