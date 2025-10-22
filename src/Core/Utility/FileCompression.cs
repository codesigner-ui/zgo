using System.IO.Compression;
using System.Formats.Tar;

public class FileCompression
{
    public static void ExtractZip(string zipFilePath, string outputDirectory)
    {
        
            // 确保目标目录存在
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // 使用内置方法解压
        ZipFile.ExtractToDirectory(zipFilePath, outputDirectory, overwriteFiles: true); // overwriteFiles参数在.NET Core及以上版本支持
            
    }
    
    public static void ExtractTarGz(string tgzFilePath, string outputDirectory)
    {
        // 确保输出目录存在
        Directory.CreateDirectory(outputDirectory);

        // 1. 打开 .tar.gz 文件流
        using FileStream fileStream = File.OpenRead(tgzFilePath);
        // 2. 创建 GZipStream 解压流
        using GZipStream decompressor = new GZipStream(fileStream, CompressionMode.Decompress);
        // 3. 将解压后的 tar 流提取到目标目录
        TarFile.ExtractToDirectory(decompressor, outputDirectory, overwriteFiles: true);
    }
}