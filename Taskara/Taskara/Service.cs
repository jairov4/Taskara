using Db4objects.Db4o;
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
		IObjectContainer ObjectContainer;

		public Service(IObjectContainer container)
		{
			ObjectContainer = container;
		}

		string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = MD5.Create();
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
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
			var bytes = new byte[256];
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
		}

		public Patient GetPatientById(long id)
		{
			var patient = ObjectContainer.Ext().GetByID(id);
			return patient as Patient;
		}

		public void SavePatient(Patient patient)
		{
			ObjectContainer.Store(patient);
		}
	}
}
