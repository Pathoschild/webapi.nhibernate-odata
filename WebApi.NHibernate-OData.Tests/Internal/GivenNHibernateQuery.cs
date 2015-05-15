using System;
using System.Linq;

using Newtonsoft.Json;

using NHibernate;
using NHibernate.Linq;

using NUnit.Framework;

using Pathoschild.WebApi.NhibernateOdata.Internal;

using QueryInterceptor;

using WebApi.NHibernate_OData.Tests.Mappings;
using WebApi.NHibernate_OData.Tests.Models;

namespace WebApi.NHibernate_OData.Tests.Internal
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    [Category("Integration")]
    public class GivenNHibernateQuery
    {
        private static readonly ISessionFactory SessionFactory = NHibernateHelper.SessionFactory;
        private ISession _session;

        [SetUp]
        public void SetUp()
        {
            this._session = SessionFactory.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            this._session.Dispose();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void When_querying_nullable_Then_queries_database(bool withVisitor)
        {
            var visitor = new FixNullableBooleanVisitor();
            var odataQuery = Helpers.Build<Child>("$filter=Parent/Id eq 61 and Id eq 11");
            var children = this._session.Query<Child>();

            var results = odataQuery.ApplyTo(children).Cast<object>();
            if (withVisitor)
            {
                results = results.InterceptWith(visitor);
            }

            var list = results.ToList();
            Assert.That(list, Has.Count.EqualTo(1));
        }

        [Test]
        public void When_expanding_children_Then_works()
        {
            Console.WriteLine("What it should look like:");
            var r = this._session.Query<Parent>().Select(x => new { x.Id, x.Children }).ToList();
            Console.WriteLine("{0} results", r.Count);

            var odataQuery = Helpers.Build<Parent>("$select=Id,Children&$expand=Children");
            var parents = this._session.Query<Parent>();

            var results = odataQuery.ApplyTo(parents).Cast<object>();

            var json = JsonConvert.SerializeObject(results);

            Console.WriteLine(json);
        }

        [Test]
        public void When_projecting_one_column_Then_only_queries_one_column()
        {
            Console.WriteLine("What it should look like:");
            var query = this._session.Query<Parent>().Select(x => new { x.Name }).ToList();
            Console.WriteLine("{0} results", query.Count);

            var odataQuery = Helpers.Build<Parent>("$select=Name");
            var parents = this._session.Query<Parent>();

            var results = odataQuery.ApplyTo(parents).Cast<object>();

            var json = JsonConvert.SerializeObject(results);

            Console.WriteLine(json);

            Assert.Inconclusive("Gotta check the output of NHibernate");
        }

        [Test]
        public void When_filtering_one_column_with_substringof_Then_uses_where_like()
        {
            var visitor = new FixSubstringOfVisitor();
            Console.WriteLine("What it should look like:");
            var r = this._session.Query<Parent>().Where(x => x.Name.Contains("parent"));
            r = r.InterceptWith(visitor);
            Console.WriteLine("{0} results", r.ToList().Count);

            var odataQuery = Helpers.Build<Parent>("$filter=substringof('parent',Name) eq true");
            var parents = this._session.Query<Parent>();
            parents = parents.InterceptWith(visitor);

            var results = odataQuery.ApplyTo(parents).Cast<Parent>().ToList();
            Assert.That(results, Has.Count.EqualTo(1));
        }

        [Test]
        public void When_filtering_one_column_with_eq_Then_uses_where()
        {
            Console.WriteLine("What it should look like:");
            var r = this._session.Query<Parent>().Where(x => x.Name == "parent");
            Console.WriteLine("{0} results", r.ToList().Count);

            var odataQuery = Helpers.Build<Parent>("$filter=Name eq 'parent 61'");
            var parents = this._session.Query<Parent>();

            var results = odataQuery.ApplyTo(parents).Cast<Parent>().ToList();
            Assert.That(results, Has.Count.EqualTo(1));
        }
    }
}
