using FluentNHibernate.Mapping;
using Pathoschild.WebApi.NhibernateOdata.Tests.Models;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Mappings
{
	public class ParentMap : ClassMap<Parent>
	{
		public ParentMap()
		{
			this.Table("Parents");
            this.Id(x => x.Id);
            this.Map(x => x.Name);
            this.Map(x => x.CreatedOn);
            this.Map(x => x.Value);

			this.HasMany(x => x.Children)
				.KeyColumn("ParentId")
				.Inverse()
				.Cascade.All();
		}
	}
}
