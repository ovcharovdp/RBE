using SevenZip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBalance
{
    class Program
    {
        static void Main(string[] args)
        {
            //var features = SevenZip.SevenZipExtractor.CurrentLibraryFeatures;
            //Console.WriteLine(((uint)features).ToString("X6"));
            List<string> files;
            using (var tmp = new SevenZipExtractor(@"c:\Denis\ALL.ZIP"))
            {
                 files = tmp.ArchiveFileData.Where(p => p.FileName.EndsWith("STR")).Select(p=>p.FileName).ToList();
                //for (int i = 0; i < tmp.ArchiveFileData.Count; i++)
                //{
                tmp.ExtractFiles(@".\Out\", files.ToArray());
                // }
                // To extract more than 1 file at a time or when you definitely know which files to extract,
                // use something like
                //tmp.ExtractFiles(@"d:\Temp\Result", 1, 3, 5);
            };
            System.IO.StreamReader file =    new System.IO.StreamReader(@".\Out\"+ files[0]);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                //counter++;
            }

            file.Close();
        }
    }
}
