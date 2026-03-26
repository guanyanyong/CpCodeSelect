using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class Generate500CodeZiRan
    {
        private static Random random = new Random();
        private static HashSet<string> allPossibleStrings;

        static Generate500CodeZiRan()
        {
            // 生成所有可能的字符串 (000-999)
            allPossibleStrings = GenerateAllPossibleStrings();
        }

        /// <summary>
        /// 生成350注数据
        /// </summary>
        /// <param name="list270"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static List<string> Generate(List<string> list270, Random random = null)
        { 
            if (random == null) random = Generate500CodeZiRan.random;
            //Console.WriteLine("=== 字符串选择程序 ===\n");


            // 1. 生成集合A (270个字符串，可重复)
            //List<string> setA = list270;
            //if (setA == null || setA.Count <= 0)
            //    setA = GenerateSetA(270, random);

            // 2. 快速找到子集B（110-120个）
            //var subsetB = FindSubsetB_Fast(setA, 90, 100, random);

            // 3. 生成集合C（A中没有的字符串）
            var setC = allPossibleStrings.ToList();


            // 5. 从C中取字符串形成D
            List<string> setD = GenerateSetD_Random(setC, 500, random);
            return setD;
        }

        // 从集合C中随机抽取字符串形成集合D（随机获取）
        static List<string> GenerateSetD_Random(List<string> setC, int count, Random random)
        {
            if (count <= 0) return new List<string>();

            if (count > setC.Count)
            {
                //Console.WriteLine($"警告：需要{count}个字符串，但C中只有{setC.Count}个");
                // 如果不够，打乱顺序返回所有
                return setC.OrderBy(x => random.Next()).ToList();
            }

            // 使用部分洗牌算法（Fisher-Yates）随机选择count个元素
            return PartialShuffle(setC, count,random);
        }

        // 部分洗牌算法：随机选择k个元素
        static List<string> PartialShuffle(List<string> list, int k, Random random)
        {
            if (random == null) random = Generate500CodeZiRan.random;
            int n = list.Count;
            var result = new List<string>(k);
            var tempList = new List<string>(list); // 复制一份避免修改原列表

            // 使用Fisher-Yates算法的部分版本
            for (int i = 0; i < k; i++)
            {
                // 从i到n-1中随机选择一个位置
                int j = random.Next(i, n);

                // 交换位置i和j的元素
                string temp = tempList[i];
                tempList[i] = tempList[j];
                tempList[j] = temp;

                // 将位置i的元素加入结果
                result.Add(tempList[i]);
            }

            return result;
        }

        // 合并B(去重)和D形成集合E
        static List<string> GenerateSetE(List<string> subsetB, List<string> setD)
        {
            HashSet<string> setE = new HashSet<string>();

            // 添加B中的字符串（去重）
            foreach (string str in subsetB)
            {
                setE.Add(str);
                if (setE.Count >= 350) break;
            }

            // 添加D中的字符串
            foreach (string str in setD)
            {
                setE.Add(str);
                if (setE.Count >= 350) break;
            }

            return setE.ToList();
        }
        // 生成所有可能的字符串 (000-999)
        static HashSet<string> GenerateAllPossibleStrings()
        {
            var strings = new HashSet<string>();
            for (int i = 0; i < 1000; i++)
            {
                strings.Add(i.ToString("D3"));
            }
            return strings;
        }

        // 快速找到子集B（110-120个）
        static List<string> FindSubsetB_Fast(List<string> setA, int minSize, int maxSize, Random random)
        {
            if (random == null) random = Generate500CodeZiRan.random;
            int targetSize = random.Next(minSize, maxSize + 1);
            int n = setA.Count;

            // 使用分桶策略快速选择
            var selectedIndices = new HashSet<int>();
            while (selectedIndices.Count < targetSize)
            {
                selectedIndices.Add(random.Next(n));
            }

            return selectedIndices.Select(i => setA[i]).ToList();
        }
        // 生成集合A (270个字符串，可重复)
        static List<string> GenerateSetA(int count, Random random)
        {
            List<string> setA = new List<string>();
            if (random == null) random = Generate500CodeZiRan.random;

            // 先确保A中有一定比例的重复字符串
            int uniqueCount = random.Next(50, 150); // A中大约有50-150个不同的字符串
            var uniqueStrings = allPossibleStrings
                .OrderBy(x => random.Next())
                .Take(uniqueCount)
                .ToList();

            // 为每个唯一字符串分配权重
            Dictionary<string, int> weights = new Dictionary<string, int>();
            int totalWeight = 0;
            foreach (var str in uniqueStrings)
            {
                int weight = random.Next(1, 10);
                weights[str] = weight;
                totalWeight += weight;
            }

            // 根据权重生成270个字符串
            while (setA.Count < count)
            {
                int randomValue = random.Next(0, totalWeight);
                int cumulative = 0;

                foreach (var pair in weights)
                {
                    cumulative += pair.Value;
                    if (randomValue < cumulative)
                    {
                        setA.Add(pair.Key);
                        break;
                    }
                }
            }

            // 打乱顺序
            return setA.OrderBy(x => random.Next()).ToList();
        }


    }
}
