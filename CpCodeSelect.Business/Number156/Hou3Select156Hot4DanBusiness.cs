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

namespace CpCodeSelect.Business.Number156
{
    public class NumberAndCount
    {
        /// <summary>
        /// 数字 0-9
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 对应数字的出现次数,根据提供的后3列表进行统计,比如提供最近20期的后3列表,就统计这20期的后3号码中每个数字出现的次数
        /// </summary>
        public int Count { get; set; }
    }

    public class Hou3Select156Hot4DanBusiness : Hou3Select350YiLouSetFormZhongParentBusiness
    {
        /// <summary>
        /// 评分对象列表
        /// </summary>
        public new static List<Hou3Select350_ZhouQiZhongScore> model350List = new List<Hou3Select350_ZhouQiZhongScore>();
        public new static List<Hou3Select350_ZhouQiZhongScore> currentNeedCalcList = new List<Hou3Select350_ZhouQiZhongScore>();
        public static new void InitData()
        {
            AllCode = new List<Code>();
            model350List = new List<Hou3Select350_ZhouQiZhongScore>();
            Hou3NumberCount = new Dictionary<string, int>();

            RunSkipNumber = 100;
            /*
            var RunSkipNumberStr= ConfigurationManager.AppSettings["RunSkipNumber"];
            if(!string.IsNullOrEmpty(RunSkipNumberStr) && int.TryParse(RunSkipNumberStr, out int runSkipNumber))
            {
                RunSkipNumber = runSkipNumber;
            }
            */
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

            // 计算已有号码的中挂情况
            //CalcExistCode(code);
            //CalcExist350Code(code);

            // 生成新的156码
            //GenerateCode(code);
            if (AllCode != null && AllCode.Count > RunSkipNumber)
            {
                //删除全部记录
                RemoveOldModel(code, 1);

                //生成新的156码
                Generate156Code(code);
            }
            Hou3Select156Hot4DanBusiness.code = code;
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static new void RemoveOldModel(Code code, int guaCount = 1)
        {
            //目前先删除所有的号码
            if (model350List.Count > 0)
                model350List.Clear();
            /*
            var codeDecimal = Convert.ToDecimal(code.CodeQiHao);
            List<Hou3Select350_ZhouQiZhongScore> removeList = new List<Hou3Select350_ZhouQiZhongScore>();
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
            */
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
        /// 生成156码,目前是根据最近的多少期号码进行判断冷热,取最热的4个码
        /// 然后4个码中至少出现2个,可以重复,同时必须是从小到大或者从大到小的
        /// </summary>
        /// <param name="code"></param>
        public static void Generate156Code(Code code)
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

                var hou3List = Hou3Select156Hot4DanBusiness.GenerateHou3NumbereFromCode(20);
                var dic = GenerateHotCountByHou3List(hou3List);
                List<int> dan4 = new List<int>();
                // 0,1,2取2个
                // 3,4,5取1个
                // 6,7,8,9取1个
                Random r=new Random();
                var number12 = r.Next(0, 3);
                var number3=r.Next(3, 6);
                var number4=r.Next(6, 10);
                //0,1,2取2个
                for (int i = 0; i < 3; i++)
                {
                    if (i != number12)
                    {
                        dan4.Add(dic[i].Number);
                    }
                }

                // 3,4,5取1个
                dan4.Add(dic[number3].Number);
                // 6,7,8,9取1个
                dan4.Add(dic[number4].Number);

                List<string> codeList = new List<string>();
                codeList = Generate156ByDan4(dan4);

                Hou3Select350_ZhouQiZhongScore model350 = new Hou3Select350_ZhouQiZhongScore();
                model350.Number350 = codeList;
                model350.CodeNumber = code.CodeNumber;
                model350.CodeQiHao = code.CodeQiHao;
                model350.NeedZhong = true;
                model350.KLineList = new List<KLine>();
                model350.ScoreDateList = new List<LotteryScoreData>();
                model350.YiLouKline350 = new List<YiLouKline350>();
                model350.YiLouTuLineList = new List<KLine>();
                KLine350ScoreCalc.CalcKLineHistoryList(model350, AllCode, 100);
                model350List.Add(model350);
            }
        }
        /// <summary>
        /// 通过提供的后3列表,生成每个号码的出现次数,并按照出现次数从大到小排序,如果出现次数相同则按照号码从小到大排序
        /// </summary>
        /// <param name="hou3List"></param>
        /// <returns></returns>
        public static List<NumberAndCount> GenerateHotCountByHou3List(List<string> hou3List)
        {
            var hotCountDict = new List<NumberAndCount>();
            var resultDic = new List<NumberAndCount>();
            for (var i = 0; i < 10; i++)
            {
                var record = new NumberAndCount()
                {
                    Number = i,
                    Count = 0
                };
                hotCountDict.Add(record);
            }
            //hotCountDict[1].
            foreach (var hou3 in hou3List)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (int.TryParse(hou3[i].ToString(), out int number))
                    {
                        hotCountDict[number].Count++;
                    }
                }
            }
            var list = hotCountDict.OrderByDescending(p => p.Count).ThenBy(p => p.Number).ToList();

            return list;
        }
        /// <summary>
        /// 根据传递的4个胆生成156注号码
        /// </summary>
        /// <param name="dan4"></param>
        /// <returns></returns>
        public static List<string> Generate156ByDan4(List<int> dan4)
        {
            var list = new List<string>();
            var allNumber = GenerateAllPossibleStrings();
            foreach (var number in allNumber)
            {
                if ((number[0] >= number[1] && number[0] >= number[2] && number[1] >= number[2])
                    || (number[0] <= number[1] && number[0] <= number[2] && number[1] <= number[2])
                        )
                {
                    var chuXianCount = 0;
                    //满足从小到大或者从大到小的条件
                    for (var i = 0; i < 3; i++)
                    {
                        var currentNumber = int.Parse(number[i].ToString());
                        if (dan4.Contains(currentNumber))
                        {
                            chuXianCount++;
                        }
                    }
                    //出现超过2次,含2次
                    if (chuXianCount >= 2)
                    {
                        list.Add(number);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取所有的后3号码,从000到999的字符串形式,共1000个号码
        /// </summary>
        /// <returns></returns>
        static HashSet<string> GenerateAllPossibleStrings()
        {
            var strings = new HashSet<string>();
            for (int i = 0; i < 1000; i++)
            {
                strings.Add(i.ToString("D3"));
            }
            return strings;
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
