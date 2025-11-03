using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class ThreadSafeRandom
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> _threadLocal =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        /// <summary>
        /// 生成指定范围内的随机整数
        /// </summary>
        public static int Next(int min, int max)
        {
            if (min > max)
            {
                int temp = min;
                min = max;
                max = temp;
            }

            return _threadLocal.Value.Next(min, max + 1);
        }

        /// <summary>
        /// 生成指定范围内的随机小数
        /// </summary>
        public static double NextDouble(double min, double max)
        {
            if (min > max)
            {
                double temp = min;
                min = max;
                max = temp;
            }

            return min + (_threadLocal.Value.NextDouble() * (max - min));
        }
    }
}
