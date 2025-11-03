using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class NumberSelectForYiLou
    {
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        /// <summary>
        /// 安全版本：确保 exactly 50 个不重复号码
        /// </summary>
        public static List<string> Select50NumbersSafe(
            List<string> excludedNumbers = null,
            List<string> mustIncludeNumbers = null)
        {
            excludedNumbers = excludedNumbers ?? new List<string>();
            mustIncludeNumbers = mustIncludeNumbers ?? new List<string>();

            // 验证参数
            ValidateParameters(excludedNumbers, mustIncludeNumbers);

            // 生成所有号码
            var allNumbers = GenerateAllNumbers();

            // 处理必须包含的号码：排除冲突的
            var validMustInclude = mustIncludeNumbers.Except(excludedNumbers).Distinct().ToList();

            // 计算需要随机选择的数量
            int numbersNeeded = 50 - validMustInclude.Count;

            if (numbersNeeded < 0)
            {
                throw new InvalidOperationException("必须包含的号码数量超过50个");
            }

            // 创建可用号码池：排除指定号码和必须包含的号码
            var availableNumbers = allNumbers
                .Except(excludedNumbers)
                .Except(validMustInclude)
                .ToList();

            // 验证可用号码是否足够
            if (availableNumbers.Count < numbersNeeded)
            {
                //throw new InvalidOperationException(
                //    $"可用号码不足！需要 {numbersNeeded} 个，可用 {availableNumbers.Count} 个。\n" +
                //    $"排除号码: {excludedNumbers.Count} 个，必须包含: {validMustInclude.Count} 个");
                return new List<string>();
            }

            // 随机选择号码
            var randomlySelected = SelectRandomNumbers(availableNumbers, numbersNeeded);

            // 合并结果
            var finalResult = new List<string>();
            finalResult.AddRange(validMustInclude);
            finalResult.AddRange(randomlySelected);

            // 最终验证
            ValidateFinalResult(finalResult, validMustInclude, excludedNumbers);

            // 随机打乱
            Shuffle(finalResult);

            return finalResult;
        }

        /// <summary>
        /// 随机选择号码（确保不重复）
        /// </summary>
        private static List<string> SelectRandomNumbers(List<string> source, int count)
        {
            if (count == 0) return new List<string>();

            // 方法1：洗牌后取前N个（性能好）
            var shuffled = new List<string>(source);
            Shuffle(shuffled);
            return shuffled.Take(count).ToList();
        }

        /// <summary>
        /// 验证最终结果
        /// </summary>
        private static void ValidateFinalResult(List<string> result, List<string> mustInclude, List<string> excluded)
        {
            // 验证数量
            if (result.Count != 50)
            {
                throw new InvalidOperationException($"结果数量错误：期望50，实际{result.Count}");
            }

            // 验证不重复
            if (result.Distinct().Count() != 50)
            {
                var duplicates = result.GroupBy(x => x)
                                      .Where(g => g.Count() > 1)
                                      .Select(g => g.Key)
                                      .ToList();
                throw new InvalidOperationException($"结果中存在重复号码：{string.Join(", ", duplicates)}");
            }

            // 验证包含所有必须包含的号码
            var missingMustInclude = mustInclude.Except(result).ToList();
            if (missingMustInclude.Any())
            {
                throw new InvalidOperationException($"缺少必须包含的号码：{string.Join(", ", missingMustInclude)}");
            }

            // 验证不包含排除的号码
            var containsExcluded = result.Intersect(excluded).ToList();
            if (containsExcluded.Any())
            {
                throw new InvalidOperationException($"包含了排除的号码：{string.Join(", ", containsExcluded)}");
            }

            // 验证号码范围
            var outOfRange = result.Where(n =>
            {
                int num = int.Parse(n);
                return num < 0 || num > 99;
            }).ToList();

            if (outOfRange.Any())
            {
                throw new InvalidOperationException($"包含超出范围的号码：{string.Join(", ", outOfRange)}");
            }
        }

        private static void Shuffle(List<string> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _threadLocalRandom.Value.Next(i + 1);
                string temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private static List<string> GenerateAllNumbers()
        {
            return Enumerable.Range(0, 100)
                            .Select(x => x.ToString("D2"))
                            .ToList();
        }

        private static void ValidateParameters(List<string> excludedNumbers, List<string> mustIncludeNumbers)
        {
            ValidateNumberFormat(excludedNumbers, "排除号码");
            ValidateNumberFormat(mustIncludeNumbers, "必须包含号码");

            if (mustIncludeNumbers.Distinct().Count() != mustIncludeNumbers.Count)
            {
                throw new ArgumentException("必须包含的号码列表中存在重复");
            }

            if (mustIncludeNumbers.Count > 50)
            {
                throw new ArgumentException($"必须包含的号码数量({mustIncludeNumbers.Count})不能超过50个");
            }
        }

        private static void ValidateNumberFormat(List<string> numbers, string listName)
        {
            if (numbers == null) return;

            foreach (string number in numbers)
            {
                if (!IsValidNumber(number))
                {
                    throw new ArgumentException($"{listName} '{number}' 格式无效，必须是00-99的字符串");
                }
            }
        }

        private static bool IsValidNumber(string number)
        {
            return !string.IsNullOrEmpty(number) &&
                   number.Length == 2 &&
                   int.TryParse(number, out int num) &&
                   num >= 0 && num <= 99;
        }
    }
}
