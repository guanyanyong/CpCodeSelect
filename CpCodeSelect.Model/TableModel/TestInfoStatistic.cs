using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.TableModel
{
    /// <summary>
    /// 测试信息统计
    /// </summary>
    public class TestInfoStatistic
    {
        /// <summary>
        /// 盈利
        /// </summary>
        public decimal Win { get; set; }
        /// <summary>
        /// 流水
        /// </summary>
        public decimal LiuShui { get; set; }
        /// <summary>
        /// 挂的数量
        /// </summary>
        public int GuaCount { get; set; }

    }
}
