This package provides a `[FixOdataQuery]` action filter attribute which resolves LINQ compatibility issues between [ASP.NET Web API OData] and [NHibernate]. This is currently maintained against the [nightly OData builds] and the [NHibernate 3.4 trunk] due to additional compatibility issues between their stable releases.

## Usage
Install the [NuGet package] and apply the attribute to your queryable Web API controller action:

```c#
[Queryable, FixOdataQuery]
public IQueryable<Item> Get()
{
   return ...;
}
```

Or apply it automatically to all queryable responses in `global.asax` (if you've configured the `QueryableAttribute` filter globally):

```c#
protected void Application_Start()
{
   GlobalConfiguration.Configuration.Filters.Add(new FixOdataQuery());
}
```

## Implementation details
The attribute rewrites queries which aren't supported by NHibernate.

### Nullable conditions
OData generates `Nullable<bool>` conditions, which are rewritten into regular `bool` conditions.

For example, the following query:

     /items/?$filter=Parent/ID+eq+61+and+ID+eq+11

...is converted by ASP.NET Web API OData into an expression tree like this:

     (.If ($$it.Parent == null) {
        null
     } .Else {
        (System.Nullable`1[System.Int32])($$it.Parent).ID
     } == (System.Nullable`1[System.Int32])61 AndAlso (System.Nullable`1[System.Boolean])($$it.ID == 11)) == .Constant<System.Nullable`1[System.Boolean]>(True)

If this query is passed to NHibernate, it will crash because it doesn't support `Nullable<bool>` operands for AndAlso. The attribute rewrites the query into this:

    (.If ($$it.Parent == null) {
        null
    } .Else {
        (System.Nullable`1[System.Int32])($$it.Parent).ID
    } == (System.Nullable`1[System.Int32])61 AndAlso $$it.ID == 11) == True

### SubstringOf, StartsWith, EndsWith methods

OData generates fairly complex expression trees for all these methods.

For example, the following query

    /items?$filter=substringof('parent', Name) eq true

...is converted by ASP.NET Web API OData into an expression tree like this:

	.Lambda #Lambda1<System.Func`2[Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent,System.Boolean]>(Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent $$it)
	{
	    (.If (
	        $$it.Name == null | .Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.String]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.String]).TypedProperty ==
	        null
	    ) {
	        null
	    } .Else {
	        (System.Nullable`1[System.Boolean]).Call ($$it.Name).Contains(.Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.String]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.String]).TypedProperty)
	    } == (System.Nullable`1[System.Boolean]).Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Boolean]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Boolean]).TypedProperty)
	    == .Constant<System.Nullable`1[System.Boolean]>(True)
    }

NHibernate fails to parse this into an SQL query. The `FixStringMethodsVisitor` will transform it into the following:

    .Lambda #Lambda1<System.Func`2[Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent,System.Boolean]>(Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent $$it)
    {
        (System.Nullable`1[System.Boolean]).Call ($$it.Name).Contains(.Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.String]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.String]).TypedProperty)
        == (System.Nullable`1[System.Boolean]).Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Boolean]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Boolean]).TypedProperty
    }

...and now NHibernate can parse it and generate the proper SQL queries.
    
[ASP.NET Web API OData]: http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api
[NHibernate]: http://nhforge.org/
[nightly OData builds]: http://www.myget.org/gallery/aspnetwebstacknightly
[NHibernate 3.4 trunk]: https://github.com/nhibernate/nhibernate-core
[NuGet package]: https://nuget.org/packages/Pathoschild.WebApi.NHibernate-OData