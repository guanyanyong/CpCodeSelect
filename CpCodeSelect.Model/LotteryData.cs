using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class LotteryData
    {// 索引（时间轴）
        public string Index { get; set; }

        // K值（类似股票的收盘价）
        public double KValue { get; set; }

        // 布林带 - 上轨
        public double UpperBand { get; set; }

        // 布林带 - 中轨
        public double MiddleBand { get; set; }

        // 布林带 - 下轨
        public double LowerBand { get; set; }

        // 是否中奖（true为中奖-红格子，false为未中奖-蓝格子）
        public bool IsWin { get; set; }

        // 柱子高度因子（中奖时通常更高，例如1.5-2.0，未中奖时为1.0）
        public double WinHeightFactor { get; set; } = 1.0;
        public double LostHeightFactor { get; set; } = 1.0;
        // 开奖期号
        public string PeriodNumber { get; set; }
        // 开奖号码
        public string WinningNumbers { get; set; }
        /// <summary>
        /// MACD快线（DIF），和的值
        /// </summary>
        public double MACD_DIF { get; set; }
        /// <summary>
        /// MACD快线慢线（DEA）
        /// </summary>
        public double MACD_DEA { get; set; }
        /// <summary>
        /// MACD柱状图（Histogram）通常为DIF与DEA的差值乘以2，反映快线和慢线的背离程度
        /// </summary>
        public double MACD_Histogram { get; set; }

    }
}