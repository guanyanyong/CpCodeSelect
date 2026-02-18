using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.ExModel
{
    public class MACDResult
    {
        /// <summary>
        /// 快线(MACD线) 白线
        /// </summary>
        public double DIF { get; set; } 
        /// <summary>
        /// 慢线(信号线) 绿线
        /// </summary>
        public double DEA { get; set; } 
        /// <summary>
        /// 柱状图 红篮柱
        /// </summary>
        public double Histogram { get; set; } 
    }
}
