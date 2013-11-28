using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Taskara.Model;

namespace Taskara
{
	[Serializable]
	public class AlreadyExistObjectException : InvalidOperationException
	{
		public AlreadyExistObjectException() : base("Ya existe un objeto equivalente") { }
		public AlreadyExistObjectException(string message) : base(message) { }
		public AlreadyExistObjectException(string message, Exception inner) : base(message, inner) { }
		protected AlreadyExistObjectException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	public class Service
	{
		IExtObjectContainer ObjectContainer;

		public Service(IExtObjectContainer container)
		{
			ObjectContainer = container;
		}

		string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			var sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}

		string HashPassword(string password, string salt)
		{
			return CalculateMD5Hash(string.Format("{0}:{1}", password, salt));
		}

		string GetSalt()
		{
			var r = new Random();
			var bytes = new byte[64];
			r.NextBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		public bool ValidateUser(string user, string password)
		{
			user = user.ToLowerInvariant().Trim();
			password = password.Trim();
			var appUser = ObjectContainer.Query<AppUser>().FirstOrDefault(x => x.Username == user);
			if (appUser == null) return false;
			var hash = HashPassword(password, appUser.Salt);
			return hash == appUser.Password;
		}

		public void CreateUser(AppUser user)
		{
			var salt = GetSalt();
			var userS = new AppUser() { Username = user.Username.ToLowerInvariant() };
			user.Password = HashPassword(user.Password, salt);
			user.Salt = salt;
			ObjectContainer.Store(user);
			ObjectContainer.Commit();
		}

		public Patient GetPatientById(long id)
		{
			var patient = ObjectContainer.GetByID(id);
			return patient as Patient;
		}

		public Patient GetPatientByDocumentId(string id, DocumentType type)
		{
			var p = ObjectContainer.Query<Patient>(x => x.Document == id && x.DocumentType == type);
			return p.FirstOrDefault();
		}

		public void SavePatient(Patient patient)
		{
			if (!ObjectContainer.IsStored(patient))
			{
				if (!string.IsNullOrWhiteSpace(patient.Document))
				{
					var contains = ObjectContainer.Query<Patient>(x => x.Document == patient.Document && x.DocumentType == patient.DocumentType).FirstOrDefault();
					if (contains != null) throw new AlreadyExistObjectException("Ya existe un paciente con la misma identificacion");
				}
			}
			patient.FirstName = patient.FirstName ?? string.Empty;
			patient.LastName = patient.LastName ?? string.Empty;
			patient.FirstName = patient.FirstName.Trim();
			patient.LastName = patient.LastName.Trim();
			ObjectContainer.Store(patient);
			ObjectContainer.Commit();
		}

		public void SavePrescription(Prescription prescription)
		{
			if (prescription.Patient == null)
			{
				throw new InvalidOperationException("Prescripcion sin paciente");
			}
			if (!ObjectContainer.IsStored(prescription))
			{
				var foundExist = ObjectContainer.Query<Prescription>(x =>
					x.Patient.DocumentType == prescription.Patient.DocumentType
					&& x.Patient.Document == prescription.Patient.Document
					&& x.Issued == prescription.Issued
				).FirstOrDefault();
				if (foundExist != null) throw new AlreadyExistObjectException("Ya existe una prescripcion para este paciente y en esta fecha");
			}
			ObjectContainer.Store(prescription, 2);
			ObjectContainer.Commit();
		}

		public void SaveProgressReport(PrescriptionProgressReport report)
		{
			if (!ObjectContainer.IsStored(report))
			{
				var foundExist = ObjectContainer.Query<PrescriptionProgressReport>(x =>
					x.Issued == report.Issued
					&& x.Prescription.Patient.Document == report.Prescription.Patient.Document
					&& x.Prescription.Patient.DocumentType == report.Prescription.Patient.DocumentType
					&& x.Prescription.Issued == report.Prescription.Issued
				).FirstOrDefault();
				if (foundExist != null) throw new AlreadyExistObjectException("Ya existe registro de este reporte de progreso");
			}

			// Asegura la prescripcion asociada, para no duplicarla
			var rp = GetPrescriptionByDateAndPatient(report.Prescription.Issued, report.Prescription.Patient.Document, report.Prescription.Patient.DocumentType);
			if (rp != null)
			{
				report.Prescription = rp;
			}
			else
			{
				// Si la prescripcion no se encuentra asegura el paciente, para crear la prescripcion con todo y paciente
				var pt = GetPatientByDocumentId(report.Prescription.Patient.Document, report.Prescription.Patient.DocumentType);
				if (pt != null)
				{
					report.Prescription.Patient = pt;
				}
			}
			ObjectContainer.Store(report, 3);
			ObjectContainer.Commit();
		}

		private Prescription GetPrescriptionByDateAndPatient(DateTime dateTime, string p, DocumentType documentType)
		{
			var r = ObjectContainer.Query<Prescription>(x => x.Issued == dateTime && x.Patient.Document == p && x.Patient.DocumentType == documentType).FirstOrDefault();
			return r;
		}

		public IList<Patient> ListPatients()
		{
			return ObjectContainer.Query<Patient>();
		}

		public IList<Prescription> ListPrescriptionsByPatient(Patient p)
		{
			if (!ObjectContainer.IsStored(p)) throw new InvalidOperationException("No se puede buscar prescripciones usando un paciente no almacenado");
			return ObjectContainer.Query<Prescription>(x => x.Patient == p);
		}

		public long GetId(object obj)
		{
			return ObjectContainer.GetID(obj);
		}

		public Prescription GetPrescriptionById(long id)
		{
			var p = ObjectContainer.GetByID(id) as Prescription;
			return p;
		}

		public PrescriptionProgressReport GetProgressReportById(long id)
		{
			var p = ObjectContainer.GetByID(id) as PrescriptionProgressReport;
			return p;
		}


		public List<PrescriptionProgressReport> ListProgressReportsByPatientId(long pid)
		{
			var p = GetPatientById(pid);
			var l = ObjectContainer.Query<PrescriptionProgressReport>(x => x.Prescription.Patient == p);
			//var l = ObjectContainer.Query<PrescriptionProgressReport>(x => x.Prescription.Patient.Document == p.Document && x.Prescription.Patient.DocumentType == p.DocumentType);			
			return new List<PrescriptionProgressReport>(l);
		}
	}
}
