using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Util.compress
{
    /// <summary>
    /// 压缩、解压工具类
    /// </summary>
    public static class ZipHelper
    {
        /// <summary>
        /// 创建 zip 存档，该存档包含指定目录的文件和目录。
        /// </summary>
        /// <param name="pathFolder">要存档的目录的路径，指定为相对路径或绝对路径。</param>
        /// <param name="pathZip">要生成的存档路径，指定为相对路径或绝对路径</param>
        /// <param name="includeBaseDirectory">包括从在存档的根的 sourceDirectoryName 的目录名称，则为 true；仅包含目录中的内容，则为 false</param>
        public static void CreateZip(string pathFolder, string pathZip, bool includeBaseDirectory)
        {
            ZipFile.CreateFromDirectory(pathFolder, pathZip,
                CompressionLevel.Fastest, includeBaseDirectory, Encoding.UTF8);
        }

        /// <summary>
        /// 将指定 zip 存档中的所有文件解压缩到文件系统的一目录下，并使用项名称的指定字符编码。
        /// </summary>
        /// <param name="pathZip">要解压缩存档的路径。</param>
        /// <param name="pathFolder">放置解压缩文件的目录的路径，指定为相对或绝对路径。</param>.
        public static void ReleaseZip(string pathZip, string pathFolder)
        {
            ZipFile.ExtractToDirectory(pathZip, pathFolder, Encoding.UTF8);
        }

        /// <summary>
        /// 通过现存的.zip文件的内容重复存档并提取有一个 .txt 扩展名的文件。
        /// </summary>
        /// <param name="pathZip">要解压缩存档的路径。</param>
        /// <param name="pathFolder">放置提取文件的目录的路径，指定为相对或绝对路径。</param>
        public static void RepeatKeepExtract(string pathZip, string pathFolder)
        {
            using (ZipArchive archive = ZipFile.OpenRead(pathZip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile(Path.Combine(pathFolder, entry.FullName));
                    }
                }
            }
        }

        /// <summary>
        /// 然后添加新文件到压缩文件
        /// </summary>
        /// <param name="PathZip">压缩文件路径</param>
        public static void ZipAddFile(string PathZip)
        {
            using (FileStream zipToOpen = new FileStream(PathZip, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry("Readme.txt");
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        writer.WriteLine("Information about this package.");
                        writer.WriteLine("========================");
                    }
                }
            }
        }
        
        /// <summary>
        /// 压缩目录内所有文件(只压缩文件，压缩为多个压缩包)
        /// </summary>
        /// <param name="directorySelected">目录选择</param>
        /// <param name="directoryPath">地址</param>
        public static void Compress(DirectoryInfo directorySelected,string directoryPath)
        {
            foreach (FileInfo fileToCompress in directorySelected.GetFiles())
            {
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) &
                       FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                               CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);

                            }
                        }
                        FileInfo info = new FileInfo(directoryPath + "\\" + fileToCompress.Name + ".gz");
                        Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                        fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 解压 .gz 格式压缩文件到当前文件夹
        /// </summary>
        /// <param name="fileToDecompress">.gz 文件对象</param>
        public static void Decompress(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }

    }
}
