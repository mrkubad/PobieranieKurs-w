using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NBPWaluty
{
    class Program
    {

        static int FindClosestPublishDate(List<string> fileNames, DateTime time, bool endDate)
        {
            int closestIndex = -1;

            while (closestIndex == -1)
            {
                closestIndex = fileNames.FindIndex(name => name.Substring(name.Length - 6) == time.ToString("yyMMdd"));
                if (closestIndex == -1)
                    if (!endDate)
                        time = time.AddDays(1);
                    else
                        time = time.AddDays(-1);
            }
            return closestIndex;
        }

        static void Main(string[] args)
        {
            // TODO: We need to check user unput


            if (DateTime.TryParse(args[1], out DateTime from))
            {
                if (DateTime.TryParse(args[2], out DateTime to))
                {
                    if (from > to) // if user reverse period points
                    {
                        var t = from;
                        from = to;
                        to = t;
                    }

                    // We need to download text files with xml names

                    // Check if range is in one year, if not we need to download multiple dir files

                    List<string> xmlFileNames = new List<string>();

                    WebClient wb = new WebClient();
                    for (int i = 0; i < Math.Abs(to.Year - from.Year) + 1; ++i)
                    {
                        xmlFileNames.AddRange(wb.DownloadString($"http://www.nbp.pl/kursy/xml/dir{from.Year + i}.txt").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where( e => e[0] == 'c'));
                    }

                    // We need to find closest date :D

                    var indexFrom = FindClosestPublishDate(xmlFileNames, from, false);
                    var indexTo = FindClosestPublishDate(xmlFileNames, to, true);

                    




                    var dbg = 0;

                }
            }



            // TODO: It is error



            //WebClient wc = new WebClient();


        }
    }
}
