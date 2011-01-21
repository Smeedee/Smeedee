using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskDefinition;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Widget.Admin.Tasks.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.Tasks.Controllers
{
    public class TasksController
    {
        private readonly TasksViewModel _viewModel;

        private readonly IRepository<TaskDefinition> _taskDefinitionRepository;
        private readonly IRepository<TaskConfiguration> _taskConfigurationRepository;
        private readonly IPersistDomainModels<TaskConfiguration> _domainModelPersister;
        private readonly IUIInvoker _uiInvoker;
        private readonly IInvokeBackgroundWorker<IEnumerable<TaskConfiguration>> _asyncClient;
        private readonly IProgressbar _progressbar;

        public TasksController(TasksViewModel viewModel,
                               IRepository<TaskDefinition> taskDefinitionRepository,
                               IRepository<TaskConfiguration> taskConfigurationRepository,
                               IPersistDomainModels<TaskConfiguration> domainModelPersister,
                               IUIInvoker uiInvoker, 
                               IProgressbar progressbar,
                               IInvokeBackgroundWorker<IEnumerable<TaskConfiguration>> asyncClient)
        {
            _viewModel = viewModel;

            _taskDefinitionRepository = taskDefinitionRepository;
            _taskConfigurationRepository = taskConfigurationRepository;
            _domainModelPersister = domainModelPersister;
            _uiInvoker = uiInvoker;
            _asyncClient = asyncClient;
            _progressbar = progressbar;

            _viewModel.SaveChanges.AfterExecute += SaveChanges;

        }

        public void Start()
        {
            UpdateViewModels();
        }

        private void UpdateViewModels()
        {
            _asyncClient.RunAsyncVoid(() =>
            {
                var tasksFromDB = _taskDefinitionRepository.Get(new AllSpecification<TaskDefinition>());
                _uiInvoker.Invoke(() => CreateViewModelTasks(tasksFromDB));
            });

            _asyncClient.RunAsyncVoid(() =>
            {
                _progressbar.ShowInBothViews("Loading data...");

                var instancesFromDB = _taskConfigurationRepository.Get(new AllSpecification<TaskConfiguration>());
                _uiInvoker.Invoke(() =>
                                      {
                                          _viewModel.RunningTasks.Clear();
                                          CreateViewModelTaskInstances(instancesFromDB);
                                          _viewModel.HasChanges = false;
                                      });

                _progressbar.HideInBothViews();
            });
        }

        private void CreateViewModelTasks(IEnumerable<TaskDefinition> tasksFromDb)
        {
            foreach (var taskDefinition in tasksFromDb)
            {
                var taskViewModel = new TaskViewModel
                                        {
                                            Name = taskDefinition.Name,
                                            ConfigurationEntries =
                                                new ObservableCollection<ConfigurationEntryViewModel>(
                                                (from setting in taskDefinition.SettingDefinitions
                                                 select new ConfigurationEntryViewModel
                                                            {
                                                                OrderIndex = setting.OrderIndex,
                                                                Name = setting.SettingName,
                                                                Type = setting.Type ?? typeof (string),
                                                                Value = setting.DefaultValue,
                                                                HelpText = setting.HelpText
                                                            }).ToArray()),
                                            Author = taskDefinition.Author,
                                            Description = taskDefinition.Description,
                                            Version = taskDefinition.Version,
                                            Webpage = taskDefinition.Webpage
                                        };
                _viewModel.AddTask(taskViewModel);
            }
        }

        private void CreateViewModelTaskInstances(IEnumerable<TaskConfiguration> activeTasksFromDb)
        {
            foreach (var taskConfiguration in activeTasksFromDb)
            {
                var taskInstanceViewModel = new TaskInstanceConfigurationViewModel
                                                {
                                                    ConfigurationEntries =
                                                        new ObservableCollection<ConfigurationEntryViewModel>
                                                        (
                                                            (from entry in taskConfiguration.Entries
                                                             select new ConfigurationEntryViewModel
                                                                        {  
                                                                            Name = entry.Name,
                                                                            Type = entry.Type,
                                                                            Value = entry.Value,
                                                                            HelpText = entry.HelpText
                                                                        }).ToArray()
                                                        ),
                                                    DispatchInterval = taskConfiguration.DispatchInterval,
                                                    RunningTaskName = taskConfiguration.Name,
                                                    AvailableTaskName = taskConfiguration.TaskName
                                                };
                _viewModel.AddTaskInstance(taskInstanceViewModel);
            }
        }

        public void SaveChanges(object sender, EventArgs eventArgs)
        {
            var taskConfigs = (from taskConfig in _viewModel.RunningTasks
                               select new TaskConfiguration
                                          {
                                              TaskName = taskConfig.AvailableTaskName,
                                              Name = taskConfig.RunningTaskName,
                                              DispatchInterval = taskConfig.DispatchInterval,
                                              Entries = (from entry in taskConfig.ConfigurationEntries
                                                         select new TaskConfigurationEntry
                                                                    {
                                                                        Name = entry.Name,
                                                                        Type = entry.Type,
                                                                        Value = entry.Value,
                                                                        HelpText = entry.HelpText
                                                                    }).ToList()
                                          }).ToArray();

            _domainModelPersister.Save(taskConfigs);
        }
    }
}