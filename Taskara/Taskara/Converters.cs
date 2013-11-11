using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Taskara.Model;

namespace Taskara
{
	public class DocumentTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return null;

			var v = (DocumentType)value;
			switch (v)
			{
				case DocumentType.TI: return "TI";
				case DocumentType.CC: return "CC";
				case DocumentType.Passport: return "Pasaporte";
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;
			if (str == null) return null;
			if (str == "TI") return DocumentType.TI;
			else if (str == "CC") return DocumentType.CC;
			else if (str == "Pasaporte") return DocumentType.Passport;
			else return null;
		}
	}

	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
