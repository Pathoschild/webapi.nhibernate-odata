using FluentMigrator;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Migrations
{
	[Profile("Default")]
	public class TestProfile : ForwardOnlyMigration
	{
		public override void Up()
		{
			this.Insert.IntoTable("Parents")
				.Row(new { Id = 61, Name = "parent 61" })
				.Row(new { Id = 63, Name = "parent 63" });

			this.Insert.IntoTable("Children")
				.Row(new { Id = 11, Name = "child 11", ParentId = 61 });
		}
	}
}
