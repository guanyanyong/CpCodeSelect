using CpCodeSelect.Model.ExModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.IndexCalc
{
    public class MACDCalculator
    {
        /// <summary>
        /// 计算MACD指标
        /// </summary>
        /// <param name="prices">输入价格序列（通常为收盘价）</param>
        /// <param name="shortPeriod">短期EMA周期，默认12</param>
        /// <param name="longPeriod">长期EMA周期，默认26</param>
        /// <param name="signalPeriod">信号线EMA周期，默认9</param>
        /// <returns>包含DIF、DEA和柱状图的结果对象</returns>
        public static List<MACDResult> Calculate(
            List<double> prices,
            int shortPeriod = 12,
            int longPeriod = 26,
            int signalPeriod = 9)
        {
            if (prices == null || prices.Count == 0)
                throw new ArgumentException("价格列表不能为空。");

            int n = prices.Count;
            var dif = new List<double>(n);
            var dea = new List<double>(n);
            var histogram = new List<double>(n);

            // 1. 计算短期和长期EMA
            var emaShort = CalculateEMA(prices, shortPeriod);
            var emaLong = CalculateEMA(prices, longPeriod);

            // 2. 计算DIF = EMA短期 - EMA长期
            for (int i = 0; i < n; i++)
            {
                dif.Add(emaShort[i] - emaLong[i]);
            }

            // 3. 计算DEA（DIF的EMA）
            dea = CalculateEMA(dif, signalPeriod);

            // 4. 计算柱状图 = (DIF - DEA) * 2
            for (int i = 0; i < n; i++)
            {
                histogram.Add((dif[i] - dea[i]) * 2);
            }

            var list =new List<MACDResult>();
            for (int i = 0; i < n; i++)
            {
                var entity = new MACDResult
                {
                    DEA = dea[i],
                    DIF = dif[i],
                    Histogram = histogram[i]
                };
                list.Add(entity);
            }
            return list;
        }
        public static MACDResult GetLatest(List<double> prices, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
        {
            if (prices == null || prices.Count == 0 || prices.Count < 150) return null;
            if(prices.Count>300) prices = prices.GetRange(prices.Count - 300, 300);
            var result = Calculate(prices, shortPeriod, longPeriod, signalPeriod);
            if (result.Count == 0) return null;
            int lastIndex = result.Count - 1;
            return new MACDResult
            {
                DEA = result[lastIndex].DEA,
                DIF = result[lastIndex].DIF,
                Histogram = result[lastIndex].Histogram
            };
        }

        /// <summary>
        /// 计算指数移动平均（EMA）
        /// </summary>
        /// <param name="data">输入数据序列</param>
        /// <param name="period">EMA周期</param>
        /// <returns>EMA序列，前period-1个元素为NaN（或0），后续为有效值</returns>
        private static List<double> CalculateEMA(List<double> data, int period)
        {
            int n = data.Count;
            var ema = new List<double>(n);
            double multiplier = 2.0 / (period + 1); // 平滑系数

            // 第一个EMA值取前period个数据的简单平均
            if (n < period)
            {
                // 数据不足，全部填充NaN
                for (int i = 0; i < n; i++)
                    ema.Add(double.NaN);
                return ema;
            }

            // 计算初始SMA
            double sum = 0;
            for (int i = 0; i < period; i++)
                sum += data[i];
            double prevEma = sum / period;
            ema.Add(prevEma); // 第period个位置对应索引period-1

            // 填充前面的位置为NaN（表示无效）
            for (int i = 0; i < period - 1; i++)
                ema.Insert(0, double.NaN); // 注意：从头部插入保持顺序

            // 计算后续EMA
            for (int i = period; i < n; i++)
            {
                double currentEma = (data[i] - prevEma) * multiplier + prevEma;
                ema.Add(currentEma);
                prevEma = currentEma;
            }

            return ema;
        }
    }
}
