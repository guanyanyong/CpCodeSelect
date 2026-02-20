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
            //if(prices.Count>300) prices = prices.GetRange(prices.Count - 300, 300);
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
            double multiplier = 2.0 / (period + 1);

            // 1. 前值填充，处理NaN
            double[] filled = new double[n];
            double lastValid = double.NaN;
            for (int i = 0; i < n; i++)
            {
                if (!double.IsNaN(data[i]))
                {
                    lastValid = data[i];
                    filled[i] = data[i];
                }
                else
                {
                    filled[i] = lastValid; // 若lastValid为NaN，则保留NaN
                }
            }

            // 2. 找到第一个有效值的索引
            int startIdx = 0;
            while (startIdx < n && double.IsNaN(filled[startIdx]))
            {
                startIdx++;
            }

            // 全部为NaN 或 有效数据不足周期数
            if (startIdx == n || n - startIdx < period)
            {
                for (int i = 0; i < n; i++)
                    ema.Add(double.NaN);
                return ema;
            }

            // 3. 计算第一个EMA（简单平均）
            double sum = 0;
            for (int i = startIdx; i < startIdx + period; i++)
                sum += filled[i];
            double prevEma = sum / period;

            // 填充前面无效区域为NaN（直到第一个EMA所在索引的前一个位置）
            for (int i = 0; i < startIdx + period - 1; i++)
                ema.Add(double.NaN);
            ema.Add(prevEma); // 第一个EMA

            // 4. 递推剩余EMA
            for (int i = startIdx + period; i < n; i++)
            {
                double currentEma = (filled[i] - prevEma) * multiplier + prevEma;
                ema.Add(currentEma);
                prevEma = currentEma;
            }

            return ema;
        }
    }
}
