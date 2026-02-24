using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class MultiThreadedNumberSelectFor156Hou3ZiRanGenerate
    { 
        private static readonly ThreadLocal<Random> _threadLocalRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        /// <summary>
        /// 使用多线程生成多组号码
        /// </summary>
        /// <param name="groupCount">要生成的组数</param> 
        /// <param name="excludedNumbers">全局排除号码</param>
        /// <param name="mustIncludeNumbers">全局必须包含号码</param>
        /// <param name="getCountPerGroup">每组号码,默认50</param>
        /// <returns>号码组列表</returns>
        public static List<List<string>> GenerateMultipleGroups(
            List<string> codeHou3List,
            int groupCount = 50)
        {

            // 使用线程安全的集合存储结果
            var results = new ConcurrentBag<List<string>>();

            //try
            //{
                // 使用 Parallel.For 进行并行处理
                Parallel.For(0, groupCount, new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>
                {
                    // 为每个线程创建独立的 Random 实例，避免线程安全问题
                    var localRandom = new Random(GetThreadSafeSeed());
                    // 生成一组号码
                    var group = GenerateSingleGroupFor156(localRandom,codeHou3List);
                    if (group != null && group.Count > 0)
                        results.Add(group);

                    // 可选：显示进度
                    //if ((i + 1) % 10 == 0)
                    //{
                    //    Console.WriteLine($"已生成 {i + 1}/{groupCount} 组号码");
                    //}

                });
            //}
            //catch (Exception ex)
            //{
            //    //Console.WriteLine($"生成第{i + 1}组号码时出错: {ex.Message}");
            //}
            return results.ToList();
        }

        /// <summary>
        /// 使用 Parallel.ForEach 的替代版本
        /// </summary>
        public static List<List<string>> GenerateMultipleGroupsV2(
            int groupCount = 50,
            List<string> excludedNumbers = null,
            List<string> mustIncludeNumbers = null)
        {
            excludedNumbers = excludedNumbers ?? new List<string>();
            mustIncludeNumbers = mustIncludeNumbers ?? new List<string>();

            var results = new ConcurrentBag<List<string>>();
            var indices = Enumerable.Range(0, groupCount).ToList();

            Parallel.ForEach(indices, new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount // 限制并发数
            }, i =>
            {
                var localRandom = new Random(GetThreadSafeSeed());
                var group = GenerateSingleGroup(localRandom, excludedNumbers, mustIncludeNumbers);
                results.Add(group);
            });

            return results.ToList();
        }

        /// <summary>
        /// 使用 Task 的异步版本
        /// </summary>
        public static async Task<List<List<string>>> GenerateMultipleGroupsAsync(
            int groupCount = 50,
            List<string> excludedNumbers = null,
            List<string> mustIncludeNumbers = null)
        {
            excludedNumbers = excludedNumbers ?? new List<string>();
            mustIncludeNumbers = mustIncludeNumbers ?? new List<string>();

            var tasks = new List<Task<List<string>>>();

            for (int i = 0; i < groupCount; i++)
            {
                // 为每个任务创建独立的 Random 实例
                var taskIndex = i;
                tasks.Add(Task.Run(() =>
                {
                    var localRandom = new Random(GetThreadSafeSeed());
                    return GenerateSingleGroup(localRandom, excludedNumbers, mustIncludeNumbers);
                }));
            }

            // 等待所有任务完成
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        /// <summary>
        /// 生成单组号码（270个）
        /// </summary>
        /// <param name="codeHou3List">原始已存在的Code后3码的列表,这里目前是固定的270</param>
        /// <returns></returns>
        private static List<string> GenerateSingleGroupFor156(
            Random random,
            List<string> codeHou3List)
        {
            return Generate156CodeZiRan.Generate(codeHou3List,random);
        }
        /// <summary>
        /// 生成单组号码（50个）
        /// </summary>
        /// <param name="random"></param>
        /// <param name="excludedNumbers"></param>
        /// <param name="mustIncludeNumbers"></param>
        /// <param name="getCountPerGroup">每组号码数,默认50,可以是20或者27</param>
        /// <returns></returns>
        private static List<string> GenerateSingleGroup(
            Random random,
            List<string> excludedNumbers,
            List<string> mustIncludeNumbers,
            int getCountPerGroup = 50)
        {
            // 生成所有号码
            var allNumbers = GenerateAllNumbers();

            // 处理必须包含的号码
            var validMustInclude = mustIncludeNumbers.Except(excludedNumbers).Distinct().ToList();

            var finalResult = new List<string>();
            //int count = 0;
            //while (true)
            //{
                // 计算需要随机选择的数量
                int numbersNeeded = getCountPerGroup - validMustInclude.Count;

                // 创建可用号码池
                var availableNumbers = allNumbers
                    .Except(excludedNumbers)
                    .Except(validMustInclude)
                    .ToList();

                //// 验证可用号码是否足够
                //if (availableNumbers.Count < numbersNeeded)
                //{
                //    //throw new InvalidOperationException("可用号码不足");
                //    count++;
                //    if (count < 1000)
                //        continue;
                //    else
                //    {
                //        //单次执行1000次后,出循环,返回空集合
                //        break;
                //    }
                //}
                //else
                //{
                    // 随机选择号码
                    var randomlySelected = SelectRandomNumbers(availableNumbers, numbersNeeded, random);

                    // 合并结果
                    finalResult.AddRange(validMustInclude);
                    finalResult.AddRange(randomlySelected);

                    // 随机打乱
                    Shuffle(finalResult, random);
                    //break;
                //}
            //}
            return finalResult;
        }

        /// <summary>
        /// 随机选择号码
        /// </summary>
        private static List<string> SelectRandomNumbers(List<string> source, int count, Random random)
        {
            if (count == 0) return new List<string>();

            var shuffled = new List<string>(source);
            Shuffle(shuffled, random);
            return shuffled.Take(count).ToList();
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

        /// <summary>
        /// 生成00-99的所有号码
        /// </summary>
        private static List<string> GenerateAllNumbers()
        {
            return Enumerable.Range(0, 100)
                            .Select(x => x.ToString("D2"))
                            .ToList();
        }

        /// <summary>
        /// 获取线程安全的随机数种子
        /// </summary>
        private static int GetThreadSafeSeed()
        {
            lock (_threadLocalRandom)
            {
                return _threadLocalRandom.Value.Next();
            }
        }
    }
}
