using CpCodeSelect.Model.ExModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.IndexCalc
{
    public class ADXCalculator
    {
        /// <summary>
        /// 计算 ADX、+DI 和 -DI 指标
        /// </summary>
        /// <param name="high">最高价序列</param>
        /// <param name="low">最低价序列</param>
        /// <param name="close">收盘价序列</param>
        /// <param name="period">周期（通常为14）</param>
        /// <returns>包含 ADX、+DI、-DI 三个数组的元组，长度与输入相同，未计算部分为 double.NaN</returns>
        public static List<ADXResult> Calculate(List<double> high, List<double> low, List<double> close, int period = 14)
        {
            if (high == null || low == null || close == null)
                throw new ArgumentNullException("输入数组不能为 null");
            if (high.Count != low.Count || high.Count != close.Count)
                throw new ArgumentException("数组长度必须相等");
            if (period < 1)
                throw new ArgumentException("周期必须大于0");

            int length = high.Count;
            double[] adx = new double[length];
            double[] plusDI = new double[length];
            double[] minusDI = new double[length];

            // 手动初始化为 NaN（兼容旧版 .NET）
            for (int i = 0; i < length; i++)
            {
                adx[i] = double.NaN;
                plusDI[i] = double.NaN;
                minusDI[i] = double.NaN;
            }

            if (length < period + 1)
                return null; // 数据不足

            // 1. 计算 TR, +DM, -DM
            double[] tr = new double[length];
            double[] plusDM = new double[length];
            double[] minusDM = new double[length];

            for (int i = 1; i < length; i++)
            {
                double highDiff = high[i] - high[i - 1];
                double lowDiff = low[i - 1] - low[i];

                // 真实波幅
                double tr1 = high[i] - low[i];
                double tr2 = Math.Abs(high[i] - close[i - 1]);
                double tr3 = Math.Abs(low[i] - close[i - 1]);
                tr[i] = Math.Max(tr1, Math.Max(tr2, tr3));

                // +DM
                plusDM[i] = (highDiff > lowDiff && highDiff > 0) ? highDiff : 0;

                // -DM
                minusDM[i] = (lowDiff > highDiff && lowDiff > 0) ? lowDiff : 0;
            }

            // 2. 平滑 TR, +DM, -DM (Wilder 平滑)
            double[] smoothTR = new double[length];
            double[] smoothPlusDM = new double[length];
            double[] smoothMinusDM = new double[length];

            // 第一个平滑值：前 period 个值的和
            double sumTR = 0, sumPlusDM = 0, sumMinusDM = 0;
            for (int i = 1; i <= period; i++)
            {
                sumTR += tr[i];
                sumPlusDM += plusDM[i];
                sumMinusDM += minusDM[i];
            }
            smoothTR[period] = sumTR;
            smoothPlusDM[period] = sumPlusDM;
            smoothMinusDM[period] = sumMinusDM;

            // 后续平滑值：smooth = prev_smooth - prev_smooth/period + current
            for (int i = period + 1; i < length; i++)
            {
                smoothTR[i] = smoothTR[i - 1] - smoothTR[i - 1] / period + tr[i];
                smoothPlusDM[i] = smoothPlusDM[i - 1] - smoothPlusDM[i - 1] / period + plusDM[i];
                smoothMinusDM[i] = smoothMinusDM[i - 1] - smoothMinusDM[i - 1] / period + minusDM[i];
            }

            // 3. 计算 +DI, -DI（从 period 开始有效）
            for (int i = period; i < length; i++)
            {
                if (smoothTR[i] != 0)
                {
                    plusDI[i] = 100 * smoothPlusDM[i] / smoothTR[i];
                    minusDI[i] = 100 * smoothMinusDM[i] / smoothTR[i];
                }
            }

            // 4. 计算 DX
            double[] dx = new double[length];
            for (int i = period; i < length; i++)
            {
                double diSum = plusDI[i] + minusDI[i];
                if (diSum != 0)
                    dx[i] = 100 * Math.Abs(plusDI[i] - minusDI[i]) / diSum;
            }

            // 5. 计算 ADX（需要至少 period 个 DX 才能平滑）
            if (length >= 2 * period)
            {
                // 第一个 ADX：前 period 个 DX 的平均
                double sumDX = 0;
                for (int i = period; i < 2 * period; i++)
                    sumDX += dx[i];
                adx[2 * period - 1] = sumDX / period;

                // 后续 ADX 使用 Wilder 平滑
                for (int i = 2 * period; i < length; i++)
                    adx[i] = (adx[i - 1] * (period - 1) + dx[i]) / period;
            }


            var list = new List<ADXResult>();
            for (int i = 0; i < high.Count; i++)
            {
                var entity = new ADXResult
                {
                    ADXWhite = adx[i],
                    DIMinusRed = minusDI[i],
                    DIPlusGreen = plusDI[i]                    
                };
                list.Add(entity);
            }
            return list;
        }


        public static ADXResult GetLatest(List<double> high, List<double> low, List<double> close, int period = 14)
        {
            
            var result = Calculate(high, low, close, period);
            if (result.Count == 0) return null;
            int lastIndex = result.Count - 1;
            return new ADXResult
            {
                ADXWhite = result[lastIndex].ADXWhite,
                DIMinusRed = result[lastIndex].DIMinusRed,
                DIPlusGreen = result[lastIndex].DIPlusGreen
            };
        }
    }
}
