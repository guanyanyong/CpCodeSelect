using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public class FileDownload 
    {

        public async Task<bool> DownloadFileByPostAsync(string url, string postData, string savePath)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                // 创建POST请求内容
                var content = new StringContent(postData, Encoding.UTF8, "application/json");

                // 发送POST请求
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // 确保目标目录存在
                    string directory = Path.GetDirectoryName(savePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // 读取响应内容（文件数据）
                    byte[] fileData = await response.Content.ReadAsByteArrayAsync();

                    // 保存文件 - .NET Framework 4.7.2使用同步方法
                    File.WriteAllBytes(savePath, fileData);

                    return true;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"下载失败，HTTP状态码: {response.StatusCode}, 错误: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"下载文件时发生错误: {ex.Message}", ex);
            }
        }
    }
}
