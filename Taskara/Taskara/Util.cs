using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Taskara
{
	public static class Util
	{
		public static void Navigate(this NavigationService svc, string uri_fmt, params object[] p)
		{
			var str = string.Format(uri_fmt, p);
			svc.Navigate(new Uri(str, UriKind.Relative));
		}

		public static void Navigate(this Page page, string uri_fmt, params object[] p)
		{
			Navigate(page.NavigationService, uri_fmt, p);
		}

		public static IDictionary<string, string> ExtractParameters(string RawUrl)
		{
			int index = RawUrl.IndexOf("?");
			if (index > 0)
			{
				RawUrl = RawUrl.Substring(index).Remove(0, 1);
				var items = RawUrl.Split('&');
				var dict = items.Select(x => x.Split('=')).ToDictionary(x => x.First(), x => x.Length > 1 ? x[1] : null);
				return dict;
			}
			return new Dictionary<string, string>();
		}
	}
}
