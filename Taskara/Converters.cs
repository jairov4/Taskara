using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
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

	public class BinaryJpegToImageConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			byte[] vals = value as byte[];
			if (vals == null) return null;
			using (var stream = new MemoryStream(vals))
			{
				var dec = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
				var frm = dec.Frames[0];
				return frm;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is BitmapSource)
			{
				var frm = BitmapFrame.Create(value as BitmapSource);
				var enc = new JpegBitmapEncoder();
				enc.Frames.Add(frm);
				var stream = new MemoryStream();
				enc.Save(stream);
				return stream.ToArray();
			}
			else if (value is Uri)
			{
				var frm = BitmapFrame.Create(value as Uri);
				var enc = new JpegBitmapEncoder();
				enc.Frames.Add(frm);
				var stream = new MemoryStream();
				enc.Save(stream);
				return stream.ToArray();
			}
			return null;
		}
	}
}
