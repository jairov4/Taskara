using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taskara.Model
{
	public class AppUser
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
	}

	public enum DocumentType
	{
		TI, CC, Passport
	}

	public enum Genre
	{
		Male, Female
	}

	public class Patient
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string Document { get; set; }
		public DocumentType DocumentType { get; set; }
		public DateTime LastProgressDate { get; set; }
		public DateTime Birthdate { get; set; }
		public string Phone { get; set; }
		public Genre Genre { get; set; }
		public byte[] PhotoData { get; set; }
		public string PhotoDataMime { get; set; }
	}

	public class Excercise
	{
		public string Name { get; set; }
		public long Repetitions { get; set; }
		public string[] Path { get; set; }
	}

	public class Prescription
	{
		public DateTime Issued { get; set; }
		public Patient Patient { get; set; }
		public IList<Excercise> Excercises { get; set; }

		public void SaveXml(System.IO.Stream str)
		{
			var ser = new System.Xml.Serialization.XmlSerializer(typeof(Prescription));
			ser.Serialize(str, this);
		}

		static public Prescription LoadXml(System.IO.Stream str)
		{
			var ser = new System.Xml.Serialization.XmlSerializer(typeof(Prescription));
			return (Prescription)ser.Deserialize(str);
		}
	}

	public class ExcerciseProgressReport
	{
		public Excercise Excercise { get; set; }
		public string Name { get; set; }
		public long Repetitions { get; set; }
		public long RepetitionsCount { get; set; }
		public long GoodRepetitionsCount { get; set; }
	}

	/// <summary>
	/// Reporte de una prescripcion
	/// </summary>
	public class PrescriptionProgressReport
	{
		public long ReportId { get; set; }
		public DateTime Issued { get; set; }

		[Obsolete("La prescripcion ya contiene el paciente")]
		public Patient Patient { get; set; }

		public Prescription Prescription { get; set; }
		public IList<ExcerciseProgressReport> Progress { get; set; }

		public void SaveXml(System.IO.Stream str)
		{
			var ser = new System.Xml.Serialization.XmlSerializer(typeof(PrescriptionProgressReport));
			ser.Serialize(str, this);
		}

		static public PrescriptionProgressReport LoadXml(System.IO.Stream str)
		{
			var ser = new System.Xml.Serialization.XmlSerializer(typeof(PrescriptionProgressReport));
			return (PrescriptionProgressReport)ser.Deserialize(str);
		}
	}
}
