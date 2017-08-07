using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace Pathoschild.WebApi.NhibernateOdata.Tests
{
	public static class Helpers
	{
		public static ODataQueryOptions<T> Build<T>(string odataQueryString)
			where T : class
		{
			ODataModelBuilder modelBuilder = new ODataConventionModelBuilder(new HttpConfiguration(), true);
			modelBuilder.EntitySet<T>("Set");
			if (!odataQueryString.Contains("?"))
			{
				odataQueryString = "?" + odataQueryString;
			}

			var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/" + odataQueryString);
			var model = modelBuilder.GetEdmModel();
			return new ODataQueryOptions<T>(new ODataQueryContext(model, typeof(T)), request);
		}
	}
}
