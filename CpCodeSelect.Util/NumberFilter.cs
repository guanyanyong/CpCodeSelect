using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class NumberFilter
    {
        /// <summary>
        /// 从00-99的100个字符串中排除指定的号码，返回剩余的号码
        /// </summary>
        /// <param name="excludedNumbers">要排除的号码列表</param>
        /// <returns>剩余的号码列表</returns>
        public static List<string> GetRemainingNumbers(List<string> excludedNumbers)
        {
            // 生成00-99的所有号码
            List<string> allNumbers = GenerateAllNumbers();

            // 验证排除的号码是否有效
            ValidateExcludedNumbers(excludedNumbers, allNumbers);

            // 使用Except方法排除指定的号码
            List<string> remainingNumbers = allNumbers.Except(excludedNumbers).ToList();

            return remainingNumbers;
        }

        /// <summary>
        /// 生成00-99的所有号码字符串
        /// </summary>
        private static List<string> GenerateAllNumbers()
        {
            return Enumerable.Range(0, 100)
                            .Select(x => x.ToString("D2"))
                            .ToList();
        }

        /// <summary>
        /// 验证排除的号码是否有效
        /// </summary>
        private static void ValidateExcludedNumbers(List<string> excludedNumbers, List<string> allNumbers)
        {
            if (excludedNumbers == null)
            {
                throw new ArgumentNullException(nameof(excludedNumbers));
            }

            if (excludedNumbers.Count != 20)
            {
                throw new ArgumentException($"必须提供恰好20个号码，当前提供了 {excludedNumbers.Count} 个");
            }

            // 检查是否有重复
            if (excludedNumbers.Distinct().Count() != 20)
            {
                throw new ArgumentException("排除的号码列表中存在重复");
            }

            // 检查所有号码是否都在00-99范围内
            foreach (var number in excludedNumbers)
            {
                if (!allNumbers.Contains(number))
                {
                    throw new ArgumentException($"号码 {number} 无效，必须是00-99格式");
                }
            }
        }

        /// <summary>
        /// 重载方法：允许指定要包含的号码而不是排除的号码
        /// </summary>
        public static List<string> GetRemainingNumbersByIncluded(List<string> includedNumbers)
        {
            if (includedNumbers == null || includedNumbers.Count != 20)
            {
                throw new ArgumentException("必须提供恰好20个包含的号码");
            }

            List<string> allNumbers = GenerateAllNumbers();
            List<string> excludedNumbers = allNumbers.Except(includedNumbers).ToList();

            return excludedNumbers;
        }
    }
}
