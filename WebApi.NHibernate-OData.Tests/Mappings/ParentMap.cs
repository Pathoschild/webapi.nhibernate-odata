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
			this.Map(x => x.ValueGuid);

			this.Component(
				x => x.Component,
				c =>
				{
					c.Map(x => x.One, "Component_One");
					c.Map(x => x.Two, "Component_Two");
					c.Map(x => x.Three, "Component_Three");
					c.Map(x => x.Four, "Component_Four");
				});

			this.HasMany(x => x.Children)
				.KeyColumn("ParentId")
				.Inverse()
				.Cascade.All();
		}
	}
}
