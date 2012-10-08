This package provides a `[FixOdataQuery]` action filter attribute which resolves LINQ compatibility issues between [ASP.NET Web API OData] and [NHibernate]. This is currently maintained against the [nightly OData builds] and the [NHibernate 3.4 trunk] due to additional compatibility issues between their stable releases.

## Usage
Install the NuGet package and apply the attribute to your queryable Web API controller action:

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


[ASP.NET Web API OData]: http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api
[NHibernate]: http://nhforge.org/
[nightly OData builds]: http://www.myget.org/gallery/aspnetwebstacknightly
[NHibernate 3.4 trunk]: https://github.com/nhibernate/nhibernate-core