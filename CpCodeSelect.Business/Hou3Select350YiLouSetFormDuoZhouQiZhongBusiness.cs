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
    public class Hou3Select350YiLouSetFormDuoZhouQiZhongBusiness: Hou3Select350YiLouSetFormZhongParentBusiness
    {
        
        /// <summary>
        /// 初始化号码
        /// </summary>
        /// <param name="code"></param>
        public static new void InitCode(Code code)
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
            if (AllCode != null && AllCode.Count > 1620)
            {
                Generate350Code(code);
                //删除超过3000条的记录 如果没有4挂的就删除
                RemoveOldModel(code, 1);
            }
            Hou3Select350YiLouSetFormDuoZhouQiZhongBusiness.code = code;
        }
        /// <summary>
        ///  删除小于指定挂数的旧记录
        /// </summary>
        /// <param name="guaCount"></param>
        public static new void RemoveOldModel(Code code, int guaCount = 1)
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
                    if (codeDecimal - currentDecimal >= 20 && model.ZhouQiZhongHouGua <= guaCount)
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
        public static new void CalcExist350Code(Code code)
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

        private static void CalcCode(Code code, Hou3Select350_ZhouQiZhong model,string hou3)
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
            //KLine350Calc.CalcYiLou(model, code);
        }

        public static new void Generate350Code(Code code)
        {

            if (AllCode != null && AllCode.Count > 1620)
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
                var hou3List = Hou3Select350YiLouSetFormDuoZhouQiZhongBusiness.GenerateHou3NumbereFromCode(1620);
                //var list = StringSelection.Generate(hou3List);
                //var numerList = DuozhouqiStringSelection.GetBatchResults(hou3List);

                //var numerList = LayeredStringSelection.GetBatchResults(hou3List);
                var numerList = new StringCollectionProcessor.CollectionGenerator().GenerateMultipleCollectionsE(hou3List);
                foreach (var number in numerList)
                {
                    if (number != null && number.CollectionE != null && number.CollectionE.Count > 0)
                    {
                        var list = number.CollectionE.OrderBy(p => p).ToList();


                        Hou3Select350_ZhouQiZhong model350 = new Hou3Select350_ZhouQiZhong();
                        model350.Number350 = list;
                        model350.CodeNumber = code.CodeNumber;
                        model350.CodeQiHao = code.CodeQiHao;
                        model350.NeedZhong = true;
                        model350.KLineList = new List<KLine>();
                        model350.YiLouKline350 = new List<YiLouKline350>();
                        model350.YiLouTuLineList = new List<KLine>();
                        model350.YiLouTuLineList=new List<KLine>();
                        KLine350Calc.CalcKLineHistoryList(model350, AllCode, 100);
                        model350List.Add(model350);
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
