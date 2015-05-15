using System.Linq;
using NUnit.Framework;
using Pathoschild.WebApi.NhibernateOdata;
using WebApi.NHibernate_OData.Tests.Models;

namespace WebApi.NHibernate_OData.Tests
{
	[TestFixture]
	public class GivenFixOdataQueryAttribute
	{
		[Test]
		public void When_applying_fixes_statically_Then_works()
		{
			var data = TestData.GetTestChildren();

			var results = FixOdataQueryAttribute.ApplyFix(data).ToList();
			Assert.That(results, Has.Count.EqualTo(1));

			var results2 = FixOdataQueryAttribute.ApplyFixWithoutGeneric(data).Cast<object>().ToList();
			Assert.That(results2, Has.Count.EqualTo(1));
		}
	}
}
