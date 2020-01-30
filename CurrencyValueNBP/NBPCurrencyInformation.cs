using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyValueNBP
{
    public class NBPCurrencyInformation
    {
        private ConcurrentBag<CurrencyData> CurrencyDatas { get; set; }
        private DateTime ListingsFrom { get; set; }
        private DateTime ListingsTo { get; set; }
        private DateTime MaxFrom = new DateTime(2001, 12, 31).Date;
        public string CurrencyCode { get; private set; }
        private NBPXmlFileNames FileNames { get; set; }
        public event EventHandler<NBPCurrencyInformation> DataChanged;
        public event EventHandler<CurrencyInformationProgressEventArgs> ProgressChanged;
        private int _indexOfDownloadedFile = 0;
        private int _rangeOfFiles = int.MinValue;
        DateTime? previousDataReport = null;
        DateTime? previousProgressReport = null;

        int counter = 0;

        public NBPCurrencyInformation(string currencyCode, DateTime from, DateTime to, bool disableWarningsAboutRange = true, NBPTable table = NBPTable.C)
        {
            if (!checkAndSetListingRange(from, to))
            {
                if (!disableWarningsAboutRange)
                {
                    throw new ArgumentOutOfRangeException("Bad range is passed");
                }
            }
            var tempFileNames = new NBPXmlFileNames(ListingsFrom.Year, ListingsTo.Year);

            switch (table)
            {
                case NBPTable.A:
                    FileNames = tempFileNames.TableA;
                    break;
                case NBPTable.B:
                    FileNames = tempFileNames.TableB;
                    break;
                case NBPTable.C:
                    FileNames = tempFileNames.TableC;
                    break;
            }
            if (checkCurrencyCode(currencyCode))
            {
                CurrencyCode = currencyCode;
            }
            else
            {
                throw new InvalidDataException("Passed value is not right currency code");
            }
            CurrencyDatas = new ConcurrentBag<CurrencyData>();
        }
        private bool checkCurrencyCode(string currencyCode)
        {
            // download known extisting listing, to check currency
            var fileLink = new NBPXmlFile("c002z200103");
            StringReader tempXml = fileLink.DownloadAsStringReader();
            bool result = false;
            foreach (string code in NBPXmlHelper.GetElementValues(tempXml, "kod_waluty"))
            {
                result = (code == currencyCode);
                if (result)
                    break;
            }
            return result;
        }
        private bool checkAndSetListingRange(DateTime from, DateTime to)
        {
            bool result = true;
            if(to < from)
            {
                DateTime temp = from;
                from = to;
                to = temp;
                result = false;
            }
            if(from.Date < MaxFrom)
            {
                from = MaxFrom;
                result = false;
            }
            if(DateTime.Now.Date < to.Date)
            {
                to = DateTime.Now.Date;
                result = false;
            }

            ListingsFrom = from;
            ListingsTo = to;

            return result;
        }
        private async Task parseDataAsync()
        {

            Progress<CurrencyData> proggress = new Progress<CurrencyData>();
            proggress.ProgressChanged += ReportProgress;
            List<Task> tasks = new List<Task>();
            foreach (NBPXmlFile file in FileNames.GetFilesBetween(ListingsFrom, ListingsTo))
            {
                if (_rangeOfFiles == int.MinValue)
                    _rangeOfFiles = FileNames.Range;
                StringReader xmlFile = await file.DownloadAsStringReaderAsync();
                tasks.Add(Task.Factory.StartNew(() => NBPXmlHelper.GetGetCurrencyInformationFromXMLProgress(xmlFile, CurrencyCode, proggress)));
                Debug.WriteLine(counter++);
            }
            await Task.WhenAll(tasks);
        }

        private void ReportProgress(object sender, CurrencyData e)
        {
            CurrencyDatas.Add(e); // add data from listing to our curreny data list
            _indexOfDownloadedFile++;
            var now = DateTime.Now;
            if (previousDataReport == null)
                previousDataReport = now;
            if (previousProgressReport == null)
                previousProgressReport = now;

            if(now - previousProgressReport >= TimeSpan.FromSeconds(1))
            {
                previousProgressReport = now;
                ProgressChanged?.Invoke(null, new CurrencyInformationProgressEventArgs { FilesParsed = _indexOfDownloadedFile, FilesOverall = _rangeOfFiles });
            }

            if (now - previousDataReport >= TimeSpan.FromSeconds(4))
            {
                previousDataReport = now;
                DataChanged?.Invoke(null, this); // invoke our event
            }
        }
        public async Task StartParsingDataAsync()
        {
            await parseDataAsync();
        }
        private CurrencyData maximumBuyingRate()
        {
            double lastMax = double.MinValue;
            CurrencyData result = null;
            foreach(CurrencyData item in CurrencyDatas)
            {
                if(Math.Max(lastMax, item.BuyingRate) != lastMax)
                {
                    lastMax = item.BuyingRate;
                    result = item;
                }
            }
            return result;
        }
        private CurrencyData minimumBuyingRate()
        {
            double lastMin = double.MaxValue;
            CurrencyData result = null;
            foreach (CurrencyData item in CurrencyDatas)
            {
                if (Math.Min(lastMin, item.BuyingRate) != lastMin)
                {
                    lastMin = item.BuyingRate;
                    result = item;
                }
            }
            return result;
        }
        private CurrencyData maximumSellingRate()
        {
            double lastMax = double.MinValue;
            CurrencyData result = null;
            foreach (CurrencyData item in CurrencyDatas)
            {
                if (Math.Max(lastMax, item.SellingRate) != lastMax)
                {
                    lastMax = item.SellingRate;
                    result = item;
                }
            }
            return result;
        }
        private CurrencyData minimumSellingRate()
        {
            double lastMin = double.MaxValue;
            CurrencyData result = null;
            foreach (CurrencyData item in CurrencyDatas)
            {
                if (Math.Min(lastMin, item.SellingRate) != lastMin)
                {
                    lastMin = item.SellingRate;
                    result = item;
                }
            }
            return result;
        }

        public CurrencyData MaximumBuyingRate { get => maximumBuyingRate(); }
        public CurrencyData MinimimBuyingRate { get => minimumBuyingRate(); }
        public CurrencyData MinimimSellingRate { get => minimumSellingRate(); }
        public CurrencyData MaximumSellingRate { get => maximumSellingRate(); }
        public CurrencyData[] BiggestDiference { get { 
                //CurrencyDatas.Where(e => e.DifferenceBeetweenRates == CurrencyDatas.Max(elem => elem.DifferenceBeetweenRates));
                return CurrencyDatas.GroupBy(e => e.DifferenceBeetweenRates).OrderByDescending(g => g.Count()).First().ToArray();
            
            } }
        public double AverageBuyingRate { get => CurrencyDatas.Average(e => e.BuyingRate); }
        public double AverageSellingRate { get => CurrencyDatas.Average(e => e.SellingRate); }
        public double StandardDeviationBuyingRate { get => CurrencyDatas.Select(e => Math.Pow(e.BuyingRate - AverageBuyingRate, 2)).Sum() / CurrencyDatas.Count(); }
        public double StandardDeviationSellingRate { get => CurrencyDatas.Select(e => Math.Pow(e.SellingRate - AverageSellingRate, 2)).Sum() / CurrencyDatas.Count(); }
    }
}
