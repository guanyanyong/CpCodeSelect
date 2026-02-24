using CpCodeSelect.Model;
using CpCodeSelect.Model.ExModel;
using CpCodeSelect.Model.Score;
using CpCodeSelect.Util;
using CpCodeSelect.Util.Scorer;
using CpCodeSelect.Util.Scorer156;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;

namespace CpCodeSelect.Business.Score
{
    public class Hou3Select156YiLouSetFormScoreAndChuShouBusiness : Hou3Select350YiLouSetFormZhongParentBusiness
    {
        /// <summary>
        /// 评分对象列表
        /// </summary>
        public new static List<Hou3Select156_ZhouQiZhongScore> model350List = new List<Hou3Select156_ZhouQiZhongScore>();
        public new static List<Hou3Select156_ZhouQiZhongScore> currentNeedCalcList = new List<Hou3Select156_ZhouQiZhongScore>();
        public static new void InitData()
        {
            AllCode = new List<Code>();
            model350List = new List<Hou3Select156_ZhouQiZhongScore>();
            Hou3NumberCount = new Dictionary<string, int>();

            var RunSkipNumberStr= ConfigurationManager.AppSettings["RunSkipNumber"];
            if(!string.IsNullOrEmpty(RunSkipNumberStr) && int.TryParse(RunSkipNumberStr, out int runSkipNumber))
            {
                RunSkipNumber = runSkipNumber;
            }
        }
        /// <summary>
        /// 初始化号码
        /// </summary>
        /// <param name="code"></param>
        public static new void InitCode(Code code, bool zhongHouDelete = false)
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
            CalcExist156Code(code);

            // 生成新的156码
            //GenerateCode(code);
            if (AllCode != null && AllCode.Count > RunSkipNumber)
            {
                //删除超过3000条的记录 如果没有4挂的就删除
                RemoveOldModel(code, 1);

                //如果不是中后删除模式,则需要生成新的号码,如果是中后删除,则不生成新的号码
                //if (!zhongHouDelete)
                //{
                    Generate350Code(code);
                //}
            }
            Hou3Select350YiLouSetFormScoreAndChuShouBusiness.code = code;
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static new void RemoveOldModel(Code code, int guaCount = 1)
        {
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou3Select156_ZhouQiZhongScore> removeList = new List<Hou3Select156_ZhouQiZhongScore>();
            int count = 0;
            int numberCount = 1000;
            if (!int.TryParse(leftNumberCountStr, out numberCount))
            {
                numberCount = 1000;

            }

            while (model350List.Count > numberCount)
            {
                for (int i = 0; i < 2; i++)
                {
                    var model = model350List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 100 )
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
        public static new void CalcExist156Code(Code code)
        {
            var hou3 = code.GetHou3String();
            foreach (var model in model350List)
            {
                CalcCode(code, model, hou3);
            }
            if (currentNeedCalcList != null)
            {
                foreach (var model in currentNeedCalcList)
                {
                    CalcCode(code, model, hou3);
                }
            }
        }


        private static void CalcCode(Code code, Hou3Select156_ZhouQiZhongScore model, string hou3)
        {
            if (model != null && model.ScoreDateList == null)
            {
                model.ScoreDateList = new List<LotteryScoreData>();
            }
            if (model.Number156.Contains(hou3))
            {
                //中了
                model.ZhongGount++;
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
            KLine156ScoreCalc.CalcKlineCurrent(model, code);
        }

        public static new void Generate350Code(Code code)
        {

            if (AllCode != null && AllCode.Count > RunSkipNumber)
            {
                int count = 0;
                //while (true)
                //{
                var takeCodeList = new List<string>();
                var excludeAllList = new List<string>();

                /*
                var numerList = NumberSelectForYiLou.Select50NumbersSafe(excludeAllList, takeCodeList);

                numerList = numerList.OrderBy(item => item).ToList();

                */
                int numberCount = 1000;
                if (!int.TryParse(leftNumberCountStr, out numberCount))
                {
                    numberCount = 1000;

                }

                if (model350List.Count >= numberCount)
                    return;
                var hou3List = Hou3Select156YiLouSetFormScoreAndChuShouBusiness.GenerateHou3NumbereFromCode(270);
                //var numerList = MultiThreadedNumberSelectFor350Hou3.GenerateMultipleGroups(hou3List, 50);
                var numerList = MultiThreadedNumberSelectFor156Hou3ZiRanGenerate.GenerateMultipleGroups(hou3List, 3);
                foreach (var number in numerList)
                {
                    if (number.Count > 0)
                    {
                        var list = number.OrderBy(p => p).ToList();


                        Hou3Select156_ZhouQiZhongScore model156 = new Hou3Select156_ZhouQiZhongScore();
                        model156.Number156 = list;
                        model156.CodeNumber = code.CodeNumber;
                        model156.CodeQiHao = code.CodeQiHao;
                        model156.NeedZhong = true;
                        model156.KLineList = new List<KLine156>();
                        model156.ScoreDateList = new List<LotteryScoreData>();
                        model156.YiLouKline350 = new List<YiLouKline350>();
                        model156.YiLouTuLineList = new List<KLine156>();
                        KLine156ScoreCalc.CalcKLineHistoryList(model156, AllCode, 100);
                        model350List.Add(model156);
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

    }
}
