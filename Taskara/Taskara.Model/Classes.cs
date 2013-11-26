using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
		public string Diganosis { get; set; }
		public string Amplifier { get; set; }
	}

	public class Excercise
	{
		public string Name { get; set; }
		public string[] Path { get; set; }
		public DayOfWeek[] WeeklyBasis { get; set; }
	}

	public class Prescription
	{
		public DateTime Issued { get; set; }
		public Patient Patient { get; set; }
		public IList<Excercise> Excercises { get; set; }

		public void SaveXml(Stream str)
		{
			var ser = new DataContractSerializer(typeof(Prescription));
			ser.WriteObject(str, this);
		}

		static public Prescription LoadXml(Stream str)
		{
			var ser = new DataContractSerializer(typeof(Prescription));
			return (Prescription)ser.ReadObject(str);
		}
	}
	
	public class ExcerciseProgressReport
	{
		public Excercise Excercise { get; set; }
		public string Name { get; set; }
		public long TotalRepetitions { get; set; }
		public long GoodRepetitions { get; set; }
	}

	/// <summary>
	/// Reporte de una prescripcion
	/// </summary>
	public class PrescriptionProgressReport
	{
		public long ReportId { get; set; }
		public DateTime Issued { get; set; }

		public Prescription Prescription { get; set; }
		public IList<ExcerciseProgressReport> Progress { get; set; }

		public void SaveXml(Stream str)
		{
			var ser = new DataContractSerializer(typeof(PrescriptionProgressReport));
			ser.WriteObject(str, this);
		}

		static public PrescriptionProgressReport LoadXml(Stream str)
		{
			var ser = new DataContractSerializer(typeof(PrescriptionProgressReport));
			return (PrescriptionProgressReport)ser.ReadObject(str);
		}
	}
}
