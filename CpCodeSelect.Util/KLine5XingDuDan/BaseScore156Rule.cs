using CpCodeSelect.Model.Score;
using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CpCodeSelect.Util.Scorer;

namespace CpCodeSelect.Util.KLine5XingDuDan
{
    /// <summary>
    /// 评分规则接口
    /// </summary>
    public interface IScore40951Rule
    {
        string RuleName { get; }
        string Description { get; }
        int CalculateScore(LotteryScoreData currentData, List<LotteryScoreData> historyData);
        bool IsValid(LotteryScoreData currentData, List<LotteryScoreData> historyData);
    }

    /// <summary>
    /// 抽象评分规则基类
    /// </summary>
    public abstract class BaseScore40951Rule : IScore40951Rule
    {
        public abstract string RuleName { get; }
        public abstract string Description { get; }
        public abstract int ScoreValue { get; } // 评分值字段

        /// <summary>
        /// 判断规则是否生效
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="historyData"></param>
        /// <returns></returns>
        public virtual bool IsValid(LotteryScoreData currentData, List<LotteryScoreData> historyData)
        {
            return true;
        }

        public virtual int CalculateScore(LotteryScoreData currentData, List<LotteryScoreData> historyData)
        {
            if (IsValid(currentData, historyData))
            {
                return ScoreValue;
            }
            return 0; // 如果条件不满足，返回0分
        }
    }

    /// <summary>
    /// 评分引擎
    /// </summary>
    public class Scoring40951Engine
    {
        private List<IScore40951Rule> _rules;

        public Scoring40951Engine()
        {
            _rules = new List<IScore40951Rule>();
        }

        public void AddRule(IScore40951Rule rule)
        {
            _rules.Add(rule);
        }

        public void RemoveRule(IScore40951Rule rule)
        {
            _rules.Remove(rule);
        }

        public int CalculateTotalScore(LotteryScoreData currentData, List<LotteryScoreData> historyData)
        {
            int totalScore = 0;
            // 清空之前的评分详情
            currentData.ScoreDetails.Clear();

            foreach (var rule in _rules)
            {
                bool isValid = rule.IsValid(currentData, historyData);

                if (isValid)
                {
                    int ruleScore = rule.CalculateScore(currentData, historyData);
                    totalScore += ruleScore;

                    // 添加评分详情 (触发的规则)
                    int expectedScore = 0;
                    if (rule is BaseScoreRule baseRule)
                    {
                        expectedScore = baseRule.ScoreValue;
                    }

                    currentData.ScoreDetails.Add(new ScoreDetail
                    {
                        RuleName = rule.RuleName,
                        Score = ruleScore, // 触发的规则，显示实际分数
                        Description = rule.Description,
                        IsValid = isValid,
                        ExpectedScore = expectedScore
                    });
                }
                else
                {
                    // 添加评分详情 (未触发的规则)
                    int expectedScore = 0;
                    if (rule is BaseScoreRule baseRule)
                    {
                        expectedScore = baseRule.ScoreValue;
                    }

                    currentData.ScoreDetails.Add(new ScoreDetail
                    {
                        RuleName = rule.RuleName,
                        Score = 0, // 未触发的规则，显示0分
                        Description = rule.Description,
                        IsValid = isValid,
                        ExpectedScore = expectedScore
                    });
                }
            }

            return totalScore;
        }

        public List<ScoreDetail> GetScoreDetails(LotteryScoreData currentData, List<LotteryScoreData> historyData)
        {
            var details = new List<ScoreDetail>();

            foreach (var rule in _rules)
            {
                bool isValid = rule.IsValid(currentData, historyData);
                int actualScore = 0;

                if (isValid)
                {
                    actualScore = rule.CalculateScore(currentData, historyData);
                }

                int expectedScore = 0;
                int scoreValue = 0;

                if (rule is BaseScoreRule baseRule)
                {
                    expectedScore = baseRule.ScoreValue;
                    scoreValue = baseRule.ScoreValue; // 使用评分规则的分值
                }

                details.Add(new ScoreDetail
                {
                    RuleName = rule.RuleName,
                    Score = actualScore,
                    Description = rule.Description,
                    IsValid = isValid,
                    ExpectedScore = expectedScore,
                    ScoreValue = scoreValue
                });
            }

            return details;
        }
    }
}