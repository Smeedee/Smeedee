using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Integration.Database.DomainModel.Repositories;


namespace MemoryLeakTesting.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            int rev = 0;
            while (true)
            {
                // Create testdata
                var ciData = new CIServer("testing", "http://testing.no");
                ciData.AddProject(new CIProject("testproject") { SystemId = "1245"});
                ciData.Projects.ElementAt(0).AddBuild(new Build()
                {
                    Status = BuildStatus.Building,
                    StartTime = DateTime.Now,
                    SystemId = "567743423445"
                });

                var csData = new Changeset()
                {
                    Revision = rev++,
                    Author = new Author("me"),
                    Comment = "testing",
                    Time = DateTime.Now
                };

                // this simulates the datacollector.
                // it will spawn one thread for each ahrvester, every 10 seconds
                // it will then write one entry to each of the databases
                var ciHarvesterThread = new Thread(() =>
                {
                    try 
                    {
                        var ciRepo = new CIServerDatabaseRepository();
                        ciRepo.Save(ciData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });

                ciHarvesterThread.Start();

                var csHarvesterThread = new Thread(() =>
                {
                    try 
                    {
                        var csRepo = new ChangesetDatabaseRepository();
                        csRepo.Save(csData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });

                csHarvesterThread.Start();

                // the datacollector waits for the harvesters to be done,
                // but for the testing well just send them off regardless
                //csHarvesterThread.Join();
                //ciHarvesterThread.Join();

                Thread.Sleep(10000);
            }
        }
    }
}
