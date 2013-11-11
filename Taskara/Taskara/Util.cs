using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Taskara
{
	public static class Util
	{
	}

	public abstract class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string prop)
		{
			if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}
