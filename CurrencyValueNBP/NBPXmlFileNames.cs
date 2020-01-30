using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CurrencyValueNBP
{
    public class NBPXmlFileNames
    {
        private List<string> fileNames;
        internal NBPXmlFileNames(int fromYear, int toYear)
        {
            fileNames = new List<string>();
            int currentYear = DateTime.Now.Year;
            using (var downloader = new WebClient())
            {
                if(fromYear == 2001)
                    fromYear++;
                for (int year = fromYear; year <= toYear; ++year)
                {
                    fileNames.AddRange(downloader.DownloadString($"http://www.nbp.pl/kursy/xml/dir{(currentYear == year ? "" : year.ToString())}.txt").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }
        private NBPXmlFileNames(IEnumerable<string> names)
        {
            fileNames = new List<string>();
            fileNames.AddRange(names);
        }
        internal NBPXmlFileNames TableC { get => new NBPXmlFileNames(fileNames.Where(e => e[0] == 'c')); }
        internal NBPXmlFileNames TableB { get => new NBPXmlFileNames(fileNames.Where(e => e[0] == 'b')); }
        internal NBPXmlFileNames TableA { get => new NBPXmlFileNames(fileNames.Where(e => e[0] == 'a')); }

        private int GetClosestIndex(DateTime date, bool before = false)
        {
            int index = -1;
            while ((index = fileNames.FindIndex(elem => elem.Substring(elem.Length - 6) == date.Date.ToString("yyMMdd"))) == -1)
            {
                date = date.AddDays(before ? -1 : 1);
            }
            return index;
        }
        private string GetFileNameAt(int index) => fileNames[index];
        private NBPXmlFile[] GetClosestFilesToDate(DateTime date, bool before)
        {
            List<NBPXmlFile> result = new List<NBPXmlFile>();
            int index = GetClosestIndex(date, before);

            result.Add(new NBPXmlFile(index, GetFileNameAt(index)));

            if (index - 1 >= 0)
                result.Add(new NBPXmlFile(index - 1, GetFileNameAt(index - 1)));
            if (index + 1 < fileNames.Count)
                result.Add(new NBPXmlFile(index + 1, GetFileNameAt(index + 1)));

            return result.ToArray();
        }
        private int GetClosestListingIndex(DateTime date, bool before = false)
        {
            var links = GetClosestFilesToDate(date, before);
            List<Tuple<int, DateTime>> noteDates = new List<Tuple<int, DateTime>>();
            for (int i = 0; i < links.Length; i++)
            {
                var listingDate = NBPXmlHelper.CreateDateTimeFromString(NBPXmlHelper.GetElementValue(links[i].DownloadAsStringReader(), "data_notowania"));
                if (listingDate.Date == date.Date)
                {
                    return links[i].Index;
                }
                else
                {
                    noteDates.Add(Tuple.Create(links[i].Index, listingDate));
                }
            }
            var matches = before ? noteDates.Where(e => e.Item2 <= date) : noteDates.Where(e => e.Item2 >= date);
            var bestFit = matches.OrderBy(e => Math.Abs(e.Item2.Ticks - date.Ticks)).First();

            return bestFit.Item1;
        }
        internal bool IsRangeValid { get => Range > 0; }
        internal int  Range { get; private set; }

        internal IEnumerable<NBPXmlFile> GetFilesBetween(DateTime from, DateTime to)
        {
            int indexFrom = GetClosestListingIndex(from);
            int indexTo = GetClosestListingIndex(to, true);
            Range = indexTo - indexFrom + 1;
            for(int i = indexFrom; i <= indexTo; ++i)
            {
                yield return new NBPXmlFile(i - indexFrom, fileNames[i]);
            }
            Range = -1;
        }


    }
}
