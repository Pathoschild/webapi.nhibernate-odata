using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;

using NHibernate;
using NHibernate.Linq;

using Pathoschild.WebApi.NhibernateOdata.Tests.Mappings;
using Pathoschild.WebApi.NhibernateOdata.Tests.Models;

namespace Pathoschild.WebApi.NhibernateOdata.Sample.Controllers
{
	[RoutePrefix("children")]
	public class ChildrenController : ApiController
	{
		private readonly ISession Session;

		public ChildrenController()
		{
			this.Session = NHibernateHelper.SessionFactory.OpenSession();
		}

		/// <summary>
		/// Gets all children and applies the fix manually on the results.
		/// </summary>
		/// <param name="options">The OData options.</param>
		/// <returns>A filtered IQueryable!</returns>
		[Route("")]
		public IQueryable Get(ODataQueryOptions<Child> options)
		{
			var results = this.Session.Query<Child>();
			var filtered = options.ApplyTo(results);
			filtered = FixOdataQueryAttribute.ApplyFixWithoutGeneric(filtered);
			return filtered;
		}
	}
}
