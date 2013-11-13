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
	public class PatientViewModel : ObservableObject
	{
		Patient _Patient;
		public Patient Patient
		{
			get { return _Patient; }
			set { _Patient = value; NotifyPropertyChanged("Patient"); }
		}

		DocumentType[] _DocumentTypes;
		public DocumentType[] DocumentTypes
		{
			get { return _DocumentTypes; }
			set { _DocumentTypes = value; NotifyPropertyChanged("DocumentTypes"); }
		}

		Genre[] _Genres;
		public Genre[] Genres
		{
			get { return _Genres; }
			set { _Genres = value; NotifyPropertyChanged("Genres"); }
		}

		bool _IsNew;
		public bool IsNew
		{
			get { return _IsNew; }
			set { _IsNew = value; NotifyPropertyChanged("IsNew"); }
		}

		public PatientViewModel()
		{
			Patient = new Patient();
			DocumentTypes = (DocumentType[])Enum.GetValues(typeof(DocumentType));
			Genres = (Genre[])Enum.GetValues(typeof(Genre));
		}

		public void CloseView()
		{
			if (IsNew)
			{
				if (!string.IsNullOrWhiteSpace(Patient.FirstName))
				{
					Save();
				}
			}
			else
			{
				App.Instance.Service.SavePatient(Patient);
			}
		}

		public void Save()
		{
			App.Instance.Service.SavePatient(Patient);
			IsNew = false;
		}

		public void OpenNew(long? id)
		{
			if (id.HasValue)
			{
				var p = App.Instance.Service.GetPatientById(id.Value);
				Patient = p;
				IsNew = false;
			}
			else
			{
				Patient = new Patient();
				IsNew = true;
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
			NavigatingOut += PatientEditPage_NavigatingOut;
		}

		void PatientEditPage_NavigatingOut(object sender, PageNavigationEventArgs e)
		{
			ViewModel.CloseView();
			e.CurrentParameter = ViewModel.IsNew ? null : (object)App.Instance.Service.GetId(ViewModel.Patient);
		}

		void PatientEditPage_NavigatedIn(object sender, PageNavigationEventArgs e)
		{
			ViewModel = new PatientViewModel();
			DataContext = ViewModel;
			ViewModel.OpenNew((long?)e.Parameter);
		}

		public PatientViewModel ViewModel { get; set; }

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.Save();
			Navigate(typeof(IndexPage));
		}
	}
}
