using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Taskara
{
	public class ExerciseExchangeDefinition
	{
		public string Level { get; set; }
		public string ShortDescription { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public int? Order { get; set; }
		public string Exercise { get; set; }
		public int Skill { get; set; }
	}

	public class ExerciseExchangeDefinitionCollection
	{
		public List<ExerciseExchangeDefinition> ExerciseDefinitions { get; set; }

		public void SaveXml(Stream str)
		{
			using (var tr = new StreamWriter(str, Encoding.UTF8))
			{
				var ser = new XmlSerializer(this.GetType());
				ser.Serialize(tr, this);
				tr.Close();
			}
		}

		public static ExerciseExchangeDefinitionCollection LoadXml(Stream str)
		{
			using (var tr = new StreamReader(str, Encoding.UTF8))
			{
				var ser = new XmlSerializer(typeof(ExerciseExchangeDefinitionCollection));
				var r = (ExerciseExchangeDefinitionCollection)ser.Deserialize(tr);
				tr.Close();
				return r;
			}
		}
	}
}
