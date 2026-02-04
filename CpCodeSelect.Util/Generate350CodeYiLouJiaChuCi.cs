using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    /// <summary>
    /// 通过最近13期的遗漏图和后面400期的出次图来生成350注号码
    /// </summary>
    public class Generate350CodeYiLouJiaChuCi
    {
        private static Random random = new Random();
        private static List<string> allPossibleStrings;

        static Generate350CodeYiLouJiaChuCi()
        {
            // 生成所有可能的字符串 (000-999)
            allPossibleStrings = GenerateAllNumbers();
        }

        /// <summary>
        /// 生成350注数据
        /// </summary>
        /// <param name="list270"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static List<string> Generate(List<string> list413, List<string> takeCodeList, List<string> excludeCodeList, Random random = null)
        {
            if (random == null) random = Generate350CodeYiLouJiaChuCi.random;
            //Console.WriteLine("=== 字符串选择程序 ===\n");


            // 生成000-999的1000个字符串
            List<string> allNumbers = GenerateAllNumbers();
            //Console.WriteLine($"已生成000-999的1000个字符串");

            // 1. 生成列表K（413个字符串）
            List<string> K = null;
            if (list413 != null && list413.Count == 413)
            {
                K = new List<string>(list413);
            }
            else
            {
                K = GenerateListK(allNumbers, random);
            }
            //Console.WriteLine($"已生成列表K，共{K.Count}个元素");

            // 2. 处理前13个号码，生成列表A和B
            //var result = ProcessFirst13Numbers(K, allNumbers, random);
            List<string> A = takeCodeList;
            List<string> B = excludeCodeList;

            //Console.WriteLine($"列表A（必须包含，出现8次）: {string.Join(", ", A)}");
            //Console.WriteLine($"列表B（排除列表，前13个中除A外的号码）: {string.Join(", ", B)}");

            // 验证A在K中的出现次数
            if (A.Count > 0)
            {
                string aNum = A[0];
                int countInK = K.Count(num => num == aNum);
                Console.WriteLine($"验证: 列表A中的'{aNum}'在K中实际出现次数: {countInK}次");
            }

            // 3. 生成列表L（K去掉前13个），并统计次数，生成列表C
            List<string> L = K.Skip(13).ToList();
            Console.WriteLine($"列表L（K去掉前13个）共{L.Count}个元素");


            List<string> C = GetListC(L, 75, 100, random);

            Console.WriteLine($"列表C（出现次数75-100）共有{C.Count}个元素");
            if (C.Count > 0)
            {
                Console.WriteLine($"列表C示例: {string.Join(", ", C.Take(C.Count))}");
            }

            // 4. 生成列表D（排除A、B、C）
            List<string> D = GenerateListD(allNumbers, A, B, L);
            Console.WriteLine($"列表D（排除A、B、C后）共有{D.Count}个元素");

            // 5. 生成列表E（从D中随机选择350-A.Count-C.Count个）
            int eCount = 350 - A.Count - C.Count;
            if (eCount < 0) eCount = 0;
            List<string> E = GenerateListE(D, eCount, allNumbers);

            Console.WriteLine($"列表E（从D中随机选择{eCount}个）共有{E.Count}个元素");
            if (E.Count > 0)
            {
                Console.WriteLine($"列表E示例: {string.Join(", ", E.Take(Math.Min(20, E.Count)))}...");
            }
            E.AddRange(A);
            E.AddRange(C);
            return E;

        }
        // 生成000-999的1000个字符串
        static List<string> GenerateAllNumbers()
        {
            List<string> numbers = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                numbers.Add(i.ToString("D3"));
            }
            return numbers;
        }

        // 生成列表K（413个字符串，从000-999中随机选择，允许重复）
        static List<string> GenerateListK(List<string> allNumbers, Random random = null)
        {
            List<string> K = new List<string>();
            if (random == null)
            {
                random = new Random(Guid.NewGuid().GetHashCode()); // 使用更随机的种子
            }
            for (int i = 0; i < 413; i++)
            {
                int index = random.Next(allNumbers.Count);
                K.Add(allNumbers[index]);
            }

            return K;
        }

        // 处理前13个号码，生成列表A和B
        static (List<string>, List<string>) ProcessFirst13Numbers(List<string> K, List<string> allNumbers, Random random = null)
        {
            // 获取前13个号码
            List<string> first13 = K.Take(13).ToList();

            // 随机选择一个作为列表A（必须包含的，出现8次）
            if (random == null)
                random = new Random(Guid.NewGuid().GetHashCode());
            string aNumber = first13[random.Next(first13.Count)];
            List<string> A = new List<string> { aNumber };

            // 确保A在K中出现8次
            EnsureACountInK(K, A, 8, allNumbers, random);

            // 列表B是前13个中除A外的号码
            List<string> B = first13.Where(num => num != aNumber).Distinct().ToList();

            return (A, B);
        }

        // 确保列表A中的号码在K中出现指定次数
        static void EnsureACountInK(List<string> K, List<string> A, int targetCount,
                                   List<string> allNumbers, Random random)
        {
            if (A.Count == 0) return;

            string aNumber = A[0];
            int currentCount = K.Count(num => num == aNumber);

            Console.WriteLine($"调整前: A号码'{aNumber}'在K中出现{currentCount}次");

            // 如果当前次数不足，补充
            if (currentCount < targetCount)
            {
                int toAdd = targetCount - currentCount;

                // 随机选择位置替换为A（但不能动前13个中的位置，因为可能包含B的号码）
                List<int> replaceableIndices = new List<int>();
                for (int i = 13; i < K.Count; i++)
                {
                    if (K[i] != aNumber) // 只替换不是A的位置
                    {
                        replaceableIndices.Add(i);
                    }
                }

                // 如果可替换的位置不足，需要扩展K列表
                if (toAdd > replaceableIndices.Count)
                {
                    Console.WriteLine($"警告: 需要添加{toAdd}个A，但只有{replaceableIndices.Count}个可替换位置");
                    toAdd = replaceableIndices.Count;
                }

                // 随机选择位置替换
                for (int i = 0; i < toAdd; i++)
                {
                    int randomIndex = random.Next(replaceableIndices.Count);
                    int replaceIndex = replaceableIndices[randomIndex];
                    K[replaceIndex] = aNumber;
                    replaceableIndices.RemoveAt(randomIndex); // 移除已使用的索引
                }

                Console.WriteLine($"已添加{toAdd}个A号码到K中");
            }
            // 如果当前次数超过，减少（但前13个中的A不能动）
            else if (currentCount > targetCount)
            {
                int toRemove = currentCount - targetCount;
                int removed = 0;

                // 从第13个之后的位置开始移除多余的A
                for (int i = 13; i < K.Count && removed < toRemove; i++)
                {
                    if (K[i] == aNumber)
                    {
                        // 用随机其他号码替换（不能是A）
                        K[i] = GetRandomNumberExcept(allNumbers, A, random);
                        removed++;
                    }
                }

                Console.WriteLine($"已移除{removed}个多余的A号码");
            }

            // 验证调整后的次数
            int finalCount = K.Count(num => num == aNumber);
            Console.WriteLine($"调整后: A号码'{aNumber}'在K中出现{finalCount}次");
        }

        // 获取除指定列表外的随机号码
        static string GetRandomNumberExcept(List<string> allNumbers, List<string> excludeList, Random random)
        {
            List<string> available = allNumbers.Where(num => !excludeList.Contains(num)).ToList();
            if (available.Count == 0) return "000"; // 备用值
            return available[random.Next(available.Count)];
        }

        // 计算列表中每个元素的出现频率
        static Dictionary<string, int> CalculateFrequency(List<string> list)
        {
            Dictionary<string, int> frequencyDict = new Dictionary<string, int>();

            foreach (string num in list)
            {
                if (frequencyDict.ContainsKey(num))
                {
                    frequencyDict[num]++;
                }
                else
                {
                    frequencyDict[num] = 1;
                }
            }

            return frequencyDict;
        }

        static List<string> GetListC(List<string> L, int minTotalCount, int maxTotalCount, Random random = null)
        {
            Shuffle(L, random);
            Dictionary<string, int> frequencyDict = CalculateFrequency(L);

            List<string> C = new List<string>();
            int currentTotal = 0;

            // 按出现次数从高到低排序
            var sortedByFrequency = frequencyDict
                .OrderByDescending(kv => kv.Value)
                .ToList();
            // 策略1：尝试找到一组号码，使总出现次数在75-100之间
            foreach (var kv in sortedByFrequency)
            {
                // 如果添加这个号码不会超过上限
                if (currentTotal + kv.Value <= maxTotalCount)
                {
                    C.Add(kv.Key);
                    currentTotal += kv.Value;

                    // 如果已经达到最小值，可以继续尝试添加更多（但不超过上限）
                    if (currentTotal > (minTotalCount+maxTotalCount)/2)
                    {
                        // 可以继续添加，但我们现在已经满足条件，可以选择停止或继续
                        // 这里选择继续，尝试使总数接近上限
                        break;
                    }
                }
            }

            // 如果策略1没有找到合适的组合，尝试策略2：随机组合
            if (currentTotal < minTotalCount)
            {
                C.Clear();
                currentTotal = 0;

                // 随机选择号码直到满足条件
                if (random == null)
                    random = new Random(Guid.NewGuid().GetHashCode());
                var availableNumbers = sortedByFrequency.ToList();

                while (currentTotal < minTotalCount && availableNumbers.Count > 0)
                {
                    int randomIndex = random.Next(availableNumbers.Count);
                    var selected = availableNumbers[randomIndex];

                    // 如果添加这个号码不会超过上限
                    if (currentTotal + selected.Value <= maxTotalCount)
                    {
                        C.Add(selected.Key);
                        currentTotal += selected.Value;
                        availableNumbers.RemoveAt(randomIndex);
                    }
                    else
                    {
                        // 这个号码太大，跳过
                        availableNumbers.RemoveAt(randomIndex);
                    }
                }
            }

            // 如果还是不够，尝试策略3：找最接近的组合
            if (currentTotal < minTotalCount)
            {
                // 使用动态规划找最接近的组合
                C = FindClosestCombination(frequencyDict, minTotalCount, maxTotalCount);
                currentTotal = C.Sum(num => frequencyDict[num]);
            }

            Console.WriteLine($"  最终选择的列表C总出现次数: {currentTotal}");
            return C;
        }

        // 使用动态规划找最接近的组合
        static List<string> FindClosestCombination(Dictionary<string, int> frequencyDict, int minTotalCount, int maxTotalCount)
        {
            var items = frequencyDict.ToList();
            int n = items.Count;

            // 动态规划数组，dp[i]表示总出现次数为i时选择的号码
            List<string>[] dp = new List<string>[maxTotalCount + 1];
            for (int i = 0; i <= maxTotalCount; i++)
            {
                dp[i] = new List<string>();
            }

            // 动态规划
            for (int i = 0; i < n; i++)
            {
                int value = items[i].Value;
                string key = items[i].Key;

                // 从后往前更新，避免重复选择
                for (int j = maxTotalCount; j >= value; j--)
                {
                    if (dp[j - value].Count > 0 || j == value)
                    {
                        // 如果这个组合更好（更接近minTotalCount）或者还没有组合
                        if (dp[j].Count == 0 ||
                            (Math.Abs(j - minTotalCount) < Math.Abs(dp[j].Count > 0 ? j - minTotalCount : int.MaxValue)))
                        {
                            dp[j] = new List<string>(dp[j - value]);
                            dp[j].Add(key);
                        }
                    }
                }

                // 处理当前物品单独的情况
                if (value <= maxTotalCount && dp[value].Count == 0)
                {
                    dp[value] = new List<string> { key };
                }
            }

            // 找到最接近minTotalCount的组合
            int bestTotal = 0;
            for (int i = minTotalCount; i <= maxTotalCount; i++)
            {
                if (dp[i].Count > 0)
                {
                    bestTotal = i;
                    break;
                }
            }

            // 如果没找到大于等于minTotalCount的，找最接近的
            if (bestTotal == 0)
            {
                for (int i = minTotalCount - 1; i >= 1; i--)
                {
                    if (dp[i].Count > 0)
                    {
                        bestTotal = i;
                        break;
                    }
                }
            }

            return bestTotal > 0 ? dp[bestTotal] : new List<string>();
        }

        // 生成列表D（从allNumbers中排除A、B、C）
        static List<string> GenerateListD(List<string> allNumbers, List<string> A, List<string> B, List<string> C)
        {
            // 合并所有要排除的号码
            HashSet<string> excludeSet = new HashSet<string>();
            excludeSet.UnionWith(A);
            excludeSet.UnionWith(B);
            excludeSet.UnionWith(C);

            return allNumbers.Where(num => !excludeSet.Contains(num)).ToList();
        }

        // 生成列表E（从D中随机选择指定数量的元素）
        static List<string> GenerateListE(List<string> D, int count, List<string> allNumbers)
        {
            if (count <= 0) return new List<string>();

            // 如果需要的数量大于D的数量，则返回所有D
            if (count >= D.Count) return new List<string>(D);

            Random random = new Random(Guid.NewGuid().GetHashCode());

            // 确保不会选择到重复的（虽然D本身不会有重复）
            List<string> result = new List<string>();
            HashSet<string> selected = new HashSet<string>();

            // 先尝试从D中选择
            List<string> tempD = new List<string>(D);

            // Fisher-Yates洗牌算法
            for (int i = tempD.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = tempD[i];
                tempD[i] = tempD[j];
                tempD[j] = temp;
            }

            // 从打乱后的D中取前count个
            for (int i = 0; i < Math.Min(count, tempD.Count); i++)
            {
                result.Add(tempD[i]);
                selected.Add(tempD[i]);
            }

            // 如果D中的数量不够，从allNumbers中随机选择（排除已选择的）
            if (result.Count < count)
            {
                List<string> allAvailable = allNumbers.Where(n => !selected.Contains(n)).ToList();
                int needed = count - result.Count;

                for (int i = 0; i < Math.Min(needed, allAvailable.Count); i++)
                {
                    int index = random.Next(allAvailable.Count);
                    result.Add(allAvailable[index]);
                    allAvailable.RemoveAt(index); // 避免重复选择
                }
            }

            return result;
        }

        /// <summary>
        /// 洗牌算法
        /// </summary>
        private static void Shuffle(List<string> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
