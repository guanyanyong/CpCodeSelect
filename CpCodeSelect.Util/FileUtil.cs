using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Vss; // AlphaVSS 的核心命名空间

namespace CpCodeSelect.Util
{

    public static class FileUtil
    {
        public static int TryTime = 1;
        public static string ReadFileFirstRecord(string filePath,int maxReadNumber=100)
        {
            Encoding encoding = null;
            var firstLineStr= string.Empty;
            int readCount = 0;
            try
            {
                using (var fs = new FileStream(filePath,
                                   FileMode.Open,
                                   FileAccess.Read,
                                   FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs, encoding ?? Encoding.UTF8))
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
                    return "";
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



        public static string ReadLockedFileFirstRecord(string filePath, int maxReadNumber = 100)
        {
            var firstLineStr = string.Empty;
            int readCount = 0;
            string volumeRoot = Path.GetPathRoot(filePath);
            if (string.IsNullOrEmpty(volumeRoot))
                throw new ArgumentException("无效的文件路径");

            string relativePath = filePath.Substring(volumeRoot.Length).TrimStart('\\');

            // 【修正点 1】将变量声明提升到 try 块之外，以便 finally 块可以访问
            IVssImplementation vssImpl = null;
            IVssBackupComponents components = null;
            Guid snapshotId = Guid.Empty;

            try
            {
                // 1. 加载 VSS
                vssImpl = VssUtils.LoadImplementation();
                components = vssImpl.CreateVssBackupComponents();

                components.InitializeForBackup(null);
                components.SetContext(VssSnapshotContext.Backup);

                // 可选：如果速度太慢或不需要一致性，可以注释掉下一行，但数据库文件建议保留
                //components.GatherWriterMetadata();

                // 2. 创建快照
                Guid snapshotSetId = components.StartSnapshotSet();

                // 【修正点 2】在这里给外部变量赋值
                snapshotId = components.AddToSnapshotSet(volumeRoot, Guid.Empty);

                components.PrepareForBackup();
                Console.WriteLine("正在创建快照 (卷将被短暂冻结)...");
                components.DoSnapshotSet();

                // 3. 获取快照路径
                VssSnapshotProperties props = components.GetSnapshotProperties(snapshotId);
                string snapshotDeviceObject = props.SnapshotDeviceObject;
                // 示例: \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1

                // 4. 拼接路径
                string cleanRoot = snapshotDeviceObject.TrimEnd('\\');
                string cleanRelative = relativePath.TrimStart('\\');
                string snapshotFilePath = $"{cleanRoot}\\{cleanRelative}";

                Console.WriteLine($"快照设备: {snapshotDeviceObject}");
                Console.WriteLine($"目标文件路径: {snapshotFilePath}");

                // 5. 读取文件
                if (File.Exists(snapshotFilePath))
                {
                    //byte[] data = File.ReadAllBytes(snapshotFilePath);
                    //Console.WriteLine($"✅ 成功读取！大小: {data.Length} 字节");

                    using (StreamReader reader = new StreamReader(snapshotFilePath))
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

                    // 这里可以添加保存逻辑
                    // File.WriteAllBytes(@"C:\Temp\backup.dat", data);
                }
                else
                {
                    Console.WriteLine($"❌ 错误：在快照中未找到文件。请检查路径是否正确。");
                    Console.WriteLine($"   卷根: {volumeRoot}");
                    Console.WriteLine($"   相对路径: {relativePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 发生错误: {ex.Message}");
                if (ex is VssException vssEx)
                {
                    Console.WriteLine($"VSS 错误代码: {vssEx.Message}");
                }
                throw; // 重新抛出以便调用者知道失败
            }
            finally
            {
                // 【修正点 3】现在 snapshotId 在这里是可见的
                if (components != null)
                {
                    try
                    {
                        if (snapshotId != Guid.Empty)
                        {
                            Console.WriteLine("正在清理快照...");
                            components.DeleteSnapshot(snapshotId, false);
                            Console.WriteLine("✅ 快照已成功删除。");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ 删除快照时出错 (可能需要手动清理): {ex.Message}");
                    }
                    finally
                    {
                        components.Dispose();
                    }
                }

                // vssImpl 通常不需要显式 Dispose，但 components 必须释放
            }

            return firstLineStr;
        }


    }
}
