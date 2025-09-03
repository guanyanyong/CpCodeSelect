using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model
{
    /// <summary>
    /// 根据位置
    /// </summary>
    public class PositionDragonTiger
    {
        public DragonTigerType DragonTigerType { get; set; }
        public PositionType BeginPositoin { get; set; }
        public PositionType EndPosition { get; set; }
        /// <summary>
        /// 是否是连和
        /// </summary>
        public bool IsLianHe { get;set; }
        /// <summary>
        /// 联和剩余次数,联和3次后,再推荐
        /// </summary>
        public int LianHeLeftCount { get; set; }
        /// <summary>
        /// 是否是和之后1
        /// </summary>
        public bool IsHeAfter1 { get; set; }
        /// <summary>
        /// 和之后多少期1
        /// </summary>
        public int HeAfterTime1   { get; set; }
        /// <summary>
        /// 推荐龙虎1
        /// </summary>
        public string TuiJianDragonTiger1 { get; set; }
        public bool IsHeTuiJiaGua { get; set; } = false;
        /// <summary>
        /// 显示提示信息
        /// 前端显示时,给出提示信息
        /// </summary>
        public string DisplayMessage { get; set; }
    }
}
