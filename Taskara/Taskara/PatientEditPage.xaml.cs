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
	public class PatientViewModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Document { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string DocumentType { get; set; }
		public DateTime LastProgressDate { get; set; }
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

		bool IsNew;

		void PatientEditPage_Loaded(object sender, RoutedEventArgs e)
		{
			var parameters = Util.ExtractParameters(NavigationService.CurrentSource.ToString());
			var vm = new PatientViewModel();
			if (parameters.ContainsKey("New"))
			{
				IsNew = true;
				DataContext = vm;
			}
			else
			{
				var id = long.Parse(parameters["Id"]);
				var p = App.Instance.Service.GetPatientById(id);
				vm.Address = p.Address;
				vm.Document = p.Document;
				vm.FirstName = p.FirstName;
				vm.LastName = p.LastName;
				vm.LastProgressDate = p.LastProgressDate;
				vm.Phone = p.Phone;
			}
		}
	}
}
