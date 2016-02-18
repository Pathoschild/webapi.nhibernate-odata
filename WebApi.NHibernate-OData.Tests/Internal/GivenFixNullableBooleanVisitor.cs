using System;
using System.Linq;
using NUnit.Framework;
using Pathoschild.WebApi.NhibernateOdata.Internal;
using Pathoschild.WebApi.NhibernateOdata.Tests.Models;
using QueryInterceptor;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Internal
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

		[Test]
		public void When_visiting_an_AndAlso_expression_with_one_nullable_and_one_non_nullable_boolean_Then_adds_cast_to_nullable()
		{
			var visitor = new FixNullableBooleanVisitor();
			var odataQuery = Helpers.Build<Parent>("$filter=substringof('q',Name) and Value eq 2");
			var data = new[] { new Parent { Id = 1, Name = "q", Value = 2 } }.AsQueryable();

			var results = odataQuery.ApplyTo(data).Cast<object>();
			results = results.InterceptWith(visitor);

			var list = results.ToList();
			Assert.That(list, Has.Count.EqualTo(1));
		}

		[Test]
		public void When_visiting_an_OrElse_expression_with_one_nullable_and_one_non_nullable_boolean_Then_adds_cast_to_nullable()
		{
			var visitor = new FixNullableBooleanVisitor();
			var odataQuery = Helpers.Build<Parent>("$filter=Id eq 1 or ValueGuid eq guid'e741c1cd-2e75-44c1-b3e8-7b0d5df435ce'");
			var data = new[] { new Parent { Id = 1, Name = "q", ValueGuid = new Guid("e741c1cd-2e75-44c1-b3e8-7b0d5df435ce") } }.AsQueryable();

			var results = odataQuery.ApplyTo(data).Cast<object>();
			results = results.InterceptWith(visitor);

			var list = results.ToList();
			Assert.That(list, Has.Count.EqualTo(1));
		}
	}
}
