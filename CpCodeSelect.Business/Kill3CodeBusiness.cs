using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Business
{
    /// <summary>
    /// 杀3码的业务逻辑
    /// </summary>
    public static class Kill3CodeBusiness
    {
        /// <summary>
        /// 挂后需要连中次数
        /// </summary>
        public static int GuaHouNeedZhongCount = 2;
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

            code.Kill3ModelList = new List<Kill3Model>();
            var qiansan = new Kill3Model
            {
                Kill3Position = Kill3Position.前三,
                Name = "前三"
            };
            SetPosition(code, qiansan);

            var zhongsan = new Kill3Model
            {
                Kill3Position = Kill3Position.中三,
                Name = "中三"
            };

            SetPosition(code, zhongsan);
            var housan = new Kill3Model
            {
                Kill3Position = Kill3Position.后三,
                Name = "后三"
            };
            SetPosition(code, housan);
            code.Kill3ModelList.Add(qiansan);
            code.Kill3ModelList.Add(zhongsan);
            code.Kill3ModelList.Add(housan);

        }
        /// <summary>
        /// 是不是杀3中
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsKill3Zhong(Code code, Kill3Model model)
        {
            if (model.Kill3Position == Kill3Position.前三)
            {
                if (code.Wan.Number == code.PreCode.Wan.Number || code.Qian.Number == code.PreCode.Qian.Number || code.Bai.Number == code.PreCode.Bai.Number)
                {
                    return true;
                }
            }
            else if (model.Kill3Position == Kill3Position.中三)
            {
                if (code.Qian.Number == code.PreCode.Qian.Number || code.Bai.Number == code.PreCode.Bai.Number || code.Shi.Number == code.PreCode.Shi.Number)
                {
                    return true;
                }
            }
            else if (model.Kill3Position == Kill3Position.后三)
            {
                if (code.Bai.Number == code.PreCode.Bai.Number || code.Shi.Number == code.PreCode.Shi.Number || code.Ge.Number == code.PreCode.Ge.Number )
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 设置杀3码的位置信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>

        public static void SetPosition(Code code, Kill3Model model)
        {
            if (code.PreCode != null)
            {
                var kill3ModelBefore = code.PreCode.Kill3ModelList.Where(p => model.Name == model.Name).FirstOrDefault();
                if (kill3ModelBefore != null)
                {
                    if (!IsKill3Zhong(code,model))
                    {
                        //当前的号码和上期的号码,有相同的号码,则表示挂
                        model.IsLianGua = true;
                        if (kill3ModelBefore.IsLianGua)
                        {
                            //上期已经是连挂
                            model.GuaCount += 1;
                            model.LianZhongCount = 0;
                        }
                        else
                        {
                            //上期不是连挂
                            model.GuaCount = 1;
                            model.LianZhongCount = 0;
                        }
                    }
                    else
                    {
                        // 中出 当前的号码和上期的号码没有出现相同的
                        if (kill3ModelBefore.IsLianGua)
                        {
                            //如果是连挂,则设置连挂后的中出
                            model.GuaHouZhong += 1;
                            model.LianZhongCount = 0;
                            if (kill3ModelBefore.GuaHouZhong >= GuaHouNeedZhongCount)
                            {
                                //如果连挂后的中出次数,大于等于需要的中出次数,则表示连挂结束
                                model.IsLianGua = false;
                                model.GuaCount = 0;
                                model.GuaHouZhong = 0;
                                model.LianZhongCount = kill3ModelBefore.GuaHouZhong;
                            }
                        }
                        else
                        {
                            //如果之前不是连挂,则是连中
                            model.IsLianGua = false;
                            model.GuaCount = 0;
                            model.GuaHouZhong = 0;
                            model.LianZhongCount= kill3ModelBefore.LianZhongCount + 1;
                        }
                    }

                }
            }
        }
    }
}
