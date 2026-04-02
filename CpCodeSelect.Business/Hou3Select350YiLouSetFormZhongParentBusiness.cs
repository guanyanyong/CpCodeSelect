using CpCodeSelect.Business;
using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect
{
    public class Hou3Select350YiLouSetFormZhongParentBusiness
    {
        public static List<Code> AllCode = new List<Code>();
        public static Dictionary<string, int> Hou3NumberCount = new Dictionary<string, int>();
        public static Code code = null;
        public static Object lockObj = new Object();
        public static List<Hou3Select350_ZhouQiZhong> model350List = new List<Hou3Select350_ZhouQiZhong>();
        public static string leftNumberCountStr = ConfigurationManager.AppSettings["LeftNumberCount"];
        public static string touziAmountBeiStr = ConfigurationManager.AppSettings["TouZiAmountBei"];
        public static List<Hou3Select350_ZhouQiZhong> currentNeedCalcList = new List<Hou3Select350_ZhouQiZhong>();
        public static int RunSkipNumber=100;
        public static int TouZiAmountBei = 2;

        static Hou3Select350YiLouSetFormZhongParentBusiness()
        {
            int bei = 2;
            var TouZiAmountBeiStr = ConfigurationManager.AppSettings["TouZiAmountBei"];
            if (!string.IsNullOrEmpty(TouZiAmountBeiStr) && int.TryParse(TouZiAmountBeiStr, out bei))
            {
                TouZiAmountBei = bei;
            }
        }

        public static void InitData()
        {
            AllCode = new List<Code>();
            model350List = new List<Hou3Select350_ZhouQiZhong>();
            Hou3NumberCount = new Dictionary<string, int>();

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
            //CalcExist350Code(code);

            // 生成新的350码
            //GenerateCode(code);
            //if (AllCode != null && AllCode.Count > 1620)
            //{
            //    Generate350Code(code);
            //    //删除超过3000条的记录 如果没有4挂的就删除
            //    RemoveOldModel(code, 1);
            //}
            Hou3Select350YiLouSetFormZhongParentBusiness.code = code;
        }

        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static void RemoveOldModel(Code code, int guaCount = 1)
        {
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou3Select350_ZhouQiZhong> removeList = new List<Hou3Select350_ZhouQiZhong>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }

            while (model350List.Count >= numberCount)
            {
                for (int i = 0; i < 50; i++)
                {
                    var model = model350List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 10 && model.ZhouQiZhongHouGua <= guaCount && model.GuaCount <= 6)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    model350List.Remove(model);
                }
                removeList.Clear();
                count++;
                // 如果执行10次号码还是大于1000,则退出循环
                if (count > 10)
                    break;
            }

            //超过30期就删除
            {
                for (int i = 0; i < model350List.Count; i++)
                {
                    var model = model350List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 30)
                    {
                        removeList.Add(model);
                    }
                }
                foreach (var model in removeList)
                {
                    model350List.Remove(model);
                }
                removeList.Clear();
            }
        }
        public static void CalcExist350Code(Code code)
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
                KLine350Calc.CalcKlineCurrent(model, code);
            }
        }

        public static void Generate500Code(Code code)
        {

            if (AllCode != null && AllCode.Count > 270)
            {
                //while (true)
                //{
                var takeCodeList = new List<string>();
                var excludeAllList = new List<string>();

                /*
                var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);

                numerList = numerList.OrderBy(item => item).ToList();

                */
                if (model350List == null || model350List.Count <= 0)
                {
                    var numerList = GetAllCombinations();
                    foreach (var number in numerList)
                    {
                        if (number.Count > 0)
                        {
                            var list = number.OrderBy(p => p).ToList();


                        }
                    }
                }

                //count++;
                //if (count > 3500)
                //{
                //    // 如果计算1000次还没有有效数据,则退出
                //    break;
                //}
            }
        }

        private static void Add(Code code,List<string> list)
        {

            Hou3Select350_ZhouQiZhong model350 = new Hou3Select350_ZhouQiZhong();
            model350.Number350 = list;
            model350.CodeNumber = code.CodeNumber;
            model350.CodeQiHao = code.CodeQiHao;
            model350.NeedZhong = true;
            model350.KLineList = new List<KLine>();
            model350.YiLouKline350 = new List<YiLouKline350>();
            model350.YiLouTuLineList = new List<KLine>();
            KLine350Calc.CalcKLineHistoryList(model350, AllCode, 100);
            model350List.Add(model350);
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


        /// <summary>
        /// 从0-9中选择5个字符的所有组合，返回List<List<string>>格式
        /// 每个内部List<string>包含5个数字字符串
        /// </summary>
        /// <returns>包含所有组合的列表，每个组合是包含5个字符串的List</returns>
        public static List<List<string>> GetAllCombinations()
        {
            string digits = "0123456789";
            List<List<string>> result = new List<List<string>>();

            // 使用递归生成组合
            GenerateCombinations(digits, new List<string>(), 0, 5, result);

            return result;
        }

        /// <summary>
        /// 递归生成组合的辅助方法
        /// </summary>
        /// <param name="digits">源字符串</param>
        /// <param name="current">当前构建的组合字符串列表</param>
        /// <param name="start">起始索引</param>
        /// <param name="k">还需要选择的字符数量</param>
        /// <param name="result">结果列表</param>
        private static void GenerateCombinations(string digits, List<string> current, int start, int k, List<List<string>> result)
        {
            // 如果已经选择了5个字符，添加到结果中
            if (k == 0)
            {
                result.Add(new List<string>(current));
                return;
            }

            // 从start开始选择字符
            for (int i = start; i <= digits.Length - k; i++)
            {
                current.Add(digits[i].ToString());
                GenerateCombinations(digits, current, i + 1, k - 1, result);
                current.RemoveAt(current.Count - 1); // 回溯
            }
        }

    }
}
