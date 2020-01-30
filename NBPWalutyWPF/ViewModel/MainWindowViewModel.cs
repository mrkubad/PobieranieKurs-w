using NBPWalutyWPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CurrencyValueNBP;
using NBPWalutyWPF.Model;

namespace NBPWalutyWPF.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ComboboxItems { get; private set; }
        public ICommand DateFromSelected { get; private set; }
        public ICommand DateToSelected { get; private set; }
        public ICommand StartButtonClicked { get; private set; }
        private string currencyCode;
        public string CurrencyCode { get => currencyCode; set { currencyCode = value; IsDateFromEnabled = true; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrencyCode")); } }
        private DateTime dateFrom;
        public DateTime DateFrom { get => dateFrom; private set { dateFrom = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DateFrom")); } }
        private DateTime dateTo;
        public bool isDateToDisabled = false;
        public bool IsDateToDisabled { get => isDateToDisabled; private set { isDateToDisabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDateToDisabled")); } }
        private bool isButtonEnabled;
        public ObservableCollection<CurrencyDataModel> ListViewData { get; private set; }
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { isButtonEnabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsButtonEnabled")); }
        }

        private bool isDateFromEnabled;

        public bool IsDateFromEnabled
        {
            get { return isDateFromEnabled; }
            set { isDateFromEnabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDateFromEnabled")); }
        }
        private int progressBarValue;

        public int ProgressBarValue
        {
            get { return progressBarValue; }
            set { progressBarValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressBarValue")); }
        }


        public DateTime DateTo { get => dateTo; private set { dateTo = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DateTo")); } }
        private NBPCurrencyInformation ci;
        public MainWindowViewModel()
        {
            ListViewData = new ObservableCollection<CurrencyDataModel>();
            DateFromSelected = new RelayCommand(o => {
                DateTime? casting = (o as DatePicker).SelectedDate;
                DateFrom = casting.GetValueOrDefault();
                IsDateToDisabled = true;
            });

            DateToSelected = new RelayCommand(o =>
            {
                DateTo = (o as DatePicker).SelectedDate.GetValueOrDefault();
                IsButtonEnabled = true;
            });
            StartButtonClicked = new RelayCommand(async o =>
            {
                ListViewData.Clear();
                ListViewData.Add(new CurrencyDataModel(CurrencyCode));
                IsButtonEnabled = false;
                IsDateToDisabled = false;
                IsDateFromEnabled = false;
                ProgressBarValue = 0;
                ci = new NBPCurrencyInformation(CurrencyCode, DateFrom, DateTo);
                ci.ProgressChanged += Ci_ProgressChanged;
                ci.DataChanged += Ci_DataChanged;
                await ci.StartParsingDataAsync();
                ProgressBarValue = 100;
                Ci_DataChanged(ci, ci);
                IsButtonEnabled = true;
                IsDateToDisabled = true;
                IsDateFromEnabled = true;
            });

            ComboboxItems = new ObservableCollection<string> { "USD", "EUR", "CHF", "GBP" };
        }

        private void Ci_DataChanged(object sender, NBPCurrencyInformation e)
        {
            ListViewData.Where(item => item.CurrencyCode == e.CurrencyCode).First().FeedNewValues(e);
        }

        private void Ci_ProgressChanged(object sender, CurrencyInformationProgressEventArgs e)
        {
            ProgressBarValue = e.Progress;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
