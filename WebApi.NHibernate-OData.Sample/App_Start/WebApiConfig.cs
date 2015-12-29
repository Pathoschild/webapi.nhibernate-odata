using System.Web.Http;

namespace Pathoschild.WebApi.NhibernateOdata.Sample
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();

			config.Formatters.Remove(config.Formatters.XmlFormatter);
			config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
		}
	}
}
