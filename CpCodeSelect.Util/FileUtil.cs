using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public static class FileUtil
    {
        public static int TryTime = 1;
        public static string ReadFileFirstRecord(string filePath,int maxReadNumber=100)
        {
            var firstLineStr= string.Empty;
            int readCount = 0;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    if (reader != null)
                    {
                        firstLineStr = reader.ReadLine();
                        while (string.IsNullOrEmpty(firstLineStr))
                        {
                            firstLineStr = reader.ReadLine();
                            readCount++;
                            if (readCount >= maxReadNumber)
                                break;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                if (TryTime > 4)
                {
                    TryTime = 1;
                    throw ex;
                }
                else
                {
                    TryTime++;
                    return ReadFileFirstRecord(filePath, maxReadNumber);
                }
            }
            
            return firstLineStr;

        }
        /// <summary>
        /// 读取文件,返回所有的文件列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="maxReadNumber"></param>
        /// <returns></returns>
        public static List<string> ReadFileAllRecods(string filePath, int maxReadNumber = 20)
        {
            var lineStr = string.Empty;
            int readCount = 0;
            List<string> codeList = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                if (reader != null)
                {
                    lineStr = reader.ReadLine();
                    while (!string.IsNullOrEmpty(lineStr))
                    {
                        lineStr = reader.ReadLine();
                        readCount++;
                        codeList.Add(lineStr);
                        if (readCount >= maxReadNumber)
                            break;

                    }
                }
            }
            return codeList;

        }
    }
}
