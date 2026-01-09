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
    public class LayeredStringSelection
    {
        #region 线程安全的随机数生成器

        private static readonly ThreadLocal<Random> threadLocalRandom =
            new ThreadLocal<Random>(() =>
            {
                int seed = Thread.CurrentThread.ManagedThreadId ^
                          Environment.TickCount ^
                          Guid.NewGuid().GetHashCode();
                return new Random(seed);
            });

        #endregion

        #region 主程序入口

        public static List<BatchResult> GetBatchResults(List<string> sourceList)
        {
            Console.WriteLine("=== 分层随机抽样字符串选择系统 ===\n");
            Console.WriteLine("核心算法：");
            Console.WriteLine("1. 将集合A中的字符串按出现次数分为5层：");
            Console.WriteLine("   J层: 出现次数 > 5");
            Console.WriteLine("   K层: 出现次数 = 4");
            Console.WriteLine("   L层: 出现次数 = 3");
            Console.WriteLine("   M层: 出现次数 = 2");
            Console.WriteLine("   N层: 出现次数 = 1");
            Console.WriteLine("2. 从J层开始，依次从每层随机抽取70%的字符串");
            Console.WriteLine("3. 直到满足B的总出现次数在620-660之间\n");

            // 生成所有可能的字符串
            var allStrings = Enumerable.Range(0, 1000)
                .Select(i => i.ToString("D3"))
                .ToArray();

            Stopwatch totalSw = Stopwatch.StartNew();

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

                    foreach (var record in freqDict.Keys)
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

                // 3. 将A中的字符串分层
                var layers = BuildLayers(freqDict);
                DisplayLayersInfo(layers);

                // 4. 生成集合C
                var setC = GenerateSetC(uniqueA, allStrings);
                Console.WriteLine($"✅ 集合C生成成功: {setC.Count}个字符串\n");

                // 5. 并行生成50个不同的E集合
                int targetBatchCount = 50;
                Console.WriteLine($"🚀 开始并行生成 {targetBatchCount} 个不同的E集合...\n");

                var results = GenerateMultipleEWithLayeredApproach(
                    setA, freqDict, uniqueA, setC, layers,
                    minUniqueB, maxUniqueB, targetBatchCount, allStrings);

                //totalSw.Stop();

                //// 6. 显示结果统计
                //DisplayResults(results, setA, K, totalSw.ElapsedMilliseconds, layers);

                //// 7. 保存结果到文件
                //SaveResultsToFiles(results, setA, layers);


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

        #region 分层数据结构

        /// <summary>
        /// 字符串分层结构
        /// </summary>
        class StringLayers
        {
            // 各层定义
            public List<string> LayerJ { get; set; } = new List<string>(); // 出现次数 > 5
            public List<string> LayerK { get; set; } = new List<string>(); // 出现次数 = 4
            public List<string> LayerL { get; set; } = new List<string>(); // 出现次数 = 3
            public List<string> LayerM { get; set; } = new List<string>(); // 出现次数 = 2
            public List<string> LayerN { get; set; } = new List<string>(); // 出现次数 = 1

            // 各层的出现次数（固定值）
            public int CountJ => 6; // >5，实际使用时需要查询具体次数
            public int CountK => 4;
            public int CountL => 3;
            public int CountM => 2;
            public int CountN => 1;

            // 各层权重（用于控制抽取比例）
            public double BasePercentage { get; set; } = 0.7; // 基础抽取比例70%
        }

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
            public Dictionary<string, double> LayerPercentages { get; set; } // 各层实际抽取比例
            public int ThreadId { get; set; }
            public string EHash { get; set; }
        }

        #endregion

        #region 核心算法实现

        /// <summary>
        /// 获取当前线程的Random实例
        /// </summary>
        private static Random GetThreadRandom()
        {
            return threadLocalRandom.Value;
        }

        /// <summary>
        /// 生成集合A
        /// </summary>
        private static (List<string> setA, Dictionary<string, int> freqDict, HashSet<string> uniqueA, int K)
            GenerateSetA(int count, string[] allStrings, Random random)
        {
            var setA = new List<string>(count);
            var freqDict = new Dictionary<string, int>();

            // 控制不同字符串数量
            int K = random.Next(650, 751);
            var selectedUnique = allStrings.OrderBy(x => random.Next()).Take(K).ToList();

            // 设计分层友好的频率分布
            var frequencies = new Dictionary<string, int>();
            int remaining = count;

            // 分配基础频率，确保有足够的字符串进入各层
            // 目标：创建合理的分层结构
            int targetJ = K / 4;      // 25%的字符串出现次数>5
            int targetK = K / 5;      // 20%的字符串出现次数=4
            int targetL = K / 5;      // 20%的字符串出现次数=3
            int targetM = K / 5;      // 20%的字符串出现次数=2
            int targetN = K - targetJ - targetK - targetL - targetM; // 剩余的15%出现次数=1

            // 为J层分配字符串（出现次数>5）
            var jStrings = selectedUnique.Take(targetJ).ToList();
            foreach (var str in jStrings)
            {
                int freq = random.Next(6, 11); // 6-10次
                frequencies[str] = freq;
                remaining -= freq;
            }

            // 为K层分配字符串（出现次数=4）
            var kStrings = selectedUnique.Skip(targetJ).Take(targetK).ToList();
            foreach (var str in kStrings)
            {
                frequencies[str] = 4;
                remaining -= 4;
            }

            // 为L层分配字符串（出现次数=3）
            var lStrings = selectedUnique.Skip(targetJ + targetK).Take(targetL).ToList();
            foreach (var str in lStrings)
            {
                frequencies[str] = 3;
                remaining -= 3;
            }

            // 为M层分配字符串（出现次数=2）
            var mStrings = selectedUnique.Skip(targetJ + targetK + targetL).Take(targetM).ToList();
            foreach (var str in mStrings)
            {
                frequencies[str] = 2;
                remaining -= 2;
            }

            // 为N层分配字符串（出现次数=1）
            var nStrings = selectedUnique.Skip(targetJ + targetK + targetL + targetM).Take(targetN).ToList();
            foreach (var str in nStrings)
            {
                frequencies[str] = 1;
                remaining -= 1;
            }

            // 调整：如果还有剩余次数，随机分配到已有字符串
            while (remaining > 0)
            {
                string str = selectedUnique[random.Next(K)];
                frequencies[str] += 1;
                remaining -= 1;
            }

            // 生成集合A并计算频率
            foreach (var kvp in frequencies)
            {
                for (int i = 0; i < kvp.Value; i++)
                    setA.Add(kvp.Key);

                freqDict[kvp.Key] = kvp.Value;
            }

            // 打乱顺序
            setA = setA.OrderBy(x => random.Next()).ToList();

            return (setA, freqDict, new HashSet<string>(selectedUnique), K);
        }

        /// <summary>
        /// 根据频率字典构建分层结构
        /// </summary>
        private static StringLayers BuildLayers(Dictionary<string, int> freqDict)
        {
            var layers = new StringLayers();

            foreach (var kvp in freqDict)
            {
                string str = kvp.Key;
                int count = kvp.Value;

                if (count > 5)
                    layers.LayerJ.Add(str);
                else if (count == 4)
                    layers.LayerK.Add(str);
                else if (count == 3)
                    layers.LayerL.Add(str);
                else if (count == 2)
                    layers.LayerM.Add(str);
                else if (count == 1)
                    layers.LayerN.Add(str);
            }

            return layers;
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
        /// 生成集合C
        /// </summary>
        private static List<string> GenerateSetC(HashSet<string> uniqueA, string[] allStrings)
        {
            return allStrings.Where(s => !uniqueA.Contains(s)).ToList();
        }

        /// <summary>
        /// 使用分层方法生成多个E集合
        /// </summary>
        private static List<BatchResult> GenerateMultipleEWithLayeredApproach(
            List<string> setA,
            Dictionary<string, int> freqDict,
            HashSet<string> uniqueA,
            List<string> setC,
            StringLayers layers,
            int minUniqueB,
            int maxUniqueB,
            int targetCount,
            string[] allStrings)
        {
            var results = new ConcurrentBag<BatchResult>();
            var generatedHashes = new ConcurrentDictionary<string, bool>();

            int completedBatches = 0;
            int totalAttempts = 0;
            object progressLock = new object();

            // 并行生成
            Parallel.For(0, 1 * 1, new ParallelOptions
            {
                MaxDegreeOfParallelism = 1
            }, (i, state) =>
            {
                if (results.Count >= targetCount)
                {
                    state.Break();
                    return;
                }

                var random = GetThreadRandom();
                Interlocked.Increment(ref totalAttempts);

                try
                {
                    // 随机调整各层抽取比例（围绕70%上下浮动）
                    double basePercentage = 0.7;
                    var layerPercentages = new Dictionary<string, double>
                    {
                        ["J"] = Math.Max(0.5, Math.Min(0.9, basePercentage + (random.NextDouble() * 0.4 - 0.2))),
                        ["K"] = Math.Max(0.5, Math.Min(0.9, basePercentage + (random.NextDouble() * 0.4 - 0.2))),
                        ["L"] = Math.Max(0.5, Math.Min(0.9, basePercentage + (random.NextDouble() * 0.4 - 0.2))),
                        ["M"] = Math.Max(0.5, Math.Min(0.9, basePercentage + (random.NextDouble() * 0.4 - 0.2))),
                        ["N"] = Math.Max(0.5, Math.Min(0.9, basePercentage + (random.NextDouble() * 0.4 - 0.2)))
                    };

                    // 使用分层方法生成B
                    var result = GenerateBatchWithLayers(
                        setA, freqDict, uniqueA, setC, layers,
                        minUniqueB, maxUniqueB, layerPercentages, random);

                    if (result != null)
                    {
                        string eHash = CalculateEHash(result.SetE);

                        if (generatedHashes.TryAdd(eHash, true))
                        {
                            result.BatchId = results.Count + 1;
                            result.ThreadId = Thread.CurrentThread.ManagedThreadId;
                            result.EHash = eHash;
                            result.LayerPercentages = layerPercentages;

                            results.Add(result);

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
                    // 忽略错误，继续尝试
                }

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
        /// 使用分层方法生成单个批次
        /// </summary>
        private static BatchResult GenerateBatchWithLayers(
            List<string> setA,
            Dictionary<string, int> freqDict,
            HashSet<string> uniqueA,
            List<string> setC,
            StringLayers layers,
            int minUniqueB,
            int maxUniqueB,
            Dictionary<string, double> layerPercentages,
            Random random)
        {
            // 1. 使用分层随机抽样选择B集合
            var (selectedStrings, totalCount) = SelectBWithLayeredApproach(
                freqDict, layers, layerPercentages, minUniqueB, maxUniqueB, 620, 660, random);

            if (selectedStrings == null) return null;

            // 2. 生成子集B
            var subsetB = GenerateSubsetB(setA, selectedStrings);

            // 3. 计算需要从C中取的数量
            int needFromC = 350 - selectedStrings.Count;

            if (needFromC > setC.Count || needFromC < 0) return null;

            // 4. 从C中抽取形成D（使用简单随机）
            var setD = setC.OrderBy(x => random.Next()).Take(needFromC).ToList();

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

        /// <summary>
        /// 分层随机抽样选择B集合
        /// </summary>
        private static (HashSet<string> selectedStrings, int totalCount)
            SelectBWithLayeredApproach(
                Dictionary<string, int> freqDict,
                StringLayers layers,
                Dictionary<string, double> layerPercentages,
                int minUniqueCount,
                int maxUniqueCount,
                int minTotalCount,
                int maxTotalCount,
                Random random)
        {
            // 多次尝试，调整抽取比例直到满足条件
            for (int attempt = 0; attempt < 50; attempt++)
            {
                var selectedStrings = new HashSet<string>();
                int totalCount = 0;

                // 按照J->K->L->M->N的顺序抽取
                SelectFromLayer(layers.LayerJ, freqDict, layerPercentages["J"], selectedStrings, ref totalCount, random);
                SelectFromLayer(layers.LayerK, freqDict, layerPercentages["J"], selectedStrings, ref totalCount, random);
                SelectFromLayer(layers.LayerL, freqDict, layerPercentages["L"], selectedStrings, ref totalCount, random);
                SelectFromLayer(layers.LayerM, freqDict, layerPercentages["M"], selectedStrings, ref totalCount, random);
                SelectFromLayer(layers.LayerN, freqDict, layerPercentages["N"], selectedStrings, ref totalCount, random);

                // 检查条件
                if (totalCount >= minTotalCount && totalCount <= maxTotalCount &&
                    selectedStrings.Count >= minUniqueCount && selectedStrings.Count <= maxUniqueCount)
                {
                    return (selectedStrings, totalCount);
                }

                // 如果总数太小，增加抽取比例
                if (totalCount < minTotalCount)
                {
                    foreach (var key in layerPercentages.Keys.ToList())
                    {
                        layerPercentages[key] = Math.Min(0.9, layerPercentages[key] + 0.05);
                    }
                }
                // 如果总数太大，减少抽取比例
                else if (totalCount > maxTotalCount)
                {
                    foreach (var key in layerPercentages.Keys.ToList())
                    {
                        layerPercentages[key] = Math.Max(0.3, layerPercentages[key] - 0.05);
                    }
                }

                // 如果不同字符串数量不在范围内，调整
                if (selectedStrings.Count < minUniqueCount || selectedStrings.Count > maxUniqueCount)
                {
                    // 主要调整N层（出现次数=1的层），对总数影响最小
                    if (selectedStrings.Count < minUniqueCount)
                        layerPercentages["N"] = Math.Min(0.9, layerPercentages["N"] + 0.1);
                    else
                        layerPercentages["N"] = Math.Max(0.3, layerPercentages["N"] - 0.1);
                }
            }

            return (null, 0);
        }

        /// <summary>
        /// 从指定层中抽取字符串
        /// </summary>
        private static void SelectFromLayer(
            List<string> layer,
            Dictionary<string, int> freqDict,
            double percentage,
            HashSet<string> selectedStrings,
            ref int totalCount,
            Random random)
        {
            if (layer.Count == 0) return;

            // 计算要抽取的数量
            int targetCount = (int)Math.Ceiling(layer.Count * percentage);
            targetCount = Math.Max(1, Math.Min(targetCount, layer.Count));

            // 随机打乱并抽取
            var shuffled = layer.OrderBy(x => random.Next()).Take(targetCount).ToList();

            foreach (var str in shuffled)
            {
                selectedStrings.Add(str);
                totalCount += freqDict[str];
            }
        }

        /// <summary>
        /// 智能调整分层抽取（更精确的控制）
        /// </summary>
        private static (HashSet<string> selectedStrings, int totalCount)
            SmartLayeredSelection(
                Dictionary<string, int> freqDict,
                StringLayers layers,
                int minUniqueCount,
                int maxUniqueCount,
                int minTotalCount,
                int maxTotalCount,
                Random random)
        {
            // 计算各层的基本信息
            var layerInfo = new[]
            {
                new { Name = "J", Strings = layers.LayerJ, Weight = 6, MinPct = 0.3, MaxPct = 0.9 },
                new { Name = "K", Strings = layers.LayerK, Weight = 4, MinPct = 0.3, MaxPct = 0.9 },
                new { Name = "L", Strings = layers.LayerL, Weight = 3, MinPct = 0.3, MaxPct = 0.9 },
                new { Name = "M", Strings = layers.LayerM, Weight = 2, MinPct = 0.3, MaxPct = 0.9 },
                new { Name = "N", Strings = layers.LayerN, Weight = 1, MinPct = 0.3, MaxPct = 0.9 }
            };

            // 尝试多次找到合适的抽取比例
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var selectedStrings = new HashSet<string>();
                int totalCount = 0;
                int uniqueCount = 0;

                // 确定各层抽取比例
                var percentages = new Dictionary<string, double>();
                foreach (var layer in layerInfo)
                {
                    // 基础70%，加上随机扰动
                    double pct = 0.7 + (random.NextDouble() * 0.4 - 0.2);
                    pct = Math.Max(layer.MinPct, Math.Min(layer.MaxPct, pct));
                    percentages[layer.Name] = pct;
                }

                // 按照顺序抽取
                foreach (var layer in layerInfo)
                {
                    if (totalCount >= maxTotalCount) break;

                    int targetCount = (int)Math.Ceiling(layer.Strings.Count * percentages[layer.Name]);
                    targetCount = Math.Max(0, Math.Min(targetCount, layer.Strings.Count));

                    if (targetCount == 0) continue;

                    // 随机抽取
                    var selectedFromLayer = layer.Strings
                        .OrderBy(x => random.Next())
                        .Take(targetCount)
                        .ToList();

                    foreach (var str in selectedFromLayer)
                    {
                        if (totalCount >= maxTotalCount) break;

                        selectedStrings.Add(str);
                        totalCount += freqDict[str];
                        uniqueCount++;
                    }
                }

                // 检查条件
                if (totalCount >= minTotalCount && totalCount <= maxTotalCount &&
                    uniqueCount >= minUniqueCount && uniqueCount <= maxUniqueCount)
                {
                    return (selectedStrings, totalCount);
                }

                // 动态调整策略
                if (totalCount < minTotalCount)
                {
                    // 增加高频层的抽取比例
                    percentages["J"] = Math.Min(0.9, percentages["J"] + 0.1);
                    percentages["K"] = Math.Min(0.9, percentages["K"] + 0.05);
                }
                else if (totalCount > maxTotalCount)
                {
                    // 减少高频层的抽取比例
                    percentages["J"] = Math.Max(0.3, percentages["J"] - 0.1);
                    percentages["K"] = Math.Max(0.3, percentages["K"] - 0.05);
                }

                if (uniqueCount < minUniqueCount)
                {
                    // 增加低频层的抽取比例
                    percentages["N"] = Math.Min(0.9, percentages["N"] + 0.15);
                    percentages["M"] = Math.Min(0.9, percentages["M"] + 0.1);
                }
                else if (uniqueCount > maxUniqueCount)
                {
                    // 减少低频层的抽取比例
                    percentages["N"] = Math.Max(0.3, percentages["N"] - 0.15);
                    percentages["M"] = Math.Max(0.3, percentages["M"] - 0.1);
                }
            }

            return (null, 0);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 生成子集B
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
            if (setE.Count != 350) return false;
            if (subsetB.Count < 620 || subsetB.Count > 660) return false;

            // 检查B中每个选中的字符串都包含了在A中的所有出现次数
            var bFreq = subsetB.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());
            var aFreq = setA.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());

            foreach (var kvp in bFreq)
            {
                if (kvp.Value != aFreq[kvp.Key]) return false;
            }

            // 检查D中的字符串都不在A中
            var stringsInA = new HashSet<string>(setA);
            if (setD.Any(s => stringsInA.Contains(s))) return false;

            return true;
        }

        #endregion

        #region 结果显示与保存

        /// <summary>
        /// 显示分层信息
        /// </summary>
        private static void DisplayLayersInfo(StringLayers layers)
        {
            Console.WriteLine("📊 集合A分层统计:");
            Console.WriteLine($"   J层(>5次): {layers.LayerJ.Count}个字符串");
            Console.WriteLine($"   K层(=4次): {layers.LayerK.Count}个字符串");
            Console.WriteLine($"   L层(=3次): {layers.LayerL.Count}个字符串");
            Console.WriteLine($"   M层(=2次): {layers.LayerM.Count}个字符串");
            Console.WriteLine($"   N层(=1次): {layers.LayerN.Count}个字符串");
            Console.WriteLine($"   总计: {layers.LayerJ.Count + layers.LayerK.Count + layers.LayerL.Count + layers.LayerM.Count + layers.LayerN.Count}个不同字符串\n");

            // 计算各层在总出现次数中的占比
            int totalOccurrences = 0;
            int jOccurrences = 0, kOccurrences = 0, lOccurrences = 0, mOccurrences = 0, nOccurrences = 0;

            foreach (var str in layers.LayerJ)
                jOccurrences += 6; // 近似值，实际需要查询具体次数

            foreach (var str in layers.LayerK)
                kOccurrences += 4;

            foreach (var str in layers.LayerL)
                lOccurrences += 3;

            foreach (var str in layers.LayerM)
                mOccurrences += 2;

            foreach (var str in layers.LayerN)
                nOccurrences += 1;

            totalOccurrences = jOccurrences + kOccurrences + lOccurrences + mOccurrences + nOccurrences;

            Console.WriteLine("📈 各层出现次数占比:");
            Console.WriteLine($"   J层: {jOccurrences}次 ({jOccurrences * 100.0 / totalOccurrences:F1}%)");
            Console.WriteLine($"   K层: {kOccurrences}次 ({kOccurrences * 100.0 / totalOccurrences:F1}%)");
            Console.WriteLine($"   L层: {lOccurrences}次 ({lOccurrences * 100.0 / totalOccurrences:F1}%)");
            Console.WriteLine($"   M层: {mOccurrences}次 ({mOccurrences * 100.0 / totalOccurrences:F1}%)");
            Console.WriteLine($"   N层: {nOccurrences}次 ({nOccurrences * 100.0 / totalOccurrences:F1}%)");
            Console.WriteLine();
        }

        /// <summary>
        /// 显示结果统计
        /// </summary>
        private static void DisplayResults(
            List<BatchResult> results,
            List<string> setA,
            int K,
            long elapsedMs,
            StringLayers layers)
        {
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("📈 分层随机抽样 - 生成结果统计");
            Console.WriteLine(new string('=', 60));

            Console.WriteLine($"集合A大小: {setA.Count} (含重复)");
            Console.WriteLine($"[A减]大小(K): {K}");
            Console.WriteLine($"成功生成E集合数量: {results.Count}");
            Console.WriteLine($"总耗时: {elapsedMs}ms\n");

            if (results.Count == 0) return;

            // 1. 线程分布
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

            // 2. B集合统计
            var bSizes = results.Select(r => r.TotalB).ToList();
            var uniqueBCounts = results.Select(r => r.SelectedStrings.Count).ToList();

            Console.WriteLine("📊 B集合统计:");
            Console.WriteLine($"  总出现次数范围: {bSizes.Min()} - {bSizes.Max()}");
            Console.WriteLine($"  平均出现次数: {bSizes.Average():F1}");
            Console.WriteLine($"  不同字符串数量范围: {uniqueBCounts.Min()} - {uniqueBCounts.Max()}");
            Console.WriteLine($"  平均不同字符串数: {uniqueBCounts.Average():F1}\n");

            // 3. 分析B集合的层分布
            Console.WriteLine("🔍 B集合分层分布分析:");

            // 随机选择一个批次分析
            if (results.Count > 0)
            {
                var sampleResult = results[0];
                var layerDistribution = AnalyzeLayerDistribution(sampleResult.SelectedStrings, layers);

                Console.WriteLine($"  示例批次各层抽取情况:");
                Console.WriteLine($"    J层: {layerDistribution["J"]}/{layers.LayerJ.Count} ({layerDistribution["J"] * 100.0 / layers.LayerJ.Count:F1}%)");
                Console.WriteLine($"    K层: {layerDistribution["K"]}/{layers.LayerK.Count} ({layerDistribution["K"] * 100.0 / layers.LayerK.Count:F1}%)");
                Console.WriteLine($"    L层: {layerDistribution["L"]}/{layers.LayerL.Count} ({layerDistribution["L"] * 100.0 / layers.LayerL.Count:F1}%)");
                Console.WriteLine($"    M层: {layerDistribution["M"]}/{layers.LayerM.Count} ({layerDistribution["M"] * 100.0 / layers.LayerM.Count:F1}%)");
                Console.WriteLine($"    N层: {layerDistribution["N"]}/{layers.LayerN.Count} ({layerDistribution["N"] * 100.0 / layers.LayerN.Count:F1}%)");
            }

            // 4. E集合差异度分析
            Console.WriteLine($"\n🔍 E集合差异度分析:");

            var allEStrings = results.SelectMany(r => r.SetE).Distinct().ToList();
            Console.WriteLine($"  所有E集合覆盖的字符串总数: {allEStrings.Count}/1000");
            Console.WriteLine($"  总体覆盖率: {allEStrings.Count / 1000.0 * 100:F1}%");

            // 计算平均Jaccard相似度
            if (results.Count > 1)
            {
                double totalSimilarity = 0;
                int comparisonCount = 0;

                for (int i = 0; i < Math.Min(10, results.Count); i++)
                {
                    for (int j = i + 1; j < Math.Min(10, results.Count); j++)
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
        }

        /// <summary>
        /// 分析B集合的层分布
        /// </summary>
        private static Dictionary<string, int> AnalyzeLayerDistribution(HashSet<string> selectedStrings, StringLayers layers)
        {
            var distribution = new Dictionary<string, int>
            {
                ["J"] = selectedStrings.Intersect(layers.LayerJ).Count(),
                ["K"] = selectedStrings.Intersect(layers.LayerK).Count(),
                ["L"] = selectedStrings.Intersect(layers.LayerL).Count(),
                ["M"] = selectedStrings.Intersect(layers.LayerM).Count(),
                ["N"] = selectedStrings.Intersect(layers.LayerN).Count()
            };

            return distribution;
        }

        /// <summary>
        /// 保存结果到文件
        /// </summary>
        private static void SaveResultsToFiles(List<BatchResult> results, List<string> setA, StringLayers layers)
        {
            if (results.Count == 0) return;

            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string baseDir = $"LayeredSelectionResults_{timestamp}";
                System.IO.Directory.CreateDirectory(baseDir);

                Console.WriteLine($"\n💾 正在保存结果到目录: {baseDir}");

                // 1. 保存固定集合A
                System.IO.File.WriteAllLines($"{baseDir}/0_Fixed_SetA.txt", setA);

                // 2. 保存分层信息
                var layerInfo = new List<string>
                {
                    "=== 集合A分层信息 ===",
                    $"生成时间: {DateTime.Now}",
                    $"",
                    $"J层(出现次数 > 5): {layers.LayerJ.Count}个字符串",
                    string.Join(" ", layers.LayerJ.Take(50)),
                    $"",
                    $"K层(出现次数 = 4): {layers.LayerK.Count}个字符串",
                    string.Join(" ", layers.LayerK.Take(50)),
                    $"",
                    $"L层(出现次数 = 3): {layers.LayerL.Count}个字符串",
                    string.Join(" ", layers.LayerL.Take(50)),
                    $"",
                    $"M层(出现次数 = 2): {layers.LayerM.Count}个字符串",
                    string.Join(" ", layers.LayerM.Take(50)),
                    $"",
                    $"N层(出现次数 = 1): {layers.LayerN.Count}个字符串",
                    string.Join(" ", layers.LayerN.Take(50))
                };

                System.IO.File.WriteAllLines($"{baseDir}/0_Layer_Info.txt", layerInfo);

                // 3. 保存每个E集合
                foreach (var result in results)
                {
                    string batchDir = $"{baseDir}/E{result.BatchId:00}_Thread{result.ThreadId}";
                    System.IO.Directory.CreateDirectory(batchDir);

                    // 保存B、D、E
                    System.IO.File.WriteAllLines($"{batchDir}/SubsetB.txt", result.SubsetB);
                    System.IO.File.WriteAllLines($"{batchDir}/SetD.txt", result.SetD);
                    System.IO.File.WriteAllLines($"{batchDir}/SetE.txt", result.SetE.OrderBy(s => s));

                    // 分析B的层分布
                    var layerDistribution = AnalyzeLayerDistribution(result.SelectedStrings, layers);

                    // 保存元数据
                    var metaLines = new List<string>
                    {
                        $"=== E集合 {result.BatchId:00} 信息 ===",
                        $"生成时间: {DateTime.Now}",
                        $"生成线程: {result.ThreadId}",
                        $"",
                        $"子集B:",
                        $"  总字符串数: {result.SubsetB.Count}",
                        $"  不同字符串数: {result.SelectedStrings.Count}",
                        $"",
                        $"B集合分层分布:",
                        $"  J层: {layerDistribution["J"]}/{layers.LayerJ.Count} ({layerDistribution["J"] * 100.0 / layers.LayerJ.Count:F1}%)",
                        $"  K层: {layerDistribution["K"]}/{layers.LayerK.Count} ({layerDistribution["K"] * 100.0 / layers.LayerK.Count:F1}%)",
                        $"  L层: {layerDistribution["L"]}/{layers.LayerL.Count} ({layerDistribution["L"] * 100.0 / layers.LayerL.Count:F1}%)",
                        $"  M层: {layerDistribution["M"]}/{layers.LayerM.Count} ({layerDistribution["M"] * 100.0 / layers.LayerM.Count:F1}%)",
                        $"  N层: {layerDistribution["N"]}/{layers.LayerN.Count} ({layerDistribution["N"] * 100.0 / layers.LayerN.Count:F1}%)",
                        $"",
                        $"集合D: {result.SetD.Count}个字符串",
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
                    "=== 分层随机抽样系统 - 结果汇总 ===",
                    $"生成时间: {DateTime.Now}",
                    $"",
                    $"集合A信息:",
                    $"  总字符串数: {setA.Count}",
                    $"  不同字符串数: {new HashSet<string>(setA).Count}",
                    $"",
                    $"分层信息:",
                    $"  J层(>5次): {layers.LayerJ.Count}个字符串",
                    $"  K层(=4次): {layers.LayerK.Count}个字符串",
                    $"  L层(=3次): {layers.LayerL.Count}个字符串",
                    $"  M层(=2次): {layers.LayerM.Count}个字符串",
                    $"  N层(=1次): {layers.LayerN.Count}个字符串",
                    $"",
                    $"生成的E集合数量: {results.Count}",
                    $"",
                    $"各E集合信息:"
                };

                foreach (var result in results)
                {
                    summaryLines.Add(
                        $"E{result.BatchId:00}: " +
                        $"B大小={result.SubsetB.Count}, " +
                        $"B不同={result.SelectedStrings.Count}, " +
                        $"E大小={result.SetE.Count}, " +
                        $"线程={result.ThreadId}");
                }

                System.IO.File.WriteAllLines($"{baseDir}/00_Summary.txt", summaryLines);

                // 5. 保存所有E集合的合并
                var allEStrings = results.SelectMany(r => r.SetE).Distinct().OrderBy(s => s).ToList();
                System.IO.File.WriteAllLines($"{baseDir}/All_E_Strings.txt", allEStrings);

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