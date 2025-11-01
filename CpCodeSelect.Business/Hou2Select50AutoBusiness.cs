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
    public static class Hou2Select50AutoBusiness
    {
        public static List<Code> AllCode = new List<Code>();
        public static Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
        public static List<string> Hou2_20Numer = new List<string>();
        public static Object lockObj = new Object();
        public static List<Hou2Select50_20Model> modelList = new List<Hou2Select50_20Model>();
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

            //删除超过3000条的记录 如果没有3挂的就删除
            RemoveOldModel(3);
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static void RemoveOldModel(int guaCount=3)
        {
            List<Hou2Select50_20Model> removeList = new List<Hou2Select50_20Model>();
            while (modelList.Count > 3000)
            {
                for(int i = 0; i < 50; i++)
                {
                    var model = modelList[i];
                    if (model.GuaCount <= guaCount)
                    {
                        removeList.Add(model);
                    }
                }
                foreach(var model in removeList)
                {
                    modelList.Remove(model);
                }
                removeList.Clear();
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
                    if (model.GuaCount >= 5)
                    {
                        model.NeedZhong = false;
                    }
                }
            }
        }
        /// <summary>
        /// 生成50个号码
        /// </summary>
        public static void GenerateCode(Code code)
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
