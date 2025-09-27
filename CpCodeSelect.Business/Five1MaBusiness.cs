using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;

namespace CpCodeSelect.Business
{
    public static class Five1MaBusiness
    {

        public static Dictionary<int, int> MaZhuanhuan = new Dictionary<int, int>();
        static Five1MaBusiness()
        {
            MaZhuanhuan.Add(0, 5);
            MaZhuanhuan.Add(1, 6);
            MaZhuanhuan.Add(2, 9);
            MaZhuanhuan.Add(3, 7);
            MaZhuanhuan.Add(4, 8);
            MaZhuanhuan.Add(5, 0);
            MaZhuanhuan.Add(6, 1);
            MaZhuanhuan.Add(7, 3);
            MaZhuanhuan.Add(8, 4);
            MaZhuanhuan.Add(9, 2);
        }
        /// <summary>
        /// 初始化5星1码
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

            code.Five1MaModelList = new List<Five1MaModel>();

            SetFive1Ma(code);


        }

        /// <summary>
        /// 设置5星1码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static void SetFive1Ma(Code code)
        {
            var preTuijianList = new List<Five1MaModel>();
            if (code.PreCode != null && code.PreCode.Five1MaModelList != null && code.PreCode.Five1MaModelList.Count > 0)
            {
                preTuijianList = code.PreCode.Five1MaModelList;
            }
            //处理看当期是否有对子
            Dictionary<int, int> dic = new Dictionary<int, int>();
            SetDictionaryAndTuijian(dic, code.Wan.Number, preTuijianList);
            SetDictionaryAndTuijian(dic, code.Qian.Number, preTuijianList);
            SetDictionaryAndTuijian(dic, code.Bai.Number, preTuijianList);
            SetDictionaryAndTuijian(dic, code.Shi.Number, preTuijianList);
            SetDictionaryAndTuijian(dic, code.Ge.Number, preTuijianList);

            foreach (var item in preTuijianList)
            {
                code.Five1MaModelList.Add(new Five1MaModel
                {
                    Number = item.Number,
                    GuaCount = item.GuaCount + 1,
                    QiHao = code.CodeQiHao,
                    CodeNumber = code.CodeNumber
                });
            }

            var duiZiList = dic.Where(x => x.Value > 1).Select(x => x.Key).ToList();
            if (duiZiList.Count > 0)
            {
                foreach (var item in duiZiList)
                {
                    var tuijianNumber = MaZhuanhuan[item];
                    if (preTuijianList.Where(p => p.Number == tuijianNumber).Count() == 0)
                    {
                        //如果推荐列表中没有，就添加 
                        code.Five1MaModelList.Add(new Five1MaModel
                        {
                            Number = tuijianNumber,
                            GuaCount = 0,
                            QiHao = code.CodeQiHao,
                            CodeNumber = code.CodeNumber
                        });
                    }
                }
            }

        }
        public static void SetDictionaryAndTuijian(Dictionary<int, int> dic, int number, List<Five1MaModel> tuijianList)
        {
            var five1MaModel= tuijianList.Where(p => p.Number == number).FirstOrDefault();
            if (five1MaModel!=null)
            {
                tuijianList.Remove(five1MaModel);
            }
            if (dic.ContainsKey(number))
            {
                dic[number] = dic[number] + 1;
            }
            else
            {
                dic[number] = 1;
            }
        }
    }
}
