using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class NumberSelector
    {
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        /// <summary>
        /// 从80个号码中随机分成5组，每组随机抽取10个号码
        /// </summary>
        /// <param name="inputNumbers">输入的80个号码（00-99格式）</param>
        /// <returns>包含5组，每组10个号码的列表</returns>
        public static List<List<string>> SelectNumbersFrom80(List<string> inputNumbers)
        {
            // 参数验证
            if (inputNumbers == null || inputNumbers.Count != 80)
            {
                throw new ArgumentException("必须提供恰好80个号码");
            }

            // 验证号码格式
            foreach (var number in inputNumbers)
            {
                if (!IsValidNumber(number))
                {
                    throw new ArgumentException($"号码格式无效: {number}，必须是00-99格式");
                }
            }

            Random random = _threadLocalRandom.Value;

            // 第一步：将80个号码随机分成5组（每组16个号码）
            List<List<string>> initialGroups = DistributeTo5Groups(inputNumbers, random);

            // 第二步：从每组中随机抽取10个号码
            List<List<string>> finalGroups = Select10FromEachGroup(initialGroups, random);

            return finalGroups;
        }

        /// <summary>
        /// 将80个号码随机分成5组，每组16个
        /// </summary>
        private static List<List<string>> DistributeTo5Groups(List<string> numbers, Random random)
        {
            // 随机打乱所有号码
            List<string> shuffledNumbers = numbers.OrderBy(x => random.Next()).ToList();

            List<List<string>> groups = new List<List<string>>();

            // 分成5组，每组16个号码
            for (int i = 0; i < 5; i++)
            {
                List<string> group = shuffledNumbers.Skip(i * 16).Take(16).ToList();
                groups.Add(group);
            }

            return groups;
        }

        /// <summary>
        /// 从每组16个号码中随机抽取10个
        /// </summary>
        private static List<List<string>> Select10FromEachGroup(List<List<string>> groups, Random random)
        {
            List<List<string>> result = new List<List<string>>();

            foreach (var group in groups)
            {
                // 从每组中随机抽取10个号码
                List<string> selectedNumbers = group.OrderBy(x => random.Next())
                                                  .Take(10)
                                                  .OrderBy(x => x)  // 可选：对结果排序
                                                  .ToList();
                result.Add(selectedNumbers);
            }

            return result;
        }

        /// <summary>
        /// 验证号码格式是否为00-99
        /// </summary>
        private static bool IsValidNumber(string number)
        {
            if (string.IsNullOrEmpty(number) || number.Length != 2)
                return false;

            return int.TryParse(number, out int num) && num >= 0 && num <= 99;
        }

        /// <summary>
        /// 生成00-99的测试数据（随机选择80个）
        /// </summary>
        public static List<string> GenerateTestNumbers()
        {
            Random random = new Random();

            // 生成00-99的所有号码
            List<string> allNumbers = Enumerable.Range(0, 100)
                                              .Select(x => x.ToString("D2"))
                                              .ToList();

            // 随机选择80个
            return allNumbers.OrderBy(x => random.Next()).Take(80).ToList();
        }
    }
}
