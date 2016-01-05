using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;

using NHibernate;
using NHibernate.Linq;

using Pathoschild.WebApi.NhibernateOdata.Tests.Mappings;
using Pathoschild.WebApi.NhibernateOdata.Tests.Models;

namespace Pathoschild.WebApi.NhibernateOdata.Sample.Controllers
{
	[RoutePrefix("parents")]
	public class ParentsController : ApiController
	{
		private readonly ISession Session;

		public ParentsController()
		{
			this.Session = NHibernateHelper.SessionFactory.OpenSession();
		}

		/// <summary>
		/// Gets all parents and applies the fix automatically on the results.
		/// </summary>
		/// <returns>A filtered IQueryable!</returns>
		[Route("")]
		[EnableQuery]
		[FixOdataQuery]
		public IQueryable<Parent> Get()
		{
			return this.Session.Query<Parent>();
		}
	}
}
