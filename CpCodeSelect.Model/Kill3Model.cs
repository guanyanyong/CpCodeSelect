using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Kill3Model
    {
        public string Name { get; set; }
        /// <summary>
        /// 杀3码的位置
        /// </summary>
        public Kill3Position Kill3Position { get; set; }
        /// <summary>
        /// 是否连挂
        /// </summary>
        public bool IsLianGua { get; set; }
        /// <summary>
        /// 连挂次数
        /// </summary>
        public int GuaCount { get; set; }
        /// <summary>
        /// 挂后中几个
        /// </summary>
        public int GuaHouZhong { get;set; }
        /// <summary>
        /// 连中次数
        /// </summary>
        public int LianZhongCount { get; set; }


    }
}
