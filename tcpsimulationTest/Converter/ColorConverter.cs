using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace tcpsimulationTest.Converter
{
    /// <summary>
    /// 自定义事件转换
    /// </summary>
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class ColorConverter : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string state = value as string;
            if (state.Contains("失败"))
            {
                return Brushes.Red;
            }
            else if (state.Contains("已"))
            {
                return Brushes.Green;
            }
            else//一般情况
                return Brushes.Black;

        }


        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return DependencyProperty.UnsetValue;
        }
    }
}
