using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Taskara
{
	/// <summary>
	/// Lógica de interacción para WeekDaysSelector.xaml
	/// </summary>
	public partial class WeekDaysSelector : UserControl
	{
		public WeekDaysSelector()
		{
			InitializeComponent();
			DataContextChanged += WeekDaysSelector_DataContextChanged;
		}

		void WeekDaysSelector_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			days = DataContext as ICollection<DayOfWeek>;
			if (days == null) return;
			b0.IsChecked = days.Contains(DayOfWeek.Monday);
			b1.IsChecked = days.Contains(DayOfWeek.Tuesday);
			b2.IsChecked = days.Contains(DayOfWeek.Wednesday);
			b3.IsChecked = days.Contains(DayOfWeek.Thursday);
			b4.IsChecked = days.Contains(DayOfWeek.Friday);
			b5.IsChecked = days.Contains(DayOfWeek.Saturday);
			b6.IsChecked = days.Contains(DayOfWeek.Sunday);
		}

		ICollection<DayOfWeek> days;

		private void b_Update(object sender, RoutedEventArgs e)
		{
			if (days == null) return;
			var r = (ToggleButton)sender;
			if (r == b0) UpdateState(r, DayOfWeek.Monday);
			if (r == b1) UpdateState(r, DayOfWeek.Tuesday);
			if (r == b2) UpdateState(r, DayOfWeek.Wednesday);
			if (r == b3) UpdateState(r, DayOfWeek.Thursday);
			if (r == b4) UpdateState(r, DayOfWeek.Friday);
			if (r == b5) UpdateState(r, DayOfWeek.Saturday);
			if (r == b6) UpdateState(r, DayOfWeek.Sunday);
		}

		private void UpdateState(ToggleButton r, DayOfWeek d)
		{
			if (r.IsChecked.HasValue && r.IsChecked.Value && !days.Contains(d))
				days.Add(d);
			if (r.IsChecked.HasValue && !r.IsChecked.Value && days.Contains(d))
				days.Remove(d);
		}
	}
}
