This package provides a `[FixOdataQuery]` action filter attribute which resolves LINQ compatibility issues between ASP.NET Web API OData and NHibernate. This is currently maintained against the [nightly OData builds](http://www.myget.org/gallery/aspnetwebstacknightly) and the [NHibernate 3.4 trunk](https://github.com/nhibernate/nhibernate-core) due to additional compatibility issues between their stable releases.

## Usage
Install the NuGet package and apply the attribute to your queryable Web API controller action:

```c#
[Queryable, FixOdataQuery]
public IQueryable<Item> Get()
{
   return ...;
}
```

Or apply it automatically to all queryable responses in `global.asax`:

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

The nullable booleans aren't supported by NHibernate. The attribute rewrites the query into this:

     (.If ($$it.Parent == null) {
        0
     } .Else {
        ($$it.Idea).ID
     } == 61 && $$it.ID == 11) == True