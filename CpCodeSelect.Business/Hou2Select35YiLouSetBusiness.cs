using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Util;
using CpCodeSelect.Util.DataGenerate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;
using static CpCodeSelect.Util.DataGenerate.DuozhouqiStringSelection;

namespace CpCodeSelect.Business
{
    public static class Hou2Select35YiLouSetBusiness
    {
        public static List<Code> AllCode = new List<Code>();
        public static Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
        public static Code code = null;
        public static Object lockObj = new Object();
        public static List<Hou2Select35Model> model35List = new List<Hou2Select35Model>();
        public static string leftNumberCountStr = ConfigurationManager.AppSettings["LeftNumberCount"];

        public static void InitData()
        {
            AllCode = new List<Code>();
            model35List = new List<Hou2Select35Model>();
            Hou2NumberCount = new Dictionary<string, int>();
        }
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
            //Hou2_20Numer = Generate20Code();

            // 计算已有号码的中挂情况
            //CalcExistCode(code);
            CalcExist350Code(code);

            // 生成新的350码
            //GenerateCode(code);
            if (AllCode != null && AllCode.Count > 350)
            {
                Generate350Code(code);
                //删除超过3000条的记录 如果没有4挂的就删除
                RemoveOldModel(code, 1);
            }
            Hou2Select35YiLouSetBusiness.code = code;
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static void RemoveOldModel(Code code, int guaCount = 1)
        {
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou2Select35Model> removeList = new List<Hou2Select35Model>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }

            while (model35List.Count >= numberCount)
            {
                for (int i = 0; i < 50; i++)
                {
                    var model = model35List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 20 && model.GuaCount <= guaCount)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    model35List.Remove(model);
                }
                removeList.Clear();
                count++;
                // 如果执行10次号码还是大于1000,则退出循环
                if (count > 10)
                    break;
            }

            //超过30期就删除
            {
                for (int i = 0; i < model35List.Count; i++)
                {
                    var model = model35List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 30)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    model35List.Remove(model);
                }
                removeList.Clear();
            }
        }
        public static void CalcExist350Code(Code code)
        {
            var hou3 = code.GetHou3String();
            foreach (var model in model35List)
            {
                if (model.Number35.Contains(hou3))
                {
                    //中了
                    model.ZhongGount++;
                    model.NeedZhong = false;
                    //model.Zhong3BeforeGua = model.Zhong2BeforeGua;
                    //model.Zhong2BeforeGua = model.ZhongBeforeGua;
                    //model.ZhongBeforeGua = model.GuaCount;

                    model.GuaCount = 0;
                }
                else
                {
                    //挂了
                    model.GuaCount++;
                    model.ZhongGount = 0;
                }

                //计算当前期的K线
                //KLine350Calc.CalcKlineCurrent(model, code);
            }
        }

        public static void Generate350Code(Code code)
        {

            if (AllCode != null && AllCode.Count > 130)
            {
                if (AllCode.Count > 130)
                {
                    int count = 0;
                    while (true)
                    {
                        var takeCodeList = new List<string>();
                        var excludeAllList = new List<string>();
                        var AllCode = Hou2Select35YiLouSetBusiness.AllCode;
                        List<Code> LeftCode = new List<Code>();
                        LeftCode.AddRange(AllCode);

                        if (string.IsNullOrEmpty(code.NumberCondition))
                        {


                            int n0 = 0;
                            int n1 = 1;
                            int n2 = 0;
                            int n3 = 12;

                            Cacl(n0, ref LeftCode, ref excludeAllList, ref takeCodeList);
                            Cacl(n1, ref LeftCode, ref excludeAllList, ref takeCodeList);
                            Cacl(n2, ref LeftCode, ref excludeAllList, ref takeCodeList);
                            Cacl(n3, ref LeftCode, ref excludeAllList, ref takeCodeList);
                        }
                        else
                        {
                            int n0 = 0;
                            int n1 = 1;
                            int n2 = 0;
                            int n3 = 12;

                            Cacl(n0, ref LeftCode, ref excludeAllList, ref takeCodeList);
                            Cacl(n1, ref LeftCode, ref excludeAllList, ref takeCodeList);
                            Cacl(n2, ref LeftCode, ref excludeAllList, ref takeCodeList);
                            Cacl(n3, ref LeftCode, ref excludeAllList, ref takeCodeList);

                            takeCodeList = takeCodeList.Distinct().ToList();
                            excludeAllList = excludeAllList.Distinct().ToList();
                            if (!excludeAllList.Any(item => takeCodeList.Contains(item)))
                            {
                                /*
                                var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);

                                numerList = numerList.OrderBy(item => item).ToList();

                                */
                                var numerList = MultiThreadedNumberSelectForYiLou.GenerateMultipleGroups(50, excludeAllList, takeCodeList, 35);
                                foreach (var number in numerList)
                                {
                                    if (number.Count > 0)
                                    {
                                        var list = number.OrderBy(p => p).ToList();
                                        Hou2Select35Model model = new Hou2Select35Model();
                                        model.Number35 = list;
                                        model.CodeNumber = code.CodeNumber;
                                        model.CodeQiHao = code.CodeQiHao;
                                        model.NeedZhong = true;
                                        model35List.Add(model);
                                    }
                                }
                                break;
                            }

                            count++;
                            if (count > 3500)
                            {
                                // 如果计算1000次还没有有效数据,则退出
                                break;
                            }

                        }
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
        /// 从Code列表中生成指定数量的后3号码,没有做滤重操作
        /// </summary>
        /// <param name="number"></param>
        /// <param name="AllCode"></param>
        /// <returns></returns>
        public static List<string> GenerateHou3NumbereFromCode(int number)
        {

            List<string> Hou3NumberCount = new List<string>();
            Hou3NumberCount.Clear();
            foreach (var code in AllCode)
            {
                if (Hou3NumberCount.Count == number)
                {
                    break;
                }
                var key = code.GetHou3String();
                Hou3NumberCount.Add(key);
            }
            return Hou3NumberCount.ToList();
        }

    }
}
