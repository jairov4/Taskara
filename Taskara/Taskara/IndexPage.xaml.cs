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
using Taskara.Model;

namespace Taskara
{
	public class IndexPageViewModel
	{
		public IList<Patient> Patients { get; set; }
		public Patient SelectedPatient { get; set; }
	}

	/// <summary>
	/// Lógica de interacción para IndexPage.xaml
	/// </summary>
	public partial class IndexPage : Page
	{
		public IndexPage()
		{
			InitializeComponent();
			Loaded += IndexPage_Loaded;
		}

		public IndexPageViewModel ViewModel { get; set; }

		void IndexPage_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel = new IndexPageViewModel();
			DataContext = ViewModel;
			ViewModel.Patients = App.Instance.Service.ListPatients();
		}

		private void btnCreateRecipe_Click(object sender, RoutedEventArgs e)
		{
			Navigate(typeof(PrescriptionEditPage));
		}

		private void btnNewPatient_Click(object sender, RoutedEventArgs e)
		{
			Navigate(typeof(PatientEditPage));
		}

		private void lstPatients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			if (ViewModel.SelectedPatient != null)
			{
				var id = App.Instance.Service.GetId(ViewModel.SelectedPatient);
				this.Navigate(typeof(PatientEditPage), id);
			}
		}

		private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (grdPrescriptions.Visibility == System.Windows.Visibility.Visible)
			{
				grdPrescriptions.Visibility = System.Windows.Visibility.Collapsed;
			}
		}
	}
}
