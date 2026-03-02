using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.Scorer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CpCodeSelect.Util.KLine5XingDuDan.Rules;
namespace CpCodeSelect.Util.KLine5XingDuDan
{
    public class ScorerUtil40951
    {

        // 为评分引擎添加评分规则的方法
        public static void InitializeScoringRulesForEngine(Scoring40951Engine engine)
        {
            engine.AddRule(new KValueBelowMiddleNoBet40951Rule());
            engine.AddRule(new TrendSegmentNoBet40951Rule());
            engine.AddRule(new ConfirmPointBeforeTrend40951Rule());
            //engine.AddRule(new BigGapBetweenZeroOrOneStrong40951Rule());
            engine.AddRule(new ThreeTrackSameDirection40951Rule());
            engine.AddRule(new TwoTrackSameDirection40951Rule());
            engine.AddRule(new TrackOppositeDirection40951Rule());
            engine.AddRule(new KValueBreakMiddleNotTouchUpper40951Rule());
            engine.AddRule(new KValueNearUpperRail40951Rule()); // 添加K值接近上轨的评分规则
            engine.AddRule(new YiLouValue40951Rule()); // 添加遗漏值评分规则
            engine.AddRule(new BollingerUpperDecline40951Rule()); // 添加布林上轨下降评分规则
            engine.AddRule(new ContinuousChuShouLimit40951Rule()); // 添加连续出手限制评分规则
            engine.AddRule(new SecondChuShouLimit40951Rule()); // 添加连续第二手限制规则
            engine.AddRule(new StopAfterWin40951Rule()); // 添加出手中了以后停一期的规则
        }
    }
}
