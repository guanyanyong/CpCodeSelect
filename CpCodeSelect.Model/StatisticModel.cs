using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    /// <summary>
    /// 统计类型
    /// </summary>
    public class StatisticModel
    {
        /// <summary>
        /// 开奖期号
        /// </summary>
        public string CodeQiHao { get; set; }
        /// <summary>
        /// 开奖号
        /// </summary>
        public string CodeNumber { get; set; }
        /// <summary>
        /// 统计的位置号码信息
        /// </summary>
        public PositionNumber PositionNumber { get; set; }
        /// <summary>
        /// 位置类型,万千百十个
        /// </summary>
        public PositionType PositionType { get; set; }
        /// <summary>
        /// 统计类型,大小单双
        /// </summary>
        public string StatisticType {  get; set; }
        /// <summary>
        /// 号码
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 挂的次数
        /// </summary>
        public int GuaCount { get; set; }
        /// <summary>
        /// 挂后中
        /// </summary>
        public int GuaHouZhong { get; set; }

    }
}
