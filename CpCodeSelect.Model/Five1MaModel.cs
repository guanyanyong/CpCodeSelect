using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Five1MaModel
    {
        /// <summary>
        /// 推荐的数值
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 挂的次数
        /// </summary>
        public int GuaCount { get; set; }
        /// <summary>
        /// 推荐期号
        /// </summary>
        public string QiHao { get; set; }
        /// <summary>
        /// 期号对应的号码
        /// </summary>
        public string CodeNumber { get; set; }
    }
}
