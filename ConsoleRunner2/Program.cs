using System;
using System.IO;
using System.Linq;

namespace ConsoleRunner2
{
    class Program
    {
        const string projectPath = @"D:\Visual Studio 2017\Projects\NextGen Spice\";

        static string GetProjectName(string filePath)
        {
            return filePath.Substring(projectPath.Length, filePath.IndexOf('\\', projectPath.Length) - projectPath.Length);
        }

        static void Main(string[] args)
        {

            var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories).GroupBy(GetProjectName);


            long sizeTotal = 0;

            foreach (var grp in files)
            {
                long sizeGrp = 0;
                foreach (var file in grp)
                {
                    FileInfo info = new FileInfo(file);
                    sizeGrp += info.Length;
                }
                Console.WriteLine($"{(sizeGrp/1000.0).ToString("F").PadLeft(10)}kb\t- {grp.Key}");
                sizeTotal += sizeGrp;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine($"{(sizeTotal / 1000.0).ToString("F").PadLeft(10)}kb\t- TOTAL");

        }
    }
}
