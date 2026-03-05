using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Scorer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;

namespace CpCodeSelect.Business.Hou2Selct27Three18
{
    public class Hou2Selct27Three18SetFormBusiness : Hou3Select350YiLouSetFormZhongParentBusiness
    {
        /// <summary>
        /// 评分对象列表
        /// </summary>
        public new static List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public new static List<Hou3Select350_ZhouQiZhongScore> currentNeedCalcList = new List<Hou3Select350_ZhouQiZhongScore>();

        public static List<string> Hou2_20Numer = new List<string>();
        public static new void InitData()
        {
            AllCode = new List<Code>();
            model350List = new List<Hou3Select350_ZhouQiZhongScore>();
            Hou3NumberCount = new Dictionary<string, int>();

            var RunSkipNumberStr = ConfigurationManager.AppSettings["RunSkipNumber"];
            if (!string.IsNullOrEmpty(RunSkipNumberStr) && int.TryParse(RunSkipNumberStr, out int runSkipNumber))
            {
                RunSkipNumber = runSkipNumber;
            }
        }
        /// <summary>
        /// 初始化号码
        /// </summary>
        /// <param name="code"></param>
        public static void InitCode(Code code, bool zhongHouDelete = false)
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

            Hou2_20Numer = Generate20Code();
            // 计算已有号码的中挂情况
            //CalcExistCode(code);
            CalcExist350Code(code);

            // 生成新的350码
            //GenerateCode(code);
            if (AllCode != null && AllCode.Count > RunSkipNumber)
            {
                //删除超过3000条的记录 如果没有4挂的就删除
                RemoveOldModel(code, 1);

                //如果不是中后删除模式,则需要生成新的号码,如果是中后删除,则不生成新的号码
                if (!zhongHouDelete)
                {
                    GenerateCode(code);
                }
            }
            Hou2Selct27Three18SetFormBusiness.code = code;
        }

        public new static void CalcExist350Code(Code code)
        {
            var hou3 = code.GetHou3String();
            foreach (var model in model350List)
            {
                if (model.Number350.Contains(hou3))
                {
                    //中了
                    model.ZhongGount++;
                    model.NeedZhong = false;
                    model.Zhong3BeforeGua = model.Zhong2BeforeGua;
                    model.Zhong2BeforeGua = model.ZhongBeforeGua;
                    model.ZhongBeforeGua = model.GuaCount;

                    model.GuaCount = 0;

                    // 判断是否在中后周期内
                    if (model.Zhong2BeforeGua >= 2 && model.ZhongBeforeGua <= 1)
                    {

                        model.IsZhouQiZhongHou = true;
                    }
                    else
                    {
                        model.IsZhouQiZhongHou = false;
                    }

                    //判断是否周期内挂
                    if (model.Zhong3BeforeGua >= 2 && model.Zhong2BeforeGua <= 1 && model.ZhongBeforeGua >= 2)
                    {
                        model.ZhouQiZhongHouGua++;
                    }

                    if (model.Zhong2BeforeGua <= 1 && model.ZhongBeforeGua <= 1)
                    {
                        model.ZhouQiZhongHouGua = 0;
                    }
                }
                else
                {
                    //挂了
                    model.GuaCount++;
                    model.ZhongGount = 0;
                    if (model.Zhong2BeforeGua >= 2 && model.ZhongBeforeGua <= 1)
                    {
                        model.IsZhouQiZhongHou = true;
                    }
                    if (model.GuaCount >= 2)
                    {
                        //挂超过2次后就不是周期内
                        model.IsZhouQiZhongHou = false;
                    }
                }

                //计算当前期的K线
                var record = new Hou3Select270_ZhouQiZhong();
                record.Number270 = model.Number350;
                record.CodeNumber = model.CodeNumber;
                record.CodeQiHao = model.CodeQiHao;
                record.ZhouQiZhongHouGua = model.ZhouQiZhongHouGua;
                record.ZhongGount = model.ZhongGount;
                record.ZhouQiZhongHouGua = model.ZhouQiZhongHouGua;
                record.IsZhouQiZhongHou = model.IsZhouQiZhongHou;
                record.KLineList = model.KLineList;
                record.ZhongGount = model.ZhongGount;
                record.ZhongBeforeGua = model.ZhongBeforeGua;
                record.Zhong2BeforeGua = model.Zhong2BeforeGua;
                record.Zhong3BeforeGua = model.Zhong3BeforeGua;
                KLine270Calc.CalcKlineCurrent(record, code);
                model.KLineList = record.KLineList;
            }
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static void RemoveOldModel(Code code, int guaCount = 4)
        {
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou3Select350_ZhouQiZhongScore> removeList = new List<Hou3Select350_ZhouQiZhongScore>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }
            //删除全部的记录,每次生成新的50个号码
            while (model350List.Count >= 1)
            {
                model350List.RemoveAt(0);
            }

        }
        public static void CalcExistCode(Code code)
        {
            var hou2 = code.GetHou2String();
            foreach (var model in model350List)
            {
                if (model.Number350.Contains(hou2))
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
                    var AllCode = Hou2Selct27Three18SetFormBusiness.AllCode;
                    List<Code> LeftCode = new List<Code>();
                    LeftCode.AddRange(AllCode);

                    Cacl(2, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(2, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(3, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(1, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(0, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(1, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(0, ref LeftCode, ref excludeAllList, ref takeCodeList);
                    Cacl(4, ref LeftCode, ref excludeAllList, ref takeCodeList);

                    takeCodeList = takeCodeList.Distinct().ToList();
                    excludeAllList = excludeAllList.Distinct().ToList();
                    if (!excludeAllList.Any(item => takeCodeList.Contains(item)))
                    {
                        /*
                        var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);

                        numerList = numerList.OrderBy(item => item).ToList();

                        */
                        var numerList = MultiThreadedNumberSelectForYiLou3Xing.GenerateMultipleGroups(50, excludeAllList, takeCodeList, code.GetNumberCount);
                        foreach (var number in numerList)
                        {
                            if (number.Count > 0)
                            {
                                var list = number.OrderBy(p => p).ToList();
                                Hou3Select350_ZhouQiZhongScore model = new Hou3Select350_ZhouQiZhongScore();
                                model.Number350 = list;
                                model.CodeNumber = code.CodeNumber;
                                model.CodeQiHao = code.CodeQiHao;
                                model.NeedZhong = true;

                                Hou3Select270_ZhouQiZhong model270 = new Hou3Select270_ZhouQiZhong();
                                model270.Number270 = list;
                                model270.CodeNumber = code.CodeNumber;
                                model270.CodeQiHao = code.CodeQiHao;
                                model270.NeedZhong = true;
                                model270.KLineList = new List<KLine>();
                                KLine270Calc.CalcKLineHistoryList(model270, AllCode, 100);
                                model.KLineList = model270.KLineList;

                                model350List.Add(model);
                            }
                        }
                        break;
                    }

                    count++;
                    if (count > 500)
                    {
                        // 如果计算500次还没有有效数据,则退出
                        break;
                    }

                }
            }
        }

        public static void Cacl(int n1, ref List<Code> LeftCode, ref List<string> excludeAllList, ref List<string> takeCodeList)
        {

            if (n1 > 0)
            {
                var excludeList = GenerateHou3NumbereFromCode(n1, LeftCode);
                excludeAllList.AddRange(excludeList);
                LeftCode = GetCodeFromOriginExceptNumerCode(LeftCode, excludeAllList, n1);
            }
            if (LeftCode.Count > 0)
            {
                takeCodeList.Add(LeftCode.Take(1).FirstOrDefault().GetHou3String());
                LeftCode = LeftCode.Skip(1).ToList();
            }
        }


        /// <summary>
        /// 从Code列表中生成指定数量的后2号码,已经做了滤重操作
        /// </summary>
        /// <param name="number"></param>
        /// <param name="AllCode"></param>
        /// <returns></returns>
        public static List<string> GenerateHou3NumbereFromCode(int number, List<Code> AllCode)
        {
            Dictionary<string, int> Hou3NumberCount = new Dictionary<string, int>();
            Hou3NumberCount.Clear();
            foreach (var code in AllCode)
            {
                var key = code.GetHou3String();
                if (!Hou3NumberCount.Keys.Contains(key))
                {
                    Hou3NumberCount.Add(key, 1);
                }
                else
                {
                    Hou3NumberCount[key] = Hou3NumberCount[key] + 1;
                }
                if (Hou3NumberCount.Keys.Count == number)
                {
                    break;
                }
            }
            return Hou3NumberCount.Keys.ToList();
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
                var hou3Str = code.GetHou3String();
                if (exceptList.Contains(hou3Str))
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
        /// 根据历史记录的后二生成20码
        /// </summary>
        /// <returns></returns>
        public static List<string> Generate20Code()
        {
            Hou3NumberCount.Clear();
            foreach (var code in AllCode)
            {
                var key = code.GetHou3String();
                if (!Hou3NumberCount.Keys.Contains(key))
                {
                    Hou3NumberCount.Add(key, 1);
                }
                else
                {
                    Hou3NumberCount[key] = Hou3NumberCount[key] + 1;
                }
                if (Hou3NumberCount.Keys.Count == 20)
                {
                    break;
                }
            }
            return Hou3NumberCount.Keys.ToList();
        }


    }
}
