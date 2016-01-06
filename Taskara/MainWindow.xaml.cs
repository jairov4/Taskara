using Microsoft.Win32;
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
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			new IndexPage();
			new PatientEditPage();
			new PrescriptionEditPage();
			//navigationSurface.Navigate(typeof(LoginPage), null, true);
			navigationSurface.Navigate(typeof(IndexPage));
		}

		private void keyboard_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F12)
			{
				var dlg = new SaveFileDialog();
				dlg.Title = "Exportar backup de base de datos";
				dlg.Filter = "Base de datos (*.yap)|*.yap|Todos los archivos (*.*)|*.*";
				var r = dlg.ShowDialog();
				if (r != true) return;
				App.Instance.ObjectContainer.Backup(dlg.FileName);
			}			
		}
	}
}
