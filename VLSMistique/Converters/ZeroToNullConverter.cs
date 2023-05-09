/* ZeroToNullConverter implementation based on common WPF IValueConverter examples. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VLSMistique.Converters
{
    /// <summary> Converts a zero to null. To stop certain grid elements from containing something. </summary>
    public class ZeroToNullConverter : IValueConverter
    {
        /// <summary> Converts a zero to null. </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == "0")
                return null;

            else
                return value;
        }

        /// <summary> Converts it back. </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
