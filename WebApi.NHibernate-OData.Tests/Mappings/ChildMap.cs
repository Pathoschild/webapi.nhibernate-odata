using FluentNHibernate.Mapping;

using WebApi.NHibernate_OData.Tests.Models;

namespace WebApi.NHibernate_OData.Tests.Mappings
{
    public class ChildMap : ClassMap<Child>
    {
        public ChildMap()
        {
            this.Table("Children");
            this.Id(x => x.Id);
            this.Map(x => x.Name);

            this.References(x => x.Parent, "ParentId");
        }
    }
}
