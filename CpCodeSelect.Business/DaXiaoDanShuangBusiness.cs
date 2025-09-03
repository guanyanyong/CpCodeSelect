using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Business
{
    public static class DaXiaoDanShuangBusiness
    {
        /// <summary>
        /// 初始化大小单双
        /// </summary>
        /// <param name="code"></param>
        public static void InitDaXiaoDanShuang(Code code)
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
        }

        /// <summary>
        /// 设置位置的大小单双属性
        /// </summary>
        /// <param name="code"></param>
        /// <param name="positionNumber"></param>
        public static void SetPositionNumberDaXiaoDanShuang(Code code, PositionNumber positionNumber)
        {
            positionNumber.DaXiao = positionNumber.Number > 4 ? DaXiaoType.大 : DaXiaoType.小;
            positionNumber.DanShuang = positionNumber.DanShuang = (positionNumber.Number % 2 == 0) ? DanShuangType.双 : DanShuangType.单;
            if (code.PreCode == null)
            {
                // 如果没有上一期号码，初始化连开次数
                //设置大小
                if (positionNumber.DaXiao == DaXiaoType.大)
                {
                    positionNumber.DaLianKai = 1;
                    positionNumber.XiaoLianKai = 0;
                }
                else
                {
                    positionNumber.DaLianKai = 0;
                    positionNumber.XiaoLianKai = 1;
                }
                //设置单双
                if (positionNumber.DanShuang == DanShuangType.单)
                {

                    positionNumber.DanLianKai = 1;
                    positionNumber.ShuangLianKai = 0;
                }
                else
                {
                    positionNumber.DanLianKai = 0;
                    positionNumber.ShuangLianKai = 1;
                }
            }
            else
            {
                //如果有上一期号码,需要设置大小单双的连开数值
                //设置大小
                if (positionNumber.DaXiao == DaXiaoType.大)
                {
                    var preDaxiao = DaXiaoType.大;
                    int preDaxiaoLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDaxiao = code.PreCode.Wan.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Wan.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDaxiao = code.PreCode.Qian.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Qian.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDaxiao = code.PreCode.Bai.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Bai.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDaxiao = code.PreCode.Shi.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Shi.DaLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDaxiao = code.PreCode.Ge.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Ge.DaLianKai;
                    }
                    if (preDaxiao == DaXiaoType.大)
                    {
                        positionNumber.DaLianKai = preDaxiaoLianKai + 1;
                        positionNumber.XiaoLianKai = 0;
                    }
                    else
                    {
                        positionNumber.DaLianKai = 1;
                        positionNumber.XiaoLianKai = 0;
                    }
                }
                else
                {
                    var preDaxiao = DaXiaoType.小;
                    int preDaxiaoLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDaxiao = code.PreCode.Wan.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Wan.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDaxiao = code.PreCode.Qian.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Qian.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDaxiao = code.PreCode.Bai.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Bai.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDaxiao = code.PreCode.Shi.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Shi.XiaoLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDaxiao = code.PreCode.Ge.DaXiao;
                        preDaxiaoLianKai = code.PreCode.Ge.XiaoLianKai;
                    }
                    if (preDaxiao == DaXiaoType.小)
                    {
                        positionNumber.XiaoLianKai = preDaxiaoLianKai + 1;
                        positionNumber.DaLianKai = 0;
                    }
                    else
                    {
                        positionNumber.XiaoLianKai = 1;
                        positionNumber.DaLianKai = 0;
                    }
                }
                //设置单双
                if (positionNumber.DanShuang == DanShuangType.单)
                {
                    var preDanShuang = DanShuangType.单;
                    int preDanShuangLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDanShuang = code.PreCode.Wan.DanShuang;
                        preDanShuangLianKai = code.PreCode.Wan.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDanShuang = code.PreCode.Qian.DanShuang;
                        preDanShuangLianKai = code.PreCode.Qian.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDanShuang = code.PreCode.Bai.DanShuang;
                        preDanShuangLianKai = code.PreCode.Bai.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDanShuang = code.PreCode.Shi.DanShuang;
                        preDanShuangLianKai = code.PreCode.Shi.DanLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDanShuang = code.PreCode.Ge.DanShuang;
                        preDanShuangLianKai = code.PreCode.Ge.DanLianKai;
                    }
                    if (preDanShuang == DanShuangType.单)
                    {
                        positionNumber.DanLianKai = preDanShuangLianKai + 1;
                        positionNumber.ShuangLianKai = 0;
                    }
                    else
                    {
                        positionNumber.DanLianKai = 1;
                        positionNumber.ShuangLianKai = 0;
                    }
                }
                else
                {
                    var preDanShuang = DanShuangType.双;
                    int preDanShuangLianKai = 0;
                    if (positionNumber.PositionType == PositionType.万)
                    {
                        preDanShuang = code.PreCode.Wan.DanShuang;
                        preDanShuangLianKai = code.PreCode.Wan.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.千)
                    {
                        preDanShuang = code.PreCode.Qian.DanShuang;
                        preDanShuangLianKai = code.PreCode.Qian.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.百)
                    {
                        preDanShuang = code.PreCode.Bai.DanShuang;
                        preDanShuangLianKai = code.PreCode.Bai.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.十)
                    {
                        preDanShuang = code.PreCode.Shi.DanShuang;
                        preDanShuangLianKai = code.PreCode.Shi.ShuangLianKai;
                    }
                    if (positionNumber.PositionType == PositionType.个)
                    {
                        preDanShuang = code.PreCode.Ge.DanShuang;
                        preDanShuangLianKai = code.PreCode.Ge.ShuangLianKai;
                    }
                    if (preDanShuang == DanShuangType.双)
                    {
                        positionNumber.ShuangLianKai = preDanShuangLianKai + 1;
                        positionNumber.DanLianKai = 0;
                    }
                    else
                    {
                        positionNumber.ShuangLianKai = 1;
                        positionNumber.DanLianKai = 0;
                    }
                }

                //根据大小单双的连开次数,设置推荐号码
                //todo 目前设置的是连开4期推荐,后续可以更改为可配置
                if (positionNumber.XiaoLianKai >= 4)
                {
                    positionNumber.DaXiaoTuijianNumber = "小";
                }
                else if (positionNumber.DaLianKai >= 4)
                {
                    positionNumber.DaXiaoTuijianNumber = "大";
                }

                if (positionNumber.DanLianKai >= 4)
                {
                    positionNumber.DanShuangTuijianNumber = "单";
                }
                else if (positionNumber.ShuangLianKai >= 4)
                {
                    positionNumber.DanShuangTuijianNumber = "双";
                }
            }
        }
    }
}
