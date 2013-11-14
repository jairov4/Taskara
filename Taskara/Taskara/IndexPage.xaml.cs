using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class IndexPageViewModel : ObservableObject
	{
		ObservableCollection<Patient> _Patients;
		public ObservableCollection<Patient> Patients
		{
			get { return _Patients; }
			set { _Patients = value; NotifyPropertyChanged("Patients"); }
		}

		Patient _SelectedPatient;
		public Patient SelectedPatient
		{
			get { return _SelectedPatient; }
			set { _SelectedPatient = value; UpdatePrescriptions(); NotifyPropertyChanged("SelectedPatient"); }
		}

		private void UpdatePrescriptions()
		{
			if (SelectedPatient == null)
			{
				Prescriptions = null;
			}
			else
			{
				var prescriptions = App.Instance.Service.ListPrescriptionsByPatient(SelectedPatient);
				Prescriptions = new ObservableCollection<Prescription>(prescriptions);
			}
		}

		ObservableCollection<Prescription> _Prescriptions;
		public ObservableCollection<Prescription> Prescriptions
		{
			get { return _Prescriptions; }
			set { _Prescriptions = value; NotifyPropertyChanged("Prescriptions"); }
		}

		Prescription _SelectedPrescription;
		public Prescription SelectedPrescription
		{
			get { return _SelectedPrescription; }
			set { _SelectedPrescription = value; NotifyPropertyChanged("SelectedPrescription"); }
		}
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
			ViewModel.Patients = new ObservableCollection<Patient>(App.Instance.Service.ListPatients());
		}

		private void btnCreateRecipe_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.SelectedPatient != null)
				Navigate(typeof(PrescriptionEditPage), ViewModel.SelectedPatient);
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

		private void btnViewRecipes_Click(object sender, RoutedEventArgs e)
		{
			if (grdPrescriptions.Visibility == Visibility.Visible)
				VisualStateManager.GoToElementState(root, "HidePrescriptionsPane", true);
			else
				VisualStateManager.GoToElementState(root, "ShowPrescriptionsPane", true);
		}

		private void btnClosePrescriptions_Click(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToElementState(root, "HidePrescriptionsPane", true);
		}

		private void btnViewPrescription_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.SelectedPrescription != null)
				Navigate(typeof(PrescriptionEditPage), App.Instance.Service.GetId(ViewModel.SelectedPrescription));
		}
	}
}
