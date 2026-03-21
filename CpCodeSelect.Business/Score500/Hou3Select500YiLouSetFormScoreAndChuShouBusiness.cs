using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.Scorer156;
using CpCodeSelect.Util.Scorer500;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;

namespace CpCodeSelect.Business.Score500
{
    public class Hou3Select500YiLouSetFormScoreAndChuShouBusiness : Hou3Select350YiLouSetFormZhongParentBusiness
    {
        /// <summary>
        /// 评分对象列表
        /// </summary>
        public new static List<Hou3Select500_ZhouQiZhongScore> model350List = new List<Hou3Select500_ZhouQiZhongScore>();
        public new static List<Hou3Select500_ZhouQiZhongScore> currentNeedCalcList = new List<Hou3Select500_ZhouQiZhongScore>();

        public static Object LockModel500List = new object();
        public static new void InitData()
        {
            AllCode = new List<Code>();
            model350List = new List<Hou3Select500_ZhouQiZhongScore>();
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
            // 生成新的156码
            Generate500Code(code);

            // 计算已有号码的中挂情况
            //CalcExistCode(code);
            CalcExist500Code(code);


            Hou3Select500YiLouSetFormScoreAndChuShouBusiness.code = code;
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static new void RemoveOldModel(Code code, int guaCount = 1)
        {
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou3Select500_ZhouQiZhongScore> removeList = new List<Hou3Select500_ZhouQiZhongScore>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }

            while (model350List.Count >= numberCount)
            {
                for (int i = 0; i < 2; i++)
                {
                    var model = model350List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 150)
                    {
                        removeList.Add(model);
                    }
                }
                lock (LockModel500List)
                {

                    foreach (var model in removeList)
                    {
                        model350List.Remove(model);
                    }
                }
                removeList.Clear();
                count++;
                // 如果执行10次号码还是大于1000,则退出循环
                if (count > 10)
                    break;
            }
            /*
            //超过30期就删除
            {
                var overNumberDelete = (numberCount / 50 + 10);
                if (overNumberDelete < 30) overNumberDelete = 30;
                for (int i = 0; i < model350List.Count; i++)
                {
                    var model = model350List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= overNumberDelete && model.ShouNumber < 1)
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
            */
        }
        /// <summary>
        /// 计算已有号码的中挂情况,同时计算K线数据
        /// </summary>
        /// <param name="code"></param>
        public static void CalcExist500Code(Code code)
        {
            foreach (var model in model350List.ToList())
            {
                CalcCode(code, model);
            }
            if (currentNeedCalcList != null)
            {
                foreach (var model in currentNeedCalcList)
                {
                    CalcCode(code, model);
                }
            }
        }


        private static void CalcCode(Code code, Hou3Select500_ZhouQiZhongScore model)
        {
            if (model != null && model.ScoreDateList == null)
            {
                model.ScoreDateList = new List<LotteryScoreData>();
            }
            if (
                model.PositionType == PositionType.万 && model.Number500.Contains(code.Wan.Number.ToString())
                || model.PositionType == PositionType.千 && model.Number500.Contains(code.Qian.Number.ToString())
                || model.PositionType == PositionType.百 && model.Number500.Contains(code.Bai.Number.ToString())
                || model.PositionType == PositionType.十 && model.Number500.Contains(code.Shi.Number.ToString())
                || model.PositionType == PositionType.个 && model.Number500.Contains(code.Ge.Number.ToString())
                )

            {
                //中了
                model.ZhongGount++;
                model.Zhong3BeforeGua = model.Zhong2BeforeGua;
                model.Zhong2BeforeGua = model.ZhongBeforeGua;
                model.ZhongBeforeGua = model.GuaCount;

                model.GuaCount = 0;

                //先设置之前的一个遗漏K
                if (model.YiLouKline500 != null && model.YiLouKline500.Count > 0)
                {
                    var klin = model.YiLouKline500[model.YiLouKline500.Count - 1];
                    klin.YiLouZhongCount = 0;
                    klin.YiLouGuaCount= model.ZhongBeforeGua;
                }

                //添加新的遗漏K
                YiLouKline500 kline500 = new YiLouKline500();
                kline500.Code500Code = model.Number500;
                kline500.CodeQiHao = code.CodeQiHao;
                kline500.CodeNumber = code.CodeNumber;
                kline500.YiLouGuaCount = 0;
                kline500.YiLouZhongCount= model.ZhongGount++;
                model.YiLouKline500.Add(kline500);


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

                if(model.YiLouKline500!=null && model.YiLouKline500.Count > 0)
                {

                    var klin = model.YiLouKline500[model.YiLouKline500.Count - 1];
                    klin.YiLouZhongCount = 0;
                    klin.YiLouGuaCount++;
                }

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
            KLine500ScoreCalc.CalcKlineCurrent(model, code);
        }

        public static new void Generate500Code(Code code)
        {
            if (model350List.Count > 0) return;
            if (AllCode != null && AllCode.Count > RunSkipNumber)
            {
                //while (true)
                //{
                var takeCodeList = new List<string>();
                var excludeAllList = new List<string>();

                /*
                var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);

                numerList = numerList.OrderBy(item => item).ToList();

                */
                var hou3List = Hou3Select500YiLouSetFormScoreAndChuShouBusiness.GenerateHou3NumbereFromCode(270);
                //var numerList = MultiThreadedNumberSelectFor350Hou3.GenerateMultipleGroups(hou3List, 50);
                var numerList = GetAllCombinations();
                lock(LockModel500List)
                {
                    foreach (var number in numerList)
                    {
                        if (number.Count > 0)
                        {
                            var list = number.OrderBy(p => p).ToList();

                            AddToList(list, PositionType.万);
                            AddToList(list, PositionType.千);
                            AddToList(list, PositionType.百);
                            AddToList(list, PositionType.十);
                            AddToList(list, PositionType.个);
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
        private static void AddToList(List<string> list ,PositionType positionType)
        {
            Hou3Select500_ZhouQiZhongScore model500 = new Hou3Select500_ZhouQiZhongScore();
            model500.Number500 = list;
            model500.CodeNumber = code.CodeNumber;
            model500.CodeQiHao = code.CodeQiHao;
            model500.NeedZhong = true;
            model500.PositionType = positionType;
            model500.KLineList = new List<KLine156>();
            model500.ScoreDateList = new List<LotteryScoreData>();
            model500.YiLouKline500 = new List<YiLouKline500>();
            model500.YiLouTuLineList = new List<KLine156>();
            KLine500ScoreCalc.CalcKLineHistoryList(model500, AllCode, 100);
            model350List.Add(model500);
        }
        /// <summary>
        /// 从Code列表中生成指定数量的后3号码,没有做滤重操作
        /// </summary>
        /// <param name="number"></param>
        /// <param name="AllCode"></param>
        /// <returns></returns>
        public static new List<string> GenerateHou3NumbereFromCode(int number)
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
