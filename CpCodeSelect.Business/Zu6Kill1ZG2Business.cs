using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;
using static CpCodeSelect.Model.Zu6Kill1ZG2Model;

namespace CpCodeSelect.Business
{
    public static class Zu6Kill1ZG2Business
    {
        /// <summary>
        /// 挂后需要连中次数
        /// </summary>
        public static int GuaHouNeedZhongCount;
        /// <summary>
        /// 初始化大小单双
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

            code.Zu6Kill1ZG2ModelList = new List<Zu6Kill1ZG2Model>();

            var qiansan = new Zu6Kill1ZG2Model
            {
                Zu6Kill1Position = Zu6Kill1Position.前三,
                Name = "前三"
            };
            InitZu6Kill1ZG2Model(qiansan);
            SetKill1(code, qiansan);

            var zhongsan = new Zu6Kill1ZG2Model
            {
                Zu6Kill1Position = Zu6Kill1Position.中三,
                Name = "中三"
            };

            InitZu6Kill1ZG2Model(zhongsan);
            SetKill1(code, zhongsan);
            var housan = new Zu6Kill1ZG2Model
            {
                Zu6Kill1Position = Zu6Kill1Position.后三,
                Name = "后三"
            };
            InitZu6Kill1ZG2Model(housan);
            SetKill1(code, housan);
            code.Zu6Kill1ZG2ModelList.Add(qiansan);
            code.Zu6Kill1ZG2ModelList.Add(zhongsan);
            code.Zu6Kill1ZG2ModelList.Add(housan);

        }

        private static void InitZu6Kill1ZG2Model(Zu6Kill1ZG2Model model)
        {
            if (model != null && model.Zu6Kill1ZG2Items == null)
            {
                model.Zu6Kill1ZG2Items = new List<Zu6Kill1ZG2Model.Zu6Kill1ZG2Item>();
                for (int i = 0; i <= 9; i++)
                {
                    var item = new Zu6Kill1ZG2Model.Zu6Kill1ZG2Item()
                    {
                        Number = i,
                        GuaCount = 0,
                        IsLianGua = false,
                        LianZhongCount = 0
                    };
                    model.Zu6Kill1ZG2Items.Add(item);
                }
            }
        }
        /// <summary>
        /// 设置杀1码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static void SetKill1(Code code, Zu6Kill1ZG2Model model)
        {
            if (model.Zu6Kill1Position == Zu6Kill1Position.前三)
            {
                Zu6Kill1ZG2Model preModel = null;
                if (code.PreCode != null)
                    preModel = code.PreCode.Zu6Kill1ZG2ModelList.Where(p => p.Name == model.Name).FirstOrDefault();
                if (code.Wan.Number == code.Qian.Number || code.Qian.Number == code.Bai.Number || code.Wan.Number == code.Bai.Number)
                {
                    //如果出现对子,则表示不中
                    foreach (var item in model.Zu6Kill1ZG2Items)
                    {
                        if (preModel != null)
                        {
                            var preItem = preModel.Zu6Kill1ZG2Items.Where(p => p.Number == item.Number).FirstOrDefault();
                            if(preItem!=null && preItem.IsZG2)
                            {
                                item.IsZG2 = true;
                                //是中跟挂停的连挂
                                item.LianZhongCount = 0;
                                item.GuaCount = preItem.GuaCount + 1;
                                item.IsLianGua = true;
                                item.ZG2Count = preItem.ZG2Count;
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.IsLianGua = true;
                                item.GuaCount = preItem.GuaCount + 1;
                                if (item.GuaCount == 1)
                                {
                                    item.LianZhongCount = 0;
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                }
                                else
                                {

                                    item.LianZhongCount = 0;
                                    item.IsZG2 = true;
                                    item.ZG2Count = 0;
                                }
                            }
                        }
                        else
                        {
                            //没有之前的记录 第一次挂 和挂一个逻辑一样
                            item.IsLianGua = true;
                            item.GuaCount =  1;
                            item.LianZhongCount = 0;
                            item.IsZG2 = false;
                            item.ZG2Count = 0;
                        }
                    }
                }
                else
                {
                    //循环设置所有的杀1码
                    foreach (var item in model.Zu6Kill1ZG2Items)
                    {
                        Zu6Kill1ZG2Item preItem = null;
                        if (preModel != null)
                        {
                            preItem = preModel.Zu6Kill1ZG2Items.Where(p => p.Number == item.Number).FirstOrDefault();
                        }
                        if (item.Number == code.Wan.Number || item.Number == code.Qian.Number || item.Number == code.Bai.Number)
                        {
                            //出现挂
                            if (preItem != null && preItem.IsZG2)
                            {
                                item.IsZG2 = true;
                                //是中跟2中出现挂
                                item.LianZhongCount = 0;
                                item.GuaCount = preItem.GuaCount + 1;
                                item.IsLianGua = true;
                                item.ZG2Count = preItem.ZG2Count;
                            }
                            else if(preItem!=null)
                            {
                                //之前不是中跟挂停 现在挂了
                                item.IsLianGua = true;
                                item.GuaCount = preItem.GuaCount + 1;
                                if (item.GuaCount == 1)
                                {
                                    item.LianZhongCount = 0;
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                }
                                else
                                {

                                    item.LianZhongCount = 0;
                                    item.IsZG2 = true;
                                    item.ZG2Count = 0;
                                }
                            }
                            else
                            {
                                //之前没有记录 第一次挂 和挂一个逻辑一样
                                //没有之前的记录 第一次挂 和挂一个逻辑一样
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.LianZhongCount = 0;
                                item.IsZG2 = false;
                                item.ZG2Count = 0;
                            }
                        }
                        else
                        {
                            //号码出现了中
                            if (preItem != null )
                            {
                                if (preItem.IsZG2)
                                {
                                    //当前是中跟挂停中的中
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    if (preItem.GuaCount>1)
                                    {
                                        //
                                        item.IsZG2 = true;
                                        item.LianZhongCount = 1;
                                        item.ZG2Count = preItem.ZG2Count+1;
                                    }else if (preItem.GuaCount <= 1)
                                    {
                                        //中跟挂停结束
                                        item.IsZG2 = false;
                                        item.ZG2Count = 0;
                                        item.LianZhongCount = preItem.LianZhongCount;
                                    }
                                }
                                else
                                {
                                    //当前不是中跟挂停中的中
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                    item.LianZhongCount = preItem.LianZhongCount+1;
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                }
                            }
                            else
                            {
                                //出现中,并且之前没有记录
                                item.IsZG2 = false;
                                item.ZG2Count = 0;
                                item.LianZhongCount =  1;
                                item.IsLianGua = false;
                                item.GuaCount = 0;
                            }
                        }
                    }
                }
            }
            else if (model.Zu6Kill1Position == Zu6Kill1Position.中三)
            {
                Zu6Kill1ZG2Model preModel = null;
                if (code.PreCode != null)
                    preModel = code.PreCode.Zu6Kill1ZG2ModelList.Where(p => p.Name == model.Name).FirstOrDefault();
                if (code.Shi.Number == code.Qian.Number || code.Qian.Number == code.Bai.Number || code.Shi.Number == code.Bai.Number)
                {
                    //如果出现对子,则表示不中
                    foreach (var item in model.Zu6Kill1ZG2Items)
                    {
                        if (preModel != null)
                        {
                            var preItem = preModel.Zu6Kill1ZG2Items.Where(p => p.Number == item.Number).FirstOrDefault();
                            if (preItem != null && preItem.IsZG2)
                            {
                                item.IsZG2 = true;
                                //是中跟挂停的连挂
                                item.LianZhongCount = 0;
                                item.GuaCount = preItem.GuaCount + 1;
                                item.IsLianGua = true;
                                item.ZG2Count = preItem.ZG2Count;
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.IsLianGua = true;
                                item.GuaCount = preItem.GuaCount + 1;
                                if (item.GuaCount == 1)
                                {
                                    item.LianZhongCount = 0;
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                }
                                else
                                {

                                    item.LianZhongCount = 0;
                                    item.IsZG2 = true;
                                    item.ZG2Count = 0;
                                }
                            }
                        }
                        else
                        {
                            //没有之前的记录 第一次挂 和挂一个逻辑一样
                            item.IsLianGua = true;
                            item.GuaCount = 1;
                            item.LianZhongCount = 0;
                            item.IsZG2 = false;
                            item.ZG2Count = 0;
                        }
                    }
                }
                else
                {
                    //循环设置所有的杀1码
                    foreach (var item in model.Zu6Kill1ZG2Items)
                    {
                        Zu6Kill1ZG2Item preItem = null;
                        if (preModel != null)
                        {
                            preItem = preModel.Zu6Kill1ZG2Items.Where(p => p.Number == item.Number).FirstOrDefault();
                        }
                        if (item.Number == code.Shi.Number || item.Number == code.Qian.Number || item.Number == code.Bai.Number)
                        {
                            //出现挂
                            if (preItem != null && preItem.IsZG2)
                            {
                                item.IsZG2 = true;
                                //是中跟2中出现挂
                                item.LianZhongCount = 0;
                                item.GuaCount = preItem.GuaCount + 1;
                                item.IsLianGua = true;
                                item.ZG2Count = preItem.ZG2Count;
                            }
                            else if (preItem != null)
                            {
                                //之前不是中跟挂停 现在挂了
                                item.IsLianGua = true;
                                item.GuaCount = preItem.GuaCount + 1;
                                if (item.GuaCount == 1)
                                {
                                    item.LianZhongCount = 0;
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                }
                                else
                                {

                                    item.LianZhongCount = 0;
                                    item.IsZG2 = true;
                                    item.ZG2Count = 0;
                                }
                            }
                            else
                            {
                                //之前没有记录 第一次挂 和挂一个逻辑一样
                                //没有之前的记录 第一次挂 和挂一个逻辑一样
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.LianZhongCount = 0;
                                item.IsZG2 = false;
                                item.ZG2Count = 0;
                            }
                        }
                        else
                        {
                            //号码出现了中
                            if (preItem != null)
                            {
                                if (preItem.IsZG2)
                                {
                                    //当前是中跟挂停中的中
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    if (preItem.GuaCount > 1)
                                    {
                                        //
                                        item.IsZG2 = true;
                                        item.LianZhongCount = 1;
                                        item.ZG2Count = preItem.ZG2Count+1;
                                    }
                                    else if (preItem.GuaCount <= 1)
                                    {
                                        //中跟挂停结束
                                        item.IsZG2 = false;
                                        item.ZG2Count = 0;
                                        item.LianZhongCount = preItem.LianZhongCount;
                                    }
                                }
                                else
                                {
                                    //当前不是中跟挂停中的中
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                    item.LianZhongCount = preItem.LianZhongCount + 1;
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                }
                            }
                            else
                            {
                                //出现中,并且之前没有记录
                                item.IsZG2 = false;
                                item.ZG2Count = 0;
                                item.LianZhongCount =  1;
                                item.IsLianGua = false;
                                item.GuaCount = 0;
                            }
                        }
                    }
                }
            }
            else if (model.Zu6Kill1Position == Zu6Kill1Position.后三)
            {
                Zu6Kill1ZG2Model preModel = null;
                if (code.PreCode != null)
                    preModel = code.PreCode.Zu6Kill1ZG2ModelList.Where(p => p.Name == model.Name).FirstOrDefault();
                if (code.Shi.Number == code.Ge.Number || code.Shi.Number == code.Bai.Number || code.Ge.Number == code.Bai.Number)
                {
                    //如果出现对子,则表示不中
                    foreach (var item in model.Zu6Kill1ZG2Items)
                    {
                        if (preModel != null)
                        {
                            var preItem = preModel.Zu6Kill1ZG2Items.Where(p => p.Number == item.Number).FirstOrDefault();
                            if (preItem != null && preItem.IsZG2)
                            {
                                item.IsZG2 = true;
                                //是中跟挂停的连挂
                                item.LianZhongCount = 0;
                                item.GuaCount = preItem.GuaCount + 1;
                                item.IsLianGua = true;
                                item.ZG2Count = preItem.ZG2Count;
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.IsLianGua = true;
                                item.GuaCount = preItem.GuaCount + 1;
                                if (item.GuaCount == 1)
                                {
                                    item.LianZhongCount = 0;
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                }
                                else
                                {

                                    item.LianZhongCount = 0;
                                    item.IsZG2 = true;
                                    item.ZG2Count = 0;
                                }
                            }
                        }
                        else
                        {
                            //没有之前的记录 第一次挂 和挂一个逻辑一样
                            item.IsLianGua = true;
                            item.GuaCount = 1;
                            item.LianZhongCount = 0;
                            item.IsZG2 = false;
                            item.ZG2Count = 0;
                        }
                    }
                }
                else
                {
                    //循环设置所有的杀1码
                    foreach (var item in model.Zu6Kill1ZG2Items)
                    {
                        Zu6Kill1ZG2Item preItem = null;
                        if (preModel != null)
                        {
                            preItem = preModel.Zu6Kill1ZG2Items.Where(p => p.Number == item.Number).FirstOrDefault();
                        }
                        if (item.Number == code.Ge.Number || item.Number == code.Shi.Number || item.Number == code.Bai.Number)
                        {
                            //出现挂
                            if (preItem != null && preItem.IsZG2)
                            {
                                item.IsZG2 = true;
                                //是中跟2中出现挂
                                item.LianZhongCount = 0;
                                item.GuaCount = preItem.GuaCount + 1;
                                item.IsLianGua = true;
                                item.ZG2Count = preItem.ZG2Count;
                            }
                            else if (preItem != null)
                            {
                                //之前不是中跟挂停 现在挂了
                                item.IsLianGua = true;
                                item.GuaCount = preItem.GuaCount + 1;
                                if (item.GuaCount == 1)
                                {
                                    item.LianZhongCount = 0;
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                }
                                else
                                {

                                    item.LianZhongCount = 0;
                                    item.IsZG2 = true;
                                    item.ZG2Count = 0;
                                }
                            }
                            else
                            {
                                //之前没有记录 第一次挂 和挂一个逻辑一样
                                //没有之前的记录 第一次挂 和挂一个逻辑一样
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.LianZhongCount = 0;
                                item.IsZG2 = false;
                                item.ZG2Count = 0;
                            }
                        }
                        else
                        {
                            //号码出现了中
                            if (preItem != null)
                            {
                                if (preItem.IsZG2)
                                {
                                    //当前是中跟挂停中的中
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    if (preItem.GuaCount > 1)
                                    {
                                        //
                                        item.IsZG2 = true;
                                        item.LianZhongCount = 1;
                                        item.ZG2Count = preItem.ZG2Count + 1;
                                    }
                                    else if (preItem.GuaCount <= 1)
                                    {
                                        //中跟挂停结束
                                        item.IsZG2 = false;
                                        item.ZG2Count = 0;
                                        item.LianZhongCount = preItem.LianZhongCount;
                                    }
                                }
                                else
                                {
                                    //当前不是中跟挂停中的中
                                    item.IsZG2 = false;
                                    item.ZG2Count = 0;
                                    item.LianZhongCount = preItem.LianZhongCount + 1;
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                }
                            }
                            else
                            {
                                //出现中,并且之前没有记录
                                item.IsZG2 = false;
                                item.ZG2Count = 0;
                                item.LianZhongCount =  1;
                                item.IsLianGua = false;
                                item.GuaCount = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
