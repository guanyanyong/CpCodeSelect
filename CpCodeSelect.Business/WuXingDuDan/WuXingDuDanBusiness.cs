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
using CpCodeSelect.Business.Score;
using CpCodeSelect.Util.KLine5XingDuDan;

namespace CpCodeSelect.Business.WuXingDuDan
{
    public class WuXingDuDanBusiness : Hou3Select350YiLouSetFormZhongParentBusiness
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


            //先删除记录

            //删除超过3000条的记录 如果没有4挂的就删除
            //RemoveOldModel(code, 1);

            // 计算已有号码的中挂情况
            //CalcExistCode(code);
            CalcExist156Code(code);

            // 生成新的156码

            GenerateDuDanCode(code);

            WuXingDuDanBusiness.code = code;
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

            while (model350List.Count >= numberCount)
            {
                for (int i = 0; i < 2; i++)
                {
                    var model = model350List[i];
                    var currentDecimal = Convert.ToDecimal(model.CodeQiHao);
                    if (codeDecimal - currentDecimal >= 150 )
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
            
            foreach (var model in model350List)
            {
                CalcCode(code, model, code.CodeNumber);
            }
            if (currentNeedCalcList != null)
            {
                foreach (var model in currentNeedCalcList)
                {
                    CalcCode(code, model, code.CodeNumber);
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
            KLine40951ScoreCalc.CalcKlineCurrent(model, code);
        }

        public static void GenerateDuDanCode(Code code)
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

                if (model350List.Count <= 0)
                {
                    //如果没有号码,生成胆0-9的号码
                    for (var i= 0;i< 10; i++)
                    {
                        var list = GetDanAllString(i);
                        Hou3Select156_ZhouQiZhongScore model156 = new Hou3Select156_ZhouQiZhongScore();
                        model156.Number156 = list.ToList();
                        model156.CodeNumber = code.CodeNumber;
                        model156.CodeQiHao = code.CodeQiHao;
                        model156.NeedZhong = true;
                        model156.KLineList = new List<KLine156>();
                        model156.ScoreDateList = new List<LotteryScoreData>();
                        model156.YiLouKline350 = new List<YiLouKline350>();
                        model156.YiLouTuLineList = new List<KLine156>();
                        model156.DanNumber = i.ToString();
                        KLine40951ScoreCalc.CalcKLineHistoryList(model156, AllCode, 100);
                        model350List.Add(model156);

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

        // 生成所有可能的字符串 (000-999)
        public static HashSet<string> GenerateAllPossibleStrings()
        {
            var strings = new HashSet<string>();
            for (int i = 0; i < 100000; i++)
            {
                strings.Add(i.ToString("D5"));
            }
            return strings;
        }
        /// <summary>
        /// 返回指定胆的所有号码
        /// </summary>
        /// <param name="danNumber"></param>
        /// <returns></returns>

        public static HashSet<string> GetDanAllString(int danNumber)
        {
            var list = GenerateAllPossibleStrings();
            var resultList=new HashSet<string>();
            foreach(var record in list)
            {
                if (record.IndexOf(danNumber.ToString()) != -1)
                {
                    resultList.Add(record);
                }
            }
            return resultList;
        }


    }
}
