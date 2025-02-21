
using System.Globalization;

namespace BingoNumbers.Views
{
	public  class InvertBoolConverter:IValueConverter
	{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value != null && !(bool)value;
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value != null && !(bool)value;
		}
	}
}
