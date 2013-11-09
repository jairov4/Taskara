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
	/// Lógica de interacción para IndexPage.xaml
	/// </summary>
	public partial class IndexPage : Page
	{
		public IndexPage()
		{
			InitializeComponent();
		}

		private void btnCreateRecipe_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("PrescriptionEditPage.xaml", UriKind.Relative));
		}

		private void btnNewPatient_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("PatientEditPage.xaml?New", UriKind.Relative), "New");
		}
	}
}
