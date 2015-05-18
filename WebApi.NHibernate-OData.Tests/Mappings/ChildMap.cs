using FluentNHibernate.Mapping;
using Pathoschild.WebApi.NhibernateOdata.Tests.Models;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Mappings
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
