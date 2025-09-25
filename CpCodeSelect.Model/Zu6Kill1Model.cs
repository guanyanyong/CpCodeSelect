using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Zu6Kill1Model
    {
        public string Name { get; set; }
        /// <summary>
        /// 位置 前三 中三 后三
        /// </summary>
        public Zu6Kill1Position Zu6Kill1Position { get; set; }

        public List<Zu6Kill1Item> Zu6Kill1Items { get; set; }


        public class Zu6Kill1Item
        {
            public int Number { get; set; }
            /// <summary>
            /// 是否连挂
            /// </summary>
            public bool IsLianGua { get; set; }
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
