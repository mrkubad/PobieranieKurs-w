using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NBPWalutyWPF
{
    public class CallendarDateRangeAdapter
    {
        public static DateTime GetEnd(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(EndDateProperty);
        }

        public static void SetEnd(DependencyObject obj, DateTime value)
    {
        obj.SetValue(EndDateProperty, value);
    }


    public static readonly DependencyProperty EndDateProperty = DependencyProperty.RegisterAttached("End", typeof(DateTime), typeof(CallendarDateRangeAdapter), new PropertyMetadata(new DateTime(), OnEndDateChanged));


        private static void OnEndDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var firstBlackout = (sender as DatePicker).BlackoutDates[0];
            var secondBlackout = (sender as DatePicker).BlackoutDates[1];

            firstBlackout.End = (e.NewValue as DateTime?).Value.AddDays(-1).Date;
            secondBlackout.Start = DateTime.Now.AddDays(1).Date;

        }
        public static DateTime GetBlockAfter(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(EndDateProperty);
        }

        public static void SetBlockAfter(DependencyObject obj, DateTime value)
        {
            obj.SetValue(EndDateProperty, value);
        }


        public static readonly DependencyProperty BlockAfterProperty = DependencyProperty.RegisterAttached("BlockAfter", typeof(DateTime), typeof(CallendarDateRangeAdapter), new PropertyMetadata(new DateTime(), OnBlockAfterChanged));


        private static void OnBlockAfterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
           (sender as DatePicker).BlackoutDates.Add(new CalendarDateRange { Start = (e.NewValue as DateTime?).Value.AddDays(1).Date });
        }
    }
}
