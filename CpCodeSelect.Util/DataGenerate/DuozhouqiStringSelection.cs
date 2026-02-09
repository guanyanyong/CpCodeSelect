using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.DataGenerate
{
    /// <summary>
    /// 多线程安全的字符串选择程序
    /// 功能：
    /// 1. 生成固定的集合A（1620个字符串，可重复）
    /// 2. 在相同A下生成50个不同的集合E
    /// 3. 所有集合E满足：350个不重复字符串，来自B(去重)+C的补集
    /// 4. B满足：总出现次数620-660，不同字符串数量在计算范围内
    /// </summary>
    public class DuozhouqiStringSelection
    {
        #region 线程安全的随机数生成器

        // 每个线程独立的Random实例，确保线程安全
        private static readonly ThreadLocal<Random> threadLocalRandom =
            new ThreadLocal<Random>(() =>
            {
                // 使用线程ID、时间戳和Guid创建唯一种子
                int seed = Thread.CurrentThread.ManagedThreadId ^
                          Environment.TickCount ^
                          Guid.NewGuid().GetHashCode();
                return new Random(seed);
            });

        // 全局种子计数器（用于需要新Random实例的场景）
        //private static int globalSeedCounter = 0;

        #endregion

        #region 主程序入口

        public static List<BatchResult> GetBatchResults(List<string> sourceList)
        {
            Console.WriteLine("=== 多线程字符串选择系统 ===\n");
            Console.WriteLine("功能说明：");
            Console.WriteLine("1. 生成固定的集合A（1620个字符串，可重复）");
            Console.WriteLine("2. 在相同A下生成50个不同的集合E");
            Console.WriteLine("3. 每个E包含350个不重复字符串（000-999）");
            Console.WriteLine("4. B集合满足：总出现次数620-660，不同字符串数量在计算范围内\n");

            // 生成所有可能的字符串 (000-999)
            var allStrings = Enumerable.Range(0, 1000)
                .Select(i => i.ToString("D3"))
                .ToArray();

            // 记录总执行时间
            //Stopwatch totalSw = Stopwatch.StartNew();

            try
            {
                List<string> setA = null;
                Dictionary<string, int> freqDict = null;
                HashSet<string> uniqueA = null;
                int K = 0;

                //初始化集合A

                if (sourceList != null && sourceList.Count > 0)
                {
                    setA = sourceList;
                    freqDict = new Dictionary<string, int>();
                    uniqueA = new HashSet<string>();
                    foreach (var record in setA)
                    {
                        if (freqDict.Keys.Contains(record))
                        {
                            freqDict[record]++;
                        }
                        else
                        {
                            freqDict.Add(record, 1);
                        }
                    }

                    foreach(var record in freqDict.Keys)
                    {
                        uniqueA.Add(record);
                    }
                    K = uniqueA.Count;
                }
                else
                {
                    // 1. 生成固定的集合A
                    (setA, freqDict, uniqueA, K) = GenerateSetA(1620, allStrings, GetThreadRandom());
                }
                Console.WriteLine("✅ 固定集合A生成成功");
                Console.WriteLine($"   集合A大小: {setA.Count} (含重复)");
                Console.WriteLine($"   [A减]大小(K): {K}");
                Console.WriteLine($"   A中不同字符串数量: {uniqueA.Count}\n");

                // 2. 计算B中不同字符串数量的范围
                var (minUniqueB, maxUniqueB) = CalculateBUniqueRange(K);
                Console.WriteLine("📊 B集合参数计算:");
                Console.WriteLine($"   B中不同字符串数量范围: {minUniqueB} - {maxUniqueB}");
                Console.WriteLine($"   B的总出现次数范围: 620 - 660\n");

                // 3. 生成集合C（A中未出现的字符串）
                var setC = GenerateSetC(uniqueA, allStrings);
                Console.WriteLine($"✅ 集合C生成成功: {setC.Count}个字符串\n");

                // 4. 并行生成50个不同的E集合
                int targetBatchCount = 50;
                Console.WriteLine($"🚀 开始并行生成 {targetBatchCount} 个不同的E集合...\n");

                var results = GenerateMultipleEWithSameA(
                    setA, freqDict, uniqueA, setC,
                    minUniqueB, maxUniqueB, targetBatchCount, allStrings);

                //totalSw.Stop();

                // 5. 显示结果统计
                //DisplayResults(results, setA, K, totalSw.ElapsedMilliseconds);

                // 6. 保存结果到文件
                //SaveResultsToFiles(results, setA);

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 程序执行出错: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }

            //Console.WriteLine("\n🎉 程序执行完成！按任意键退出...");
            //Console.ReadKey();
            return null;
        }

        #endregion

        #region 核心数据结构

        /// <summary>
        /// 批次结果类
        /// </summary>
        public class BatchResult
        {
            public int BatchId { get; set; }
            public List<string> SubsetB { get; set; }
            public HashSet<string> SelectedStrings { get; set; }
            public List<string> SetD { get; set; }
            public HashSet<string> SetE { get; set; }
            public int TotalB { get; set; }
            public int StrategyB { get; set; }
            public int StrategyD { get; set; }
            public int ThreadId { get; set; }
            public string EHash { get; set; }
        }

        #endregion

        #region 核心算法实现

        /// <summary>
        /// 获取当前线程的Random实例（线程安全）
        /// </summary>
        private static Random GetThreadRandom()
        {
            return threadLocalRandom.Value;
        }

        /// <summary>
        /// 生成集合A（1620个字符串，可重复）
        /// </summary>
        private static (List<string> setA, Dictionary<string, int> freqDict, HashSet<string> uniqueA, int K)
            GenerateSetA(int count, string[] allStrings, Random random)
        {
            var setA = new List<string>(count);
            var freqDict = new Dictionary<string, int>();

            // 1. 确定不同字符串数量K（650-750之间，确保有解）
            int K = random.Next(650, 751);
            var selectedUnique = allStrings.OrderBy(x => random.Next()).Take(K).ToList();

            // 2. 设计合理的频率分布
            var frequencies = new Dictionary<string, int>();
            int remaining = count;

            // 每个字符串至少出现1次
            foreach (var str in selectedUnique)
            {
                frequencies[str] = 1;
                remaining--;
            }

            // 3. 将字符串分组，实现不同的频率分布
            int groupCount = 5; // 5个组，创建不同频率模式
            var groups = new List<List<string>>();
            for (int i = 0; i < groupCount; i++)
                groups.Add(new List<string>());

            // 分配字符串到组
            for (int i = 0; i < selectedUnique.Count; i++)
                groups[i % groupCount].Add(selectedUnique[i]);

            // 4. 为每组分配额外次数
            foreach (var group in groups)
            {
                if (remaining <= 0) break;

                int extra = random.Next(group.Count * 2, group.Count * 5 + 1);
                extra = Math.Min(extra, remaining);

                for (int j = 0; j < extra; j++)
                {
                    string str = group[random.Next(group.Count)];
                    frequencies[str]++;
                    remaining--;
                }
            }

            // 5. 剩余次数随机分配
            while (remaining > 0)
            {
                string str = selectedUnique[random.Next(K)];
                frequencies[str]++;
                remaining--;
            }

            // 6. 生成集合A并计算频率
            foreach (var kvp in frequencies)
            {
                for (int i = 0; i < kvp.Value; i++)
                    setA.Add(kvp.Key);

                freqDict[kvp.Key] = kvp.Value;
            }

            // 7. 打乱顺序
            setA = setA.OrderBy(x => random.Next()).ToList();

            return (setA, freqDict, new HashSet<string>(selectedUnique), K);
        }

        /// <summary>
        /// 计算B中不同字符串数量的范围
        /// </summary>
        private static (int min, int max) CalculateBUniqueRange(int K)
        {
            double factor1 = 0.8, factor2 = 0.6;
            int minUniqueB = Math.Max(1, (int)Math.Ceiling(350 - (1000 - K) * factor1));
            int maxUniqueB = Math.Min(K, (int)Math.Floor(350 - (1000 - K) * factor2));

            return (minUniqueB, maxUniqueB);
        }

        /// <summary>
        /// 生成集合C（A中未出现的字符串）
        /// </summary>
        private static List<string> GenerateSetC(HashSet<string> uniqueA, string[] allStrings)
        {
            return allStrings.Where(s => !uniqueA.Contains(s)).ToList();
        }

        /// <summary>
        /// 在相同A下生成多个不同的E集合
        /// </summary>
        private static List<BatchResult> GenerateMultipleEWithSameA(
            List<string> setA,
            Dictionary<string, int> freqDict,
            HashSet<string> uniqueA,
            List<string> setC,
            int minUniqueB,
            int maxUniqueB,
            int targetCount,
            string[] allStrings)
        {
            // 用于收集结果的并发集合
            var results = new ConcurrentBag<BatchResult>();

            // 用于去重的并发字典（记录已生成的E哈希值）
            var generatedHashes = new ConcurrentDictionary<string, bool>();

            // 进度跟踪
            int completedBatches = 0;
            int totalAttempts = 0;
            object progressLock = new object();

            // 并行生成
            Parallel.For(0, targetCount * 5, new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, (i, state) =>
            {
                // 如果已经生成足够数量，提前退出
                if (results.Count >= targetCount)
                {
                    state.Break();
                    return;
                }

                // 获取当前线程的Random实例
                var random = GetThreadRandom();

                // 增加尝试计数
                Interlocked.Increment(ref totalAttempts);

                try
                {
                    // 随机选择策略组合
                    int strategyB = random.Next(1, 8);  // 7种B选择策略
                    int strategyD = random.Next(1, 8);  // 7种D抽取策略

                    // 生成批次
                    var result = GenerateSingleBatch(
                        setA, freqDict, uniqueA, setC,
                        minUniqueB, maxUniqueB,
                        strategyB, strategyD, random);

                    if (result != null)
                    {
                        // 计算E的哈希值
                        string eHash = CalculateEHash(result.SetE);

                        // 检查是否重复
                        if (generatedHashes.TryAdd(eHash, true))
                        {
                            result.BatchId = results.Count + 1;
                            result.ThreadId = Thread.CurrentThread.ManagedThreadId;
                            result.EHash = eHash;
                            result.StrategyB = strategyB;
                            result.StrategyD = strategyD;

                            results.Add(result);

                            // 更新进度
                            lock (progressLock)
                            {
                                completedBatches = results.Count;
                                if (completedBatches % 5 == 0 || completedBatches == targetCount)
                                {
                                    Console.WriteLine($"  已生成 {completedBatches}/{targetCount} 个E集合");
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略单个批次的错误，继续尝试
                }

                // 如果已经生成足够数量，提前退出
                if (results.Count >= targetCount)
                {
                    state.Break();
                }
            });

            Console.WriteLine($"\n生成完成: 成功 {results.Count}/{targetCount} 个不同的E集合");
            Console.WriteLine($"总尝试次数: {totalAttempts}");

            return results.OrderBy(r => r.BatchId).ToList();
        }

        /// <summary>
        /// 生成单个批次
        /// </summary>
        private static BatchResult GenerateSingleBatch(
            List<string> setA,
            Dictionary<string, int> freqDict,
            HashSet<string> uniqueA,
            List<string> setC,
            int minUniqueB,
            int maxUniqueB,
            int strategyB,
            int strategyD,
            Random random)
        {
            // 1. 查找子集B
            var (selectedStrings, totalCount) = FindSubsetB(
                freqDict, minUniqueB, maxUniqueB, 620, 660, strategyB, random);

            if (selectedStrings == null) return null;

            // 2. 生成子集B（包含选中字符串的所有出现次数）
            var subsetB = GenerateSubsetB(setA, selectedStrings);

            // 3. 计算需要从C中取的数量
            int needFromC = 350 - selectedStrings.Count;

            if (needFromC > setC.Count || needFromC < 0) return null;

            // 4. 从C中抽取形成D
            var setD = RandomSelectFromC(setC, needFromC, strategyD, random);

            // 5. 生成集合E
            var setE = GenerateSetE(selectedStrings, setD);

            // 6. 验证结果
            if (!ValidateResults(setA, subsetB, setC, setD, setE)) return null;

            return new BatchResult
            {
                SubsetB = subsetB,
                SelectedStrings = selectedStrings,
                SetD = setD,
                SetE = setE,
                TotalB = totalCount
            };
        }

        #endregion

        #region B选择策略（7种）

        /// <summary>
        /// 查找子集B（根据策略选择）
        /// </summary>
        private static (HashSet<string> selectedStrings, int totalCount)
            FindSubsetB(
                Dictionary<string, int> freqDict,
                int minUniqueCount,
                int maxUniqueCount,
                int minTotalCount,
                int maxTotalCount,
                int strategy,
                Random random)
        {
            var freqList = freqDict.ToList();

            switch (strategy)
            {
                case 1: return Strategy_HighFrequencyFirst(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                case 2: return Strategy_LowFrequencyFirst(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                case 3: return Strategy_IntervalSelection(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                case 4: return Strategy_RandomWalk(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                case 5: return Strategy_BlockSelection(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                case 6: return Strategy_SymmetricSelection(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                case 7: return Strategy_GradualAdjustment(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
                default: return Strategy_HighFrequencyFirst(freqList, minUniqueCount, maxUniqueCount, minTotalCount, maxTotalCount, random);
            }
        }

        // 策略1：高频优先
        private static (HashSet<string>, int) Strategy_HighFrequencyFirst(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderByDescending(kvp => kvp.Value).ToList();

            for (int uniqueCount = minUniqueCount; uniqueCount <= maxUniqueCount; uniqueCount++)
            {
                var candidate = new HashSet<string>();
                int total = 0;

                // 选择高频字符串
                for (int i = 0; i < uniqueCount; i++)
                {
                    candidate.Add(sorted[i].Key);
                    total += sorted[i].Value;
                }

                if (total >= minTotalCount && total <= maxTotalCount)
                    return (candidate, total);
            }

            return (null, 0);
        }

        // 策略2：低频优先
        private static (HashSet<string>, int) Strategy_LowFrequencyFirst(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderBy(kvp => kvp.Value).ToList();

            for (int uniqueCount = minUniqueCount; uniqueCount <= maxUniqueCount; uniqueCount++)
            {
                var candidate = new HashSet<string>();
                int total = 0;

                // 选择低频字符串
                for (int i = 0; i < uniqueCount; i++)
                {
                    candidate.Add(sorted[i].Key);
                    total += sorted[i].Value;
                }

                // 如果总数太小，添加一些高频的
                if (total < minTotalCount)
                {
                    var highFreq = sorted.OrderByDescending(kvp => kvp.Value)
                        .Where(kvp => !candidate.Contains(kvp.Key))
                        .ToList();

                    foreach (var kvp in highFreq)
                    {
                        if (total >= maxTotalCount || candidate.Count >= maxUniqueCount) break;

                        candidate.Add(kvp.Key);
                        total += kvp.Value;
                    }
                }

                if (total >= minTotalCount && total <= maxTotalCount &&
                    candidate.Count >= minUniqueCount && candidate.Count <= maxUniqueCount)
                    return (candidate, total);
            }

            return (null, 0);
        }

        // 策略3：间隔选择
        private static (HashSet<string>, int) Strategy_IntervalSelection(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderBy(kvp => kvp.Value).ToList();
            int n = sorted.Count;

            int interval = random.Next(2, 7); // 间隔2-6

            for (int uniqueCount = minUniqueCount; uniqueCount <= maxUniqueCount; uniqueCount++)
            {
                var candidate = new HashSet<string>();
                int total = 0;
                int selected = 0;

                // 从不同起点开始
                int start = random.Next(interval);

                for (int i = start; i < n && selected < uniqueCount; i += interval)
                {
                    candidate.Add(sorted[i].Key);
                    total += sorted[i].Value;
                    selected++;
                }

                // 如果不够，补充
                if (selected < uniqueCount)
                {
                    for (int i = 0; i < n && selected < uniqueCount; i++)
                    {
                        if (!candidate.Contains(sorted[i].Key))
                        {
                            candidate.Add(sorted[i].Key);
                            total += sorted[i].Value;
                            selected++;
                        }
                    }
                }

                // 调整总数
                candidate = AdjustTotal(candidate, sorted, total, minTotalCount, maxTotalCount, minUniqueCount, maxUniqueCount);
                total = candidate.Sum(s => sorted.First(k => k.Key == s).Value);

                if (candidate.Count >= minUniqueCount && candidate.Count <= maxUniqueCount &&
                    total >= minTotalCount && total <= maxTotalCount)
                    return (candidate, total);
            }

            return (null, 0);
        }

        // 策略4：随机游走
        private static (HashSet<string>, int) Strategy_RandomWalk(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderBy(kvp => kvp.Value).ToList();
            int n = sorted.Count;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                int uniqueCount = random.Next(minUniqueCount, maxUniqueCount + 1);
                var candidate = new HashSet<string>();
                int total = 0;

                // 随机起点
                int current = random.Next(n);

                while (candidate.Count < uniqueCount)
                {
                    // 随机游走
                    int step = random.Next(-3, 4);
                    current = (current + step + n) % n;

                    if (!candidate.Contains(sorted[current].Key))
                    {
                        candidate.Add(sorted[current].Key);
                        total += sorted[current].Value;
                    }
                }

                if (total >= minTotalCount && total <= maxTotalCount)
                    return (candidate, total);
            }

            return (null, 0);
        }

        // 策略5：分块选择
        private static (HashSet<string>, int) Strategy_BlockSelection(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderBy(kvp => kvp.Value).ToList();
            int n = sorted.Count;

            // 分块数量
            int blockCount = random.Next(3, 7); // 3-6块
            int blockSize = n / blockCount;

            for (int uniqueCount = minUniqueCount; uniqueCount <= maxUniqueCount; uniqueCount++)
            {
                var candidate = new HashSet<string>();
                int total = 0;

                // 从每块中选择一定数量
                int perBlock = uniqueCount / blockCount;
                int remainder = uniqueCount % blockCount;

                for (int block = 0; block < blockCount; block++)
                {
                    int start = block * blockSize;
                    int end = (block == blockCount - 1) ? n : start + blockSize;
                    int takeFromBlock = perBlock + (block < remainder ? 1 : 0);

                    if (takeFromBlock <= 0) continue;

                    // 从块中选择
                    var blockItems = sorted.Skip(start).Take(end - start).ToList();
                    int offset = random.Next(Math.Max(1, blockItems.Count - takeFromBlock + 1));

                    for (int i = 0; i < takeFromBlock && offset + i < blockItems.Count; i++)
                    {
                        var item = blockItems[offset + i];
                        candidate.Add(item.Key);
                        total += item.Value;
                    }
                }

                // 调整
                candidate = AdjustTotal(candidate, sorted, total, minTotalCount, maxTotalCount, minUniqueCount, maxUniqueCount);
                total = candidate.Sum(s => sorted.First(k => k.Key == s).Value);

                if (candidate.Count >= minUniqueCount && candidate.Count <= maxUniqueCount &&
                    total >= minTotalCount && total <= maxTotalCount)
                    return (candidate, total);
            }

            return (null, 0);
        }

        // 策略6：对称选择
        private static (HashSet<string>, int) Strategy_SymmetricSelection(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderBy(kvp => kvp.Value).ToList();
            int n = sorted.Count;

            for (int uniqueCount = minUniqueCount; uniqueCount <= maxUniqueCount; uniqueCount++)
            {
                var candidate = new HashSet<string>();
                int total = 0;

                // 从两端对称选择
                int fromEachEnd = uniqueCount / 2;
                int fromMiddle = uniqueCount % 2;

                // 从高频端选择
                for (int i = 0; i < fromEachEnd; i++)
                {
                    int idx = n - 1 - (i + random.Next(10)) % (n / 2);
                    candidate.Add(sorted[idx].Key);
                }

                // 从低频端选择
                for (int i = 0; i < fromEachEnd; i++)
                {
                    int idx = (i + random.Next(10)) % (n / 2);
                    candidate.Add(sorted[idx].Key);
                }

                // 从中间选择
                if (fromMiddle > 0)
                {
                    int middleIndex = n / 2 + random.Next(n / 4);
                    candidate.Add(sorted[middleIndex].Key);
                }

                total = candidate.Sum(s => sorted.First(k => k.Key == s).Value);

                // 调整
                candidate = AdjustTotal(candidate, sorted, total, minTotalCount, maxTotalCount, minUniqueCount, maxUniqueCount);
                total = candidate.Sum(s => sorted.First(k => k.Key == s).Value);

                if (candidate.Count >= minUniqueCount && candidate.Count <= maxUniqueCount &&
                    total >= minTotalCount && total <= maxTotalCount)
                    return (candidate, total);
            }

            return (null, 0);
        }

        // 策略7：渐进调整
        private static (HashSet<string>, int) Strategy_GradualAdjustment(
            List<KeyValuePair<string, int>> freqList,
            int minUniqueCount, int maxUniqueCount,
            int minTotalCount, int maxTotalCount,
            Random random)
        {
            var sorted = freqList.OrderBy(kvp => kvp.Value).ToList();

            // 基准选择（高频）
            int baseUniqueCount = (minUniqueCount + maxUniqueCount) / 2;
            var candidate = new HashSet<string>();
            int total = 0;

            for (int i = 0; i < baseUniqueCount; i++)
            {
                candidate.Add(sorted[i].Key);
                total += sorted[i].Value;
            }

            // 进行调整
            int adjustment = random.Next(1, 6);

            if (adjustment < 3)
            {
                // 替换一些高频为中频
                int replaceCount = Math.Min(adjustment, candidate.Count / 3);
                for (int i = 0; i < replaceCount && i < candidate.Count; i++)
                {
                    string toReplace = candidate.ElementAt(i);
                    total -= sorted.First(k => k.Key == toReplace).Value;
                    candidate.Remove(toReplace);

                    // 选择中频
                    int midIndex = baseUniqueCount + (i * 3) % (sorted.Count - baseUniqueCount);
                    candidate.Add(sorted[midIndex].Key);
                    total += sorted[midIndex].Value;
                }
            }
            else
            {
                // 添加一些低频
                int addCount = adjustment - 2;
                for (int i = 0; i < addCount && candidate.Count < maxUniqueCount; i++)
                {
                    int lowIndex = sorted.Count - 1 - (i * 2) % (sorted.Count / 3);
                    if (!candidate.Contains(sorted[lowIndex].Key))
                    {
                        candidate.Add(sorted[lowIndex].Key);
                        total += sorted[lowIndex].Value;
                    }
                }
            }

            // 调整总数
            candidate = AdjustTotal(candidate, sorted, total, minTotalCount, maxTotalCount, minUniqueCount, maxUniqueCount);
            total = candidate.Sum(s => sorted.First(k => k.Key == s).Value);

            if (candidate.Count >= minUniqueCount && candidate.Count <= maxUniqueCount &&
                total >= minTotalCount && total <= maxTotalCount)
                return (candidate, total);

            return (null, 0);
        }

        // 调整总数以满足范围
        private static HashSet<string> AdjustTotal(
            HashSet<string> candidate,
            List<KeyValuePair<string, int>> sorted,
            int currentTotal,
            int minTotal,
            int maxTotal,
            int minUnique,
            int maxUnique)
        {
            if (currentTotal >= minTotal && currentTotal <= maxTotal)
                return candidate;

            var result = new HashSet<string>(candidate);
            int total = currentTotal;

            // 如果总数太小，添加字符串
            if (total < minTotal)
            {
                var available = sorted.Where(kvp => !result.Contains(kvp.Key))
                    .OrderByDescending(kvp => kvp.Value)
                    .ToList();

                foreach (var kvp in available)
                {
                    if (total >= maxTotal || result.Count >= maxUnique) break;

                    result.Add(kvp.Key);
                    total += kvp.Value;
                }
            }

            // 如果总数太大，移除字符串
            if (total > maxTotal)
            {
                var toRemove = result.OrderBy(s => sorted.First(k => k.Key == s).Value).ToList();

                foreach (var str in toRemove)
                {
                    if (total <= maxTotal || result.Count <= minUnique) break;

                    result.Remove(str);
                    total -= sorted.First(k => k.Key == str).Value;
                }
            }

            return result;
        }

        #endregion

        #region D抽取策略（7种）

        /// <summary>
        /// 从C中随机抽取形成D
        /// </summary>
        private static List<string> RandomSelectFromC(
            List<string> setC, int count, int strategy, Random random)
        {
            if (count <= 0) return new List<string>();

            switch (strategy)
            {
                case 1: return Strategy_CompletelyRandom(setC, count, random);
                case 2: return Strategy_PartitionRandom(setC, count, random);
                case 3: return Strategy_IntervalSampling(setC, count, random);
                case 4: return Strategy_SymmetricSampling(setC, count, random);
                case 5: return Strategy_BlockRandom(setC, count, random);
                case 6: return Strategy_NumericFeature(setC, count, random);
                case 7: return Strategy_PatternSampling(setC, count, random);
                default: return Strategy_CompletelyRandom(setC, count, random);
            }
        }

        // 策略1：完全随机
        private static List<string> Strategy_CompletelyRandom(List<string> setC, int count, Random random)
        {
            return setC.OrderBy(x => random.Next()).Take(count).ToList();
        }

        // 策略2：分区随机
        private static List<string> Strategy_PartitionRandom(List<string> setC, int count, Random random)
        {
            int partitions = random.Next(2, 6); // 2-5个分区
            var result = new List<string>();
            var shuffled = setC.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < count; i++)
            {
                int partition = i % partitions;
                int start = partition * (shuffled.Count / partitions);
                int end = (partition == partitions - 1) ? shuffled.Count : start + (shuffled.Count / partitions);

                if (start >= end) continue;

                int idx = random.Next(start, end);
                result.Add(shuffled[idx]);
            }

            return result.Take(count).ToList();
        }

        // 策略3：间隔采样
        private static List<string> Strategy_IntervalSampling(List<string> setC, int count, Random random)
        {
            var all = setC.OrderBy(x => random.Next()).ToList();
            var sampled = new List<string>();

            if (count >= all.Count) return all;

            double step = (double)all.Count / count;
            int offset = random.Next((int)step);

            for (int i = 0; i < count; i++)
            {
                int idx = (int)(offset + i * step) % all.Count;
                sampled.Add(all[idx]);
            }

            return sampled.Distinct().Take(count).ToList();
        }

        // 策略4：对称采样
        private static List<string> Strategy_SymmetricSampling(List<string> setC, int count, Random random)
        {
            var sorted = setC.OrderBy(s => s).ToList();
            var symmetric = new List<string>();

            for (int i = 0; i < count; i++)
            {
                if (i % 2 == 0)
                {
                    // 从前端取
                    int idx = (i / 2 + random.Next(10)) % (sorted.Count / 2);
                    symmetric.Add(sorted[idx]);
                }
                else
                {
                    // 从后端取
                    int idx = sorted.Count - 1 - ((i / 2 + random.Next(10)) % (sorted.Count / 2));
                    symmetric.Add(sorted[idx]);
                }
            }

            return symmetric;
        }

        // 策略5：块内随机
        private static List<string> Strategy_BlockRandom(List<string> setC, int count, Random random)
        {
            int blockSize = Math.Max(1, setC.Count / 10);
            var blocks = new List<List<string>>();

            for (int i = 0; i < setC.Count; i += blockSize)
            {
                blocks.Add(setC.Skip(i).Take(blockSize).ToList());
            }

            var result = new List<string>();
            while (result.Count < count && blocks.Count > 0)
            {
                int blockIdx = random.Next(blocks.Count);
                if (blocks[blockIdx].Count == 0)
                {
                    blocks.RemoveAt(blockIdx);
                    continue;
                }

                int itemIdx = random.Next(blocks[blockIdx].Count);
                result.Add(blocks[blockIdx][itemIdx]);
                blocks[blockIdx].RemoveAt(itemIdx);
            }

            return result;
        }

        // 策略6：基于数值特征
        private static List<string> Strategy_NumericFeature(List<string> setC, int count, Random random)
        {
            var numeric = setC.Select(s => new { Str = s, Num = int.Parse(s) }).ToList();

            // 按数值特征分组选择
            var groups = numeric.GroupBy(x => x.Num % 10).ToList();
            var result = new List<string>();

            foreach (var group in groups.OrderBy(g => random.Next()))
            {
                int takeFromGroup = Math.Max(1, count / groups.Count);
                var items = group.OrderBy(x => random.Next()).Take(takeFromGroup).Select(x => x.Str).ToList();
                result.AddRange(items);

                if (result.Count >= count) break;
            }

            return result.Take(count).ToList();
        }

        // 策略7：模式采样
        private static List<string> Strategy_PatternSampling(List<string> setC, int count, Random random)
        {
            var patterns = new List<string>();
            int patternLength = random.Next(2, 5);

            for (int i = 0; i < count; i++)
            {
                int baseIdx = (i * patternLength) % Math.Max(1, setC.Count - patternLength);
                patterns.Add(setC[baseIdx]);
            }

            return patterns;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 生成子集B（包含选中字符串的所有出现次数）
        /// </summary>
        private static List<string> GenerateSubsetB(List<string> setA, HashSet<string> selectedStrings)
        {
            return setA.Where(s => selectedStrings.Contains(s)).ToList();
        }

        /// <summary>
        /// 生成集合E
        /// </summary>
        private static HashSet<string> GenerateSetE(HashSet<string> selectedStrings, List<string> setD)
        {
            var setE = new HashSet<string>(selectedStrings);
            foreach (var str in setD) setE.Add(str);
            return setE;
        }

        /// <summary>
        /// 计算E的哈希值
        /// </summary>
        private static string CalculateEHash(HashSet<string> setE)
        {
            // 排序后连接字符串
            var sorted = setE.OrderBy(s => s).ToList();
            return string.Join(",", sorted);
        }

        /// <summary>
        /// 验证结果
        /// </summary>
        private static bool ValidateResults(
            List<string> setA,
            List<string> subsetB,
            List<string> setC,
            List<string> setD,
            HashSet<string> setE)
        {
            // 1. 检查E大小
            if (setE.Count != 350) return false;

            // 2. 检查B总个数
            if (subsetB.Count < 620 || subsetB.Count > 660) return false;

            // 3. 检查B中每个选中的字符串都包含了在A中的所有出现次数
            var bFreq = subsetB.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());
            var aFreq = setA.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());

            foreach (var kvp in bFreq)
            {
                if (kvp.Value != aFreq[kvp.Key]) return false;
            }

            // 4. 检查D中的字符串都不在A中
            var stringsInA = new HashSet<string>(setA);
            if (setD.Any(s => stringsInA.Contains(s))) return false;

            return true;
        }

        #endregion

        #region 结果显示与保存

        /// <summary>
        /// 显示结果统计
        /// </summary>
        private static void DisplayResults(
            List<BatchResult> results,
            List<string> setA,
            int K,
            long elapsedMs)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("📈 生成结果统计");
            Console.WriteLine(new string('=', 60));

            Console.WriteLine($"集合A大小: {setA.Count} (含重复)");
            Console.WriteLine($"[A减]大小(K): {K}");
            Console.WriteLine($"成功生成E集合数量: {results.Count}");
            Console.WriteLine($"总耗时: {elapsedMs}ms\n");

            if (results.Count == 0) return;

            // 1. 策略使用统计
            var strategyStats = results
                .GroupBy(r => new { r.StrategyB, r.StrategyD })
                .Select(g => new
                {
                    Strategy = $"B{g.Key.StrategyB}-D{g.Key.StrategyD}",
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            Console.WriteLine("🔄 策略使用统计:");
            foreach (var stat in strategyStats.Take(10))
            {
                Console.WriteLine($"  {stat.Strategy}: {stat.Count}次");
            }
            if (strategyStats.Count > 10)
                Console.WriteLine($"  ... 还有{strategyStats.Count - 10}种策略组合\n");

            // 2. 线程分布
            var threadStats = results
                .GroupBy(r => r.ThreadId)
                .Select(g => new { ThreadId = g.Key, Count = g.Count() })
                .OrderByDescending(t => t.Count)
                .ToList();

            Console.WriteLine("🧵 线程分布:");
            foreach (var stat in threadStats)
            {
                Console.WriteLine($"  线程{stat.ThreadId}: {stat.Count}个批次");
            }
            Console.WriteLine();

            // 3. B集合统计
            var bSizes = results.Select(r => r.TotalB).ToList();
            var uniqueBCounts = results.Select(r => r.SelectedStrings.Count).ToList();

            Console.WriteLine("📊 B集合统计:");
            Console.WriteLine($"  总出现次数范围: {bSizes.Min()} - {bSizes.Max()}");
            Console.WriteLine($"  平均出现次数: {bSizes.Average():F1}");
            Console.WriteLine($"  不同字符串数量范围: {uniqueBCounts.Min()} - {uniqueBCounts.Max()}");
            Console.WriteLine($"  平均不同字符串数: {uniqueBCounts.Average():F1}\n");

            // 4. E集合差异度分析
            Console.WriteLine("🔍 E集合差异度分析:");

            // 计算所有E集合的并集
            var allEStrings = results.SelectMany(r => r.SetE).Distinct().ToList();
            Console.WriteLine($"  所有E集合覆盖的字符串总数: {allEStrings.Count}/1000");
            Console.WriteLine($"  总体覆盖率: {allEStrings.Count / 1000.0 * 100:F1}%");

            // 计算平均Jaccard相似度
            if (results.Count > 1)
            {
                double totalSimilarity = 0;
                int comparisonCount = 0;

                for (int i = 0; i < Math.Min(20, results.Count); i++)
                {
                    for (int j = i + 1; j < Math.Min(20, results.Count); j++)
                    {
                        var set1 = results[i].SetE;
                        var set2 = results[j].SetE;

                        int intersection = set1.Intersect(set2).Count();
                        int union = set1.Union(set2).Count();

                        double similarity = union > 0 ? (double)intersection / union : 0;
                        totalSimilarity += similarity;
                        comparisonCount++;
                    }
                }

                double avgSimilarity = comparisonCount > 0 ? totalSimilarity / comparisonCount : 0;
                Console.WriteLine($"  E集合间平均相似度: {avgSimilarity:F3} (0=完全不同, 1=完全相同)");
                Console.WriteLine($"  平均差异度: {1 - avgSimilarity:F3}");
            }

            // 5. 显示前5个批次详情
            Console.WriteLine("\n📋 前5个E集合详情:");
            for (int i = 0; i < Math.Min(5, results.Count); i++)
            {
                var result = results[i];
                Console.WriteLine($"  E{i + 1:00}: 策略B{result.StrategyB}-D{result.StrategyD}, " +
                                 $"B大小={result.TotalB}, B不同={result.SelectedStrings.Count}, " +
                                 $"线程{result.ThreadId}");
            }
        }

        /// <summary>
        /// 保存结果到文件
        /// </summary>
        private static void SaveResultsToFiles(List<BatchResult> results, List<string> setA)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("\n⚠️ 没有结果可保存");
                return;
            }

            try
            {
                // 创建输出目录
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string baseDir = $"StringSelectionResults_{timestamp}";
                System.IO.Directory.CreateDirectory(baseDir);

                Console.WriteLine($"\n💾 正在保存结果到目录: {baseDir}");

                // 1. 保存固定的集合A
                string setAPath = $"{baseDir}/0_Fixed_SetA.txt";
                System.IO.File.WriteAllLines(setAPath, setA);

                // 2. 保存A的元数据
                var aMeta = new List<string>
                {
                    "=== 固定集合A信息 ===",
                    $"生成时间: {DateTime.Now}",
                    $"总字符串数: {setA.Count} (含重复)",
                    $"不同字符串数: {new HashSet<string>(setA).Count}",
                    $"",
                    "出现频率最高的10个字符串:"
                };

                var top10 = setA.GroupBy(s => s)
                    .Select(g => new { String = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10);

                foreach (var item in top10)
                {
                    aMeta.Add($"  {item.String}: {item.Count}次");
                }

                System.IO.File.WriteAllLines($"{baseDir}/0_SetA_Metadata.txt", aMeta);

                // 3. 保存每个E集合
                foreach (var result in results)
                {
                    string batchDir = $"{baseDir}/E{result.BatchId:00}_B{result.StrategyB}D{result.StrategyD}";
                    System.IO.Directory.CreateDirectory(batchDir);

                    // 保存B、D、E
                    System.IO.File.WriteAllLines($"{batchDir}/SubsetB.txt", result.SubsetB);
                    System.IO.File.WriteAllLines($"{batchDir}/SetD.txt", result.SetD);
                    System.IO.File.WriteAllLines($"{batchDir}/SetE.txt", result.SetE.OrderBy(s => s));

                    // 保存元数据
                    var metaLines = new List<string>
                    {
                        $"=== E集合 {result.BatchId:00} 信息 ===",
                        $"生成时间: {DateTime.Now}",
                        $"生成线程: {result.ThreadId}",
                        $"使用策略: B{result.StrategyB}-D{result.StrategyD}",
                        $"",
                        $"子集B:",
                        $"  总字符串数: {result.SubsetB.Count}",
                        $"  不同字符串数: {result.SelectedStrings.Count}",
                        $"",
                        $"集合D: {result.SetD.Count}个字符串",
                        $"",
                        $"集合E: {result.SetE.Count}个不重复字符串",
                        $"",
                        $"集合E内容 (前50个，排序后):",
                        string.Join(" ", result.SetE.OrderBy(s => s).Take(50))
                    };

                    System.IO.File.WriteAllLines($"{batchDir}/Metadata.txt", metaLines);
                }

                // 4. 保存汇总信息
                var summaryLines = new List<string>
                {
                    "=== 多线程字符串选择系统 - 结果汇总 ===",
                    $"生成时间: {DateTime.Now}",
                    $"",
                    $"集合A信息:",
                    $"  总字符串数: {setA.Count}",
                    $"  不同字符串数: {new HashSet<string>(setA).Count}",
                    $"",
                    $"生成的E集合数量: {results.Count}",
                    $"",
                    $"各E集合信息:"
                };

                foreach (var result in results)
                {
                    summaryLines.Add(
                        $"E{result.BatchId:00}: " +
                        $"策略B{result.StrategyB}-D{result.StrategyD}, " +
                        $"B大小={result.SubsetB.Count}, " +
                        $"B不同={result.SelectedStrings.Count}, " +
                        $"E大小={result.SetE.Count}, " +
                        $"线程={result.ThreadId}");
                }

                System.IO.File.WriteAllLines($"{baseDir}/00_Summary.txt", summaryLines);

                // 5. 保存所有E集合的合并
                var allEStrings = results.SelectMany(r => r.SetE).Distinct().OrderBy(s => s).ToList();
                System.IO.File.WriteAllLines($"{baseDir}/All_E_Strings.txt", allEStrings);

                // 6. 保存覆盖情况分析
                var coverageLines = new List<string>
                {
                    "=== 覆盖情况分析 ===",
                    $"",
                    $"所有E集合覆盖的字符串总数: {allEStrings.Count}/1000",
                    $"总体覆盖率: {allEStrings.Count / 1000.0 * 100:F1}%",
                    $"",
                    $"未覆盖的字符串 ({1000 - allEStrings.Count}个):"
                };

                var allStrings = Enumerable.Range(0, 1000).Select(i => i.ToString("D3")).ToList();
                var uncovered = allStrings.Except(allEStrings).ToList();

                if (uncovered.Count > 0)
                {
                    for (int i = 0; i < uncovered.Count; i++)
                    {
                        if (i > 0 && i % 20 == 0) coverageLines.Add("");
                        coverageLines[coverageLines.Count - 1] += uncovered[i] + " ";
                    }
                }
                else
                {
                    coverageLines.Add("  (全部覆盖)");
                }

                System.IO.File.WriteAllLines($"{baseDir}/Coverage_Analysis.txt", coverageLines);

                Console.WriteLine($"✅ 结果保存成功！");
                Console.WriteLine($"   生成目录: {baseDir}");
                Console.WriteLine($"   总覆盖字符串数: {allEStrings.Count}/1000 ({allEStrings.Count / 1000.0 * 100:F1}%)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 保存结果时出错: {ex.Message}");
            }
        }

        #endregion
    }
}
