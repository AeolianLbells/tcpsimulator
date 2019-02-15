using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using tcpsimulationTest.Model;

namespace tcpsimulationTest.Converter

{
    [ValueConversion(typeof(ObservableCollection<Paneldata>), typeof(SolidColorBrush))]
    public class Hightlight_Converter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ObservableCollection<Paneldata> state = value as ObservableCollection<Paneldata>;

            foreach (var item in state)
            {
                if (item.Stuts == "机器")
                {
                    return Brushes.Orange;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return DependencyProperty.UnsetValue;
        }
    }
}
