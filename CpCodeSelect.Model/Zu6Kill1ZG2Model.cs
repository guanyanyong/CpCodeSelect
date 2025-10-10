using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    /// <summary>
    /// 组六杀1码 中跟挂停
    /// </summary>
    public class Zu6Kill1ZG2Model
    {
        public string Name { get; set; }
        /// <summary>
        /// 位置 前三 中三 后三
        /// </summary>
        public Zu6Kill1Position Zu6Kill1Position { get; set; }

        public List<Zu6Kill1ZG2Item> Zu6Kill1ZG2Items { get; set; }


        public class Zu6Kill1ZG2Item
        {
            public int Number { get; set; }
            /// <summary>
            /// 是否是中跟挂停
            /// </summary>
            public bool IsZG2 { get; set; } = false;
            public int ZG2Count { get; set; } = 0;
            /// <summary>
            /// 中跟挂停后的中次数
            /// </summary>
            public int ZGGTZhongCount { get; set; } = 0;
            /// <summary>
            /// 是否连挂
            /// </summary>
            public bool IsLianGua { get; set; } = false;
            /// <summary>
            /// 连挂次数
            /// </summary>
            public int GuaCount { get; set; }
            /// <summary>
            /// 连中次数
            /// </summary>
            public int LianZhongCount { get; set; }
        }
    }
}
