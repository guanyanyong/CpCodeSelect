using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.CacheModel
{

    public class LotteryRecord
    {
        public string cptypeid { get; set; }
        public string qihao { get; set; }
        public string code { get; set; }  // 现在存储5位开奖号
        public string cpsx { get; set; }
    }

    // 定义API响应的数据模型
    public class ApiResponse
    {
        public LotteryRecord latest_data { get; set; }
        public string message { get; set; }
        public string error { get; set; }
    }


    public class MultiResponse
    {
        public int count { get; set; }
        public List<LotteryRecord> data { get; set; }
        public string message { get; set; }
        public string error { get; set; }
    }

}
