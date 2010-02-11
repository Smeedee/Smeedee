using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Integration.Database.DomainModel.Repositories;


namespace MemoryLeakTesting.Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            int numClients = 5;

            while(true)
            {
                // this simulates the webServices
                // the repos will create its own isessionfactory and open a session

                var changeSetWebServiceThread = new Thread(() =>
                {
                    try 
                    {
                        var startTime = DateTime.Now;
                        var csRepo = new ChangesetDatabaseRepository();
                        var csData = csRepo.Get(new AllSpecification<Changeset>());
                        Console.WriteLine("{0}: {1} Number of dataitems: {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now - startTime, csData.Count());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                });

                changeSetWebServiceThread.Start();

                var ciWebServiceThread = new Thread(() =>
                {
                    try
                    {
                        var startTime = DateTime.Now;
                        var ciRepo = new CIServerDatabaseRepository();
                        var ciData = ciRepo.Get(new AllSpecification<CIServer>());
                        Console.WriteLine("{0}: {1} Number of dataitems: {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now - startTime, ciData.Count());

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                });

                ciWebServiceThread.Start();

                Thread.Sleep(10000 / numClients);
            }
        }
    }
}
