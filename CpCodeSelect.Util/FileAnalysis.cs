using CpCodeSelect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public static class FileAnalysis
    {
        /// <summary>
        /// 解析从文件中获取的数字字符串,分析出期号和号码
        /// 202506120017	02956
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        public static Code GetCodeByStr(string strCode)
        {
            Code codeResult = null;
            if (!string.IsNullOrEmpty(strCode))
            {
                var codeArray = strCode.Split(new char[] { '\t', ' ', }, StringSplitOptions.RemoveEmptyEntries);
                if (codeArray.Length == 2)
                {
                    codeResult = new Code
                    {
                        CodeQiHao = codeArray[0],
                        CodeNumber = codeArray[1]
                    };
                }
            }
            return codeResult;
        }

        /// <summary>
        /// 把codeList的字符串解析为Code
        /// </summary>
        /// <param name="strCodeList"></param>
        /// <returns></returns>
        public static List<Code> GetCodeListByCodeListStr(List<string> strCodeList)
        {
            List<Code> codeList = null;
            if (strCodeList != null && strCodeList.Count > 0)
            {
                codeList = new List<Code>();
                foreach (var strCode in strCodeList)
                {
                    if (!string.IsNullOrEmpty(strCode))
                    {
                        var codeArray = strCode.Split(new char[] { '\t', ' ', }, StringSplitOptions.RemoveEmptyEntries);
                        if (codeArray.Length == 2)
                        {
                            var code = new Code
                            {
                                CodeQiHao = codeArray[0],
                                CodeNumber = codeArray[1]
                            };
                            codeList.Add(code);
                        }
                    }
                }

            }
            return codeList;
        }
    }
}
