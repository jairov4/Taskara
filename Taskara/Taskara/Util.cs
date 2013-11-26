using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using Taskara.Model;

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
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}

	public class PageFunctionResult
	{
		/// <summary>
		/// Page function result
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// Recovery state
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// Context indicating whats for called the page function
		/// </summary>
		public object Context { get; set; }
	}

	public class PageFunctionParameter
	{
		/// <summary>
		/// Page to pass the result
		/// </summary>
		public Type ReturnTarget { get; set; }

		/// <summary>
		/// The state to recover when page returns
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// Use context to pass the command or the message that trigger the 
		//  calling to page function
		/// </summary>
		public object Context { get; set; }

		public PageFunctionParameter()
		{
		}

		public PageFunctionParameter(Type returnTarget, object state, object context)
		{
			ReturnTarget = returnTarget;
			State = state;
			Context = context;
		}
	}
}
