using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

using APD.DomainModel.CI;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using APD.DomainModel.SourceControl;
using APD.DomainModel.Users;
using APD.DomainModel.ProjectInfo;


namespace APD.Integration.Database.DomainModel.Repositories
{
    internal class NHibConfig
    {
        public static Configuration CreateConfiguration(string dbFilePath)
        {
            var configuration = new Configuration();     
            configuration.Properties.Add("connection.connection_string", string.Format("Data Source={0};Version=3;New=True", dbFilePath));
            configuration.Properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            configuration.Properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
            configuration.Properties.Add("query.substitutions", "true=1;false=0");
            configuration.Properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
            AddAssembliesToConfiguration(configuration);
            CreateDbSchema(configuration);
            return configuration;
        }

        private static void AddAssembliesToConfiguration(Configuration configuration)
        {
            configuration.AddAssembly(typeof (Userdb).Assembly);
        }

        public static Configuration CreateConfiguration()
        {
            var configuration = new Configuration();
            configuration.Configure();
            AddAssembliesToConfiguration(configuration);
            CreateDbSchema(configuration);
            return configuration;
        }

        private static void CreateDbSchema(Configuration configuration) 
        {
            var schemaExport = new SchemaExport(configuration);
            using (var sqlliteConnection = new SQLiteConnection())
            {
                sqlliteConnection.ConnectionString = configuration.Properties["connection.connection_string"];
                var command = new SQLiteCommand("SELECT Name FROM sqlite_master WHERE type='table'",
                                                sqlliteConnection);
                sqlliteConnection.Open();

                var reader = command.ExecuteReader();

                if (!reader.HasRows)
                    new SchemaExport(configuration).Create(false, true);
            }
        }

        public static ISessionFactory CreateSessionFactory()
        {
            var configuration = CreateConfiguration();
            return configuration.BuildSessionFactory();
        }
    }
}
