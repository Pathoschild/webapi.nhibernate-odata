using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Pathoschild.WebApi.NhibernateOdata.Internal
{
	/// <summary>Provides helper methods for action filters on the Web API, including strongly-typed access to the controller context.</summary>
	public class ActionFilterHelper
	{
		/*********
		** Accessors
		*********/
		/// <summary>The singleton instance.</summary>
		public static readonly ActionFilterHelper Instance = new ActionFilterHelper();


		/*********
		** Public methods
		*********/
		/// <summary>Get the controller handling the HTTP request.</summary>
		/// <typeparam name="TController">The expected controller type.</typeparam>
		/// <param name="context">The current HTTP controller context.</param>
		/// <returns>Returns the controller instance.</returns>
		/// <exception cref="InvalidOperationException">The controller handling this request is not a Web API controller.</exception>
		public TController GetController<TController>(HttpControllerContext context)
			where TController : ApiController
		{
			TController controller = context.Controller as TController;
			if (controller == null)
				throw new InvalidOperationException(String.Format("The controller handling this request is not of type '{0}' (actually of type '{1}')", typeof(TController), context.Controller.GetType()));

			return controller;
		}

		/// <summary>Get the action return type.</summary>
		/// <param name="message">The HTTP response message.</param>
		/// <param name="allowNull">Whether to recognize <c>null</c> as a valid return type.</param>
		/// <returns>Returns the action return type, or <c>null</c> if the action didn't return anything.</returns>
		public Type GetReturnType(HttpResponseMessage message, bool allowNull = false)
		{
			if (message == null || !(message.Content is ObjectContent))
				return null;
			if (allowNull || (message.Content as ObjectContent).Value == null)
				return null;
			return (message.Content as ObjectContent).ObjectType;
		}

		/// <summary>Get whether the response has a return type.</summary>
		/// <param name="message">The HTTP response to check.</param>
		/// <param name="allowNull">Whether to recognize <c>null</c> as a valid return type.</param>
		public bool HasReturnType(HttpResponseMessage message, bool allowNull = false)
		{
			return this.GetReturnType(message, allowNull) != null;
		}

		/// <summary>Get whether the response has an expected return type.</summary>
		/// <typeparam name="T">The expected return type.</typeparam>
		/// <param name="message">The HTTP response to check.</param>
		/// <param name="allowNull">Whether to recognize <c>null</c> as a valid return type.</param>
		public bool HasReturnType<T>(HttpResponseMessage message, bool allowNull = false)
		{
			Type returnType = this.GetReturnType(message, allowNull);
			return returnType != null && typeof(T).IsAssignableFrom(returnType);
		}

		/// <summary>Get the return value from the response.</summary>
		/// <typeparam name="T">The expected return value.</typeparam>
		/// <param name="response">The HTTP response message from which to extract the return value.</param>
		/// <returns>Returns the action return type.</returns>
		/// <exception cref="InvalidCastException">The action filter does not return <see cref="T"/>.</exception>
		public T GetReturnValue<T>(HttpResponseMessage response)
		{
			T value;
			if (!response.TryGetContentValue(out value))
				throw new InvalidCastException(String.Format("The response does not have a return type of {0}.", typeof(T)));
			return value;
		}

		/// <summary>Apply a filter to the queryable return value of the action.</summary>
		/// <typeparam name="TEntity">The element type of the query.</typeparam>
		/// <param name="message">The HTTP response message to modify.</param>
		/// <param name="filter">The filter to apply.</param>
		/// <exception cref="InvalidOperationException">The <see cref="message"/> is not available, or does not return <see cref="IQueryable{TEntity}"/>.</exception>
		public void ApplyFilter<TEntity>(HttpResponseMessage message, Expression<Func<TEntity, bool>> filter)
		{
			// validate
			if (message == null)
				throw new InvalidOperationException("Cannot apply query filter: HTTP response message is not available yet.");
			if (!this.HasReturnType<IQueryable<TEntity>>(message))
			{
				Type returnType = this.GetReturnType(message);
				throw new InvalidOperationException(String.Format("Cannot apply query filter: the action does not return a queryable list of {0} (it actually returns {1}).", typeof(TEntity).Name, returnType != null ? returnType.ToString() : "void"));
			}

			// get response
			ObjectContent content = (ObjectContent)message.Content;
			Type entityType = content.ObjectType.GetGenericArguments().First();

			MethodInfo method = typeof(ActionFilterHelper).GetMethod("ApplyFilterImpl", BindingFlags.Instance | BindingFlags.NonPublic);
			method = method.MakeGenericMethod(new[] { entityType, typeof(TEntity) });
			method.Invoke(this, new object[] { content, filter });
		}


		/*********
		** Protected methods
		*********/
		/// <summary>Apply a filter to the queryable return value of the action.</summary>
		/// <typeparam name="TActualEntity">The actual element type of the query. This type is derived from <typeparamref name="TFilteredEntity"/>.</typeparam>
		/// <typeparam name="TFilteredEntity">The element type filtered by the <paramref name="filter"/>.</typeparam>
		/// <param name="content">The object content whose value to modify.</param>
		/// <param name="filter">The filter to apply.</param>
		protected void ApplyFilterImpl<TActualEntity, TFilteredEntity>(ObjectContent content, Expression<Func<TFilteredEntity, bool>> filter)
			where TActualEntity : TFilteredEntity
		{
			// construct lambda: (TActualEntity entity) => filter(entity)
			ParameterExpression param = Expression.Parameter(typeof(TActualEntity), "entity");
			InvocationExpression invocation = Expression.Invoke(filter, param);
			Expression<Func<TActualEntity, bool>> lambda = Expression.Lambda<Func<TActualEntity, bool>>(invocation, param);

			// apply filter
			content.Value = (content.Value as IQueryable<TActualEntity>).Where(lambda);
		}
	}
}
