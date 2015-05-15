using System.Linq;

using NUnit.Framework;

using Pathoschild.WebApi.NhibernateOdata.Internal;

using QueryInterceptor;

using WebApi.NHibernate_OData.Tests.Models;

namespace WebApi.NHibernate_OData.Tests.Internal
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class GivenFixNullableBooleanVisitor
	{
		[Test]
		public void When_visiting_an_expression_with_nullable_boolean_Then_converts_it_to_non_nullable()
		{
			var visitor = new FixNullableBooleanVisitor();
			var odataQuery = Helpers.Build<Child>("$filter=Parent/Id eq 61 and Id eq 11");
			var data = TestData.GetTestChildren();

			var results = odataQuery.ApplyTo(data).Cast<object>();
			results = results.InterceptWith(visitor);

			var list = results.ToList();
			Assert.That(list, Has.Count.EqualTo(1));
		}
	}
}
