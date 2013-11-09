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
	public class PatientViewModel
	{
		public Patient Patient { get; set; }

		public PatientViewModel()
		{
			Patient = new Patient();
		}
	}

	/// <summary>
	/// Lógica de interacción para PatientEditPage.xaml
	/// </summary>
	public partial class PatientEditPage : Page
	{
		public PatientEditPage()
		{
			InitializeComponent();
			Loaded += PatientEditPage_Loaded;
		}

		public PatientViewModel ViewModel { get; set; }

		void PatientEditPage_Loaded(object sender, RoutedEventArgs e)
		{
			var parameters = Util.ExtractParameters(NavigationService.CurrentSource.ToString());
			ViewModel = new PatientViewModel();			
			DataContext = ViewModel;
			if (!parameters.ContainsKey("New"))
			{
				var id = long.Parse(parameters["Id"]);
				var p = App.Instance.Service.GetPatientById(id);
				ViewModel.Patient = p;
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			App.Instance.Service.SavePatient(ViewModel.Patient);
			NavigationService.Navigate(new Uri("IndexPage.xaml", UriKind.Relative));
		}
	}
}
