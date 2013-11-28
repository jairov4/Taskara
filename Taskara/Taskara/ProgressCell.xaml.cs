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
		public DateTime Date { get; set; }
		public int Good { get; set; }
		public int Total { get; set; }

		public Visibility SmileyVisibility
		{
			get
			{
				return Good > 0 ? Visibility.Visible : Visibility.Hidden;
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
