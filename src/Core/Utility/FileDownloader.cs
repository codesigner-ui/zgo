namespace Zgo.Core;

public class FileDownloader
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task DownloadFileAsync(string url, string localFilePath, IProgress<long> progress = null)
    {
        try
        {
            // 发送请求并获取响应
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode(); // 确保响应成功

                // 获取内容总长度（用于显示进度）
                long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                long bytesRead = 0L;

                // 获取响应流和文件流
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[81920]; // 80KB缓冲区
                    int bytesReceived;

                    // 从网络流读取数据并写入文件流
                    while ((bytesReceived = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesReceived);
                        bytesRead += bytesReceived;

                        // 报告进度
                        progress?.Report(bytesRead);
                    }
                }
            }
            Console.WriteLine($"文件下载完成: {localFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"下载文件时出错: {ex.Message}");
            throw;
        }
    }
}