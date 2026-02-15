using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                if (TryTime > 12)
                {
                    TryTime = 1;
                    Thread.Sleep(2000);
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


        public static string ReadFileNumberRecord(string filePath, int lineNumber=1, int maxReadNumber = 100)
        {
            var firstLineStr = string.Empty;
            
            int readCount = 0; //读的总行数
            int readValidCount = 0; //读的有效行数
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    if (reader != null)
                    {
                        
                        firstLineStr = reader.ReadLine();
                        while (true)
                        {
                            if (string.IsNullOrEmpty(firstLineStr))
                            {
                                firstLineStr = reader.ReadLine();
                                readCount++;
                                if (readCount >= maxReadNumber)
                                    break;
                            }
                            else
                            {
                                readValidCount++;
                                if(readValidCount == lineNumber)
                                {
                                    //如果读到的是指定的行号，则返回
                                   break;
                                }
                                else
                                {
                                    //如果没有读到指定的行号,则继续读取下一行
                                    firstLineStr = reader.ReadLine();
                                }
                                if (readValidCount >= maxReadNumber)
                                    break;
                            }

                        }
                    }
                }
            }
            catch (IOException ex)
            {
                if (TryTime > 8)
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
