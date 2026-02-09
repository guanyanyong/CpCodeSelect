using CpCodeSelect.Util.Scorer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.Scorer
{
    public class ScorerUtil
    {

        // 为评分引擎添加评分规则的方法
        public static void InitializeScoringRulesForEngine(ScoringEngine engine)
        {
            engine.AddRule(new KValueBelowMiddleNoBetRule());
            engine.AddRule(new TrendSegmentNoBetRule());
            engine.AddRule(new ConfirmPointBeforeTrendRule());
            engine.AddRule(new BigGapBetweenZeroOrOneStrongRule());
            engine.AddRule(new ThreeTrackSameDirectionRule());
            engine.AddRule(new TwoTrackSameDirectionRule());
            engine.AddRule(new TrackOppositeDirectionRule());
            engine.AddRule(new KValueBreakMiddleNotTouchUpperRule());
            engine.AddRule(new KValueNearUpperRailRule()); // 添加K值接近上轨的评分规则
            engine.AddRule(new YiLouValueRule()); // 添加遗漏值评分规则
            engine.AddRule(new BollingerUpperDeclineRule()); // 添加布林上轨下降评分规则
            engine.AddRule(new ContinuousChuShouLimitRule()); // 添加连续出手限制评分规则
            engine.AddRule(new SecondChuShouLimitRule()); // 添加连续第二手限制规则
            engine.AddRule(new StopAfterWinRule()); // 添加出手中了以后停一期的规则
        }
    }
}
