using OxyPlot;
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
		private ProgressReport progressReport;

		PlotModel _PlotModel;
		public PlotModel PlotModel
		{
			get { return _PlotModel; }
			set { _PlotModel = value; NotifyPropertyChanged("PlotModel"); }
		}

		public void OpenView()
		{

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
		}
	}
}
