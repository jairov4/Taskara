using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using Taskara.Model;

namespace Taskara
{
	public static class Util
	{
	}

	public class XmlDocumentsExchange
	{
		public void AddInfoFromFile(XDocument doc, Service svc)
		{
			var id = (long)doc.Root.Attribute("Id");
			var prescription = svc.GetPrescriptionById(id);
			if (prescription == null) prescription = AdaptPrescription(doc, svc);
			svc.SavePrescription(prescription);
			var rp = AdaptReportInfo(doc, svc);
			svc.SaveProgressReport(rp);
		}

		public Prescription AdaptPrescription(XDocument doc, Service svc)
		{
			var pr = new Prescription();
			pr.Patient = ReadPatient(doc, svc);
			//ex.Issued = (DateTime)doc.Root.Attribute("Date");
			pr.Issued = ParseDiegosDate(doc.Root.Attribute("Date"));
			pr.Excercises = new List<Excercise>();

			var activities = doc.Elements("Activity");
			foreach (var activityElement in activities)
			{
				var excercise = new Excercise();
				excercise.Name = (string)activityElement.Attribute("Exercise");
				excercise.Repetitions = (long)activityElement.Attribute("MinCorrect");
				excercise.Path = new[] { excercise.Name };
				pr.Excercises.Add(excercise);
			}
			return pr;
		}

		private DateTime ParseDiegosDate(XAttribute xAttribute)
		{
			var str = (string)xAttribute;
			var p = str.Split('/');
			var day = int.Parse(p[0]);
			var month = int.Parse(p[1]);
			var year = int.Parse(p[2]);
			return new DateTime(year, month, day);
		}

		public PrescriptionProgressReport AdaptReportInfo(XDocument doc, Service svc)
		{
			var rpt = new PrescriptionProgressReport();
			var patient = ReadPatient(doc, svc);
			rpt.Patient = patient;
			rpt.Progress = new List<ExcerciseProgressReport>();
			rpt.ReportId = (long)doc.Root.Attribute("Id");

			var activities = doc.Elements("Activity");
			foreach (var activityElement in activities)
			{
				var progress = new ExcerciseProgressReport();
				progress.Name = (string)activityElement.Attribute("Exercise");
				progress.Repetitions = (long)activityElement.Attribute("MinCorrect");
				progress.RepetitionsCount = (long)activityElement.Attribute("CurrentRepeat");
				progress.GoodRepetitionsCount = (long)activityElement.Attribute("CorrectRepeats");
				rpt.Progress.Add(progress);
			}

			return rpt;
		}

		private static Patient ReadPatient(XDocument doc, Service svc)
		{
			var patientNode = doc.Element("Patient");
			if (patientNode == null) throw new InvalidOperationException("Formato invalido, informacion de paciente no se encuentra");
			var patientId = (long)patientNode.Attribute("Id");
			var patient = svc.GetPatientById(patientId);
			if (patient == null)
			{
				patient = new Patient();
				patient.FirstName = (string)patientNode.Attribute("Name");
			}
			return patient;
		}
	}

	public abstract class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string prop)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}

	public class PageFunctionResult
	{
		/// <summary>
		/// Page function result
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// Recovery state
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// Context indicating whats for called the page function
		/// </summary>
		public object Context { get; set; }
	}

	public class PageFunctionParameter
	{
		/// <summary>
		/// Page to pass the result
		/// </summary>
		public Type ReturnTarget { get; set; }

		/// <summary>
		/// The state to recover when page returns
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// Use context to pass the command or the message that trigger the 
		//  calling to page function
		/// </summary>
		public object Context { get; set; }

		public PageFunctionParameter()
		{
		}

		public PageFunctionParameter(Type returnTarget, object state, object context)
		{
			ReturnTarget = returnTarget;
			State = state;
			Context = context;
		}
	}
}
