using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Taskara
{
	/// <summary>
	/// Lógica de interacción para ImageSelector.xaml
	/// </summary>
	public partial class ImageSelector : UserControl
	{
		public ImageSelector()
		{
			InitializeComponent();			
			DataContext = this;
			Loaded += ImageSelector_Loaded;
			MouseEnter += ImageSelector_MouseEnter;
			MouseLeave += ImageSelector_MouseLeave;
		}

		void ImageSelector_Loaded(object sender, RoutedEventArgs e)
		{
			if (IsMouseOver) btnChange.Visibility = System.Windows.Visibility.Visible;
			else btnChange.Visibility = System.Windows.Visibility.Hidden;
		}

		void ImageSelector_MouseLeave(object sender, MouseEventArgs e)
		{
			btnChange.Visibility = System.Windows.Visibility.Hidden;
		}

		void ImageSelector_MouseEnter(object sender, MouseEventArgs e)
		{
			btnChange.Visibility = System.Windows.Visibility.Visible;
		}

		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageSelector), new FrameworkPropertyMetadata(null));						
	}
}
