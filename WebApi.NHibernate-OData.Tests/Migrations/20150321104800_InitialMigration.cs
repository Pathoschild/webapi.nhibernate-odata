using FluentMigrator;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Migrations
{
	[Migration(20150321104800)]
	public class InitialMigration : Migration
	{
		public override void Up()
		{
			this.Create.Table("Parents")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50).NotNullable()
				.WithColumn("CreatedOn").AsDate().NotNullable()
				.WithColumn("Value").AsDecimal(18,2).NotNullable();

			this.Create.Table("Children")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
				.WithColumn("Name").AsAnsiString(50).NotNullable()
				.WithColumn("ParentId").AsInt32().NotNullable().ForeignKey("Parents", "Id");
		}

		public override void Down()
		{
			this.Delete.Table("Children");
			this.Delete.Table("Parents");
		}
	}
}
