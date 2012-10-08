using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Filters;
using Pathoschild.WebApi.NhibernateOdata.Internal;
using QueryInterceptor;

namespace Pathoschild.WebApi.NhibernateOdata
{
	/// <summary>Intercepts action filter responses to rewrite unsupported OData queries before they're parsed by NHibernate.</summary>
	public class FixOdataQueryAttribute : ActionFilterAttribute
	{
		/*********
		** Public methods
		*********/
		/// <summary>The hook invoked after an action returns.</summary>
		/// <param name="context">The HTTP response context.</param>
		public override void OnActionExecuted(HttpActionExecutedContext context)
		{
			if (!ActionFilterHelper.Instance.HasReturnType<IQueryable<object>>(context.Response))
				return;

			ObjectContent content = (ObjectContent)context.Response.Content;
			Type entityType = content.ObjectType.GetGenericArguments().First();

			this
				.GetType()
				.GetMethod("ApplyFix", BindingFlags.Public | BindingFlags.Instance)
				.MakeGenericMethod(entityType)
				.Invoke(this, new object[] { content });
		}


		/*********
		** Protected methods
		*********/
		/// <summary>Cause the deferred query to be immediately executed.</summary>
		/// <typeparam name="TItem">The query element type.</typeparam>
		/// <param name="content">The content to execute.</param>
		/// <dev>This method is invoked with reflection in <see cref="OnActionExecuted"/>; beware changing its signature.</dev>
		public void ApplyFix<TItem>(ObjectContent content)
		{
			// get queryable return value
			IQueryable<TItem> query = content.Value as IQueryable<TItem>;
			content.Value = query.InterceptWith(new FixNullableBooleanVisitor());
		}
	}
}
