using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.Scorer.Rules;
using CpCodeSelect.Util.Scorer156.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.Scorer500
{
    public class ScorerUtil500
    {

        // 为评分引擎添加评分规则的方法
        public static void InitializeScoringRulesForEngine(Scoring156Engine engine)
        {
            engine.AddRule(new KValueBelowMiddleNoBet156Rule());
            engine.AddRule(new TrendSegmentNoBet156Rule());
            engine.AddRule(new ConfirmPointBeforeTrend156Rule());
            //engine.AddRule(new BigGapBetweenZeroOrOneStrong156Rule());
            engine.AddRule(new ThreeTrackSameDirection156Rule());
            engine.AddRule(new TwoTrackSameDirection156Rule());
            engine.AddRule(new TrackOppositeDirection156Rule());
            engine.AddRule(new KValueBreakMiddleNotTouchUpper156Rule());
            engine.AddRule(new KValueNearUpperRail156Rule()); // 添加K值接近上轨的评分规则
            engine.AddRule(new YiLouValue156Rule()); // 添加遗漏值评分规则
            engine.AddRule(new BollingerUpperDecline156Rule()); // 添加布林上轨下降评分规则
            engine.AddRule(new ContinuousChuShouLimit156Rule()); // 添加连续出手限制评分规则
            engine.AddRule(new SecondChuShouLimit156Rule()); // 添加连续第二手限制规则
            engine.AddRule(new StopAfterWin156Rule()); // 添加出手中了以后停一期的规则
        }
    }
}
