//#region File header

//// <copyright>
//// This library is free software; you can redistribute it and/or
//// modify it under the terms of the GNU Lesser General Public
//// License as published by the Free Software Foundation; either
//// version 2.1 of the License, or (at your option) any later version.
//// 
//// This library is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//// Lesser General Public License for more details.
//// 
//// You should have received a copy of the GNU Lesser General Public
//// License along with this library; if not, write to the Free Software
//// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//// /copyright> 
//// 
//// <contactinfo>
//// The project webpage is located at http://agileprojectdashboard.org/
//// which contains all the neccessary information.
//// </contactinfo>

//#endregion

//using System;
//using System.Data.SQLite;
//using System.Reflection;
//using FluentNHibernate.Cfg;
//using FluentNHibernate.Cfg.Db;
//using NHibernate;
//using NHibernate.Cfg;
//using NHibernate.Tool.hbm2ddl;


//namespace APD.Plugin.Database
//{
//    public class FluentNHibConfig
//    {
//        private readonly IPersistenceConfigurer databaseConfig;
//        private readonly Assembly entityMappingsAssembly;
//        private static ISessionFactory sessionFactory = null;
//        private static object lockObject = new object();

//        public FluentNHibConfig(IPersistenceConfigurer databaseConfig, Assembly entityMappingsAssembly)
//        {
//            this.databaseConfig = databaseConfig;
//            this.entityMappingsAssembly = entityMappingsAssembly;
//            sessionFactory = null;
//        }
        
//        public ISession OpenNewSession()
//        {
//            lock (lockObject)
//            {
//                if (sessionFactory == null)
//                {
//                    sessionFactory = CreateSessionFactory();
//                }

//                return sessionFactory.OpenSession();
//            }
//        }

//        public ISessionFactory CreateSessionFactory()
//        {
//            return Fluently.Configure()
//                    .Database(databaseConfig)
//                    .Mappings(m => m.FluentMappings.AddFromAssembly(entityMappingsAssembly))
//                    .ExposeConfiguration(config => ValidateAndBuildDatabase(config))
//                    .BuildSessionFactory();
//        }

//        private void ValidateAndBuildDatabase(Configuration configuration)
//        {
//            //TODO: Needs a better solution here
//            SQLiteConfiguration sqLiteConfiguration = databaseConfig as SQLiteConfiguration;
//            if( sqLiteConfiguration == null )
//                throw new ArgumentOutOfRangeException("The given database configuration is not for SQLLite.(SQLiteConfiguration)");

//            string connectionString = sqLiteConfiguration.ToProperties()["connection.connection_string"];
//            if( string.IsNullOrEmpty(connectionString) )
//                throw new ArgumentException("The given database configuration does not contain a connection string.");

//            //connectionString = connectionString.Replace("New=True;", "");

//            using(var sqlliteConnection = new SQLiteConnection())
//            {
//                sqlliteConnection.ConnectionString = connectionString;
//                var command = new SQLiteCommand("SELECT Name FROM sqlite_master WHERE type='table'",
//                                                sqlliteConnection);
//                sqlliteConnection.Open();

//                var reader = command.ExecuteReader();
                
//                if(!reader.HasRows)
//                    new SchemaExport(configuration).Create(false, true);
//            }
//        }

//        // TODO: implement this
//        private bool AllTablesExists(Configuration configuration)
//        {
//            foreach (var mapping in configuration.ClassMappings)
//            {
//                // TODO: check that all the right columns are there for each mapping
//            }

//            return true;
//        }
//    }
//}