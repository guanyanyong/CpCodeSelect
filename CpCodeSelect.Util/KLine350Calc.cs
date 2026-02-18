using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.Config;
using System.Threading;
using CpCodeSelect.Util.IndexCalc;

namespace CpCodeSelect.Util
{
    public class KLine350Calc
    {
        public static void CalcKlineCurrent(Hou3Select350_ZhouQiZhongScore scoreModel, Code code)
        {
            var newModel = new Hou3Select350_ZhouQiZhong();
            newModel.Number350 = scoreModel.Number350;
            newModel.ZhongBeforeGua = scoreModel.ZhongBeforeGua;
            newModel.Zhong2BeforeGua = scoreModel.Zhong2BeforeGua;
            newModel.Zhong3BeforeGua = scoreModel.Zhong3BeforeGua;
            newModel.KLineList = scoreModel.KLineList;
            newModel.YiLouKline350 = scoreModel.YiLouKline350;
            newModel.YiLouTuLineList = scoreModel.YiLouTuLineList;
            newModel.CodeNumber = scoreModel.CodeNumber;
            newModel.CodeQiHao = scoreModel.CodeQiHao;
            newModel.GuaCount = scoreModel.GuaCount;
            newModel.ZhongGount = scoreModel.ZhongGount;
            newModel.NeedZhong = scoreModel.NeedZhong;
            newModel.IsZhouQiZhongHou = scoreModel.IsZhouQiZhongHou;
            newModel.ZhouQiZhongHouGua = scoreModel.ZhouQiZhongHouGua;
            newModel.ScoreDateList = scoreModel.ScoreDateList;
            //newModel.IsShow = scoreModel.IsShow;
            newModel = CalcKlineCurrent(newModel, code);

            scoreModel.Score = newModel.Score;
            scoreModel.IsChuShou = newModel.IsChuShou;
            scoreModel.ShouNumber = newModel.ShouNumber;
        }
        public static Hou3Select350_ZhouQiZhong CalcKlineCurrent(Hou3Select350_ZhouQiZhong model, Code code)
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

            //遗漏图的逻辑
            KLine yiLouTuKLine = null;


            if (model.KLineList.Count == 0)
            {
                // 第一次执行
                newYiLouKline350 = new YiLouKline350();

                //判断是否中奖
                if (model.Number350.Contains(hou3Str))
                {
                    yiLouTuKLine = new KLine();
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
                        newYiLouKline350.IsZhong = true;
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

                    //遗漏图的逻辑
                    yiLouTuKLine.IsZhong = true;
                    yiLouTuKLine.CurrentGuaCount = 0;
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
                    yiLouTuKLine = new KLine();
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
                        newYiLouKline350.IsZhong = true;
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


                    //遗漏图的逻辑
                    yiLouTuKLine.IsZhong = true;
                    yiLouTuKLine.CurrentGuaCount = model.ZhongBeforeGua;

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
            if (model.KLineList.Count >= 150)
            {
                //超过150期开始计算MACD指标
                var macdResult = MACDCalculator.GetLatest(model.KLineList.Select(p => p.KValue).ToList());
                kline.MACDResult = macdResult;
            }

            kline.CurrentGuaCount = model.GuaCount;
            kline.CurrentZhongCount = model.ZhongGount;

            kline.Code350Code = model.Number350;
            kline.CodeQiHao = code.CodeQiHao;
            kline.CodeNumber = code.CodeNumber;
            kline.IsZhong = isZhong;
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
                if (model.YiLouKline350.Count >= 150)
                {
                    //超过150期开始计算MACD指标
                    var macdResult = MACDCalculator.GetLatest(model.YiLouKline350.Select(p => p.KValue).ToList());
                    newYiLouKline350.MACDResult = macdResult;
                }

                newYiLouKline350.CurrentGuaCount = model.GuaCount;
                newYiLouKline350.CurrentZhongCount = model.ZhongGount;

                newYiLouKline350.Code350Code = model.Number350;
                newYiLouKline350.CodeQiHao = code.CodeQiHao;
                newYiLouKline350.CodeNumber = code.CodeNumber;

                //如果中奖了 添加新的记录
                model.YiLouKline350.Add(newYiLouKline350);
            }

            //处理遗漏图的逻辑
            if (isZhong)
            {
                //遗漏图的逻辑
                yiLouTuKLine.Code350Code = model.Number350;
                yiLouTuKLine.CodeQiHao = code.CodeQiHao;
                yiLouTuKLine.CodeNumber = code.CodeNumber;
                yiLouTuKLine.CurrentGuaCount = model.ZhongBeforeGua;
                model.YiLouTuLineList.Add(yiLouTuKLine);
            }


            return model;
        }

        public static void CalcKLineHistoryList(Hou3Select350_ZhouQiZhongScore scoreModel, List<Code> AllCode, int number = 100)
        {
            var newModel = new Hou3Select350_ZhouQiZhong();
            newModel.Number350 = scoreModel.Number350;
            newModel.ZhongBeforeGua = scoreModel.ZhongBeforeGua;
            newModel.Zhong2BeforeGua = scoreModel.Zhong2BeforeGua;
            newModel.Zhong3BeforeGua = scoreModel.Zhong3BeforeGua;
            newModel.KLineList = scoreModel.KLineList;
            newModel.YiLouKline350 = scoreModel.YiLouKline350;
            newModel.YiLouTuLineList = scoreModel.YiLouTuLineList;
            newModel.CodeNumber = scoreModel.CodeNumber;
            newModel.CodeQiHao = scoreModel.CodeQiHao;
            newModel.GuaCount = scoreModel.GuaCount;
            newModel.ZhongGount = scoreModel.ZhongGount;
            newModel.NeedZhong = scoreModel.NeedZhong;
            newModel.IsZhouQiZhongHou = scoreModel.IsZhouQiZhongHou;
            newModel.ZhouQiZhongHouGua = scoreModel.ZhouQiZhongHouGua;
            newModel.ScoreDateList = scoreModel.ScoreDateList;
            //newModel.ScoreDateList = scoreModel.ScoreDateList;
            //newModel.IsShow = scoreModel.IsShow;
            newModel = CalcKLineHistoryList(newModel, AllCode, number);
            scoreModel.Score = newModel.Score;
            scoreModel.IsChuShou = newModel.IsChuShou;
            scoreModel.ShouNumber = newModel.ShouNumber;
        }

        /// <summary>
        /// 计算K线列表 根据所有的开奖号以及需要计算的期数
        /// </summary>
        /// <param name="model">需要计算的对象</param>
        /// <param name="AllCode">所有的开奖号</param>
        /// <param name="number">需要往前计算的期数</param>
        public static Hou3Select350_ZhouQiZhong CalcKLineHistoryList(Hou3Select350_ZhouQiZhong model, List<Code> AllCode, int number = 100)
        {
            if (number < 100) number = 100;
            KLine beforeKLine = null;

            //遗漏K的逻辑
            YiLouKline350 lastKLine350 = null;

            YiLouKline350 newYiLouKline350 = null;
            var isZhong = false;


            //遗漏图的逻辑
            KLine yiLouTuKLine = null;

            //评分逻辑
            LotteryScoreData scoreData = null;

            if (model.ScoreDateList == null) model.ScoreDateList = new List<LotteryScoreData>();

            var runCount = number;
            for (; runCount > 0; runCount--)
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
                        yiLouTuKLine = new KLine();
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
                                newYiLouKline350.YiLouZhongCount = 1;
                            }

                            newYiLouKline350.YiLouGuaCount = 0;
                            newYiLouKline350.IsZhong = true;
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

                        //遗漏图的逻辑
                        yiLouTuKLine.IsZhong = true;
                        yiLouTuKLine.CurrentGuaCount = 0;

                        isZhong = true;


                        //评分的逻辑
                        //scoreData = new LotteryScoreData();
                        //scoreData.IsZhongJiang = true;
                        //scoreData.YiLouValue = model.GuaCount;
                        //scoreData.LianXuZhongJiangCount = model.ZhongGount;
                        //scoreData.KValue = kline.KValue;
                        //scoreData.BollingerBands = kline.Bolling;

                        //scoreData.Number350 = model.Number350;
                        //scoreData.QiHao = code.CodeQiHao;
                        //scoreData.Number = code.CodeNumber;

                        //CalcScore(scoreData, model);
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


                        //评分的逻辑
                        //scoreData = new LotteryScoreData();
                        //scoreData.IsZhongJiang = false;
                        //scoreData.YiLouValue = model.GuaCount;
                        //scoreData.LianXuZhongJiangCount = model.ZhongGount;
                        //scoreData.KValue = kline.KValue;
                        //scoreData.BollingerBands = kline.Bolling;

                        //scoreData.Number350 = model.Number350;
                        //scoreData.QiHao = code.CodeQiHao;
                        //scoreData.Number = code.CodeNumber;

                        //CalcScore(scoreData, model);
                    }
                }
                else
                {
                    //不是第一次执行 判断是否中奖
                    if (model.Number350.Contains(hou3Str))
                    {
                        yiLouTuKLine = new KLine();
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
                            newYiLouKline350.IsZhong = true;
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

                        //遗漏图的逻辑
                        yiLouTuKLine.IsZhong = true;
                        yiLouTuKLine.CurrentGuaCount = model.ZhongBeforeGua;
                        isZhong = true;


                        //评分的逻辑
                        //scoreData = new LotteryScoreData();
                        //scoreData.IsZhongJiang = true;
                        //scoreData.YiLouValue = model.GuaCount;
                        //scoreData.LianXuZhongJiangCount = model.ZhongGount;
                        //scoreData.KValue = kline.KValue;
                        //scoreData.BollingerBands = kline.Bolling;

                        //scoreData.Number350 = model.Number350;
                        //scoreData.QiHao = code.CodeQiHao;
                        //scoreData.Number = code.CodeNumber;

                        //CalcScore(scoreData, model);

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

                if (model.KLineList.Count >= 150)
                {
                    //超过150期开始计算MACD指标
                    var macdResult = MACDCalculator.GetLatest(model.KLineList.Select(p => p.KValue).ToList());
                    kline.MACDResult = macdResult;
                }


                kline.Code350Code = model.Number350;
                kline.CodeQiHao = code.CodeQiHao;
                kline.CodeNumber = code.CodeNumber;
                kline.CurrentGuaCount = model.GuaCount;
                kline.CurrentZhongCount = model.ZhongGount;
                kline.IsZhong = isZhong;
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
                //处理遗漏图的逻辑
                if (isZhong)
                {
                    //遗漏图的逻辑
                    yiLouTuKLine.Code350Code = model.Number350;
                    yiLouTuKLine.CodeQiHao = code.CodeQiHao;
                    yiLouTuKLine.CodeNumber = code.CodeNumber;
                    yiLouTuKLine.CurrentGuaCount = model.ZhongBeforeGua;
                    model.YiLouTuLineList.Add(yiLouTuKLine);
                }

                //处理评分逻辑

                //评分的逻辑
                //进行评分,之后加入列表

                //评分的逻辑
                //scoreData = new LotteryScoreData();
                //scoreData.IsZhongJiang = isZhong;
                //scoreData.YiLouValue = model.GuaCount;
                //scoreData.LianXuZhongJiangCount = model.ZhongGount;
                //scoreData.KValue = kline.KValue;
                //scoreData.BollingerBands = kline.Bolling;

                //scoreData.Number350 = model.Number350;
                //scoreData.QiHao = code.CodeQiHao;
                //scoreData.Number = code.CodeNumber;

                //model.ScoreDateList.Add(scoreData);
                //CalcScore(scoreData, model);
                //model.Score = scoreData.Score;
                //model.IsChuShou = scoreData.IsChuShou;
                //model.ShouNumber = scoreData.HandNumber;

            }
            return model;
        }
        //计算评分的逻辑
        private static void CalcScore(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            CalcScoreBefore(currentData, model);
            CalcScoreValue(currentData, model);
        }
        /// <summary>
        /// 计算评分之前的逻辑
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="model"></param>
        private static void CalcScoreBefore(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {

            // 计算遗漏值
            //CalculateYiLouValue(currentData,model);

            // 计算布林带
            //CalculateBollingerBands(currentData, model);

            // 计算其他指标
            CalculateOtherIndicators(currentData, model);


        }
        /// <summary>
        /// 计算评分数值
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="model"></param>
        private static void CalcScoreValue(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            RecalculateAllScoresAsync(currentData, model);
        }

        private static void RecalculateAllScoresAsync(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            var scoringEngine = new ScoringEngine();
            ScorerUtil.InitializeScoringRulesForEngine(scoringEngine);
            var historyData = model.ScoreDateList;
            // 首先重置所有周期相关字段，确保重新计算不会受到之前结果的影响

            for (int i = historyData.Count-2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                historyData[i].IsChuShou = false;
                historyData[i].IsChuShouSuccess = false;
                historyData[i].IsCycleComplete = false;
                historyData[i].IsCycleBurst = false;
                historyData[i].CycleNumber = 0;
                historyData[i].CycleStep = 0;
                historyData[i].HandNumber = 0;
                historyData[i].IsPartOfCycle = false;
            }

            // 第一步：计算每期的评分
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                var historyForScoring = historyData.Take(i).ToList();
                historyData[i].Score = scoringEngine.CalculateTotalScore(
                    historyData[i],
                    historyForScoring
                );

                // 根据评分规则设置是否出手标志
                // 需要考虑评分>=70且不在趋势段内且K值在中轨上
                bool isScoreHighEnough = historyData[i].Score >= 70;
                bool isNotInTrendSegment = !historyData[i].IsQuShiDuan;
                bool isKValueAboveMiddle = historyData[i].BollingerBands != null &&
                    historyData[i].KValue >= historyData[i].BollingerBands.MiddleValue;

                // 检查是否连续出手超过2期，如果是则必须停一期
                bool canContinueChuShou = true;
                if (i >= 2)
                {
                    // 检查前两期是否都在出手
                    bool previousTwoAreChuShou = historyData[i - 1].IsChuShou &&
                                                 historyData[i - 2].IsChuShou;

                    if (previousTwoAreChuShou)
                    {
                        // 如果前两期都在出手，则当前期不能出手，必须停一期
                        canContinueChuShou = false;
                    }
                }

                historyData[i].IsChuShou = isScoreHighEnough && isNotInTrendSegment && isKValueAboveMiddle && canContinueChuShou;
            }

            // 第二步：根据开奖结果确定出手成功性（先只设置出手成功状态）
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                // 检查当前期的前一期（上一期）是否出手
                if (i > 0 && historyData[i - 1].IsChuShou)
                {
                    // 上一期出手，当前期开奖结果决定上一期出手是否成功
                    historyData[i - 1].IsChuShouSuccess = historyData[i].IsZhongJiang;
                }
            }

            // 第三步：按时间顺序重新计算所有出手数据的周期信息
            // 先重置周期相关状态
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                historyData[i].IsCycleComplete = false;
                historyData[i].IsCycleBurst = false;
                historyData[i].IsChuShouSuccess = false;
            }

            // 第四步：按时间顺序重新计算所有出手数据的周期和步骤信息
            // 严格按顺序执行，确保每次调用CalculateChuShouCycleAndHandNumber时依赖的数据已经计算好
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                if (historyData[i].IsChuShou)
                {
                    CalculateChuShouCycleAndHandNumber(historyData[i], model);
                }
            }

            // 第五步：然后计算出手成功性
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                if (i > 0 && historyData[i - 1].IsChuShou)
                {
                    historyData[i - 1].IsChuShouSuccess = historyData[i].IsZhongJiang;
                }
            }

            // 第六步：标记周期完成和爆掉状态
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                if (i > 0 && historyData[i - 1].IsChuShou)
                {
                    // 检查上一期出手是否完成了其所在周期
                    if (historyData[i].IsZhongJiang) // 如果当前期中奖，则上一期出手所在的周期完成
                    {
                        // 标记上一期出手完成其周期
                        historyData[i - 1].IsCycleComplete = true;

                        // 同时标记同一周期内的所有出手为完成
                        int currentCycleNumber = historyData[i - 1].CycleNumber;
                        for (int j = 0; j < historyData.Count; j++)
                        {
                            if (historyData[j].IsChuShou &&
                                historyData[j].CycleNumber == currentCycleNumber)
                            {
                                historyData[j].IsCycleComplete = true;
                                historyData[j].IsCycleBurst = false; // 完成周期，不是爆掉
                            }
                        }
                    }
                    // 检查周期是否因第N步未中奖而爆掉
                    else if (historyData[i - 1].CycleStep == GetCycleLength() &&
                            i < historyData.Count &&
                            !historyData[i].IsZhongJiang)
                    {
                        // 标记上一期出手导致周期爆掉
                        historyData[i - 1].IsCycleBurst = true;

                        // 同时标记整个周期爆掉
                        int currentCycleNumber = historyData[i - 1].CycleNumber;
                        for (int j = 0; j < historyData.Count; j++)
                        {
                            if (historyData[j].IsChuShou &&
                                historyData[j].CycleNumber == currentCycleNumber)
                            {
                                historyData[j].IsCycleBurst = true;
                                historyData[j].IsCycleComplete = false; // 爆掉，不是完成
                            }
                        }
                    }
                }
            }

            // 第七步：最后再重新计算一次周期信息，确保一致性
            for (int i = historyData.Count - 2; i < historyData.Count; i++)
            {
                if (i < 0) i = 0;
                if (historyData[i].IsChuShou)
                {
                    CalculateChuShouCycleAndHandNumber(historyData[i], model);
                }
            }
        }
        private static void RecalculateAllScoresAsyncOld(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            var scoringEngine = new ScoringEngine();
            ScorerUtil.InitializeScoringRulesForEngine(scoringEngine);
            var historyData = model.ScoreDateList;
            // 首先重置所有周期相关字段，确保重新计算不会受到之前结果的影响

            for (int i = 0; i < historyData.Count; i++)
            {
                historyData[i].IsChuShou = false;
                historyData[i].IsChuShouSuccess = false;
                historyData[i].IsCycleComplete = false;
                historyData[i].IsCycleBurst = false;
                historyData[i].CycleNumber = 0;
                historyData[i].CycleStep = 0;
                historyData[i].HandNumber = 0;
                historyData[i].IsPartOfCycle = false;
            }

            // 第一步：计算每期的评分
            for (int i = 0; i < historyData.Count; i++)
            {
                var historyForScoring = historyData.Take(i).ToList();
                historyData[i].Score = scoringEngine.CalculateTotalScore(
                    historyData[i],
                    historyForScoring
                );

                // 根据评分规则设置是否出手标志
                // 需要考虑评分>=70且不在趋势段内且K值在中轨上
                bool isScoreHighEnough = historyData[i].Score >= 70;
                bool isNotInTrendSegment = !historyData[i].IsQuShiDuan;
                bool isKValueAboveMiddle = historyData[i].BollingerBands != null &&
                    historyData[i].KValue >= historyData[i].BollingerBands.MiddleValue;

                // 检查是否连续出手超过2期，如果是则必须停一期
                bool canContinueChuShou = true;
                if (i >= 2)
                {
                    // 检查前两期是否都在出手
                    bool previousTwoAreChuShou = historyData[i - 1].IsChuShou &&
                                                 historyData[i - 2].IsChuShou;

                    if (previousTwoAreChuShou)
                    {
                        // 如果前两期都在出手，则当前期不能出手，必须停一期
                        canContinueChuShou = false;
                    }
                }

                historyData[i].IsChuShou = isScoreHighEnough && isNotInTrendSegment && isKValueAboveMiddle && canContinueChuShou;
            }

            // 第二步：根据开奖结果确定出手成功性（先只设置出手成功状态）
            for (int i = 0; i < historyData.Count; i++)
            {
                // 检查当前期的前一期（上一期）是否出手
                if (i > 0 && historyData[i - 1].IsChuShou)
                {
                    // 上一期出手，当前期开奖结果决定上一期出手是否成功
                    historyData[i - 1].IsChuShouSuccess = historyData[i].IsZhongJiang;
                }
            }

            // 第三步：按时间顺序重新计算所有出手数据的周期信息
            // 先重置周期相关状态
            for (int i = 0; i < historyData.Count; i++)
            {
                historyData[i].IsCycleComplete = false;
                historyData[i].IsCycleBurst = false;
                historyData[i].IsChuShouSuccess = false;
            }

            // 第四步：按时间顺序重新计算所有出手数据的周期和步骤信息
            // 严格按顺序执行，确保每次调用CalculateChuShouCycleAndHandNumber时依赖的数据已经计算好
            for (int i = 0; i < historyData.Count; i++)
            {
                if (historyData[i].IsChuShou)
                {
                    CalculateChuShouCycleAndHandNumber(historyData[i],model);
                }
            }

            // 第五步：然后计算出手成功性
            for (int i = 0; i < historyData.Count; i++)
            {
                if (i > 0 && historyData[i - 1].IsChuShou)
                {
                    historyData[i - 1].IsChuShouSuccess = historyData[i].IsZhongJiang;
                }
            }

            // 第六步：标记周期完成和爆掉状态
            for (int i = 0; i < historyData.Count; i++)
            {
                if (i > 0 && historyData[i - 1].IsChuShou)
                {
                    // 检查上一期出手是否完成了其所在周期
                    if (historyData[i].IsZhongJiang) // 如果当前期中奖，则上一期出手所在的周期完成
                    {
                        // 标记上一期出手完成其周期
                        historyData[i - 1].IsCycleComplete = true;

                        // 同时标记同一周期内的所有出手为完成
                        int currentCycleNumber = historyData[i - 1].CycleNumber;
                        for (int j = 0; j < historyData.Count; j++)
                        {
                            if (historyData[j].IsChuShou &&
                                historyData[j].CycleNumber == currentCycleNumber)
                            {
                                historyData[j].IsCycleComplete = true;
                                historyData[j].IsCycleBurst = false; // 完成周期，不是爆掉
                            }
                        }
                    }
                    // 检查周期是否因第N步未中奖而爆掉
                    else if (historyData[i - 1].CycleStep == GetCycleLength() &&
                            i < historyData.Count &&
                            !historyData[i].IsZhongJiang)
                    {
                        // 标记上一期出手导致周期爆掉
                        historyData[i - 1].IsCycleBurst = true;

                        // 同时标记整个周期爆掉
                        int currentCycleNumber = historyData[i - 1].CycleNumber;
                        for (int j = 0; j < historyData.Count; j++)
                        {
                            if (historyData[j].IsChuShou &&
                                historyData[j].CycleNumber == currentCycleNumber)
                            {
                                historyData[j].IsCycleBurst = true;
                                historyData[j].IsCycleComplete = false; // 爆掉，不是完成
                            }
                        }
                    }
                }
            }

            // 第七步：最后再重新计算一次周期信息，确保一致性
            for (int i = 0; i < historyData.Count; i++)
            {
                if (historyData[i].IsChuShou)
                {
                    CalculateChuShouCycleAndHandNumber(historyData[i],model);
                }
            }
        }

        /// <summary>
        /// 计算其他指标
        /// </summary>
        /// <param name="currentData"></param>
        private static void CalculateOtherIndicators(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            // 判断是否大遗漏
            currentData.IsDaYiLou = currentData.YiLouValue >= 2;

            // 计算连中连挂次数
            CalculateConsecutiveCounts(currentData, model);

            // 判断是否确认点
            CheckConfirmPoint(currentData, model);

            // 判断是否趋势段
            CheckTrendSegment(currentData, model);

            // 判断是否出手：根据评分规则，当总评分达到70分且不在趋势段内且K值在中轨之上时可以出手投注
            bool isScoreHighEnough = currentData.Score >= 70;
            bool isNotInTrendSegment = !currentData.IsQuShiDuan;
            bool isKValueAboveMiddle = currentData.BollingerBands != null &&
                                      currentData.KValue >= currentData.BollingerBands.MiddleValue;

            // 检查是否连续出手超过2期，如果是则必须停一期
            bool canContinueChuShou = true;
            var HistoryData = model.ScoreDateList;
            if (HistoryData.Count >= 2)
            {
                // 检查前两期是否都在出手
                bool previousTwoAreChuShou = HistoryData[HistoryData.Count - 1].IsChuShou &&
                                             HistoryData[HistoryData.Count - 2].IsChuShou;

                if (previousTwoAreChuShou)
                {
                    // 如果前两期都在出手，则当前期不能出手，必须停一期
                    canContinueChuShou = false;
                }
            }

            // 在当前期决定是否出手
            if (isScoreHighEnough && isNotInTrendSegment && isKValueAboveMiddle && canContinueChuShou)
            {
                currentData.IsChuShou = true;

                // 计算出手周期和手数
                CalculateChuShouCycleAndHandNumber(currentData, model);
            }
            else
            {
                currentData.IsChuShou = false;
                currentData.HandNumber = 0;  // 未出手时手数为0
                currentData.IsPartOfCycle = false;  // 未出手时不属于周期
            }

            // 检查历史数据中的上一期，如果上一期决定出手，则在当前期验证出手结果
            if (HistoryData.Count > 0)
            {
                var previousData = HistoryData.Last(); // 获取上一期数据
                if (previousData.IsChuShou) // 如果上一期决定出手
                {
                    // 验证上一期出手的结果：上一期出手后，下一期（即当前期）是否中奖
                    // 在上一期出手后，当前期开奖的中奖结果决定了上一期出手是否成功
                    previousData.IsChuShouSuccess = currentData.IsZhongJiang;

                    // 检查上一期出手是否完成了其所在周期
                    if (currentData.IsZhongJiang) // 如果当前期中奖，则上一期出手所在的周期完成
                    {
                        previousData.IsCycleComplete = true;

                        // 同时也需要标记同一周期内的其他出手也已完成周期
                        for (int i = HistoryData.Count - 1; i >= 0; i--)
                        {
                            if (HistoryData[i].IsChuShou &&
                                HistoryData[i].CycleNumber == previousData.CycleNumber)
                            {
                                HistoryData[i].IsCycleComplete = true;
                                HistoryData[i].IsCycleBurst = false; // 完成周期，不是爆掉
                            }
                            else if (HistoryData[i].CycleNumber < previousData.CycleNumber)
                            {
                                // 如果周期号更小，说明不再同一周期内，可以停止查找
                                break;
                            }
                        }
                    }
                    else if (previousData.CycleStep == GetCycleLength()) // 如果上一期出手是第N步（根据配置）
                    {
                        // 如果当前期没有中奖，并且上一期出手是第N步，则周期爆掉
                        if (!currentData.IsZhongJiang)
                        {
                            // 标记整个周期爆掉
                            int currentCycleNumber = previousData.CycleNumber;
                            for (int i = HistoryData.Count - 1; i >= 0; i--)
                            {
                                if (HistoryData[i].IsChuShou &&
                                    HistoryData[i].CycleNumber == currentCycleNumber)
                                {
                                    HistoryData[i].IsCycleBurst = true;
                                    HistoryData[i].IsCycleComplete = false; // 爆掉，不是完成
                                }
                                else if (HistoryData[i].CycleNumber < currentCycleNumber)
                                {
                                    // 如果周期号更小，说明不再同一周期内，可以停止查找
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算连中连挂次数
        /// </summary>
        /// <param name="currentData"></param>
        private static void CalculateConsecutiveCounts(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            if(model.ScoreDateList == null) model.ScoreDateList=new List<LotteryScoreData>();
            var HistoryData = model.ScoreDateList;
            if (HistoryData.Count == 0)
            {
                currentData.LianXuZhongJiangCount = currentData.IsZhongJiang ? 1 : 0;
                currentData.LianXuWeiZhongJiangCount = currentData.IsZhongJiang ? 0 : 1;
                return;
            }

            var prevData = HistoryData.Last();
            if (currentData.IsZhongJiang)
            {
                currentData.LianXuZhongJiangCount = prevData.IsZhongJiang ?
                    prevData.LianXuZhongJiangCount + 1 : 1;
                currentData.LianXuWeiZhongJiangCount = 0;
            }
            else
            {
                currentData.LianXuZhongJiangCount = 0;
                currentData.LianXuWeiZhongJiangCount = prevData.IsZhongJiang ?
                    1 : prevData.LianXuWeiZhongJiangCount + 1;
            }
        }

        /// <summary>
        /// 检查确认点
        /// </summary>
        /// <param name="currentData"></param>
        private static void CheckConfirmPoint(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            var HistoryData = model.ScoreDateList;
            // 确认点：大遗漏之后中奖，且在中奖后理论周期内再次中奖
            // 理论周期是2，即遗漏值为0或1
            if (currentData.IsZhongJiang) // 当前中奖
            {
                if (HistoryData.Count >= 1) // 至少有1期历史数据
                {
                    // 查找最近的大遗漏
                    int lastBigGapIndex = -1;
                    for (int i = HistoryData.Count - 1; i >= 0; i--)
                    {
                        if (HistoryData[i].IsDaYiLou)
                        {
                            lastBigGapIndex = i;
                            break;
                        }
                    }

                    if (lastBigGapIndex >= 0)
                    {
                        // 从大遗漏后开始找第一次中奖
                        int firstWinAfterGap = -1;
                        for (int i = lastBigGapIndex + 1; i < HistoryData.Count; i++)
                        {
                            if (HistoryData[i].IsZhongJiang)
                            {
                                firstWinAfterGap = i;
                                break;
                            }
                        }

                        // 如果找到了大遗漏后的第一次中奖
                        if (firstWinAfterGap >= 0)
                        {
                            // 当前期是否相对于第一次中奖在理论周期内
                            // 计算从第一次中奖到现在有多少期（不包含第一次中奖期本身）
                            int periodsSinceFirstWin = HistoryData.Count - firstWinAfterGap;

                            // 当前期是否在理论周期内（遗漏值≤1 且 自从第一次中奖以来期数不超过2）
                            bool isInCycle = currentData.YiLouValue <= 1 && periodsSinceFirstWin <= 2;

                            if (isInCycle)
                            {
                                // 检查从第一次中奖后到当前期之间是否都是在理论周期内
                                bool allInCycle = true;
                                for (int i = firstWinAfterGap; i < HistoryData.Count; i++)
                                {
                                    if (HistoryData[i].IsZhongJiang)
                                    {
                                        // 检查该中奖期是否在理论周期内（遗漏值 <= 1）
                                        if (HistoryData[i].YiLouValue > 1)
                                        {
                                            allInCycle = false;
                                            break;
                                        }
                                    }
                                }

                                if (allInCycle)
                                {
                                    currentData.IsQueRenDian = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检查趋势段
        /// </summary>
        /// <param name="currentData"></param>
        private static void CheckTrendSegment(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            // 如果当前期是大遗漏，则趋势段结束或不开始，重新计算从当前期开始
            if (currentData.IsDaYiLou)
            {
                currentData.IsQuShiDuan = false;
                currentData.QuShiDuanZhongJiangCount = 0; // 重置趋势段中奖计数
                currentData.DaYiLouHouLiLunZhouQiNeiZhongJiangShu = 0; // 重置大遗漏后理论周期内中奖数
                return;
            }
            var HistoryData = model.ScoreDateList;
            // 如果历史数据为空，当前期不是趋势段
            if (HistoryData.Count == 0)
            {
                currentData.IsQuShiDuan = false;
                currentData.QuShiDuanZhongJiangCount = 0;
                currentData.DaYiLouHouLiLunZhouQiNeiZhongJiangShu = 0;
                return;
            }

            // 修正的趋势段逻辑：只按大遗漏后理论上中出4个才是趋势段的逻辑
            // 大遗漏过后理论周期内开出第4个中奖的当期及之后开始是趋势段
            int latestBigGapIndex = -1;
            for (int i = HistoryData.Count - 1; i >= 0; i--)
            {
                if (HistoryData[i].IsDaYiLou)
                {
                    latestBigGapIndex = i;
                    break;
                }
            }

            if (latestBigGapIndex >= 0)
            {
                // 重新计算：从大遗漏后的第一期开始，统计理论周期内的中奖次数
                int winsAfterBigGap = 0;

                // 从大遗漏后第一期开始计算，直到当前期
                for (int i = latestBigGapIndex + 1; i < HistoryData.Count; i++)
                {
                    // 只统计中奖且在理论周期内（遗漏值≤1）的
                    if (HistoryData[i].IsZhongJiang && HistoryData[i].YiLouValue <= 1)
                    {
                        winsAfterBigGap++;
                    }
                }

                // 如果当前期也在理论周期内（遗漏值≤1）且中奖，则计入
                if (currentData.IsZhongJiang && currentData.YiLouValue <= 1)
                {
                    winsAfterBigGap++;
                }

                // 记录大遗漏后理论周期内的中奖数
                currentData.DaYiLouHouLiLunZhouQiNeiZhongJiangShu = winsAfterBigGap;

                // 判断当前期是否在大遗漏后的理论周期内（遗漏值≤1）
                bool isWithinTheoryPeriod = currentData.YiLouValue <= 1;

                // 趋势段条件：当前期在大遗漏后的理论周期内，且理论周期内已经有4个中奖
                bool shouldEnterTrendSegment = isWithinTheoryPeriod && winsAfterBigGap >= 4;

                if (shouldEnterTrendSegment)
                {
                    currentData.IsQuShiDuan = true;
                    currentData.QuShiDuanZhongJiangCount = winsAfterBigGap; // 更新趋势段中奖计数
                }
                else
                {
                    currentData.IsQuShiDuan = false;
                    currentData.QuShiDuanZhongJiangCount = winsAfterBigGap; // 更新趋势段中奖计数
                }
            }
            else
            {
                // 如果没有找到大遗漏，当前期不是趋势段
                currentData.IsQuShiDuan = false;
                currentData.QuShiDuanZhongJiangCount = 0;
                currentData.DaYiLouHouLiLunZhouQiNeiZhongJiangShu = 0;
            }
        }

        /// <summary>
        /// 计算出手周期和出手手数
        /// 严格按照"中奖或达到设定周期数"来完成一个周期的规则
        /// </summary>
        /// <param name="currentData"></param>
        public static void CalculateChuShouCycleAndHandNumber(LotteryScoreData currentData, Hou3Select350_ZhouQiZhong model)
        {
            // 如果没有出手，直接返回
            if (!currentData.IsChuShou)
            {
                currentData.HandNumber = 0;
                currentData.IsPartOfCycle = false;
                currentData.CycleNumber = 0;
                currentData.CycleStep = 0;
                return;
            }
            var HistoryData = model.ScoreDateList;
            // 优化：预先获取目标数据的索引，避免在循环中重复调用IndexOf
            int targetIndex = -1;
            for (int i = 0; i < HistoryData.Count; i++)
            {
                if (ReferenceEquals(HistoryData[i], currentData))
                {
                    targetIndex = i;
                    break;
                }
            }

            // 如果没找到目标数据，直接返回
            if (targetIndex == -1)
                return;

            // 从头开始重新计算所有出手的周期信息，以确保准确性
            // 找出所有出手记录及其索引
            var allChuShouRecords = new List<(int index, LotteryScoreData data)>();
            for (int i = 0; i < HistoryData.Count; i++)
            {
                if (HistoryData[i].IsChuShou)
                {
                    allChuShouRecords.Add((i, HistoryData[i]));

                    // 如果已经计算到了目标出手，停止添加新记录（但我们仍需要完整循环来计算中间值）
                    if (i == targetIndex)
                        break;
                }
            }

            // 按照严格的周期规则进行计算：
            // 1. 每个周期从步骤1开始
            // 2. 当出现中奖或达到第N步时，当前周期完成，下一出手开始新周期
            int currentCycle = 1;
            int stepOfCurrentCycle = 1;

            for (int i = 0; i < allChuShouRecords.Count; i++)
            {
                var (index, record) = allChuShouRecords[i];

                // 检查是否需要开启新周期
                if (i > 0) // 不是第一个出手
                {
                    var (prevIndex, prevRecord) = allChuShouRecords[i - 1];

                    // 检查前一个出手是否导致周期完成
                    // 在前一个出手之后，检查是否会开启新周期
                    int nextResultIndex = prevIndex + 1; // 前一个出手的下一期结果

                    if (nextResultIndex < HistoryData.Count)
                    {
                        // 如果前一个出手的下一期中奖了，或者前一个出手是第8步且下一期未中奖，则前一个周期完成
                        bool prevCycleCompleted = false;

                        // 对于第一个出手，我们不能检查它的前一个出手
                        // 检查是否前一个出手导致了周期完成
                        if (HistoryData[nextResultIndex].IsZhongJiang || prevRecord.CycleStep == GetCycleLength())
                        {
                            currentCycle = prevRecord.CycleNumber + 1;
                            stepOfCurrentCycle = 1;
                            prevCycleCompleted = true;
                        }
                        else
                        {
                            // 继续当前周期
                            stepOfCurrentCycle = prevRecord.CycleStep + 1;
                        }
                    }
                    else
                    {
                        // 这是当前最新的数据，无法检查是否会中奖，保守估计继续当前周期
                        stepOfCurrentCycle = prevRecord.CycleStep + 1;
                    }
                }

                // 限制步骤不超过设定的周期长度
                stepOfCurrentCycle = Math.Min(stepOfCurrentCycle, GetCycleLength());

                // 为当前出手分配周期信息
                record.CycleNumber = currentCycle;
                record.CycleStep = stepOfCurrentCycle;
                record.IsPartOfCycle = true;
                record.HandNumber = stepOfCurrentCycle;

                // 如果这是我们要计算的目标数据，保存结果
                if (index == targetIndex)
                {
                    currentData.CycleNumber = record.CycleNumber;
                    currentData.CycleStep = record.CycleStep;
                    currentData.IsPartOfCycle = record.IsPartOfCycle;
                    currentData.HandNumber = record.HandNumber;
                }
            }

            // 调试输出（如果需要）
            // Console.WriteLine($"周期[{currentData.CycleNumber}] 步骤[{currentData.CycleStep}] - 期号: {currentData.QiHao}");
        }
        public static CheckResult KLineIsEnoughAddBuLinTopGui(List<KLine> kLineList)
        {
            var checkResult = KLineIsEnough(kLineList);
            if (checkResult.Result)
            {
                var kLine = kLineList.Last();
                //再加一个布林上轨的判断
                if (kLine.Bolling != null && (kLine.KValue + 1.857) >= kLine.Bolling.BollUpperValue)
                {
                    checkResult.Result = false;
                    checkResult.Message = "将要接近布林上轨";
                }
                else
                {
                    if (kLine != null && kLine.Bolling != null)
                    {
                        checkResult.KValue = kLine.KValue;
                        checkResult.Bolling = kLine.Bolling;
                    }
                }
            }

            if (checkResult.Result)
            {

                //判断上轨是不是下降
                var kLine = kLineList[kLineList.Count - 1];
                var kLinePre1 = kLineList[kLineList.Count - 2];
                var kLinePre2 = kLineList[kLineList.Count - 3];
                var kLinePre3 = kLineList[kLineList.Count - 4];
                var KLinePre4 = kLineList[kLineList.Count - 5];
                var KLinePre5 = kLineList[kLineList.Count - 6];
                var KLinePre6 = kLineList[kLineList.Count - 7];
                var KLinePre7 = kLineList[kLineList.Count - 8];
                var KLinePre8 = kLineList[kLineList.Count - 9];
                var count = 0;
                if (kLine.Bolling.BollUpperValue <= kLinePre1.Bolling.BollUpperValue) count++;
                if (kLinePre1.Bolling.BollUpperValue <= kLinePre2.Bolling.BollUpperValue) count++;
                if (kLinePre2.Bolling.BollUpperValue <= kLinePre3.Bolling.BollUpperValue) count++;
                if (kLinePre3.Bolling.BollUpperValue <= KLinePre4.Bolling.BollUpperValue) count++;
                if (KLinePre4.Bolling.BollUpperValue <= KLinePre5.Bolling.BollUpperValue) count++;
                //if( KLinePre5.Bolling.BollUpperValue <= KLinePre6.Bolling.BollUpperValue) count++;
                //if( KLinePre6.Bolling.BollUpperValue <= KLinePre7.Bolling.BollUpperValue) count++;
                //if( KLinePre7.Bolling.BollUpperValue <= KLinePre8.Bolling.BollUpperValue) count++;
                //再加一个布林上轨的判断 是否是最近5期有1期下降
                if (count >= 1)
                {
                    checkResult.Result = false;
                    checkResult.Message = "布林上轨最近5期有1期下降";
                }
            }
            return checkResult;
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



        /// <summary>
        /// 重新初始化处理器
        /// </summary>
        /// <summary>
        /// 获取当前配置的周期长度
        /// </summary>
        /// <returns></returns>
        public static int GetCycleLength()
        {
            return AppConfig.Current.TradingSettings.CycleLength;
        }
    }
}
