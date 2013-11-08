using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taskara.Model
{
    public class User
    {
		public string Username { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
    }

	public enum DocumentType
	{
		TI, CC, Passport
	}

	public class Patient
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string Document { get; set; }
		public DocumentType DocumentType { get; set; }
		public DateTime LastProgress { get; set; }
		public DateTime Birthdate { get; set; }
		public byte[] PhotoData { get; set; }
		public string PhotoDataMime { get; set; }
	}
		
	public class Prescription
	{
		public DateTime Issued { get; set; }
		public Patient Patient { get; set; }
		public Guid[] Excercises { get; set; }
	}

	public class ProgressReport
	{
		public DateTime Issued { get; set; }
		public Patient Patient { get; set; }
	}
}
