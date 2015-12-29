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

See the [Samples](#samples) section for more details.

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

### Component mappings

OData generates a null check when querying a property of a component, which is not necessary since NHibernate will map it
directly to a column. If the component has more than 3 properties mapped, NHibernate fails to create a query from the LINQ expression.

For example, the following query

	/items?$filter=Parent/Two eq 61

...is converted by ASP.NET Web API OData into an expression tree like this:

	.Lambda #Lambda1<System.Func`2[Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent,System.Boolean]>(Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent $$it)
	{
		(.If ($$it.Component == null) {
		 null
		} .Else {
		 (System.Nullable`1[System.Int32])($$it.Component).Two
		} == (System.Nullable`1[System.Int32]).Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Int32]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Int32]).TypedProperty)
		== True
	}

NHibernate fails to parse this into an SQL query. The `FixComponentNullCheckVisitor` will transform it into the following:

	.Lambda #Lambda1<System.Func`2[Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent,System.Boolean]>(Pathoschild.WebApi.NhibernateOdata.Tests.Models.Parent $$it)
	{
		(System.Nullable`1[System.Int32])($$it.Component).Two
		 == (System.Nullable`1[System.Int32]).Constant<System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Int32]>(System.Web.Http.OData.Query.Expressions.LinqParameterContainer+TypedLinqParameterContainer`1[System.Int32]).TypedProperty)
	}

...and now NHibernate can parse it and generate the proper SQL queries.

## Links
	
[ASP.NET Web API OData]: http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api
[NHibernate]: http://nhforge.org/
[nightly OData builds]: http://www.myget.org/gallery/aspnetwebstacknightly
[NHibernate 3.4 trunk]: https://github.com/nhibernate/nhibernate-core
[NuGet package]: https://nuget.org/packages/Pathoschild.WebApi.NHibernate-OData

## Contributing

Feel free to fork this repo, write a test for your use-case or bug and fix it.

Included in the repository are StyleCop and ReSharper settings file. If possible, please use them appropriately
to easily follow guidelines for style in the project. Use tabs and `git config core.autocrlf true` as well!

### Contributors

See the [CONTRIBUTORS](CONTRIBUTORS.md) file.

## Samples

A project named WebApi.NHibernate-OData.Sample exists with usage examples.

There are two controllers for the two ways to use it.

The `parents` controller uses an attribute to modify the output along with the `EnableQueryAttribute` of Web API 2.

The `children` controller call the fix directly in the action method if specific handling is necessary.

    http://localhost:15394/parents?$filter=not%20substringof(%27wot%27,%20Name)%20and%20startswith(Name,%20%27parent%2061%27)&$select=Id,Name
	http://localhost:15394/children?$filter=Parent/Component/Two eq 61