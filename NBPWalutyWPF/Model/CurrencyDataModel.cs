using CurrencyValueNBP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NBPWalutyWPF.Model
{
    class CurrencyDataModel : INotifyPropertyChanged
    {
        private string _currencyCode;
        public string CurrencyCode { 
            get => _currencyCode;
            set { _currencyCode = value; NotifyPropertyChanged(); }
        }
        private double _averageSellingRate;
        public double AverageSellingRate
        {
            get => _averageSellingRate;
            set { _averageSellingRate = value; NotifyPropertyChanged(); }
        }
        private double _averageBuyingRate;
        public double AverageBuyingRate
        {
            get => _averageBuyingRate;
            set { _averageBuyingRate = value; NotifyPropertyChanged(); }
        }
        private double _standardDevationSelling;
        public double StandardDevationSelling
        { 
            get => _standardDevationSelling; 
            set { _standardDevationSelling = value; NotifyPropertyChanged(); }
        }
        private double _standardDevationBuying;
        public double StandardDevationBuying
        {
            get => _standardDevationBuying;
            set { _standardDevationBuying = value; NotifyPropertyChanged(); }
        }

        private double _maximumBuyingRate;
        public double MaximumBuyingRate
        {
            get => _maximumBuyingRate;
            set { _maximumBuyingRate = value; NotifyPropertyChanged(); }
        }
        private string _maximumBuyingRateDate;
        public string MaximumBuingRateDate
        {
            get => _maximumBuyingRateDate;
            set { _maximumBuyingRateDate = value; NotifyPropertyChanged(); }
        }
        private double _minimumBuyingRate;
        public double MinimumBuyingRate
        {
            get => _minimumBuyingRate;
            set { _minimumBuyingRate = value; NotifyPropertyChanged(); }
        }
        private string _minimumBuyingRateDate;
        public string MinimumBuingRateDate
        {
            get => _minimumBuyingRateDate;
            set { _minimumBuyingRateDate = value; NotifyPropertyChanged(); }
        }
        private double _maximumSellingRate;
        public double MaximumSellingRate
        {
            get => _maximumSellingRate;
            set { _maximumSellingRate = value; NotifyPropertyChanged(); }
        }
        private string _maximumSellingRateDate;
        public string MaximumSellingRateDate
        {
            get => _maximumSellingRateDate;
            set { _maximumSellingRateDate = value; NotifyPropertyChanged(); }
        }
        private double _minimumSellingRate;
        public double MinimumSellingRate
        {
            get => _minimumSellingRate;
            set { _minimumSellingRate = value; NotifyPropertyChanged(); }
        }
        private string _minimumSellingRateDate;
        public string MinimumSellingRateDate
        {
            get => _minimumSellingRateDate;
            set { _minimumSellingRateDate = value; NotifyPropertyChanged(); }
        }

        private CurrencyData[] _biggestDifreneces;


        public CurrencyData[] BiggestDifrences
        {
            get { return _biggestDifreneces; }
            set { _biggestDifreneces = value;  NotifyPropertyChanged(); }
        }



        public CurrencyDataModel(string currenyCode)
        {
            CurrencyCode = currenyCode;
        }

        public CurrencyDataModel(NBPCurrencyInformation data)
        {
            FeedNewValues(data);
        }

        public void FeedNewValues(NBPCurrencyInformation data)
        {
            CurrencyCode = data.CurrencyCode;
            AverageSellingRate = data.AverageSellingRate;
            AverageBuyingRate = data.AverageBuyingRate;
            StandardDevationBuying = data.StandardDeviationBuyingRate;
            StandardDevationSelling = data.StandardDeviationSellingRate;
            var temp = data.MinimimSellingRate;
            MinimumSellingRate = temp.SellingRate;
            MinimumSellingRateDate = temp.ListingDateString;
            temp = data.MaximumSellingRate;
            MaximumSellingRate = temp.SellingRate;
            MaximumSellingRateDate = temp.ListingDateString;
            temp = data.MinimimBuyingRate;
            MinimumBuyingRate = temp.BuyingRate;
            MinimumBuingRateDate = temp.ListingDateString;
            temp = data.MaximumBuyingRate;
            MaximumBuyingRate = temp.BuyingRate;
            MaximumBuingRateDate = temp.ListingDateString;
            BiggestDifrences = data.BiggestDiference;
        }


        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
