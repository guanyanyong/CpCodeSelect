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
    public class KLine350Calc
    {
        public static void CalcKlineCurrent(Hou3Select350_ZhouQiZhong model, Code code)
        {
            var kline = new KLine();
            var hou3Str = code.GetHou3String();

            //遗漏K的逻辑
            YiLouKline350 lastYiLouKLine350 = null;
            if (model.YiLouKline350 != null && model.YiLouKline350.Count > 0)
            {
                lastYiLouKLine350 = model.YiLouKline350.LastOrDefault();
            }
            YiLouKline350 newYiLouKline350 = null;
            var isZhong = false;

            if (model.KLineList.Count == 0)
            {
                // 第一次执行
                newYiLouKline350 = new YiLouKline350();

                //判断是否中奖
                if (model.Number350.Contains(hou3Str))
                {
                    //中了 K值加1.857
                    kline.KValue = 1.857;

                    //遗漏K的逻辑
                    //中了 判断最后一个K线的遗漏值是否在2个以内,是的话表示在周期内中
                    if (model.ZhongBeforeGua <= 2)
                    {
                        newYiLouKline350.KValue = 0.3786;

                        if (lastYiLouKLine350 != null)
                        {
                            newYiLouKline350.YiLouZhongCount = lastYiLouKLine350.YiLouZhongCount + 1;
                        }
                        else
                        {
                            newYiLouKline350.YiLouZhongCount = 1;
                        }

                        newYiLouKline350.YiLouGuaCount = 0;
                    }
                    else
                    {
                        //中了 之前的遗漏值大于2 说明不在周期内中
                        newYiLouKline350.KValue = -1.0;

                        if (lastYiLouKLine350 != null)
                        {
                            newYiLouKline350.YiLouGuaCount = lastYiLouKLine350.YiLouGuaCount + 1;
                        }
                        else
                        {
                            newYiLouKline350.YiLouGuaCount = 1;
                        }
                        newYiLouKline350.YiLouZhongCount = 0;
                    }

                    isZhong = true;

                }
                else
                {
                    //没中 K值减1.0
                    kline.KValue = -1.0;

                    //遗漏K的逻辑 继续遗漏中 更新最后遗漏号码的遗漏值
                    //kline.KValue = -1.0;
                    newYiLouKline350.CurrentGuaCount = model.GuaCount;
                    isZhong = false;
                }
            }
            else
            {
                //不是第一次执行 判断是否中奖
                if (model.Number350.Contains(hou3Str))
                {
                    //中了 K值加1.857
                    kline.KValue = model.KLineList[model.KLineList.Count - 1].KValue + 1.857;

                    //处理遗漏K的逻辑
                    newYiLouKline350 = new YiLouKline350();
                    //中了 K值加0.3786
                    if (model.ZhongBeforeGua <= 2)
                    {
                        if (model.YiLouKline350.Count >= 1)
                        {
                            newYiLouKline350.KValue = model.YiLouKline350[model.YiLouKline350.Count - 1].KValue + 0.3786;
                        }
                        else
                        {
                            newYiLouKline350.KValue = 0.3786;
                        }


                        if (lastYiLouKLine350 != null)
                        {
                            newYiLouKline350.YiLouZhongCount = lastYiLouKLine350.YiLouZhongCount + 1;
                        }
                        else
                        {
                            newYiLouKline350.YiLouZhongCount = 1;
                        }

                        newYiLouKline350.YiLouGuaCount = 0;
                    }
                    else
                    {
                        //中了 之前的遗漏值大于2 说明不在周期内中
                        if (model.YiLouKline350.Count >= 1)
                        {
                            newYiLouKline350.KValue = model.YiLouKline350[model.YiLouKline350.Count - 1].KValue - 1.0;
                        }
                        else
                        {
                            newYiLouKline350.KValue = -1;
                        }


                        if (lastYiLouKLine350 != null)
                        {
                            newYiLouKline350.YiLouGuaCount = lastYiLouKLine350.YiLouGuaCount + 1;
                        }
                        else
                        {
                            newYiLouKline350.YiLouGuaCount = 1;
                        }
                        newYiLouKline350.YiLouZhongCount = 0;
                    }

                    isZhong = true;
                }
                else
                {
                    //没中 K值减1.0
                    kline.KValue = model.KLineList[model.KLineList.Count - 1].KValue - 1.0;

                    //处理遗漏K逻辑 K值减1.0
                    if (lastYiLouKLine350 != null)
                        lastYiLouKLine350.CurrentGuaCount = model.GuaCount;
                    isZhong = false;
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
            kline.CurrentZhongCount = model.ZhongGount;

            kline.Code350Code = model.Number350;
            kline.CodeQiHao = code.CodeQiHao;
            kline.CodeNumber = code.CodeNumber;
            //把当前期号和号码保存到K线中
            model.KLineList.Add(kline);


            //处理遗漏K线逻辑
            //如果中了 把新生成的遗漏K线添加到集合中
            if (isZhong)
            {
                if (model.YiLouKline350.Count >= 20)
                {
                    //超过20期开始计算布林带
                    var result = BollingerBandsSimple.CalculateBollingerBands(
                        prices: model.YiLouKline350.Select(p => p.KValue).ToArray(),
                        period: 20,
                        stdDevMultiplier: 2.0);
                    newYiLouKline350.Bolling = new Bolling
                    {
                        MiddleValue = result.middle,
                        BollUpperValue = result.upper,
                        BollLowerValue = result.lower,
                    };
                }
                newYiLouKline350.CurrentGuaCount = model.GuaCount;
                newYiLouKline350.CurrentZhongCount = model.ZhongGount;

                newYiLouKline350.Code350Code = model.Number350;
                newYiLouKline350.CodeQiHao = code.CodeQiHao;
                newYiLouKline350.CodeNumber = code.CodeNumber;

                //如果中奖了 添加新的记录
                model.YiLouKline350.Add(newYiLouKline350);
            }
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

            //遗漏K的逻辑
            YiLouKline350 lastKLine350 = null;
            
            YiLouKline350 newYiLouKline350 = null;
            var isZhong = false;

            var runCount = number;
            for (runCount = 100; runCount > 0; runCount--)
            {
                if (model.YiLouKline350 != null && model.YiLouKline350.Count > 0)
                {
                    lastKLine350 = model.YiLouKline350.LastOrDefault();
                }
                var kline = new KLine();
                var code = AllCode[runCount - 1];
                var hou3Str = code.GetHou3String();

                if (beforeKLine == null)
                {
                    newYiLouKline350 = new YiLouKline350();
                    //第一次执行
                    //判断是否中奖
                    if (model.Number350.Contains(hou3Str))
                    {
                        //中了 K值加1.857
                        kline.KValue = 1.857;

                        model.ZhongGount++;
                        model.NeedZhong = false;
                        model.Zhong3BeforeGua = model.Zhong2BeforeGua;
                        model.Zhong2BeforeGua = model.ZhongBeforeGua;
                        model.ZhongBeforeGua = model.GuaCount;

                        model.GuaCount = 0;

                        //遗漏K的逻辑
                        //中了 判断最后一个K线的遗漏值是否在2个以内,是的话表示在周期内中
                        if (model.ZhongBeforeGua <= 2)
                        {
                            newYiLouKline350.KValue = 0.3786;

                            if (lastKLine350 != null)
                            {
                                newYiLouKline350.YiLouZhongCount = lastKLine350.YiLouZhongCount + 1;
                            }
                            else
                            {
                                newYiLouKline350.YiLouZhongCount =  1;
                            }

                            newYiLouKline350.YiLouGuaCount = 0;
                        }
                        else
                        {
                            //中了 之前的遗漏值大于2 说明不在周期内中
                            newYiLouKline350.KValue = -1.0;
                            if (lastKLine350 != null)
                            {
                                newYiLouKline350.YiLouGuaCount = lastKLine350.YiLouGuaCount + 1;
                            }
                            else
                            {
                               newYiLouKline350.YiLouGuaCount = 1;
                            }
                            newYiLouKline350.YiLouZhongCount = 0;
                        }
                        isZhong = true;
                    }
                    else
                    {
                        //没中 K值减1.0
                        kline.KValue = -1.0;
                        model.GuaCount = 1;
                        model.ZhongGount = 0;

                        //遗漏K的逻辑 继续遗漏中 更新最后遗漏号码的遗漏值
                        //kline.KValue = -1.0;
                        newYiLouKline350.CurrentGuaCount = model.GuaCount;
                        isZhong = false;
                    }
                }
                else
                {
                    //不是第一次执行 判断是否中奖
                    if (model.Number350.Contains(hou3Str))
                    {
                        //中了 K值加1.857
                        kline.KValue = beforeKLine.KValue + 1.857;

                        model.ZhongGount++;
                        model.NeedZhong = false;
                        model.Zhong3BeforeGua = model.Zhong2BeforeGua;
                        model.Zhong2BeforeGua = model.ZhongBeforeGua;
                        model.ZhongBeforeGua = model.GuaCount;

                        model.GuaCount = 0;


                        //处理遗漏K的逻辑
                        newYiLouKline350 = new YiLouKline350();
                        //中了 K值加0.3786
                        if (model.ZhongBeforeGua <= 2)
                        {
                            if (model.YiLouKline350.Count >= 1)
                            {
                                newYiLouKline350.KValue = model.YiLouKline350[model.YiLouKline350.Count - 1].KValue + 0.3786;
                            }
                            else
                            {
                                newYiLouKline350.KValue = 0.3786;
                            }

                            if (lastKLine350 != null)
                            {
                                newYiLouKline350.YiLouZhongCount = lastKLine350.YiLouZhongCount + 1;
                            }
                            else
                            {
                                newYiLouKline350.YiLouZhongCount = 1;
                            }

                            newYiLouKline350.YiLouGuaCount = 0;
                        }
                        else
                        {
                            //中了 之前的遗漏值大于2 说明不在周期内中
                            if (model.YiLouKline350.Count >= 1)
                            {
                                newYiLouKline350.KValue = model.YiLouKline350[model.YiLouKline350.Count - 1].KValue - 1.0;
                            }
                            else
                            {
                                newYiLouKline350.KValue = -1.0;
                            }

                            if (lastKLine350 != null)
                            {
                                newYiLouKline350.YiLouGuaCount = lastKLine350.YiLouGuaCount + 1;
                            }
                            else
                            {
                                newYiLouKline350.YiLouGuaCount = 1;
                            }
                            newYiLouKline350.YiLouZhongCount = 0;
                        }

                        isZhong = true;
                    }
                    else
                    {
                        //没中 K值减1.0
                        //注意 因为是倒退 所以没中要加1.0
                        kline.KValue = beforeKLine.KValue - 1.0;

                        model.GuaCount++;
                        model.ZhongGount = 0;

                        //处理遗漏K逻辑 遗漏数加1
                        if (lastKLine350 != null)
                            lastKLine350.CurrentGuaCount = model.GuaCount;
                        isZhong = false;
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
                kline.CurrentZhongCount = model.ZhongGount;

                beforeKLine = kline;


                //处理遗漏K线逻辑
                //如果中了 把新生成的遗漏K线添加到集合中
                if (isZhong)
                {
                    if (model.YiLouKline350 != null && model.YiLouKline350.Count > 0 && model.YiLouKline350.Count >= 20)
                    {
                        //超过20期开始计算布林带
                        var result = BollingerBandsSimple.CalculateBollingerBands(
                            prices: model.YiLouKline350.Select(p => p.KValue).ToArray(),
                            period: 20,
                            stdDevMultiplier: 2.0);
                        newYiLouKline350.Bolling = new Bolling
                        {
                            MiddleValue = result.middle,
                            BollUpperValue = result.upper,
                            BollLowerValue = result.lower,
                        };
                    }
                    newYiLouKline350.CurrentGuaCount = model.GuaCount;
                    newYiLouKline350.CurrentZhongCount = model.ZhongGount;

                    newYiLouKline350.Code350Code = model.Number350;
                    newYiLouKline350.CodeQiHao = code.CodeQiHao;
                    newYiLouKline350.CodeNumber = code.CodeNumber;

                    //如果中奖了 添加新的记录
                    model.YiLouKline350.Add(newYiLouKline350);
                }
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
            checkResult.Result = true;
            var count = kLineList.Count;
            var blowMiddleCount = 0;
            //0 最新5期都要在中轨上
            for (var i = count; i > count - 5; i--)
            {
                var kline = kLineList[i - 1];
                if (!kline.IsOverMiddle)
                {
                    checkResult.Result = false;
                    checkResult.Message = "最近5期至少一期不在中轨上";
                    break;
                }
            }
            //0.1 遗漏2期后还是在中轨上
            {
                var kline = kLineList[count - 1];
                if (kline.KValue - 2 <= kline.Bolling.MiddleValue)
                {
                    checkResult.Result = false;
                    if (!string.IsNullOrEmpty(checkResult.Message))
                    {
                        checkResult.Message += Environment.NewLine + "\r\n挂2期后,K线到中下轨";
                    }
                    else
                    {

                        checkResult.Message = "挂2期后,K线到中下轨";
                    }
                }
            }

            ////0.2 最近5期至少一期不在中轨上
            //for (var i = count; i > count - 5; i--)
            //{
            //    var kline = kLineList[i - 1];
            //    if (!kline.IsOverMiddle)
            //    {
            //        checkResult.Result = false;
            //        if (!string.IsNullOrEmpty(checkResult.Message))
            //        {
            //            checkResult.Message += Environment.NewLine + "\n\r最近5期至少一期不在中轨上";
            //        }
            //        else
            //        {
            //            checkResult.Message = "最近5期至少一期不在中轨上";
            //        }
            //        break;
            //    }
            //}
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
                if (!string.IsNullOrEmpty(checkResult.Message))
                {
                    checkResult.Message += Environment.NewLine + "\n\r最近20期内是否有10个在中轨下";
                }
                else
                {
                    checkResult.Message = "最近20期内是否有10个在中轨下";
                }
            }
            //1.1 最近50期内是否有20个在中轨下
            blowMiddleCount = 0;
            for (var i = count; i > count - 50; i--)
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

            if (blowMiddleCount >= 20)
            {
                checkResult.Result = false;
                if (!string.IsNullOrEmpty(checkResult.Message))
                {
                    checkResult.Message += Environment.NewLine + "\n\r最近50期内是否有20个在中轨下";
                }
                else
                {
                    checkResult.Message = "最近50期内是否有20个在中轨下";
                }
            }


            //2 最近30期内是否有超过7挂含7挂
            for (var i = count; i > count - 30; i--)
            {
                var kline = kLineList[i - 1];
                if (kline.CurrentGuaCount >= 7)
                {
                    checkResult.Result = false;
                    if (!string.IsNullOrEmpty(checkResult.Message))
                    {
                        checkResult.Message += Environment.NewLine + "\n\r最近30期有超过7挂";
                    }
                    else
                    {
                        checkResult.Message = "最近30期有超过7挂";
                    }
                    break;
                }
            }

            //2.1 最近70期内是否有超过8挂含8挂
            for (var i = count; i > count - 70; i--)
            {
                var kline = kLineList[i - 1];
                if (kline.CurrentGuaCount >= 8)
                {
                    checkResult.Result = false;
                    if (!string.IsNullOrEmpty(checkResult.Message))
                    {
                        checkResult.Message += Environment.NewLine + "\n\r最近70期有超过8挂";
                    }
                    else
                    {
                        checkResult.Message = "最近70期有超过8挂";
                    }
                    break;
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
                if (!string.IsNullOrEmpty(checkResult.Message))
                {
                    checkResult.Message += Environment.NewLine + "\n\r连续10个有7个下降";
                }
                else
                {
                    checkResult.Message = "连续10个有7个下降";
                }
            }
            //4 70期内是否有在理论周期内开出8个以上的。
            bool liankai8 = false;
            for (var i = count; i > count - 70; i--)
            {
                int lianKaiCount = 0;
                if (kLineList[i - 1].CurrentGuaCount <= 1)
                {
                    lianKaiCount = 1;
                    //如果当前的挂的次数小于等于1 则表示在理论周期内开出
                    //继续向后查找
                    for (int j = i - 1; j > count - 70; j--)
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
                                    if (!string.IsNullOrEmpty(checkResult.Message))
                                    {
                                        checkResult.Message += Environment.NewLine + "\n\r70期内存在理论周期内开出8个以上";
                                    }
                                    else
                                    {
                                        checkResult.Message = "70期内存在理论周期内开出8个以上";
                                    }
                                    liankai8 = true;
                                    break;
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
                if (liankai8)
                {
                    break;
                }
            }


            return checkResult;

        }
    }
}
