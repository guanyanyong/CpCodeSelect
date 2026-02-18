using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.ExModel
{
    public class MACDResult
    {
        public double DIF { get; set; } //快线(MACD线) 白线
        public double DEA { get; set; } //慢线(信号线) 绿线
        public double Histogram { get; set; } //柱状图 红绿柱
    }
}
