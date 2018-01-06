using Individua.Util.compress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.CLI
{
    public static class ZipTest
    {
        public static void CreateZip(string pathFolder, string pathZip, bool includeBaseDirectory)
        {
            ZipHelper.CreateZip(pathFolder, pathZip, includeBaseDirectory);
        }

        public static void ReleaseZip(string pathZip, string pathFolder)
        {
            ZipHelper.ReleaseZip(pathZip, pathFolder);
        }

        public static void RepeatKeepExtract(string pathZip, string pathFolder)
        {
            ZipHelper.RepeatKeepExtract(pathZip, pathFolder);
        }

        public static void ZipAddFile(string PathZip)
        {
            ZipHelper.ZipAddFile(PathZip);
        }

        public static void Compress(DirectoryInfo directorySelected, string directoryPath)
        {
            ZipHelper.Compress(directorySelected, directoryPath);
        }
        public static void Decompress(FileInfo fileToDecompress)
        {
            ZipHelper.Decompress(fileToDecompress);
        }
    }
}
