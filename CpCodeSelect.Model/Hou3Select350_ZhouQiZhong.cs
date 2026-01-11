using CpCodeSelect.Model.ExModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Hou3Select350_ZhouQiZhong
    {
        public static int ShowNumber = 6;
        /// <summary>
        /// 350个号码
        /// </summary>
        public List<string> Number350 { get; set; }

        /// <summary>
        /// 是否需要中 初始需要
        /// </summary>
        public bool NeedZhong { get; set; } = true;

        /// <summary>
        /// 对应的期号
        /// </summary>
        public string CodeQiHao { get; set; }
        /// <summary>
        /// 对应的期号号码
        /// </summary>
        public string CodeNumber { get; set; }
        /// <summary>
        /// 当前连挂次数
        /// </summary>

        public int GuaCount { get; set; } = 0;
        /// <summary>
        /// 当前连中次数
        /// </summary>
        public int ZhongGount { get; set; } = 0;
        /// <summary>
        /// 中之前挂的次数
        /// </summary>
        public int ZhongBeforeGua { get; set; } = 0;
        /// <summary>
        /// 3中之前挂的次数
        /// </summary>
        public int Zhong3BeforeGua { get; set; } = 0;
        /// <summary>
        /// 2中之前挂的次数
        /// </summary>
        public int Zhong2BeforeGua { get;set; } = 0;
        /// <summary>
        /// 当前是否在周期内的中后周期
        /// </summary>
        public bool IsZhouQiZhongHou { get; set; } = false;
        /// <summary>
        /// 周期内中后周期内挂的次数
        /// </summary>
        public int ZhouQiZhongHouGua { get; set; } = 0;
        /// <summary>
        /// K线列表
        /// </summary>

        public List<KLine> KLineList { get; set; }
        public List<YiLouKline350> YiLouKline350 { get; set; }

        /// <summary>
        /// 是否需要显示
        /// </summary>
        public bool IsShow
        {
            get
            {
                return GuaCount >= ShowNumber;
            }
        }
    }
}
