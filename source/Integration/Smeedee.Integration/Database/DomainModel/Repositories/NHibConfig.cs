using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework.Logging;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.DomainModel.ProjectInfo;


namespace Smeedee.Integration.Database.DomainModel.Repositories
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
            configuration.AddAssembly(typeof (LogEntry).Assembly);
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
           // var schemaExport = new SchemaExport(configuration);
           
            // Non-destructive update mechanism. Updates schema if possible
            // without losing data. Also creates the entire DB if necessary.
            var schemaUpdate = new SchemaUpdate(configuration);
            schemaUpdate.Execute(true, true);
            if (schemaUpdate.Exceptions.Count > 0)
            {
                throw schemaUpdate.Exceptions[0] as Exception;
            }
        }

        public static ISessionFactory CreateSessionFactory()
        {
            var configuration = CreateConfiguration();
            return configuration.BuildSessionFactory();
        }
    }
}
