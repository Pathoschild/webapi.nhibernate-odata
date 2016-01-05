using System.Web.Mvc;

namespace Pathoschild.WebApi.NhibernateOdata.Sample
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
