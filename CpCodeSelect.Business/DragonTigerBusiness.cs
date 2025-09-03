using CpCodeSelect.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Business
{
    public static class DragonTigerBusiness
    {
        /// <summary>
        /// 初始化龙虎和
        /// </summary>
        /// <param name="code"></param>
        public static void InitDragonTigerBusiness(Code code)
        {
            code.DragonTigerList = new List<PositionDragonTiger>();
            //万千
            var dragonTiger1 = new PositionDragonTiger();
            dragonTiger1.BeginPositoin = PositionType.万;
            dragonTiger1.EndPosition = PositionType.千;
            if (code.Wan.Number > code.Qian.Number)
            {
                dragonTiger1.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Wan.Number < code.Qian.Number)
            {
                dragonTiger1.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger1.DragonTigerType = DragonTigerType.和;
            }

            code.DragonTigerList.Add(dragonTiger1);
            //万百
            var dragonTiger2 = new PositionDragonTiger();
            dragonTiger2.BeginPositoin = PositionType.万;
            dragonTiger2.EndPosition = PositionType.百;
            if (code.Wan.Number > code.Bai.Number)
            {
                dragonTiger2.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Wan.Number < code.Bai.Number)
            {
                dragonTiger2.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger2.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger2);
            //万十
            var dragonTiger3 = new PositionDragonTiger();
            dragonTiger3.BeginPositoin = PositionType.万;
            dragonTiger3.EndPosition = PositionType.十;
            if (code.Wan.Number > code.Shi.Number)
            {
                dragonTiger3.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Wan.Number < code.Shi.Number)
            {
                dragonTiger3.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger3.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger3);
            //万个
            var dragonTiger4 = new PositionDragonTiger();
            dragonTiger4.BeginPositoin = PositionType.万;
            dragonTiger4.EndPosition = PositionType.个;
            if (code.Wan.Number > code.Ge.Number)
            {
                dragonTiger4.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Wan.Number < code.Ge.Number)
            {
                dragonTiger4.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger4.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger4);
            //千百
            var dragonTiger5 = new PositionDragonTiger();
            dragonTiger5.BeginPositoin = PositionType.千;
            dragonTiger5.EndPosition = PositionType.百;
            if (code.Qian.Number > code.Bai.Number)
            {
                dragonTiger5.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Qian.Number < code.Bai.Number)
            {
                dragonTiger5.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger5.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger5);
            //千十
            var dragonTiger6 = new PositionDragonTiger();
            dragonTiger6.BeginPositoin = PositionType.千;
            dragonTiger6.EndPosition = PositionType.十;
            if (code.Qian.Number > code.Shi.Number)
            {
                dragonTiger6.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Qian.Number < code.Shi.Number)
            {
                dragonTiger6.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger6.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger6);
            //千个
            var dragonTiger7 = new PositionDragonTiger();
            dragonTiger7.BeginPositoin = PositionType.千;
            dragonTiger7.EndPosition = PositionType.个;
            if (code.Qian.Number > code.Ge.Number)
            {
                dragonTiger7.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Qian.Number < code.Ge.Number)
            {
                dragonTiger7.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger7.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger7);
            //百十
            var dragonTiger8 = new PositionDragonTiger();
            dragonTiger8.BeginPositoin = PositionType.百;
            dragonTiger8.EndPosition = PositionType.十;
            if (code.Bai.Number > code.Shi.Number)
            {
                dragonTiger8.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Bai.Number < code.Shi.Number)
            {
                dragonTiger8.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger8.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger8);
            //百个
            var dragonTiger9 = new PositionDragonTiger();
            dragonTiger9.BeginPositoin = PositionType.百;
            dragonTiger9.EndPosition = PositionType.个;
            if (code.Bai.Number > code.Ge.Number)
            {
                dragonTiger9.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Bai.Number < code.Ge.Number)
            {
                dragonTiger9.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger9.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger9);
            //十个
            var dragonTiger10 = new PositionDragonTiger();
            dragonTiger10.BeginPositoin = PositionType.十;
            dragonTiger10.EndPosition = PositionType.个;
            if (code.Shi.Number > code.Ge.Number)
            {
                dragonTiger10.DragonTigerType = DragonTigerType.龙;
            }
            else if (code.Shi.Number < code.Ge.Number)
            {
                dragonTiger10.DragonTigerType = DragonTigerType.虎;
            }
            else
            {
                dragonTiger10.DragonTigerType = DragonTigerType.和;
            }
            code.DragonTigerList.Add(dragonTiger10);
        }
        /// <summary>
        /// 设置位置的龙虎和属性,并且设置推荐号码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="positionNumber"></param>
        public static void SetPositionNumberDragonTiger(Code code)
        {
            List<PositionDragonTiger> beforeDragonTigerList = null;
            if (code.PreCode == null)
            {
                // 如果没有上一期号码,判断是否是和或者是和之后
                foreach (var dragonTiger in code.DragonTigerList)
                {
                    dragonTiger.IsHeAfter1 = false;
                    dragonTiger.HeAfterTime1 = 0;
                    dragonTiger.TuiJianDragonTiger1 = string.Empty;
                    dragonTiger.IsLianHe = false;

                }

            }
            else
            {
                // 如果有上一期号码,需要判断是否是和或者是和之后
                beforeDragonTigerList = code.PreCode.DragonTigerList;

                foreach (var dragonTiger in code.DragonTigerList)
                {

                    var beginPosition = dragonTiger.BeginPositoin;
                    var endPosition = dragonTiger.EndPosition;

                    var beforeDragonTiger = beforeDragonTigerList.Where(d => d.BeginPositoin == beginPosition && d.EndPosition == endPosition).FirstOrDefault();
                    if (beforeDragonTiger != null)
                    {
                        //先判断是否是连和
                        //3种情况 
                        //1.前后两期都是和,则是连和,并且连和次数为3
                        //2.前一期是连和,并且连和次数大于0,则本期也是连和,并且连和次数减1
                        //3.其他情况,则不是连和
                        if (beforeDragonTiger.DragonTigerType == DragonTigerType.和 && dragonTiger.DragonTigerType == DragonTigerType.和)
                        {
                            dragonTiger.IsLianHe = true;
                            dragonTiger.LianHeLeftCount = 3;
                            dragonTiger.DisplayMessage = "出现连和,停3期";
                            continue;
                        }
                        else if (beforeDragonTiger.IsLianHe && beforeDragonTiger.LianHeLeftCount > 0)
                        {
                            dragonTiger.LianHeLeftCount = beforeDragonTiger.LianHeLeftCount - 1;
                            dragonTiger.IsLianHe = true;
                            dragonTiger.DisplayMessage = $"连和后,剩余{dragonTiger.LianHeLeftCount}期";
                            continue;
                        }
                        else if (beforeDragonTiger.IsLianHe && beforeDragonTiger.LianHeLeftCount == 0)
                        {
                            dragonTiger.IsLianHe = false;
                            dragonTiger.LianHeLeftCount = 0;
                        }
                        else
                        {
                            dragonTiger.IsLianHe = false;
                            dragonTiger.LianHeLeftCount = 0;
                        }

                        //if (beforeDragonTiger.HeAfterTime1 > 0 && dragonTiger.DragonTigerType == DragonTigerType.和)
                        //{
                        //    // 如果上一期是和之后1,并且上一期是和,则直接不继续推荐,给出提示信息
                        //    dragonTiger.DisplayMessage = "和之后3期内出和,当前期不再推荐号码";
                        //    dragonTiger.HeAfterTime1 = 1;
                        //    continue;
                        //}

                        if (beforeDragonTiger.HeAfterTime1 == 0 && beforeDragonTiger.DragonTigerType == DragonTigerType.和)
                        {
                            //如果上一期不是和之后,但是上一期是和,则本期是和之后1
                            dragonTiger.HeAfterTime1 = 1;
                            dragonTiger.TuiJianDragonTiger1 = dragonTiger.DragonTigerType.ToString();
                            dragonTiger.IsHeAfter1 = true;
                        }
                        else if (beforeDragonTiger.IsHeAfter1)
                        {
                            //如果前一期是和之后
                            if (beforeDragonTiger.HeAfterTime1 >= 1 && beforeDragonTiger.HeAfterTime1 <= 2)
                            {
                                //如果前一期是和之后的1到3期,则本期是和之后的+1期
                                if (dragonTiger.DragonTigerType.ToString() == beforeDragonTiger.TuiJianDragonTiger1)
                                {
                                    //如果当前期的龙虎类型和上一期推荐类型相同,则中出,不需要继续推荐
                                    dragonTiger.TuiJianDragonTiger1 = string.Empty;
                                    dragonTiger.IsHeAfter1 = false;
                                    dragonTiger.HeAfterTime1 = 0;
                                }
                                else
                                {
                                    //如果当前期的龙虎类型和上一期推荐类型不同,则继续推荐,和之后数值+1
                                    dragonTiger.IsHeAfter1 = true;
                                    dragonTiger.HeAfterTime1 = beforeDragonTiger.HeAfterTime1 + 1;
                                    dragonTiger.TuiJianDragonTiger1 = beforeDragonTiger.TuiJianDragonTiger1;
                                }

                            }
                            else if (beforeDragonTiger.HeAfterTime1 == 3)
                            {
                                //前一期是和之后的第3期和当前是和之后的第4期 不管中与不中,都不继续推荐
                                dragonTiger.TuiJianDragonTiger1 = string.Empty;
                                dragonTiger.IsHeAfter1 = false;

                                dragonTiger.IsHeTuiJiaGua = false;
                                if (dragonTiger.DragonTigerType.ToString() != beforeDragonTiger.TuiJianDragonTiger1)
                                {
                                    //第四期和之后推荐的数据还是不一样,则推荐是挂
                                    dragonTiger.IsHeTuiJiaGua = true;
                                    dragonTiger.DisplayMessage = $"和之后3期未中,推荐号码{beforeDragonTiger.TuiJianDragonTiger1}已挂3期";
                                }
                            }
                        }

                    }
                }

            }
        }
    }
}
