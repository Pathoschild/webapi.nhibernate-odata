using System;

using FluentMigrator;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Migrations
{
	[Profile("Default")]
	public class TestProfile : ForwardOnlyMigration
	{
		public override void Up()
		{
			this.Insert.IntoTable("Parents")
				.Row(new { Id = 61, Name = "parent 61", CreatedOn = new DateTime(2015, 1, 1), Value = 15.15m, Component_One = "One61", Component_Two = 61, Component_Three = "Three61", Component_Four = "Four61" })
				.Row(new { Id = 63, Name = "parent 63", CreatedOn = new DateTime(2014, 1, 2), Value = 45.15m, Component_One = "One63", Component_Two = 63, Component_Three = "Three63", Component_Four = "Four63", ValueGuid = "e741c1cd-2e75-44c1-b3e8-7b0d5df435ce" });

			this.Insert.IntoTable("Children")
				.Row(new { Id = 11, Name = "child 11", ParentId = 61 });
		}
	}
}
