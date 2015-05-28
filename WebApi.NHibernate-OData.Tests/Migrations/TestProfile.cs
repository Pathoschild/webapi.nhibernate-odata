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
                .Row(new { Id = 61, Name = "parent 61", CreatedOn = new DateTime(2015, 1, 1), Value = 15.15m })
                .Row(new { Id = 63, Name = "parent 63", CreatedOn = new DateTime(2014, 1, 2), Value = 45.15m });

            this.Insert.IntoTable("Children")
                .Row(new { Id = 11, Name = "child 11", ParentId = 61 });
        }
    }
}
