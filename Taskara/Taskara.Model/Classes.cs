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

	/// <summary>
	/// Paciente
	/// </summary>
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

		/// <summary>
		/// Datos binarios de la imagen
		/// </summary>
		public byte[] PhotoData { get; set; }

		/// <summary>
		/// Usualmente "image/jpeg"
		/// </summary>
		public string PhotoDataMime { get; set; }

		public string Diganosis { get; set; }
		public string Amplifier { get; set; }
	}

	/// <summary>
	/// Ejercicio del juego
	/// </summary>
	public class Exercise
	{
		/// <summary>
		/// Nombre del ejercicio
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Secuencia de categorias en orden y nombre del ejercicio
		/// </summary>
		public string[] Path { get; set; }

		/// <summary>
		/// Dias de la semana recomendados para este ejercicio
		/// </summary>
		public DayOfWeek[] WeeklyBasis { get; set; }

		/// <summary>
		/// Expresiones regulares para el ejercicio
		/// </summary>
		public string[] Syllables { get; set; }
	}

	/// <summary>
	/// Receta prescrita por el profesional de salud encargado
	/// </summary>
	public class Prescription
	{
		/// <summary>
		/// Fechad de expedicion de la receta
		/// </summary>
		public DateTime Issued { get; set; }

		/// <summary>
		/// Paciente asociado con esta receta
		/// </summary>
		public Patient Patient { get; set; }

		/// <summary>
		/// Lista de ejercicios prescritos
		/// </summary>
		public List<Exercise> Exercises { get; set; }

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

	/// <summary>
	/// Reporte para un solo ejercicio
	/// </summary>
	public class ExerciseProgressReport
	{
		/// <summary>
		/// Ejercicio asociado
		/// </summary>
		public Exercise Exercise { get; set; }

		/// <summary>
		/// Repeticiones totales ejercitadas por el paciente
		/// </summary>
		public int TotalRepetitions { get; set; }

		/// <summary>
		/// Repeticiones acertadas
		/// </summary>
		public int GoodRepetitions { get; set; }
	}

	/// <summary>
	/// Reporte de una prescripcion
	/// </summary>
	public class PrescriptionProgressReport
	{
		// Identificador del reporte
		//public long ReportId { get; set; }

		/// <summary>
		/// Fecha de expedicion del reporte de progreso
		/// </summary>
		public DateTime Issued { get; set; }

		/// <summary>
		/// Receta asociada a este reporte de progreso
		/// </summary>
		public Prescription Prescription { get; set; }

		/// <summary>
		/// Reporte por cada ejercicio realizado
		/// </summary>
		public List<ExerciseProgressReport> Progress { get; set; }

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

	/// <summary>
	/// Coleccion de reportes de progreso. 
	/// Cada reporte contiene el informe de una sola sesion.
	/// Una sesion esta asociada a una sola fecha.
	/// </summary>
	public class PrescriptionProgressReportCollection
	{
		public List<PrescriptionProgressReport> Reports { get; set; }

		static public PrescriptionProgressReportCollection LoadXml(Stream xml)
		{
			var ser = new DataContractSerializer(typeof(PrescriptionProgressReportCollection));
			return (PrescriptionProgressReportCollection)ser.ReadObject(xml);
		}

		public void SaveXml(Stream xml)
		{
			var ser = new DataContractSerializer(typeof(PrescriptionProgressReportCollection));
			ser.WriteObject(xml, this);
		}
	}
}
