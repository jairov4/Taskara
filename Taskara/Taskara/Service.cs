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

		public void SavePatient(Patient patient)
		{
			if (!ObjectContainer.IsStored(patient))
			{
				if (!string.IsNullOrWhiteSpace(patient.Document))
				{
					var contains = ObjectContainer.Query<Patient>(x => x.Document == patient.Document && x.DocumentType == patient.DocumentType).FirstOrDefault();
					if (contains != null) throw new InvalidOperationException("Ya existe un paciente con la misma identificacion");
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
			ObjectContainer.Store(prescription, 2);
			ObjectContainer.Commit();
		}

		public void SaveProgressReport(ProgressReport report)
		{
			ObjectContainer.Store(report, 2);
			ObjectContainer.Commit();
		}

		public IList<Patient> ListPatients()
		{
			return ObjectContainer.Query<Patient>();
		}

		public IList<Prescription> ListPrescriptionsByPatient(Patient p)
		{
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

		public ProgressReport GetProgressReportById(long id)
		{
			var p = ObjectContainer.GetByID(id) as ProgressReport;
			return p;
		}
				
	}
}
