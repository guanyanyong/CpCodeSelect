using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;

namespace CpCodeSelect.Business
{
    public static class Hou3Select270YiLouSetFormZhouQiZhongBusiness
    {
        public static List<Code> AllCode = new List<Code>();
        public static Dictionary<string, int> Hou2NumberCount = new Dictionary<string, int>();
        public static Code code = null;
        public static Object lockObj = new Object();
        public static List<Hou3Select270_ZhouQiZhong> model270List = new List<Hou3Select270_ZhouQiZhong>();
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
            //Hou2_20Numer = Generate20Code();

            // 计算已有号码的中挂情况
            //CalcExistCode(code);
            CalcExist350Code(code);

            // 生成新的270码
            //GenerateCode(code);
            if (AllCode != null && AllCode.Count > 270)
            {
                Generate270Code(code);
                //删除超过3000条的记录 如果没有4挂的就删除
                RemoveOldModel(code, 1);
            }
            Hou3Select350YiLouSetFormZhouQiZhongBusiness.code = code;
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static void RemoveOldModel(Code code, int guaCount = 1)
        {
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou3Select270_ZhouQiZhong> removeList = new List<Hou3Select270_ZhouQiZhong>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }

            while (model270List.Count >= numberCount)
            {
                for (int i = 0; i < 50; i++)
                {
                    var model = model270List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 10 && model.ZhouQiZhongHouGua <= guaCount)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    model270List.Remove(model);
                }
                removeList.Clear();
                count++;
                // 如果执行10次号码还是大于1000,则退出循环
                if (count > 10)
                    break;
            }

            //超过30期就删除
            {
                for (int i = 0; i < model270List.Count; i++)
                {
                    var model = model270List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 30)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    model270List.Remove(model);
                }
                removeList.Clear();
            }
        }
        public static void CalcExist350Code(Code code)
        {
            var hou3 = code.GetHou3String();
            foreach (var model in model270List)
            {
                if (model.Number270.Contains(hou3))
                {
                    //中了
                    model.ZhongGount++;
                    model.NeedZhong = false;
                    model.Zhong3BeforeGua = model.Zhong2BeforeGua;
                    model.Zhong2BeforeGua = model.ZhongBeforeGua;
                    model.ZhongBeforeGua = model.GuaCount;

                    model.GuaCount = 0;

                    // 判断是否在中后周期内
                    if (model.Zhong2BeforeGua >= 3 && model.ZhongBeforeGua <= 2)
                    {

                        model.IsZhouQiZhongHou = true;
                    }
                    else
                    {
                        model.IsZhouQiZhongHou = false;
                    }

                    //判断是否周期内挂
                    if (model.Zhong3BeforeGua >= 3 && model.Zhong2BeforeGua <= 2 && model.ZhongBeforeGua >= 3)
                    {
                        model.ZhouQiZhongHouGua++;
                    }

                    if (model.Zhong2BeforeGua <= 2 && model.ZhongBeforeGua <= 2)
                    {
                        model.ZhouQiZhongHouGua = 0;
                    }
                }
                else
                {
                    //挂了
                    model.GuaCount++;
                    model.ZhongGount = 0;
                    if (model.Zhong2BeforeGua >= 3 && model.ZhongBeforeGua <= 2)
                    {
                        model.IsZhouQiZhongHou = true;
                    }
                    if (model.GuaCount >= 3)
                    {
                        //挂超过3次后就不是周期内
                        model.IsZhouQiZhongHou = false;
                    }
                }

                //计算当前期的K线
                KLine270Calc.CalcKlineCurrent(model, code);
            }
        }

        public static void Generate270Code(Code code)
        {

            if (AllCode != null && AllCode.Count > 270)
            {
                var takeCodeList = new List<string>();
                var excludeAllList = new List<string>();

                var hou3List = Hou3Select270YiLouSetFormZhouQiZhongBusiness.GenerateHou3NumbereFromCode(270);
                var numerList = MultiThreadedNumberSelectFor270Hou3.GenerateMultipleGroups(hou3List, 50);
                foreach (var number in numerList)
                {
                    if (number.Count > 0)
                    {
                        var list = number.OrderBy(p => p).ToList();


                        Hou3Select270_ZhouQiZhong model270 = new Hou3Select270_ZhouQiZhong();
                        model270.Number270 = list;
                        model270.CodeNumber = code.CodeNumber;
                        model270.CodeQiHao = code.CodeQiHao;
                        model270.NeedZhong = true;
                        model270.KLineList = new List<KLine>();
                        KLine270Calc.CalcKLineHistoryList(model270, AllCode, 100);
                        model270List.Add(model270);
                    }
                }
            }
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
