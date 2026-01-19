using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Hou2Select35Model : BaseCodeInfo
    {
        public static int ShowNumber = 6;
        /// <summary>
        /// 50个号码
        /// </summary>
        public List<string> Number35 { get; set; }

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
