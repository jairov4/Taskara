using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Taskara.Model;

namespace Taskara
{
	/// <summary>
	/// Lógica de interacción para App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static App Instance { get; private set; }

		public IExtObjectContainer ObjectContainer { get; private set; }
		public Service Service { get; private set; }

		const string DatabaseFilename = "database.yap";
		const string AppFolder = "Taskara";

		protected override void OnStartup(StartupEventArgs e)
		{
			Instance = this;
			base.OnStartup(e);

			Debug.WriteLine(System.Globalization.CultureInfo.CurrentUICulture);
			Debug.WriteLine(System.Globalization.CultureInfo.CurrentCulture);

			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
				new FrameworkPropertyMetadata(
					System.Windows.Markup.XmlLanguage.GetLanguage(
						System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag
					)
				)
			);

			if (Keyboard.IsKeyDown(Key.F11))
			{
				var dlg = new Microsoft.Win32.OpenFileDialog();
				dlg.Title = "Reemplazar base de datos";
				dlg.Filter = "Base de datos (*.yap)|*.yap|Todos los archivos (*.*)|*.*";
				var r = dlg.ShowDialog();
				if (r == true)
				{
					var fn = GetDefaultDatabaseFilenameAndSure();
					File.Copy(dlg.FileName, fn, true);
				}
			}

			OpenDatabaseFromAppData();
		}

		public string GetDefaultDatabaseFilenameAndSure()
		{
			var fn = GetDefaultDatabaseFilename();
			var folder = Path.GetDirectoryName(fn);
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			return fn;
		}

		public string GetDefaultDatabaseFilename()
		{
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			folder = Path.Combine(folder, AppFolder);
			var fn = Path.Combine(folder, DatabaseFilename);
			return fn;
		}

		private void OpenDatabaseFromAppData()
		{
			var fn = GetDefaultDatabaseFilenameAndSure();

			ObjectContainer = Db4oEmbedded.OpenFile(fn).Ext();
			Service = new Service(ObjectContainer);

			Debug.WriteLine("Open database: {0}", (object)fn);

			var query = ObjectContainer.Query<AppUser>();
			if (query.Count == 0)
			{
				Service.CreateUser(new AppUser { Username = "admin", Password = "admin" });
			}
		}

	}
}
