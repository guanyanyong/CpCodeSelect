using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.ZiJinFangAn
{
    public class TouZhuResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 是否需要初始化，当重新下一轮投注时传递为true
        /// </summary>
        public bool NeedInit { get; set; }
        public List<string> MessageList { get; set; }
    }
    /// <summary>
    /// 开奖结果,包含是否最大变动和消息列表
    /// </summary>
    public class KaiJiangResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 是否最大值变动，当前值超过初始本金的5%时为true，否则为false
        /// </summary>
        public bool MaxChange { get; set; }
        public List<string> MessageList { get; set; }
    }
}
