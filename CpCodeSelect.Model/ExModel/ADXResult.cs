using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.ExModel
{
    public class ADXResult
    {
        /// <summary>
        /// ADX指标值 白线
        /// </summary>
        public double ADXWhite { get; set; }
        /// <summary>
        /// +DI指标值 绿线
        /// </summary>
        public double DIPlusGreen { get; set; }
        /// <summary>
        /// -DI指标值 红线
        /// </summary>
        public double DIMinusRed { get; set; }
    }
}
