using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TeamPicture;
using Smeedee.Widget.TeamPicture.Services;
using Smeedee.Widget.TeamPicture.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widget.TeamPicture.Tests
{
    [TestClass]
    public class Shared
    {
        protected TeamPictureViewModel viewModel;
        protected Mock<IRepository<Smeedee.DomainModel.TeamPicture.TeamPicture>> repositoryMock;
        protected Mock<IPersistDomainModelsAsync<Smeedee.DomainModel.TeamPicture.TeamPicture>> persisterMock;
        protected Mock<ILog> logggerMock;
        protected Mock<ITimer> timerMock;
    	protected Mock<IWebcamService> webcamServiceMock;
        protected Mock<IProgressbar> progressbarMock;
    	protected CommandStateChangeRecorder CommandStateChangeRecorder;
    	protected WriteableBitmap newPicture;
        protected List<LogEntry> errorLog = new List<LogEntry>();

        [TestInitialize]
        public void Setup()
        {
            repositoryMock = new Mock<IRepository<DomainModel.TeamPicture.TeamPicture>>();
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<DomainModel.TeamPicture.TeamPicture>>())).Returns(
                new List<DomainModel.TeamPicture.TeamPicture>());
            persisterMock = new Mock<IPersistDomainModelsAsync<DomainModel.TeamPicture.TeamPicture>>();
            timerMock = new Mock<ITimer>();
            logggerMock = new Mock<ILog>();
            progressbarMock = new Mock<IProgressbar>();

            logggerMock.Setup(l => l.WriteEntry(It.IsAny<LogEntry>())).Callback((LogEntry entry) =>
                {
                    errorLog.Add(entry);
                });

			webcamServiceMock = new Mock<IWebcamService>();
        	webcamServiceMock.Setup(s => s.StartCapture(It.IsAny<VideoBrush>())).Callback(
        		(VideoBrush videoBrush) =>
        		{
        			webcamServiceMock.Setup(s => s.CaptureState).Returns(CaptureState.Started);
        		});
        	webcamServiceMock.Setup(s => s.CaptureImageAsync()).Callback(
        		() =>
        		{
					newPicture = new WriteableBitmap(100, 100);
					webcamServiceMock.Raise(m => m.CaptureImageCompleted += null, new CaptureImageCompletedEventArgs(newPicture));
        		});
        	webcamServiceMock.Setup(s => s.StopCapture()).Callback(
        		() =>
        		{
        			webcamServiceMock.Setup(s => s.CaptureState).Returns(CaptureState.Stopped);
        		});

            viewModel = new TeamPictureViewModel(
				repositoryMock.Object, 
				persisterMock.Object, 
				timerMock.Object,
				new NoBackgroundWorkerInvocation<IEnumerable<DomainModel.TeamPicture.TeamPicture>>(), 
				logggerMock.Object,
				webcamServiceMock.Object, 
                progressbarMock.Object);
			
			CommandStateChangeRecorder = new CommandStateChangeRecorder(viewModel);
        	CommandStateChangeRecorder.Start();

			Before();	
		}

		public virtual void Before()
		{
			
		}

        protected bool ErrorLogContainsErrorMsg (string msg)
        {
            foreach (var logEntry in errorLog)
            {
                if (logEntry.Message.Contains(msg))
                {
                    return true;
                }
            }
            return false;
        }

		protected const string MESSAGE = "hello!";
		protected const int HEIGHT = 480;
		protected const int WIDTH = 640;

		protected void Given_ViewModel_contains_valid_data()
		{
		    viewModel.Message = MESSAGE;
			viewModel.SelectedSnapshot = new WriteableBitmap(WIDTH, HEIGHT);
		}

		protected void ViewModelIsToldToRefresh()
		{
			timerMock.Raise(t => t.Elapsed += null, EventArgs.Empty);
		}

		protected void RepositoryContainsTeamPicture()
		{
			var teamPicture = new DomainModel.TeamPicture.TeamPicture()
			{
				Message = MESSAGE,
				Picture = new byte[HEIGHT * WIDTH * 4],
				PictureHeight = HEIGHT,
				PictureWidth = WIDTH
			};

			repositoryMock.Setup(r => r.Get(It.IsAny<CurrentTeamPictureSpecification>())).Returns(
				new List<DomainModel.TeamPicture.TeamPicture>()
                    {
                        teamPicture
                    }
				);
			return;
		}

		protected void RepositoryIsNotWorking()
		{
			repositoryMock.Setup(r => r.Get(It.IsAny<Specification<DomainModel.TeamPicture.TeamPicture>>()))
                .Throws(new Exception("Somethings not working!"));
		}

        protected void Camera_not_working()
        {
            webcamServiceMock.Setup(s => s.RequestAccess()).Returns(true);
            webcamServiceMock.Setup(s => s.StartCapture(It.IsAny<VideoBrush>())).Callback(
                (VideoBrush videoBrush) =>
                {
                    webcamServiceMock.Setup(s => s.CaptureState).Returns(CaptureState.Stopped);
                });
            webcamServiceMock.Setup(ws => ws.StartCapture(It.IsAny<VideoBrush>())).
                Throws(new Exception("Camera not working!"));
        }

		protected void PersisterIsNotWorking()
		{
			persisterMock.Setup(p => p.Save(It.IsAny<DomainModel.TeamPicture.TeamPicture>())).Throws(
				new Exception("This is a failure!"));
		}

		protected void User_has_given_access_to_the_Webcam()
		{
			webcamServiceMock.Setup(webcamService =>
				webcamService.RequestAccess()).Returns(true);
		}

        protected void ConfigPersisterMock_is_setup_to_return_savecomplete()
        {
            persisterMock.Setup(r => r.Save(It.IsAny<DomainModel.TeamPicture.TeamPicture>())).Raises(
                t => t.SaveCompleted += null, new SaveCompletedEventArgs());
        }

		protected void Pictures_has_been_taken()
		{
			viewModel.Snapshots.Add(new WriteableBitmap(WIDTH, HEIGHT));
			viewModel.Snapshots.Add(new WriteableBitmap(WIDTH, HEIGHT));
			viewModel.SelectedSnapshot = viewModel.Snapshots.First();
		}
    }
}
