using System;
using System.IO;

namespace Individua.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //ZipTest.CreateZip(@"D:\work\test\2018", @"D:\work\test\result.zip",true);
            //ZipTest.ReleaseZip(@"D:\work\test\result.zip", @"D:\work\test\result");
            //ZipTest.ZipAddFile(@"D:\work\test\result.zip");


            string directoryPath = @"D:\work\test";
            DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);
            //ZipTest.Compress(directorySelected,directoryPath);

            foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.gz"))
            {
                ZipTest.Decompress(fileToDecompress);
            }


            Console.ReadLine();
        }


    }
}
