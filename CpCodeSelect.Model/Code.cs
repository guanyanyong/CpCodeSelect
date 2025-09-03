using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Code
    {
        public string CodeQiHao { get; set; }
        public string CodeNumber { get; set; }
        /// <summary>
        /// 上一期号码
        /// </summary>
        public Code PreCode { get; set; }
        public PositionNumber Wan { get; set; }
        public PositionNumber Qian { get; set; }
        public PositionNumber Bai { get; set; }
        public PositionNumber Shi { get; set; }
        public PositionNumber Ge { get; set; }

        public List<PositionDragonTiger> DragonTigerList { get; set; }

    }
}
