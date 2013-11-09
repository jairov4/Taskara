using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public class PatientViewModel : INotifyPropertyChanged
	{
		public Patient Patient { get; set; }

		public PatientViewModel()
		{
			Patient = new Patient();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
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
			NavigatedIn += PatientEditPage_NavigatedIn;
		}

		void PatientEditPage_NavigatedIn(object sender, PageNavigationEventArgs e)
		{
			ViewModel = new PatientViewModel();
			DataContext = ViewModel;
			if (e.Parameter != null)
			{
				var id = (long)e.Parameter;
				var p = App.Instance.Service.GetPatientById(id);
				ViewModel.Patient = p;
				ViewModel.NotifyPropertyChanged("Patient");
			}
		}

		public PatientViewModel ViewModel { get; set; }

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			App.Instance.Service.SavePatient(ViewModel.Patient);
			Navigate(typeof(IndexPage));
		}
	}
}
