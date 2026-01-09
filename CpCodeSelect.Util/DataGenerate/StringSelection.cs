using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace StringCollectionProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("开始生成集合...");
                var generator = new CollectionGenerator();

                // 1. 生成固定的集合A和[A减]
                Console.WriteLine("\n步骤1: 生成固定的集合A和[A减]");
                var fixedResult = generator.GenerateFixedCollections();

                Console.WriteLine($"集合A大小: {fixedResult.CollectionA.Count}");
                Console.WriteLine($"[A减]大小(K): {fixedResult.CollectionADistinct.Count}");
                Console.WriteLine($"A集合中出现次数统计: {fixedResult.OccurrenceCounts.Count} 个不同的字符串");

                // 显示一些统计信息
                var stats = GetCollectionStats(fixedResult.CollectionA, fixedResult.OccurrenceCounts);
                Console.WriteLine(stats);

                // 5. 多线程生成多个集合E，每次重新生成不同的集合B
                Console.WriteLine("\n步骤5: 多线程批量生成不同的集合E");
                int batchSize = 50;
                Console.WriteLine($"每批生成 {batchSize} 个集合E");

                var allCollectionsE = generator.GenerateMultipleCollectionsE(
                    fixedResult.CollectionA,
                    fixedResult.CollectionADistinct,
                    batchSize);

                Console.WriteLine($"\n成功生成 {allCollectionsE.Count} 个集合E");

                // 验证和显示结果
                //DisplayResults(fixedResult, allCollectionsE);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"程序出错: {ex.Message}");
                Console.WriteLine($"堆栈: {ex.StackTrace}");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        static string GetCollectionStats(List<string> collectionA, Dictionary<string, int> occurrenceCounts)
        {
            var stats = new System.Text.StringBuilder();
            stats.AppendLine("集合A统计信息:");
            stats.AppendLine($"总字符串数: {collectionA.Count}");
            stats.AppendLine($"不同字符串数: {occurrenceCounts.Count}");

            // 统计出现次数的分布
            var countGroups = occurrenceCounts.GroupBy(kvp => kvp.Value)
                .Select(g => new { Count = g.Key, Frequency = g.Count() })
                .OrderByDescending(x => x.Frequency)
                .ToList();

            stats.AppendLine("出现次数分布:");
            foreach (var group in countGroups.Take(10))
            {
                stats.AppendLine($"  出现{group.Count}次: {group.Frequency}个字符串");
            }

            if (countGroups.Count > 10)
            {
                stats.AppendLine($"  其他: {countGroups.Skip(10).Sum(g => g.Frequency)}个字符串");
            }

            return stats.ToString();
        }

        static void DisplayResults(CollectionGenerationResult fixedResult, ConcurrentBag<BatchCollectionResult> allCollectionsE)
        {
            Console.WriteLine("\n=== 最终结果验证 ===");

            // 显示固定集合A的示例
            Console.WriteLine($"\n固定集合A的前20个元素:");
            Console.WriteLine(string.Join(", ", fixedResult.CollectionA.Take(20)));

            // 显示不同集合E的统计信息
            Console.WriteLine($"\n生成的集合E统计:");
            var collectionList = allCollectionsE.ToList();

            for (int i = 0; i < Math.Min(3, collectionList.Count); i++)
            {
                var result = collectionList[i];
                Console.WriteLine($"\n集合E #{i + 1}:");
                Console.WriteLine($"  集合B大小: {result.CollectionB.Count}");
                Console.WriteLine($"  集合E大小: {result.CollectionE.Count}");
                Console.WriteLine($"  集合B出现总次数: {result.BTotalOccurrences}");
                Console.WriteLine($"  集合B字符串数量: {result.BUniqueCount}");

                if (i == 0)
                {
                    Console.WriteLine($"  集合E示例: {string.Join(", ", result.CollectionE.Take(10))}...");
                }
            }

            // 检查集合E的多样性
            CheckDiversity(collectionList);
        }

        static void CheckDiversity(List<BatchCollectionResult> collectionList)
        {
            if (collectionList.Count < 2) return;

            Console.WriteLine("\n=== 集合E多样性检查 ===");

            // 检查第一个和第二个集合E的差异
            var set1 = collectionList[0].CollectionE;
            var set2 = collectionList[1].CollectionE;

            int commonCount = set1.Intersect(set2).Count();
            double similarity = (double)commonCount / set1.Count;

            Console.WriteLine($"集合E#1 和 集合E#2 的比较:");
            Console.WriteLine($"  共同元素: {commonCount} 个");
            Console.WriteLine($"  相似度: {similarity:P2}");

            if (similarity > 0.5)
            {
                Console.WriteLine("  警告: 集合E相似度过高，可能生成方式需要调整");
            }
            else
            {
                Console.WriteLine("  良好: 集合E具有较好的多样性");
            }
        }
    }

    public class CollectionGenerator
    {
        private readonly Random _globalRandom = new Random();
        private readonly object _lockObject = new object();

        // 生成固定的集合A和[A减]
        public CollectionGenerationResult GenerateFixedCollections()
        {
            // 生成所有可能的字符串 (000-999)
            var allPossibleStrings = Enumerable.Range(0, 1000)
                .Select(i => i.ToString("D3"))
                .ToList();

            // 生成集合A (1620个字符串，允许重复)
            var collectionA = new List<string>();
            for (int i = 0; i < 1620; i++)
            {
                int index = _globalRandom.Next(0, 1000);
                collectionA.Add(allPossibleStrings[index]);
            }

            // 统计出现次数
            var occurrenceCounts = new Dictionary<string, int>();
            foreach (var str in collectionA)
            {
                if (occurrenceCounts.ContainsKey(str))
                    occurrenceCounts[str]++;
                else
                    occurrenceCounts[str] = 1;
            }

            // 生成[A减] (不重复的集合)
            var collectionADistinct = new HashSet<string>(collectionA);

            return new CollectionGenerationResult
            {
                CollectionA = collectionA,
                CollectionADistinct = collectionADistinct,
                OccurrenceCounts = occurrenceCounts
            };
        }

        // 生成集合B - 确保总次数在580-660范围内
        public CollectionBResult GenerateCollectionB(List<string> collectionA)
        {
            // 统计每个字符串的出现次数
            var occurrenceCounts = new Dictionary<string, int>();
            foreach (var str in collectionA)
            {
                if (occurrenceCounts.ContainsKey(str))
                    occurrenceCounts[str]++;
                else
                    occurrenceCounts[str] = 1;
            }

            // 创建线程安全的随机数生成器
            Random random;
            lock (_lockObject)
            {
                random = new Random(_globalRandom.Next());
            }

            // 我们将使用贪心算法确保总次数在范围内
            HashSet<string> collectionB = new HashSet<string>();
            int totalOccurrences = 0;
            int targetUniqueCount = random.Next(220, 310); // 200-300个不同字符串

            // 将字符串按出现次数排序
            var sortedStrings = occurrenceCounts
                .OrderBy(kvp => kvp.Value)
                .ToList();

            // 为了确保总次数在580-660，我们需要精心选择
            // 策略：先选择一些中等出现次数的字符串，然后调整

            // 计算每个字符串的平均出现次数目标
            double targetAvgOccurrence = random.Next(640, 700) / (double)targetUniqueCount;

            // 选择接近目标平均出现次数的字符串
            var candidateStrings = sortedStrings
                .Where(kvp => Math.Abs(kvp.Value - targetAvgOccurrence) <= 2)
                .ToList();

            if (candidateStrings.Count < targetUniqueCount)
            {
                // 如果候选字符串不足，扩大范围
                candidateStrings = sortedStrings.ToList();
            }

            // 随机打乱候选字符串
            var shuffledCandidates = candidateStrings
                .OrderBy(x => random.Next())
                .ToList();

            // 选择字符串直到达到目标数量
            for (int i = 0; i < Math.Min(targetUniqueCount, shuffledCandidates.Count); i++)
            {
                var str = shuffledCandidates[i].Key;
                var count = shuffledCandidates[i].Value;

                collectionB.Add(str);
                totalOccurrences += count;
            }

            // 调整集合B以满足总次数要求
            return AdjustCollectionB(collectionB, totalOccurrences, occurrenceCounts, random);
        }

        // 调整集合B以满足总次数要求
        private CollectionBResult AdjustCollectionB(
            HashSet<string> collectionB,
            int currentTotal,
            Dictionary<string, int> occurrenceCounts,
            Random random)
        {
            int attempt = 0;
            int maxAttempts = 10000;

            while (attempt < maxAttempts)
            {
                attempt++;

                // 检查当前状态
                int uniqueCount = collectionB.Count;

                // 如果已经满足条件，直接返回
                if (currentTotal >= 640 && currentTotal <= 700 &&
                    uniqueCount >= 220 && uniqueCount <= 310)
                {
                    return new CollectionBResult
                    {
                        CollectionB = collectionB,
                        TotalOccurrences = currentTotal,
                        UniqueCount = uniqueCount
                    };
                }

                // 如果不满足条件，进行调整
                if (currentTotal < 640)
                {
                    // 总次数太低，需要增加
                    // 尝试替换一个出现次数低的字符串为出现次数高的字符串
                    if (collectionB.Count > 0)
                    {
                        // 找到集合B中出现次数最低的字符串
                        var minOccurrenceStr = collectionB
                            .OrderBy(s => occurrenceCounts[s])
                            .First();

                        // 找到不在集合B中且出现次数更高的字符串
                        var higherOccurrenceStrs = occurrenceCounts
                            .Where(kvp => !collectionB.Contains(kvp.Key) &&
                                         kvp.Value > occurrenceCounts[minOccurrenceStr])
                            .ToList();

                        if (higherOccurrenceStrs.Count > 0)
                        {
                            var replacement = higherOccurrenceStrs[random.Next(higherOccurrenceStrs.Count)];

                            // 替换
                            collectionB.Remove(minOccurrenceStr);
                            collectionB.Add(replacement.Key);

                            currentTotal = currentTotal - occurrenceCounts[minOccurrenceStr] + replacement.Value;
                        }
                    }
                }
                else if (currentTotal > 700)
                {
                    // 总次数太高，需要减少
                    // 尝试替换一个出现次数高的字符串为出现次数低的字符串
                    if (collectionB.Count > 0)
                    {
                        // 找到集合B中出现次数最高的字符串
                        var maxOccurrenceStr = collectionB
                            .OrderByDescending(s => occurrenceCounts[s])
                            .First();

                        // 找到不在集合B中且出现次数更低的字符串
                        var lowerOccurrenceStrs = occurrenceCounts
                            .Where(kvp => !collectionB.Contains(kvp.Key) &&
                                         kvp.Value < occurrenceCounts[maxOccurrenceStr])
                            .ToList();

                        if (lowerOccurrenceStrs.Count > 0)
                        {
                            var replacement = lowerOccurrenceStrs[random.Next(lowerOccurrenceStrs.Count)];

                            // 替换
                            collectionB.Remove(maxOccurrenceStr);
                            collectionB.Add(replacement.Key);

                            currentTotal = currentTotal - occurrenceCounts[maxOccurrenceStr] + replacement.Value;
                        }
                    }
                }

                // 检查字符串数量是否在范围内
                if (collectionB.Count < 220)
                {
                    // 需要添加字符串
                    var availableStrs = occurrenceCounts
                        .Where(kvp => !collectionB.Contains(kvp.Key))
                        .ToList();

                    if (availableStrs.Count > 0)
                    {
                        var newStr = availableStrs[random.Next(availableStrs.Count)];
                        collectionB.Add(newStr.Key);
                        currentTotal += newStr.Value;
                    }
                }
                else if (collectionB.Count > 310)
                {
                    // 需要移除字符串
                    var strToRemove = collectionB.ElementAt(random.Next(collectionB.Count));
                    collectionB.Remove(strToRemove);
                    currentTotal -= occurrenceCounts[strToRemove];
                }
            }

            // 如果达到最大尝试次数仍未满足条件，返回当前结果
            Console.WriteLine($"警告: 达到最大调整次数({maxAttempts})，当前总次数={currentTotal}，唯一数={collectionB.Count}");

            return new CollectionBResult
            {
                CollectionB = collectionB,
                TotalOccurrences = currentTotal,
                UniqueCount = collectionB.Count
            };
        }

        // 生成集合C, D, E
        public HashSet<string> GenerateCollectionE(HashSet<string> collectionADistinct, HashSet<string> collectionB)
        {
            HashSet<string> collectionE = null;
            // 生成所有可能的字符串 (000-999)
            var allPossibleStrings = Enumerable.Range(0, 1000)
                .Select(i => i.ToString("D3"))
                .ToHashSet();

            // 生成集合C (排除A中出现的数据)
            var collectionC = new HashSet<string>(allPossibleStrings.Except(collectionADistinct));

            // 计算需要从C中抽取的数量
            int neededFromC = 350 - collectionB.Count;

            if (neededFromC < 0)
            {
                // 如果B已经超过350，则从B中随机选择350个
                Random random;
                lock (_lockObject)
                {
                    random = new Random(_globalRandom.Next());
                }
                collectionE = new HashSet<string>(collectionB.OrderBy(x => random.Next()).Take(350));
                return collectionE;
            }

            if (collectionC.Count < neededFromC)
            {
                // 如果C不足，从全部字符串中补充（包括A中的）
                neededFromC = 350 - collectionB.Count;
                var remainingStrings = allPossibleStrings.Except(collectionB).ToList();

                Random random;
                lock (_lockObject)
                {
                    random = new Random(_globalRandom.Next());
                }

                collectionE = new HashSet<string>(collectionB);
                var shuffledRemaining = remainingStrings.OrderBy(x => random.Next()).Take(neededFromC);
                foreach (var item in shuffledRemaining)
                {
                    collectionE.Add(item);
                }
                return collectionE;
            }

            // 从C中随机抽取所需数量的元素
            Random rand;
            lock (_lockObject)
            {
                rand = new Random(_globalRandom.Next());
            }

            var collectionD = new HashSet<string>(
                collectionC.OrderBy(x => rand.Next()).Take(neededFromC)
            );

            // 生成集合E (B ∪ D)
            collectionE = new HashSet<string>(collectionB);
            foreach (var item in collectionD)
            {
                collectionE.Add(item);
            }

            return collectionE;
        }
        public List<BatchCollectionResult> GenerateMultipleCollectionsE(List<string> sourceList,int batchCount = 50)
        {
            var collectionADistinct = new HashSet<string>(sourceList);
            return GenerateMultipleCollectionsE(sourceList, collectionADistinct, batchCount).ToList();
        }
        // 多线程批量生成集合E
        public HashSet<BatchCollectionResult> GenerateMultipleCollectionsE(
            List<string> collectionA,
            HashSet<string> collectionADistinct,
            int batchCount)
        {
            var allResults = new HashSet<BatchCollectionResult>();
            var lockObj = new object();
            Console.WriteLine($"开始使用多线程生成 {batchCount} 个集合E...");

            // 使用Parallel.For进行多线程处理
            Parallel.For(0, batchCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                try
                {
                    // 每个线程独立生成集合B
                    var collectionBResult = GenerateCollectionB(collectionA);

                    // 生成集合E
                    var collectionE = GenerateCollectionE(collectionADistinct, collectionBResult.CollectionB);

                    // 保存结果
                    var result = new BatchCollectionResult
                    {
                        CollectionB = collectionBResult.CollectionB,
                        CollectionE = collectionE,
                        BTotalOccurrences = collectionBResult.TotalOccurrences,
                        BUniqueCount = collectionBResult.UniqueCount,
                        BatchIndex = i + 1
                    };
                    lock (lockObj)
                    {
                        allResults.Add(result);
                    }

                    if ((i + 1) % 10 == 0)
                    {
                        Console.WriteLine($"已生成 {i + 1}/{batchCount} 个集合E");
                        Console.WriteLine($"  当前集合B: 总次数={collectionBResult.TotalOccurrences}, 唯一数={collectionBResult.UniqueCount}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"生成集合E #{i + 1} 时出错: {ex.Message}");
                }
            });

            return allResults;
        }
    }

    // 结果类
    public class CollectionGenerationResult
    {
        public List<string> CollectionA { get; set; }
        public HashSet<string> CollectionADistinct { get; set; }
        public Dictionary<string, int> OccurrenceCounts { get; set; }
    }

    public class CollectionBResult
    {
        public HashSet<string> CollectionB { get; set; }
        public int TotalOccurrences { get; set; }
        public int UniqueCount { get; set; }
    }

    public class BatchCollectionResult
    {
        public HashSet<string> CollectionB { get; set; }
        public HashSet<string> CollectionE { get; set; }
        public int BTotalOccurrences { get; set; }
        public int BUniqueCount { get; set; }
        public int BatchIndex { get; set; }
    }
}