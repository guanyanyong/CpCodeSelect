using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Hou3Select500_ZhouQiZhong : BaseCodeInfo 
    {
        public int Score { get; set; } = 0;
        public bool IsChuShou { get; set; } = false;
        public int ShouNumber { get; set; } = -1;
        public static int ShowNumber = 6;
        public PositionType PositionType { get; set; }
        /// <summary>
        /// 350个号码
        /// </summary>
        public List<string> Number500 { get; set; }
        /// <summary>
        /// 中之前挂的次数
        /// </summary>
        public int ZhongBeforeGua { get; set; } = 0;
        /// <summary>
        /// 3中之前挂的次数
        /// </summary>
        public int Zhong3BeforeGua { get; set; } = 0;
        /// <summary>
        /// 2中之前挂的次数
        /// </summary>
        public int Zhong2BeforeGua { get;set; } = 0;
        /// <summary>
        /// 当前是否在周期内的中后周期
        /// </summary>
        public bool IsZhouQiZhongHou { get; set; } = false;
        /// <summary>
        /// 周期内中后周期内挂的次数
        /// </summary>
        public int ZhouQiZhongHouGua { get; set; } = 0;
        /// <summary>
        /// K线列表
        /// </summary>

        public List<KLine156> KLineList { get; set; }
        public List<KLine156> YiLouTuLineList { get; set; }
        public List<YiLouKline500> YiLouKline500 { get; set; }
        /// <summary>
        /// 评分数据列表
        /// </summary>
        public List<LotteryScoreData> ScoreDateList { get; set; }

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
