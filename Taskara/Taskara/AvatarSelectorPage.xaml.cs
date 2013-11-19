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
		}

		public AvatarSelectorViewModel ViewModel { get; set; }
	}
}
