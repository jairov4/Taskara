using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
	public class AvatarSelectorViewModel : ObservableObject
	{
		ImageSource _SelectedImage;
		public ImageSource SelectedImage
		{
			get { return _SelectedImage; }
			set { _SelectedImage = value; NotifyPropertyChanged("SelectedImage"); }
		}

		ObservableCollection<ImageSource> _AvailableImages;
		public ObservableCollection<ImageSource> AvailableImages
		{
			get { return _AvailableImages; }
			set { _AvailableImages = value; NotifyPropertyChanged("AvailableImages"); }
		}

		Type _ReturnTarget;
		public Type ReturnTarget
		{
			get { return _ReturnTarget; }
			set { _ReturnTarget = value; NotifyPropertyChanged("ReturnTarget"); }
		}
	}

	/// <summary>
	/// Lógica de interacción para AvatarSelectorPage.xaml
	/// </summary>
	public partial class AvatarSelectorPage : Page
	{
		public AvatarSelectorPage()
		{
			InitializeComponent();
			NavigatedIn += AvatarSelectorPage_NavigatedIn;
			NavigatingOut += AvatarSelectorPage_NavigatingOut;
		}

		void AvatarSelectorPage_NavigatingOut(object sender, PageNavigationEventArgs e)
		{

		}

		void AvatarSelectorPage_NavigatedIn(object sender, PageNavigationEventArgs e)
		{
			var relativeAvatarFolder = "Images/PkmAvatars";
			var contentFolder = Assembly.GetExecutingAssembly().Location;
			contentFolder = System.IO.Path.GetDirectoryName(contentFolder);
			contentFolder = System.IO.Path.Combine(contentFolder, relativeAvatarFolder);
			var imageFileNames = System.IO.Directory.GetFiles(contentFolder, "*.jpg");
			var packs = from imgPFn in imageFileNames
						let imgFn = System.IO.Path.GetFileName(imgPFn)
						let imgP = new Uri("/" + relativeAvatarFolder + "/" + imgFn, UriKind.Relative)
						let imgS = new BitmapImage(imgP)
						select imgS;

			ViewModel = new AvatarSelectorViewModel();
			ViewModel.AvailableImages = new ObservableCollection<ImageSource>(packs);
			DataContext = ViewModel;

			if (e.Parameter is PageFunctionParameter)
			{
				Parameter = e.Parameter as PageFunctionParameter;
			}
		}

		public PageFunctionParameter Parameter { get; set; }
		public AvatarSelectorViewModel ViewModel { get; set; }

		private void lstAvailableImages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			SendResult();
		}

		private void SendResult()
		{
			if (ViewModel.SelectedImage != null)
			{
				if (Parameter != null)
					NavigateWithResult(Parameter, ViewModel.SelectedImage);
			}
			else
			{
				MessageBox.Show("Por favor, seleccione una imagen");
			}
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			SendResult();
		}

		private void btnSelectFromFile_Click(object sender, RoutedEventArgs e)
		{
			if (Parameter == null) return;
			var dlg = new OpenFileDialog();
			dlg.Title = "Seleccionar imagen";
			dlg.Filter = "Archivos de imagen JPEG (*.jpg)|*.jpg|Todos los archivos (*.*)|*.*";
			var r = dlg.ShowDialog();
			if (r != true)
			{
				return;
			}
			var stream = dlg.OpenFile();
			var img = BitmapFrame.Create(stream);
			NavigateWithResult(Parameter, img);
		}
	}
}
