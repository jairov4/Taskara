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
	public class ProgressWeekData
	{
		public ProgressWeekData()
		{
			Days = new ProgressCellData[7];
		}

		public ProgressCellData[] Days { get; set; }
	}

	public class ProgressCellData
	{
		public PrescriptionProgressReport Report { get; set; }

		DateTime? _Date = null;
		public DateTime Date { get { return _Date ?? Report.Issued; } set { _Date = value; } }
		public int TotalGood { get { return Report == null ? 0 : Report.Progress.Sum(x => x.GoodRepetitions); } }
		public int Total { get { return Report == null ? 0 : Report.Progress.Sum(x => x.TotalRepetitions); } }

		public Visibility SmileyVisibility
		{
			get
			{
				return TotalGood > 0 ? Visibility.Visible : Visibility.Hidden;
			}
		}

		public Visibility IsFirstBeginOfWeekAndMonth
		{
			get
			{
				if (Date.DayOfWeek != Util.GetFirstDayOfWeek()) return Visibility.Hidden;
				if (Date.Day > 7) return Visibility.Hidden;
				return Visibility.Visible;
			}
		}
	}

	/// <summary>
	/// Lógica de interacción para ProgressCell.xaml
	/// </summary>
	public partial class ProgressCell : UserControl
	{
		public ProgressCell()
		{
			InitializeComponent();
		}
	}
}
