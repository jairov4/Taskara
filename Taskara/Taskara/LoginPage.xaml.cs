using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	/// Lógica de interacción para LoginPage.xaml
	/// </summary>
	public partial class LoginPage : Page
	{
		public LoginPage()
		{
			InitializeComponent();
		}

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			if (!App.Instance.Service.ValidateUser(txtUser.Text, txtPassword.Password))
			{
				txtPassword.Password = string.Empty;
				Thread.Sleep(1000);
				MessageBox.Show("Usuario o contraseña incorrecta, por favor intenta nuevamente");
				return;
			}
			NavigationService.Navigate(new Uri("IndexPage.xaml", UriKind.Relative));
		}
	}
}
