using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Mappings
{
	public static class NHibernateHelper
	{
		public static readonly ISessionFactory SessionFactory;

		static NHibernateHelper()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["WebApi.NHibernate-OData"];
			SetUpDatabase(connectionString);

			var database = MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.ConnectionString);

			bool showSql;
			if (bool.TryParse(ConfigurationManager.AppSettings["NHibernate.ShowSql"] ?? string.Empty, out showSql))
			{
				database.ShowSql().AdoNetBatchSize(0);
			}

			SessionFactory = Fluently.Configure().Database(database).Mappings(m => m.FluentMappings.AddFromAssemblyOf<ParentMap>()).BuildSessionFactory();
		}

		public static void SetUpDatabase(ConnectionStringSettings connectionString)
		{
			DeleteDatabase(connectionString);
			MigrateToLatest(typeof(NHibernateHelper).Assembly, connectionString);
		}

		public static void DeleteDatabase(ConnectionStringSettings connectionString)
		{
			var builder = new SqlConnectionStringBuilder(connectionString.ConnectionString);
			string databaseName = builder.InitialCatalog;
			builder.InitialCatalog = string.Empty;

			using (var conn = new SqlConnection(builder.ConnectionString))
			{
				conn.Open();
				using (var command = conn.CreateCommand())
				{
					command.CommandText = string.Format(
						@"IF EXISTS(select * from sys.databases where name='{0}')
						BEGIN
							DECLARE @DatabaseName nvarchar(50)
							SET @DatabaseName = N'{0}'
							DECLARE @Sql varchar(max)
							SELECT @Sql = COALESCE(@Sql,'') + 'Kill ' + Convert(varchar, SPId) + ';'
								FROM MASTER..SysProcesses
								WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId
							EXEC(@Sql)
							DROP DATABASE [{0}]
						END",
						databaseName);

					command.ExecuteNonQuery();
				}
			}
		}

		public static void CreateDatabase(ConnectionStringSettings connectionString)
		{
			var builder = new SqlConnectionStringBuilder(connectionString.ConnectionString);
			string databaseName = builder.InitialCatalog;
			builder.InitialCatalog = string.Empty;
			using (var conn = new SqlConnection(builder.ConnectionString))
			{
				conn.Open();
				using (var command = conn.CreateCommand())
				{
					command.CommandText = string.Format("IF db_id('{0}') IS NULL CREATE DATABASE [{0}]", databaseName);
					command.ExecuteNonQuery();
				}
			}
		}

		public static void MigrateToLatest(Assembly assembly, ConnectionStringSettings connectionString)
		{
			CreateDatabase(connectionString);
			ApplyMigration(assembly, connectionString);
		}

		public static void ApplyMigration(Assembly assembly, ConnectionStringSettings connectionString)
		{
			var announcer = new TextWriterAnnouncer(s => Debug.WriteLine(s));

			var migrationContext = new RunnerContext(announcer)
			{
				Profile = "Default"
			};

			var options = new ProcessorOptions { PreviewOnly = false, Timeout = 60 };
			var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();
			var processor = factory.Create(connectionString.ConnectionString, announcer, options);
			var runner = new MigrationRunner(assembly, migrationContext, processor);
			runner.MigrateUp(true);
		}
	}
}
