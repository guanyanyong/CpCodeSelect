using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Hou2Select50YiLouSetForm3guashang25zhuModel
    {
        public static int ShowNumber = 6;
        /// <summary>
        /// 50个号码
        /// </summary>
        public List<string> Number50 { get; set; }

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

        public int GuaCount { get; set; } = 0;
        public int ZhongGount { get; set; } = 0;
        /// <summary>
        /// 挂3次的次数
        /// </summary>
        public int Gua3TimeCount { get; set;} = 0;

        public string NumberToString
        {
            get
            {
                return string.Join(" ", Number50);
            }
        }

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
