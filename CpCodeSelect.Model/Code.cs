using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    public class Code
    {
        public string CodeQiHao { get; set; }
        public string CodeNumber { get; set; }
        /// <summary>
        /// 上一期号码
        /// </summary>
        public Code PreCode { get; set; }
        public PositionNumber Wan { get; set; }
        public PositionNumber Qian { get; set; }
        public PositionNumber Bai { get; set; }
        public PositionNumber Shi { get; set; }
        public PositionNumber Ge { get; set; }
        /// <summary>
        /// 龙虎列表
        /// </summary>
        public List<PositionDragonTiger> DragonTigerList { get; set; }

        /// <summary>
        /// 杀3码的位置列表
        /// </summary>
        public List<Kill3Model> Kill3ModelList { get; set; }
        /// <summary>
        /// 组六杀1码的位置列表
        /// </summary>
        public List<Zu6Kill1Model> Zu6Kill1ModelList { get; set; }

        public List<Five1MaModel> Five1MaModelList { get; set; }


        /// <summary>
        /// 组六杀1码中跟挂停的位置列表
        /// </summary>
        public List<Zu6Kill1ZGGTModel> Zu6Kill1ZGGTModelList { get; set; }
        /// <summary>
        /// 组六杀1码中跟2的位置列表
        /// </summary>

        public List<Zu6Kill1ZG2Model> Zu6Kill1ZG2ModelList { get; set; }

        /// <summary>
        /// 号码生成条件,类似于 "10-20,5-8,6-9,10-12" 逗号分割每个条件,-分割开始和结束
        /// </summary>
        public string NumberCondition { get; set; }

        /// <summary>
        /// 获取号码的数量 默认50
        /// </summary>
        public int GetNumberCount { get; set; } = 50;

        /// <summary>
        /// 获取后二字符串
        /// </summary>
        /// <returns></returns>
        public string GetHou2String()
        {
            return $"{Shi.Number}{Ge.Number}";
        }
        public string GetHou3String()
        {
            return $"{Bai.Number}{Shi.Number}{Ge.Number}";
        }
    }
}
