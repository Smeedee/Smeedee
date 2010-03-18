#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Harvester.Framework;
using APD.DomainModel.Config;
using APD.Harvester.SourceControl.Factories;
using APD.Integration.VCS.Git.DomainModel;


namespace APD.Harvester.SourceControl
{
    
    public class SourceControlHarvester : AbstractHarvester
    {
        private const int FIRST_CHANGESET_REVISION_ID = 0;
        private const string VCS_CONFIG_NAME = "vcs"; //VCS == Version Control System

        private IRepository<Changeset> changesetDbRepository;
        private IRepository<Configuration> configRepository;
        private IPersistDomainModels<Changeset> databasePersister;
        private IAssembleRepository<Changeset> csRepositoryFactory;

        public override string Name
        {
            get { return "Source Control Harvester"; }
        }


        public SourceControlHarvester(IRepository<Changeset> changesetDbDatabase, 
                                      IRepository<Configuration> configRepository,
                                      IPersistDomainModels<Changeset> databasePersister,
                                      IAssembleRepository<Changeset> csRepositoryFactory)
        {
            this.changesetDbRepository = changesetDbDatabase;
            this.configRepository = configRepository;
            this.databasePersister = databasePersister;
            this.csRepositoryFactory = csRepositoryFactory;
        }



        public override void DispatchDataHarvesting()
        {
            IEnumerable<Changeset> allSavedChangesets = changesetDbRepository.Get(new AllChangesetsSpecification());

            long latestSavedRevision = FIRST_CHANGESET_REVISION_ID;
            if (allSavedChangesets.Count() > 0)
            {
                latestSavedRevision = allSavedChangesets.First().Revision;
            }

            var vcsConfiguration = configRepository.Get(new ConfigurationByName(VCS_CONFIG_NAME)).SingleOrDefault();
            
            if (vcsConfiguration != null)
            {
                var changesetRepository = csRepositoryFactory.Assemble(vcsConfiguration);

                IEnumerable<Changeset> allNewChangesets = changesetRepository.Get(
                    new ChangesetsAfterRevisionSpecification(latestSavedRevision)
                    );


                if (changesetRepository is GitChangesetRepository)
                {
                    var oldCwd = Directory.GetCurrentDirectory();
                    var pullScriptPath = Directory.GetCurrentDirectory();
                    // cwd = C:\Smeedee_rev118_with_Git\source\Integration\APD.IntegrationTests\bin\Debug ...
                    try
                    {
                        while (!Path.GetFileName(pullScriptPath).Equals("source"))
                        {
                            pullScriptPath = Directory.GetParent(pullScriptPath).ToString();
                            Console.Write(pullScriptPath + " : ");
                            Console.WriteLine(Path.GetFileName(pullScriptPath));
                        }

                        pullScriptPath = Directory.GetParent(pullScriptPath).ToString();
                        pullScriptPath = Path.Combine(pullScriptPath, @"tools\GitSharp_binaries\pull.bat");
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine("I couldn't locate pull.bat");
                        throw new NullReferenceException();
                    }

                    Directory.SetCurrentDirectory(GitChangesetRepository.ReposDir);
                    string git_shell = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    git_shell = Path.Combine(git_shell, @"Git\bin\sh.exe");
                    //string cmdArgs = "/c \"" +  git_shell + "\" " + pullScriptPath;
                    var p = new Process
                    {
                        StartInfo =
                        {
                            CreateNoWindow = true,
                            FileName = git_shell,
                            Arguments = pullScriptPath,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };

                    Console.WriteLine("Git repository: ");
                    p.Start();
                    var readOutput = new Func<string>(p.StandardOutput.ReadToEnd);
                    var readError = new Func<string>(p.StandardError.ReadToEnd);
                    IAsyncResult outputResult = readOutput.BeginInvoke(null, null);//No blocking on async invocation. No need for a callback, we can wait for completion here. Func is defined in the System namespace of System.Core.ll
                    IAsyncResult errorResult = readError.BeginInvoke(null, null);
                    Console.WriteLine(readOutput.EndInvoke(outputResult)); //End the invocations blocking until they both complete in the thread pool
                    Console.WriteLine(readError.EndInvoke(errorResult));

                    p.WaitForExit();

                    if (p.ExitCode != 0)
                        throw new InvalidProgramException("Git exited with exit code " + p.ExitCode);
                    Directory.SetCurrentDirectory(oldCwd);
                }

                foreach (Changeset changeset in allNewChangesets)
                {
                    databasePersister.Save(changeset);
                }
            }
            else
                throw new HarvesterConfigurationException("Version Control System not configured");
        }
    }
}
