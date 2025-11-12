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
    public static class Hou2Select50YiLouSetBusiness
    {
        public static List<Code> AllCode = new List<Code>();
        public static Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
        public static List<string> Hou2_20Numer = new List<string>();
        public static Object lockObj = new Object();
        public static List<Hou2Select50_20Model> modelList = new List<Hou2Select50_20Model>();
        public static string leftNumberCountStr = ConfigurationManager.AppSettings["LeftNumberCount"];
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

            // 计算已有号码的中挂情况
            CalcExistCode(code);

            // 生成新的50码
            GenerateCode(code);

            //删除超过3000条的记录 如果没有4挂的就删除
            RemoveOldModel(4);
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static void RemoveOldModel(int guaCount = 4)
        {

            List<Hou2Select50_20Model> removeList = new List<Hou2Select50_20Model>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }

            while (modelList.Count > numberCount)
            {
                for (int i = 0; i < 50; i++)
                {
                    var model = modelList[i];
                    if (model.GuaCount <= guaCount)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    modelList.Remove(model);
                }
                removeList.Clear();
                count++;
                // 如果执行10次号码还是大于3500,则退出循环
                if (count > 10) break;
            }
        }
        public static void CalcExistCode(Code code)
        {
            var hou2 = code.GetHou2String();
            foreach (var model in modelList)
            {
                if (model.Number50.Contains(hou2))
                {
                    model.ZhongGount++;
                    model.NeedZhong = false;
                    model.GuaCount = 0;
                }
                else
                {
                    model.GuaCount++;
                    model.ZhongGount = 0;
                    //if (model.GuaCount >= 5)
                    //{
                    //    model.NeedZhong = false;
                    //}
                }
            }
        }


        public static void GenerateCode(Code code)
        {

            if (AllCode.Count > 130)
            {
                int count = 0;
                while (true)
                {
                    var takeCodeList = new List<string>();
                    var excludeAllList = new List<string>();
                    var AllCode = Hou2Select50YiLouSetBusiness.AllCode;
                    List<Code> LeftCode = new List<Code>();
                    LeftCode.AddRange(AllCode);

                    int n1 = ThreadSafeRandom.Next(4, 12);
                    int n2 = ThreadSafeRandom.Next(8, 20);
                    int n3 = ThreadSafeRandom.Next(8, 20);
                    int n4 = ThreadSafeRandom.Next(0, 0);
                    int n5 = ThreadSafeRandom.Next(0, 0);
                    int n6 = ThreadSafeRandom.Next(0, 1);
                    int n7 = ThreadSafeRandom.Next(0, 1);
                    int n8 = ThreadSafeRandom.Next(0, 1);
                    int n9 = ThreadSafeRandom.Next(0, 10);
                    int n10 = ThreadSafeRandom.Next(0, 1);
                    int n11 = ThreadSafeRandom.Next(0, 10);
                    int n12 = ThreadSafeRandom.Next(0, 1);
                    int n13 = ThreadSafeRandom.Next(8, 20);

                    Cacl(n1, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n2, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n3, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n4, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n5, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n6, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n7, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n8, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n9, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n10, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n11, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n12, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(n13, ref LeftCode, ref excludeAllList, ref takeCodeList);


                    takeCodeList = takeCodeList.Distinct().ToList();
                    excludeAllList = excludeAllList.Distinct().ToList();
                    if (!excludeAllList.Any(item => takeCodeList.Contains(item)))
                    {
                        /*
                        var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);

                        numerList = numerList.OrderBy(item => item).ToList();
                        
                        */
                        var numerList = MultiThreadedNumberSelectForYiLou.GenerateMultipleGroups(50, excludeAllList, takeCodeList);
                        foreach (var number in numerList)
                        {
                            if (number.Count > 0)
                            {
                                var list = number.OrderBy(p => p).ToList();
                                Hou2Select50_20Model model = new Hou2Select50_20Model();
                                model.Number50 = list;
                                model.CodeNumber = code.CodeNumber;
                                model.CodeQiHao = code.CodeQiHao;
                                model.NeedZhong = true;
                                modelList.Add(model);
                            }
                        }
                        break;
                    }

                    count++;
                    if (count > 1000)
                    {
                        // 如果计算100次还没有有效数据,则退出
                        break;
                    }

                }
            }
        }

        public static void Cacl(int n1, ref List<Code> LeftCode, ref List<string> excludeAllList, ref List<string> takeCodeList)
        {

            if (n1 > 0)
            {
                var excludeList = GenerateHou2NumbereFromCode(n1, LeftCode);
                excludeAllList.AddRange(excludeList);
                LeftCode = GetCodeFromOriginExceptNumerCode(LeftCode, excludeAllList, n1);
            }
            if (LeftCode.Count > 0)
            {
                takeCodeList.Add(LeftCode.Take(1).FirstOrDefault().GetHou2String());
                LeftCode = LeftCode.Skip(1).ToList();
            }
        }


        /// <summary>
        /// 从Code列表中生成指定数量的后2号码,已经做了滤重操作
        /// </summary>
        /// <param name="number"></param>
        /// <param name="AllCode"></param>
        /// <returns></returns>
        public static List<string> GenerateHou2NumbereFromCode(int number, List<Code> AllCode)
        {
            Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
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
                if (Hou2NumberCount.Keys.Count == number)
                {
                    break;
                }
            }
            return Hou2NumberCount.Keys.ToList();
        }

        /// <summary>
        /// 从Code列表中获取从指定位置开始,排除掉exceptList中的号码后的Code列表
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="exceptList"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private static List<Code> GetCodeFromOriginExceptNumerCode(List<Code> codeList, List<string> exceptList, int number)
        {
            int count = 0;
            for (int i = number; i < codeList.Count; i++)
            {
                var code = codeList[i];
                var hou2Str = code.GetHou2String();
                if (exceptList.Contains(hou2Str))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return codeList.Skip(number + count).ToList();
        }




        /// <summary>
        /// 生成50个号码
        /// </summary>
        public static void GenerateCode2(Code code)
        {
            for (int i = 0; i < 50; i++)
            {
                var numberList = Hou2Select50AutoBusiness.GetHou2_50NumerListString();
                if (numberList.Count > 0)
                {
                    Hou2Select50_20Model model = new Hou2Select50_20Model();
                    model.Number50 = numberList;
                    model.CodeNumber = code.CodeNumber;
                    model.CodeQiHao = code.CodeQiHao;
                    model.NeedZhong = true;

                    modelList.Add(model);
                }
            }
        }

        /// <summary>
        /// 根据历史记录的后二生成50码字符串
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHou2_50NumerListString()
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
                return result50Number;
            }
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
