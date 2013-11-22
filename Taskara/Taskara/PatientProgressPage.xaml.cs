using OxyPlot;
using OxyPlot.Series;
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
	public class PatientProgressViewModel : ObservableObject
	{
		private PrescriptionProgressReport progressReport;

		PlotModel _PlotModel;
		public PlotModel PlotModel
		{
			get { return _PlotModel; }
			set { _PlotModel = value; NotifyPropertyChanged("PlotModel"); }
		}

		public PatientProgressViewModel()
		{
			PlotModel = new PlotModel();

			var serie = new OxyPlot.Series.AreaSeries();
			var items = new BarItem[]{
				new BarItem(0.0),
				new BarItem(0.1),
				new BarItem(0.9),
				new BarItem(0.76),
				new BarItem(0.99),
				new BarItem(0.45),
				new BarItem(0.22),
			};

			serie.ItemsSource = items;
			serie.DataFieldY = "Value";

			serie.Fill = OxyColor.FromUInt32(0xFF30394F);

			PlotModel.Series.Add(serie);
			PlotModel.Background = OxyColor.FromUInt32(0xFF30394F);
		}

		public void OpenView(long id)
		{
			progressReport = App.Instance.Service.GetProgressReportById(id);
		}
	}

	/// <summary>
	/// Lógica de interacción para PatientProgressPage.xaml
	/// </summary>
	public partial class PatientProgressPage : Page
	{
		public PatientProgressPage()
		{
			InitializeComponent();
			NavigatedIn += PatientProgressPage_NavigatedIn;
		}

		PatientProgressViewModel ViewModel { get; set; }

		void PatientProgressPage_NavigatedIn(object sender, PageNavigationEventArgs e)
		{
			ViewModel = new PatientProgressViewModel();
			DataContext = ViewModel;
			if (e.Parameter is long)
			{
				var id = (long)e.Parameter;				
				ViewModel.OpenView(id);
			}
		}
	}
}
