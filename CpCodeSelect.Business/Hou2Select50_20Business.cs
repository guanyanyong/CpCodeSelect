using CpCodeSelect.Model;
using CpCodeSelect.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;

namespace CpCodeSelect.Business
{
    public static class Hou2Select50_20Business
    {
        public static List<Code> AllCode = new List<Code>();
        public static Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
        public static List<string> Hou2_20Numer = new List<string>();
        public static Object lockObj = new Object();
        /// <summary>
        /// 初始化号码
        /// </summary>
        /// <param name="code"></param>
        public static void InitCode(Code code)
        {
            code.Wan = new PositionNumber
            {
                PositionType = PositionType.万,
                Number = int.Parse(code.CodeNumber[0].ToString())
            };
            code.Qian = new PositionNumber
            {
                PositionType = PositionType.千,
                Number = int.Parse(code.CodeNumber[1].ToString())
            };
            code.Bai = new PositionNumber
            {
                PositionType = PositionType.百,
                Number = int.Parse(code.CodeNumber[2].ToString())
            };
            code.Shi = new PositionNumber
            {
                PositionType = PositionType.十,
                Number = int.Parse(code.CodeNumber[3].ToString())
            };
            code.Ge = new PositionNumber
            {
                PositionType = PositionType.个,
                Number = int.Parse(code.CodeNumber[4].ToString())
            };
            AllCode.Insert(0, code);
            Hou2_20Numer = Generate20Code();
        }
        /// <summary>
        /// 根据历史记录的后二生成50码字符串
        /// </summary>
        /// <returns></returns>
        public static string GetHou2_50NumerString()
        {
            lock (lockObj)
            {
                List<string> result50Number = new List<string>();
                if (Hou2_20Numer.Count >= 20)
                {
                    List<string> remaining = NumberFilter.GetRemainingNumbers(Hou2_20Numer);
                    var numberList = NumberSelector.SelectNumbersFrom80(remaining);
                    foreach (var num in numberList)
                    {
                        result50Number.AddRange(num);
                    }
                    result50Number.Sort();
                }
                return string.Join(" ", result50Number);
            }
        }
        /// <summary>
        /// 根据历史记录的后二生成20码
        /// </summary>
        /// <returns></returns>
        public static List<string> Generate20Code()
        {
            Hou2NumberCount.Clear();
            foreach (var code in AllCode)
            {
                var key = code.GetHou2String();
                if (!Hou2NumberCount.Keys.Contains(key))
                {
                    Hou2NumberCount.Add(key, 1);
                }
                else
                {
                    Hou2NumberCount[key] = Hou2NumberCount[key] + 1;
                }
                if (Hou2NumberCount.Keys.Count == 20)
                {
                    break;
                }
            }
            return Hou2NumberCount.Keys.ToList();
        }
    }
}
