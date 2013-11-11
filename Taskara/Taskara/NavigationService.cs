using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Taskara
{
	public interface INavigationPage
	{
		NavigationService NavigationService { get; set; }
		void OnNavigatedIn(object parameter);
		bool OnNavigatingOut(Type page, object parameter);
	}

	/// <summary>
	/// Servicio de navegacion propio con bajo consumo de memoria que no rompe los
	/// enlaces de datos
	/// </summary>
	public class NavigationService
	{
		ContentControl navigationSurface;

		List<Tuple<Type, object>> JournalBack;
		List<Tuple<Type, object>> JournalForward;

		Type currentPage;
		object currentParameter;
		bool currentSkipJournal;

		public NavigationService(ContentControl surf)
		{
			navigationSurface = surf;
			JournalBack = new List<Tuple<Type, object>>();
			JournalForward = new List<Tuple<Type, object>>();

			currentPage = null;
			currentParameter = null;
			currentSkipJournal = true;
		}

		public void Navigate(Type page, object parameter, bool skipJournal = false)
		{
			NavigateInternal(page, parameter, skipJournal, false);
		}

		protected void NavigateInternal(Type page, object parameter, bool skipJournal, bool onJournalAction)
		{
			// Si la pagina implementa la interfaz se le notifica su salida
			// si se niega, cancelamos la navegacion
			var actualPage = navigationSurface.Content as INavigationPage;
			if (actualPage != null)
			{
				if (!actualPage.OnNavigatingOut(page, parameter)) return;
			}

			// Instanciamos la nueva pagina
			var pageObject = Activator.CreateInstance(page);
			var navPage = pageObject as INavigationPage;
			if (navPage != null) navPage.NavigationService = this;

			// Ajustamos el Journal y procedemos
			if (!currentSkipJournal && !onJournalAction)
			{
				JournalBack.Add(new Tuple<Type, object>(currentPage, currentParameter));
			}
			if (!onJournalAction)
			{
				JournalForward.Clear();
			}

			currentPage = page;
			currentParameter = parameter;
			currentSkipJournal = skipJournal;

			// Cargamos la nueva pagina y le notificamos
			navigationSurface.Content = pageObject;
			if (navPage != null)
				navPage.OnNavigatedIn(parameter);

			System.Diagnostics.Debug.WriteLine("Navigated to: {0}", currentPage);
		}

		public void GoBack()
		{
			var actualTuple = new Tuple<Type, object>(currentPage, currentParameter);
			NavigateInternal(JournalBack.Last().Item1, JournalBack.Last().Item2, false, true);
			JournalBack.RemoveAt(JournalBack.Count - 1);
			JournalForward.Add(actualTuple);
		}

		public void GoForward()
		{
			var actualTuple = new Tuple<Type, object>(currentPage, currentParameter);
			NavigateInternal(JournalForward.Last().Item1, JournalForward.Last().Item2, false, true);
			JournalForward.RemoveAt(JournalForward.Count - 1);
			JournalBack.Add(actualTuple);
		}

		public bool CanGoBack
		{
			get { return JournalBack.Count > 0; }
		}

		public bool CanGoForward
		{
			get { return JournalForward.Count > 0; }
		}
	}

	public class NavigationSurface : ContentControl
	{
		NavigationService svc;

		public NavigationSurface()
		{
			svc = new NavigationService(this);
		}

		static NavigationSurface()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationSurface), new FrameworkPropertyMetadata(typeof(NavigationSurface)));
		}

		public void Navigate(Type target, object parameter = null, bool skipJournal = false)
		{
			svc.Navigate(target, parameter, skipJournal);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var btnBack = GetTemplateChild("btnBack") as ButtonBase;
			var btnForward = GetTemplateChild("btnForward") as ButtonBase;

			if (btnBack != null)
				btnBack.Click += btnBack_Click;

			if (btnForward != null)
				btnForward.Click += btnForward_Click;
		}

		void btnForward_Click(object sender, RoutedEventArgs e)
		{
			if (svc.CanGoForward)
				svc.GoForward();
		}

		void btnBack_Click(object sender, RoutedEventArgs e)
		{
			if (svc.CanGoBack)
				svc.GoBack();
		}
	}

	public class PageNavigationEventArgs : EventArgs
	{
		public Type Target { get; private set; }
		public object Parameter { get; private set; }
		public bool StopNavigation { get; set; }

		public PageNavigationEventArgs(Type target, object parameter)
		{
			Target = target;
			Parameter = parameter;
			StopNavigation = false;
		}
	}

	public class Page : ContentControl, INavigationPage
	{
		public Page()
		{
		}

		public NavigationService NavigationService { get; set; }

		public event EventHandler<PageNavigationEventArgs> NavigatedIn;
		public event EventHandler<PageNavigationEventArgs> NavigatingOut;

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(Page), new FrameworkPropertyMetadata(string.Empty));


		void INavigationPage.OnNavigatedIn(object parameter)
		{
			if (NavigatedIn != null)
				NavigatedIn(this, new PageNavigationEventArgs(this.GetType(), parameter));
		}

		bool INavigationPage.OnNavigatingOut(Type page, object parameter)
		{
			if (NavigatingOut != null)
			{
				var e = new PageNavigationEventArgs(page, parameter);
				NavigatingOut(this, e);
				return !e.StopNavigation;
			}
			else return true;
		}

		protected void Navigate(Type target, object parameter = null, bool skipJournal = false)
		{
			NavigationService.Navigate(target, parameter, skipJournal);
		}
	}
}
