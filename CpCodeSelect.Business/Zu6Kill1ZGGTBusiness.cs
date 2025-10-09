using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CpCodeSelect.Model.Zu6Kill1Model;
using static CpCodeSelect.Model.Zu6Kill1ZGGTModel;

namespace CpCodeSelect.Business
{
    public static class Zu6Kill1ZGGTBusiness
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

            code.Zu6Kill1ZGGTModelList = new List<Zu6Kill1ZGGTModel>();

            var qiansan = new Zu6Kill1ZGGTModel
            {
                Zu6Kill1Position = Zu6Kill1Position.前三,
                Name = "前三"
            };
            InitZu6Kill1ZGGTModel(qiansan);
            SetKill1(code, qiansan);

            var zhongsan = new Zu6Kill1ZGGTModel
            {
                Zu6Kill1Position = Zu6Kill1Position.中三,
                Name = "中三"
            };

            InitZu6Kill1ZGGTModel(zhongsan);
            SetKill1(code, zhongsan);
            var housan = new Zu6Kill1ZGGTModel
            {
                Zu6Kill1Position = Zu6Kill1Position.后三,
                Name = "后三"
            };
            InitZu6Kill1ZGGTModel(housan);
            SetKill1(code, housan);
            code.Zu6Kill1ZGGTModelList.Add(qiansan);
            code.Zu6Kill1ZGGTModelList.Add(zhongsan);
            code.Zu6Kill1ZGGTModelList.Add(housan);

        }

        private static void InitZu6Kill1ZGGTModel(Zu6Kill1ZGGTModel model)
        {
            if (model != null && model.Zu6Kill1ZGGTItems == null)
            {
                model.Zu6Kill1ZGGTItems = new List<Zu6Kill1ZGGTModel.Zu6Kill1ZGGTItem>();
                for (int i = 0; i <= 9; i++)
                {
                    var item = new Zu6Kill1ZGGTModel.Zu6Kill1ZGGTItem()
                    {
                        Number = i,
                        GuaCount = 0,
                        IsLianGua = false,
                        LianZhongCount = 0
                    };
                    model.Zu6Kill1ZGGTItems.Add(item);
                }
            }
        }
        /// <summary>
        /// 设置杀1码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static void SetKill1(Code code, Zu6Kill1ZGGTModel model)
        {
            if (model.Zu6Kill1Position == Zu6Kill1Position.前三)
            {
                Zu6Kill1ZGGTModel preModel = null;
                if (code.PreCode != null)
                    preModel = code.PreCode.Zu6Kill1ZGGTModelList.Where(p => p.Name == model.Name).FirstOrDefault();
                if (code.Wan.Number == code.Qian.Number || code.Qian.Number == code.Bai.Number || code.Wan.Number == code.Bai.Number)
                {
                    //如果出现对子,则表示不中
                    foreach (var item in model.Zu6Kill1ZGGTItems)
                    {
                        if (preModel != null)
                        {
                            var preItem = preModel.Zu6Kill1ZGGTItems.Where(p => p.Number == item.Number).FirstOrDefault();
                            if(preItem!=null && preItem.IsZGGT)
                            {
                                item.IsZGGT = true;
                                //是中跟挂停中
                                if (preItem.IsLianGua)
                                {
                                    //是中跟挂停的连挂
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.GuaCount = preItem.GuaCount + 1;
                                    item.IsLianGua = true;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                }
                                else
                                {
                                    //不是中跟挂停的连挂,当前中跟挂停次数加1
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount + 1;
                                    item.GuaCount = 1;
                                    item.IsLianGua = true;
                                }
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.LianZhongCount = 0;
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.IsZGGT = true;
                                item.ZGGTGuaCount = 1;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                        else
                        {
                            //没有之前的记录 第一次中跟挂停
                            item.LianZhongCount = 0;
                            item.IsLianGua = true;
                            item.GuaCount = 1;
                            item.IsZGGT = true;
                            item.ZGGTGuaCount = 1;
                            item.ZGGTZhongCount = 0;
                        }
                    }
                }
                else
                {
                    //循环设置所有的杀1码
                    foreach (var item in model.Zu6Kill1ZGGTItems)
                    {
                        Zu6Kill1ZGGTItem preItem = null;
                        if (preModel != null)
                        {
                            preItem = preModel.Zu6Kill1ZGGTItems.Where(p => p.Number == item.Number).FirstOrDefault();
                        }
                        if (item.Number == code.Wan.Number || item.Number == code.Qian.Number || item.Number == code.Bai.Number)
                        {
                            if (preItem != null && preItem.IsZGGT)
                            {
                                item.IsZGGT = true;
                                //是中跟挂停中
                                if (preItem.IsLianGua)
                                {
                                    //是中跟挂停的连挂
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.GuaCount = preItem.GuaCount + 1;
                                    item.IsLianGua = true;
                                    item.ZGGTGuaCount=preItem.ZGGTGuaCount;
                                }
                                else
                                {
                                    //不是中跟挂停的连挂,当前中跟挂停次数加1
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount + 1;
                                    item.GuaCount = 1;
                                    item.IsLianGua = true;
                                }
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.LianZhongCount = 0;
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.IsZGGT = true;
                                item.ZGGTGuaCount = 1;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                        else
                        {
                            //号码出现了中
                            if (preItem != null )
                            {
                                if (preItem.IsZGGT)
                                {
                                    //当前是中跟挂停中的中
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    if (preItem.ZGGTZhongCount == 0)
                                    {
                                        item.ZGGTZhongCount = 1;
                                        item.IsZGGT = true;
                                        item.LianZhongCount = 1;
                                        item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                    }else if (preItem.ZGGTZhongCount > 0)
                                    {
                                        item.IsZGGT = false;
                                        item.ZGGTGuaCount = 0;
                                        item.ZGGTZhongCount = 0;
                                        item.LianZhongCount = 2;
                                    }
                                }
                                else
                                {
                                    //当前不是中跟挂停中的中
                                    item.IsZGGT = false;
                                    item.ZGGTGuaCount = 0;
                                    item.LianZhongCount = preItem.LianZhongCount+1;
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    item.ZGGTZhongCount = 0;
                                }
                            }
                            else
                            {
                                //出现中,并且之前没有记录
                                item.IsZGGT = false;
                                item.ZGGTGuaCount = 0;
                                item.LianZhongCount = 1;
                                item.IsLianGua = false;
                                item.GuaCount = 0;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                    }
                }
            }
            else if (model.Zu6Kill1Position == Zu6Kill1Position.中三)
            {
                Zu6Kill1ZGGTModel preModel = null;
                if (code.PreCode != null)
                    preModel = code.PreCode.Zu6Kill1ZGGTModelList.Where(p => p.Name == model.Name).FirstOrDefault();
                if (code.Shi.Number == code.Qian.Number || code.Qian.Number == code.Bai.Number || code.Shi.Number == code.Bai.Number)
                {
                    //如果出现对子,则表示不中
                    foreach (var item in model.Zu6Kill1ZGGTItems)
                    {
                        if (preModel != null)
                        {
                            var preItem = preModel.Zu6Kill1ZGGTItems.Where(p => p.Number == item.Number).FirstOrDefault();
                            if (preItem != null && preItem.IsZGGT)
                            {
                                item.IsZGGT = true;
                                //是中跟挂停中
                                if (preItem.IsLianGua)
                                {
                                    //是中跟挂停的连挂
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.GuaCount = preItem.GuaCount + 1;
                                    item.IsLianGua = true;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                }
                                else
                                {
                                    //不是中跟挂停的连挂,当前中跟挂停次数加1
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount + 1;
                                    item.GuaCount = 1;
                                    item.IsLianGua = true;
                                }
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.LianZhongCount = 0;
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.IsZGGT = true;
                                item.ZGGTGuaCount = 1;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                        else
                        {
                            //没有之前的记录 第一次中跟挂停
                            item.LianZhongCount = 0;
                            item.IsLianGua = true;
                            item.GuaCount = 1;
                            item.IsZGGT = true;
                            item.ZGGTGuaCount = 1;
                            item.ZGGTZhongCount = 0;
                        }
                    }
                }
                else
                {
                    //循环设置所有的杀1码
                    foreach (var item in model.Zu6Kill1ZGGTItems)
                    {
                        Zu6Kill1ZGGTItem preItem = null;
                        if (preModel != null)
                        {
                            preItem = preModel.Zu6Kill1ZGGTItems.Where(p => p.Number == item.Number).FirstOrDefault();
                        }
                        if (item.Number == code.Shi.Number || item.Number == code.Qian.Number || item.Number == code.Bai.Number)
                        {
                            if (preItem != null && preItem.IsZGGT)
                            {
                                item.IsZGGT = true;
                                //是中跟挂停中
                                if (preItem.IsLianGua)
                                {
                                    //是中跟挂停的连挂
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.GuaCount = preItem.GuaCount + 1;
                                    item.IsLianGua = true;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                }
                                else
                                {
                                    //不是中跟挂停的连挂,当前中跟挂停次数加1
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount + 1;
                                    item.GuaCount = 1;
                                    item.IsLianGua = true;
                                }
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.LianZhongCount = 0;
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.IsZGGT = true;
                                item.ZGGTGuaCount = 1;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                        else
                        {
                            //号码出现了中
                            if (preItem != null)
                            {
                                if (preItem.IsZGGT)
                                {
                                    //当前是中跟挂停中的中
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    if (preItem.ZGGTZhongCount == 0)
                                    {
                                        item.ZGGTZhongCount = 1;
                                        item.IsZGGT = true;
                                        item.LianZhongCount = 1;
                                        item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                    }
                                    else if (preItem.ZGGTZhongCount > 0)
                                    {
                                        item.IsZGGT = false;
                                        item.ZGGTGuaCount = 0;
                                        item.ZGGTZhongCount = 0;
                                        item.LianZhongCount = 2;
                                    }
                                }
                                else
                                {
                                    //当前不是中跟挂停中的中
                                    item.IsZGGT = false;
                                    item.ZGGTGuaCount = 0;
                                    item.LianZhongCount = preItem.LianZhongCount + 1;
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    item.ZGGTZhongCount = 0;
                                }
                            }
                            else
                            {
                                //出现中,并且之前没有记录
                                item.IsZGGT = false;
                                item.ZGGTGuaCount = 0;
                                item.LianZhongCount = 1;
                                item.IsLianGua = false;
                                item.GuaCount = 0;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                    }
                }
            }
            else if (model.Zu6Kill1Position == Zu6Kill1Position.后三)
            {
                Zu6Kill1ZGGTModel preModel = null;
                if (code.PreCode != null)
                    preModel = code.PreCode.Zu6Kill1ZGGTModelList.Where(p => p.Name == model.Name).FirstOrDefault();
                if (code.Shi.Number == code.Ge.Number || code.Shi.Number == code.Bai.Number || code.Ge.Number == code.Bai.Number)
                {
                    //如果出现对子,则表示不中
                    foreach (var item in model.Zu6Kill1ZGGTItems)
                    {
                        if (preModel != null)
                        {
                            var preItem = preModel.Zu6Kill1ZGGTItems.Where(p => p.Number == item.Number).FirstOrDefault();
                            if (preItem != null && preItem.IsZGGT)
                            {
                                item.IsZGGT = true;
                                //是中跟挂停中
                                if (preItem.IsLianGua)
                                {
                                    //是中跟挂停的连挂
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.GuaCount = preItem.GuaCount + 1;
                                    item.IsLianGua = true;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                }
                                else
                                {
                                    //不是中跟挂停的连挂,当前中跟挂停次数加1
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount + 1;
                                    item.GuaCount = 1;
                                    item.IsLianGua = true;
                                }
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.LianZhongCount = 0;
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.IsZGGT = true;
                                item.ZGGTGuaCount = 1;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                        else
                        {
                            //没有之前的记录 第一次中跟挂停
                            item.LianZhongCount = 0;
                            item.IsLianGua = true;
                            item.GuaCount = 1;
                            item.IsZGGT = true;
                            item.ZGGTGuaCount = 1;
                            item.ZGGTZhongCount = 0;
                        }
                    }
                }
                else
                {
                    //循环设置所有的杀1码
                    foreach (var item in model.Zu6Kill1ZGGTItems)
                    {
                        Zu6Kill1ZGGTItem preItem = null;
                        if (preModel != null)
                        {
                            preItem = preModel.Zu6Kill1ZGGTItems.Where(p => p.Number == item.Number).FirstOrDefault();
                        }
                        if (item.Number == code.Shi.Number || item.Number == code.Ge.Number || item.Number == code.Bai.Number)
                        {
                            if (preItem != null && preItem.IsZGGT)
                            {
                                item.IsZGGT = true;
                                //是中跟挂停中
                                if (preItem.IsLianGua)
                                {
                                    //是中跟挂停的连挂
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.GuaCount = preItem.GuaCount + 1;
                                    item.IsLianGua = true;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                }
                                else
                                {
                                    //不是中跟挂停的连挂,当前中跟挂停次数加1
                                    item.LianZhongCount = 0;
                                    item.ZGGTZhongCount = 0;
                                    item.ZGGTGuaCount = preItem.ZGGTGuaCount + 1;
                                    item.GuaCount = 1;
                                    item.IsLianGua = true;
                                }
                            }
                            else
                            {
                                //之前不是中跟挂停 现在挂了
                                item.LianZhongCount = 0;
                                item.IsLianGua = true;
                                item.GuaCount = 1;
                                item.IsZGGT = true;
                                item.ZGGTGuaCount = 1;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                        else
                        {
                            //号码出现了中
                            if (preItem != null)
                            {
                                if (preItem.IsZGGT)
                                {
                                    //当前是中跟挂停中的中
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    if (preItem.ZGGTZhongCount == 0)
                                    {
                                        item.ZGGTZhongCount = 1;
                                        item.IsZGGT = true;
                                        item.LianZhongCount = 1;
                                        item.ZGGTGuaCount = preItem.ZGGTGuaCount;
                                    }
                                    else if (preItem.ZGGTZhongCount > 0)
                                    {
                                        item.IsZGGT = false;
                                        item.ZGGTGuaCount = 0;
                                        item.ZGGTZhongCount = 0;
                                        item.LianZhongCount = 2;
                                    }
                                }
                                else
                                {
                                    //当前不是中跟挂停中的中
                                    item.IsZGGT = false;
                                    item.ZGGTGuaCount = 0;
                                    item.LianZhongCount = preItem.LianZhongCount+1;
                                    item.IsLianGua = false;
                                    item.GuaCount = 0;
                                    item.ZGGTZhongCount = 0;
                                }
                            }
                            else
                            {
                                //出现中,并且之前没有记录
                                item.IsZGGT = false;
                                item.ZGGTGuaCount = 0;
                                item.LianZhongCount = 1;
                                item.IsLianGua = false;
                                item.GuaCount = 0;
                                item.ZGGTZhongCount = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
