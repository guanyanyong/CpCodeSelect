using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.ExModel
{
    public class KLine156 
    {
        public double ZhongAddValue { get; set; } = 5.4102564;
        public double GuaAddValue { get; set; } = -1;
        public string CodeQiHao { get; set; }
        public string CodeNumber { get; set; }
        public List<string> Code350Code { get; set; }
        public bool IsZhong { get; set; }
        public double KValue { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public double KaiPanValue
        {
            get
            {
                if (IsZhong)
                {
                    return KValue- ZhongAddValue;
                }
                else if(!IsZhong)
                {
                    return KValue - GuaAddValue;
                }
                return KValue;
            }
        }
        /// <summary>
        /// 收盘价
        /// </summary>
        public double ShouPanValue
        {
            get
            {
                return KValue;
            }
        }
        /// <summary>
        /// 最高价
        /// </summary>
        public double ZuiGaoValue
        {
            get
            {
                if (IsZhong)
                {
                    return KValue;
                }
                else
                {
                    return KValue - GuaAddValue;
                }
            }
        }
        /// <summary>
        /// 最低价
        /// </summary>
        public double ZuiDiValue
        {
            get
            {
                if (IsZhong)
                {
                    return KValue-ZhongAddValue;
                }
                else
                {
                    return KValue;
                }
            }
        }
        public Bolling Bolling { get; set; } //布林线
        public MACDResult MACDResult { get; set; } //MACD指标结果
        public ADXResult ADXResult { get; set; } //ADX指标结果
        /// <summary>
        /// 是否在布林中轨之上
        /// </summary>
        public bool IsOverMiddle
        {
            get
            {
                return KValue >= Bolling.MiddleValue;
            }
        }

        /// <summary>
        /// MACD是否金叉
        /// </summary>
        public bool IsGoldenCross
        {
            get
            {
                return MACDResult.DIF > MACDResult.DEA;
            }
        }
        /// <summary>
        /// ADX是否是金叉 绿线在红线之上（+DI在-DI之上）
        /// </summary>
        public bool IsADXCross
        {
            get
            {
                return ADXResult.DIPlusGreen > ADXResult.DIMinusRed;
            }
        }
        /// <summary>
        /// ADX 指标值白线是否在红线之上（ADX在-DI之上）
        /// </summary>
        public bool IsADXWhiteOverRed
        {
            get
            {
                return ADXResult.ADXWhite > ADXResult.DIMinusRed;
            }
        }
        /// <summary>
        /// 当前连挂次数
        /// </summary>
        public int CurrentGuaCount { get; set; }
        /// <summary>
        /// 当前连中次数
        /// </summary>
        public int CurrentZhongCount { get; set; }

    }
}
