using System.Globalization;

namespace BingoNumbers.Converters;

public  class InvertBoolConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{

		return value != null && !(bool)value;
	
	} //Convert

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{

		return value != null && !(bool)value;

    } //ConvertBack

} //InvertBoolConverter
