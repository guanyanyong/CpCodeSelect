using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.ExModel
{
    public class YiLouSetExModel
    {
        public string 当前期号 { get; set; }
        public string 当前开奖号 { get; set; }
        public int 遗漏数 { get; set; }
        public string 期号 { get; set; }
        public string 开奖号 { get; set; }
        public string 五十码 {get;set;}
        public string 数据来源 { get; set; }
        public int 遗漏3次次数 { get; set; }
    }
}
