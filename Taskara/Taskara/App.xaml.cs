using Db4objects.Db4o;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Taskara
{
	/// <summary>
	/// Lógica de interacción para App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static App Instance { get; private set; }

		public IObjectContainer ObjectContainer { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			Instance = this;
			base.OnStartup(e);
		}
	}
}
