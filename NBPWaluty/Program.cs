using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NBPWaluty
{
    class Program
    {
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

                    //if(to.Year - from.Year != 0) // they are multiple years with range
                    //{
                    WebClient wb = new WebClient();
                    for (int i = 0; i < Math.Abs(to.Year - from.Year) + 1; ++i)
                    {
                        xmlFileNames.AddRange(wb.DownloadString($"http://www.nbp.pl/kursy/xml/dir{from.Year + i}.txt").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    // }

                    var dbg = 0;




                }
            }



            // TODO: It is error



            //WebClient wc = new WebClient();


        }
    }
}
