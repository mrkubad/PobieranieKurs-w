using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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


                    int[] indciesFrom = new int[3];
                    int[] indciesTo = new int[3];



                    indciesFrom[1] = FindClosestPublishDate(xmlFileNames, from, false);
                    indciesTo[1]= FindClosestPublishDate(xmlFileNames, to, true);


                    indciesFrom[0] = indciesFrom[1] - 1 < 0 ? -1 : indciesFrom[1] - 1;
                    indciesFrom[2] = indciesFrom[1] + 1 >= xmlFileNames.Count ? -1 : indciesFrom[1] + 1;
                    
                    indciesTo[0] = indciesTo[1] - 1 < 0 ? -1 : indciesTo[1] - 1;
                    indciesTo[2] = indciesTo[1] + 1 >= xmlFileNames.Count ? -1 : indciesTo[1] + 1;

                    //int properIndexFrom = -1;



                    List<Tuple<int, DateTime>> listOfNotesFrom = new List<Tuple<int, DateTime>>();
                    List<Tuple<int, DateTime>> listOfNotesTo = new List<Tuple<int, DateTime>>();
                    for(int i = 0; i < 3; ++i)
                    {
                        XmlDocument temp = new XmlDocument();
                        temp.LoadXml(wb.DownloadString($"http://www.nbp.pl/kursy/xml/{xmlFileNames[indciesFrom[i]]}.xml"));

                        var dataNotowania = DateTime.Parse(temp.GetElementsByTagName("data_notowania")[0].InnerText);
                        // TODO: Jeśli data notowania jest równa dacie brzegowej, to nie szukamy dalej, w sensie, że nie potrzebnie przeglądamy zakładane wybory xD od razu
                        listOfNotesFrom.Add(Tuple.Create(indciesFrom[i], dataNotowania));
                        

                    }
                    for (int i = 0; i < 3; ++i)
                    {
                        XmlDocument temp = new XmlDocument();
                        temp.LoadXml(wb.DownloadString($"http://www.nbp.pl/kursy/xml/{xmlFileNames[indciesTo[i]]}.xml"));

                        var dataNotowania = DateTime.Parse(temp.GetElementsByTagName("data_notowania")[0].InnerText);
                        // TODO: Jeśli data notowania jest równa dacie brzegowej, to nie szukamy dalej, w sensie, że nie potrzebnie przeglądamy zakładane wybory xD od razu
                        listOfNotesTo.Add(Tuple.Create(indciesTo[i], dataNotowania));


                    }



                    var test = listOfNotesFrom[0].Item2.DayOfYear - from.DayOfYear;
                    var test1 = listOfNotesFrom[1].Item2 - from;
                    var test2 = listOfNotesFrom[2].Item2 - from;

                    var bestFitFromIndex = listOfNotesFrom.Where(elem => elem.Item2 >= from).OrderBy(elem => Math.Abs(elem.Item2.DayOfYear - from.DayOfYear)).First().Item1;




                    var bestFitToIndex = listOfNotesTo.Where(elem => elem.Item2 <= to).OrderBy(elem => Math.Abs(elem.Item2.DayOfYear - to.DayOfYear)).First().Item1;


                    //var range = bestFitToIndex - bestFitFromIndex;

                    List<Tuple<DateTime, double, double>> notesInRangeBuySell = new List<Tuple<DateTime, double, double>>();
                    //List<string> notesInRangeSell = new List<string>();

                   for(int i = bestFitFromIndex; i <= bestFitToIndex; ++i)
                   {
                        XmlDocument temp = new XmlDocument();
                        temp.LoadXml(wb.DownloadString($"http://www.nbp.pl/kursy/xml/{xmlFileNames[i]}.xml"));

                        //var nodesTest = temp.GetElementsByTagName("pozycja");
                        XmlNodeList nodesTest2 = temp.GetElementsByTagName("kod_waluty");


                        foreach (XmlNode item in nodesTest2)
                        {
                            if(item.InnerText == args[0])
                            {
                                var dataNotowania = DateTime.Parse(temp.GetElementsByTagName("data_notowania")[0].InnerText);
                                notesInRangeBuySell.Add(Tuple.Create(dataNotowania, double.Parse(item.NextSibling.InnerText), double.Parse(item.NextSibling.NextSibling.InnerText)));
                            }
                        }
                    }





                   

                    var dbg = 0;

                }
            }



            // TODO: It is error



            //WebClient wc = new WebClient();


        }
    }
}
