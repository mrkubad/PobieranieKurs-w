using System;

namespace CurrencyValueNBP
{
    /// <summary>
    /// This class represents data about our currency
    /// </summary>
    public class CurrencyData
    {
        public double BuyingRate { get; internal set; }
        public double SellingRate { get; internal set; }
        public DateTime ListingDate { get; internal set; }
        public string ListingDateString { get => ListingDate.ToString("dd.MM.yyyy"); }
        public string CurrencyCode { get; internal set; }
        public double DifferenceBeetweenRates { get => Math.Abs(SellingRate - BuyingRate); }
    }
}
