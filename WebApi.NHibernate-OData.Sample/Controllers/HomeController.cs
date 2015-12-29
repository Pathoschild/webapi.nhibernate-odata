using System.Web.Mvc;

using Pathoschild.WebApi.NhibernateOdata.Sample.Models;

namespace Pathoschild.WebApi.NhibernateOdata.Sample.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var readmeContent = this.Server.MapPath("~/bin/README.md");

			var content = CommonMark.CommonMarkConverter.Convert(System.IO.File.ReadAllText(readmeContent));

			return this.View("Index", new IndexModel { Content = content });
		}
	}
}
