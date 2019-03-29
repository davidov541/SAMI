using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using SAMI.Logging;

namespace SAMI.Application.LogViewer
{
    internal class CheckBoxToFilterArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<bool?> filterArr = value as IEnumerable<bool?>;
            return filterArr.ElementAt((int)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<bool?> lst = new List<bool?>();
            for (int i = 0; i < Enum.GetValues(typeof(LogCategory)).Length; i++)
            {
                if (i == (int)parameter)
                {
                    lst.Add((bool)value);
                }
                else
                {
                    lst.Add(null);
                }
            }
            return lst;
        }
    }
}
