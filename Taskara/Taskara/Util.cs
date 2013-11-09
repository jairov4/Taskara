using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskara
{
	public class Util
	{
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
