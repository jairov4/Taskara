using System;
using Taskara.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Taskara.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			Prescription p = new Prescription();
			p.Patient = new Patient();
			p.Patient.FirstName = "Pepe";
			p.Patient.LastName = "Lucho";
			p.Patient.LastProgressDate = DateTime.Now.AddDays(-2);
			p.Patient.Phone = "3333333";
			p.Patient.Genre = Genre.Male;
			p.Patient.DocumentType = DocumentType.TI;
			p.Patient.Document = "113322445566";
			p.Issued = DateTime.Now.AddDays(1);
			p.Exercises = new System.Collections.Generic.List<Exercise>();
			p.Exercises.Add(new Exercise()
			{
				Name = "Ej 1",
				Path = new[] { "p1", "p2", "Ej 1" }
			});
			p.Exercises.Add(new Exercise()
			{
				Name = "Ej 2",
				Path = new[] { "p1", "p2", "Ej 2" }
			});
			p.Exercises.Add(new Exercise()
			{
				Name = "Ej 2",
				Path = new[] { "p1", "p2", "Ej 3" }
			});
			var stream = new MemoryStream();			
			p.SaveXml(stream);
			stream.Seek(0, SeekOrigin.Begin);
			var r2 = Prescription.LoadXml(stream);

			Assert.IsNotNull(r2.Exercises);
			Assert.AreEqual(r2.Exercises.Count, p.Exercises.Count);
			Assert.AreEqual(r2.Issued, p.Issued);
			Assert.IsNotNull(r2.Patient);
			Assert.AreEqual(r2.Patient.FirstName, p.Patient.FirstName);

			stream.Seek(0, SeekOrigin.Begin);
			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, (int)stream.Length);
			var str = System.Text.Encoding.UTF8.GetString(bytes);
			Console.WriteLine(str);
		}
	}
}
