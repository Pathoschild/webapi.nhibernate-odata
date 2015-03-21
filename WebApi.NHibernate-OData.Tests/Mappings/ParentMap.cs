using FluentNHibernate.Mapping;

using WebApi.NHibernate_OData.Tests.Models;

namespace WebApi.NHibernate_OData.Tests.Mappings
{
    public class ParentMap : ClassMap<Parent>
    {
        public ParentMap()
        {
            this.Table("Parents");
            this.Id(x => x.Id);
            this.Map(x => x.Name);

            this.HasMany(x => x.Children)
                .KeyColumn("ParentId")
                .Inverse()
                .Cascade.All();
        }
    }
}
