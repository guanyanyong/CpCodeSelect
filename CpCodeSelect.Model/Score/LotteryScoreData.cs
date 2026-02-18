using CpCodeSelect.Model.ExModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.Score
{
    public class LotteryScoreData
    {
        public List<string> Number350 { get; set; }
        public string QiHao { get; set; }
        public string Number { get; set; }
        public string Hou3Number { get; set; }
        public bool IsZhongJiang { get; set; }
        public int YiLouValue { get; set; }
        public double KValue { get; set; }
        public Bolling BollingerBands { get; set; }
        public bool IsDaYiLou { get; set; }
        public bool IsQueRenDian { get; set; }
        public bool IsQuShiDuan { get; set; }
        public int QuShiDuanZhongJiangCount { get; set; }
        public int DaYiLouHouLiLunZhouQiNeiZhongJiangShu { get; set; } // 大遗漏后理论周期内中奖数
        public int LianXuZhongJiangCount { get; set; }
        public int LianXuWeiZhongJiangCount { get; set; }
        public bool IsChuShou { get; set; }
        public bool IsChuShouSuccess { get; set; } // 记录出手后是否中奖（在下一期验证）
        public int HandNumber { get; set; }  // 出手次数（在6次周期内的序号）
        public bool IsPartOfCycle { get; set; }  // 是否在出手周期内
        public int CycleNumber { get; set; }  // 所属出手周期编号
        public int CycleStep { get; set; }   // 在当前周期中的步骤（1-6）
        public bool IsCycleComplete { get; set; }  // 周期是否因为中奖而完成
        public bool IsCycleBurst { get; set; } // 周期是否因为6期不中奖而爆掉
        public int Score { get; set; }
        /// <summary>
        /// 当前满足条件的评分规则详情列表，包含规则名称、得分、描述等信息
        /// </summary>
        public List<ScoreDetail> ScoreDetails { get; set; }



        public LotteryScoreData()
        {
            ScoreDetails = new List<ScoreDetail>();
            HandNumber = 0;
            IsPartOfCycle = false;
            IsChuShouSuccess = false; // 默认为false
            CycleNumber = 0;
            CycleStep = 0;
            IsCycleComplete = false;
            IsCycleBurst = false;
            DaYiLouHouLiLunZhouQiNeiZhongJiangShu = 0; // 初始化大遗漏后理论周期内中奖数
        }
    }
    public class ScoreDetail
    {
        public string RuleName { get; set; }
        public int Score { get; set; }
        public string Description { get; set; }
        public bool IsValid { get; set; }
        public int ExpectedScore { get; set; }
        public int ScoreValue { get; set; } // 评分规则的设定分值
    }
}
