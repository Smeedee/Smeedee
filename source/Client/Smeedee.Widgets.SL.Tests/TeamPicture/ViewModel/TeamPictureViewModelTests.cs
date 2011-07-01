using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TeamPicture;

namespace Smeedee.Widgets.SL.Tests.TeamPicture.ViewModel
{
    [TestClass]
    public class TeamPictureViewModelTests : Shared
    {
    	[TestClass]
    	public class when_spawned : Shared
    	{
            [TestMethod]
			public void Then_assure_ViewModel_can_be_instatiated()
			{
				Assert.IsNotNull(viewModel);
			}

    		[TestMethod]
    		public void Then_assure_it_has_Snapshots()
    		{
    			Assert.IsNotNull(viewModel.Snapshots);
    		}

			[TestMethod]
			public void Then_assure_Timer_is_started()
			{
				timerMock.Verify(t => t.Start(It.IsAny<int>()), Times.Once());
			}
    	}

        [TestClass]
        public class When_camera_is_not_working : Shared
        {
            public override void Before()
            {
                Camera_not_working();
            }
            
            [TestMethod]
            public void Assure_serviceErrors_are_captured()
            {
                viewModel.ToggleWebcamOnOff.Execute(null);
                logggerMock.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Once());
                Assert.IsTrue(ErrorLogContainsErrorMsg("Camera not working!"));
            }

            [TestMethod]
            public void Assure_serviceErrors_are_shown()
            {
                Assert.IsNull(viewModel.ErrorMsg);
                viewModel.ToggleWebcamOnOff.Execute(null);
                Assert.IsTrue(viewModel.ErrorMsg.Equals("Camera is not working properly!"));
            }
        }

    	[TestClass]
    	public class When_database_is_empty : Shared
    	{
			public override void Before()
			{
				ViewModelIsToldToRefresh();
			}

            [TestMethod]
            public void Then_assure_HasStoredImage_is_false()
            {
                Assert.IsFalse(viewModel.HasStoredImage);
            }

            [TestMethod]
            [Ignore]
            //Doesn't work because of the UiInvoker
            public void Then_assure_Message_is_cleared()
            {
                viewModel.Message = "Not cleared";
                ViewModelIsToldToRefresh();
                Assert.AreEqual(0, viewModel.Message.Length);
            }
    	}

    	[TestClass]
    	public class When_Snapshot_is_selected : Shared
    	{
			public override void Before()
			{
				viewModel.Snapshots.Add(new WriteableBitmap(WIDTH, HEIGHT));
				viewModel.Snapshots.Add(new WriteableBitmap(WIDTH, HEIGHT));

				viewModel.SelectedSnapshot = viewModel.Snapshots.Last();
			}

    		[TestMethod]
    		public void Then_assure_selected_snapshot_is_set_as_CaptureBrush()
    		{
    			Assert.IsNotNull(viewModel.CaptureBrush);
				Assert.AreEqual(typeof(ImageBrush), viewModel.CaptureBrush.GetType());
    			var imageBrush = viewModel.CaptureBrush as ImageBrush;
				Assert.AreEqual(viewModel.SelectedSnapshot, imageBrush.ImageSource);
    		}

			[TestMethod]
			public void Then_assure_its_possible_to_change_SelectedSnapshot()
			{
				viewModel.SelectedSnapshot = new WriteableBitmap(100, 100);
				Assert.IsTrue(viewModel.HasSelectedSnapshot);
				viewModel.SelectedSnapshot = null;
				Assert.IsFalse(viewModel.HasSelectedSnapshot);
			}
    	}

		[TestClass]
		public class When_Snapshot_is_selected_and_Camera_is_On : Shared
		{
			public override void Before()
			{
				User_has_given_access_to_the_Webcam();

				viewModel.Snapshots.Add(new WriteableBitmap(WIDTH, HEIGHT));
				viewModel.Snapshots.Add(new WriteableBitmap(WIDTH, HEIGHT));

				viewModel.ToggleWebcamOnOff.Execute(null);


				viewModel.SelectedSnapshot = viewModel.Snapshots.Last();
			}

			[TestMethod]
			public void Then_assure_Camera_is_turned_Off()
			{
				Assert.IsFalse(viewModel.IsWebcamOn);
			}

			[TestMethod]
			public void Then_assure_TakePicture_command_is_disabled()
			{
				Assert.IsFalse(viewModel.TakePicture.CanExecute(null));
			}

			[TestMethod]
			public void Then_assure_observers_are_notified_about_TakePicture_command_change()
			{
				Assert.AreEqual(2, CommandStateChangeRecorder.Data.Count(c => c.Command == viewModel.TakePicture));
			}
		}

    	[TestClass]
    	public class When_TakePicture : Shared
    	{
			public override void Before()
			{
				User_has_given_access_to_the_Webcam();
				Webcam_capture_is_started();

				viewModel.TakePicture.Execute(null);
			}

			private void Webcam_capture_is_started()
			{
				viewModel.ToggleWebcamOnOff.Execute(null);
			}

    		[TestMethod]
    		public void Then_assure_captured_image_has_been_added_to_Snapshots()
    		{
    			Assert.AreEqual(1, viewModel.Snapshots.Count);
    		}

    		[TestMethod]
    		public void Then_assure_webcam_capture_is_stopped()
    		{
    			Assert.IsFalse(viewModel.IsWebcamOn);
    		}

    		[TestMethod]
    		public void Then_assure_TakePicture_command_is_disabled()
    		{
    			Assert.IsFalse(viewModel.TakePicture.CanExecute(null));
    		}

    		[TestMethod]
			public void Then_assure_observers_are_notified_about_TakePicture_Command_state_changed()
    		{
				Assert.AreEqual(3, CommandStateChangeRecorder.Data.Count(c => c.Command == viewModel.TakePicture));
    		}

    		[TestMethod]
    		public void Then_assure_Picture_is_set_as_SelectedSnapshot()
    		{
    			Assert.AreEqual(newPicture, viewModel.SelectedSnapshot);
    		}

    		[TestMethod]
    		public void Then_assure_Picture_is_set_as_CaptureBrush()
    		{
    			Assert.AreEqual(typeof(ImageBrush), viewModel.CaptureBrush.GetType());
    			var imageBrush = viewModel.CaptureBrush as ImageBrush;
    			Assert.AreEqual(newPicture, imageBrush.ImageSource);
    		}
    	}

    	[TestClass]
    	public class When_TakePicture_and_capturestate_is_stopped : Shared
    	{
			[TestMethod]
			public void Then_assure_it_does_not_throw_any_exceptions()
			{
				viewModel.TakePicture.Execute(null);
			}
    	}

    	[TestClass]
    	public class When_Save : Shared
    	{

			[TestMethod]
			public void Then_assure_Persister_is_used()
			{
                Given_ViewModel_contains_valid_data();
                persisterMock.Setup(p => p.Save(It.IsAny<DomainModel.TeamPicture.TeamPicture>())).Callback(
					(DomainModel.TeamPicture.TeamPicture t) =>
					{
                        Assert.AreEqual(MESSAGE, t.Message);
						Assert.AreEqual((WIDTH * HEIGHT * 4), t.Picture.Length);
						Assert.AreEqual(WIDTH, t.PictureWidth);
						Assert.AreEqual(HEIGHT, t.PictureHeight);
                        Assert.AreEqual(MAXIMIZED, t.PictureScaling);
					});
				viewModel.Save.Execute(null);

				persisterMock.Verify(p => p.Save(It.IsAny<DomainModel.TeamPicture.TeamPicture>()), Times.Once());
			}
    	}

        [TestClass]
        public class When_Refresh : Shared
        {
            public override void Before()
            {
                RepositoryContainsTeamPicture();
            }

            [TestMethod]
            public void Then_assure_settings_are_reloaded_from_db()
            {
                viewModel.Refresh.Execute(null);
                repositoryMock.Verify(r => r.Get(It.IsAny<CurrentTeamPictureSpecification>()), Times.Exactly(2));
            }

        }

    	[TestClass]
    	public class When_Save_fails : Shared
    	{
			public override void Before()
			{
				Given_ViewModel_contains_valid_data();
				PersisterIsNotWorking();

				viewModel.Save.Execute(null);
			}

			[TestMethod]
			public void Then_assure_failure_is_logged()
			{
				logggerMock.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Once());
			}
    	}

    	[TestClass]
    	public class When_Timer_Elapse_and_contains_data : Shared
    	{
			public override void Before()
			{
				RepositoryContainsTeamPicture();

				ViewModelIsToldToRefresh();
			}

			[TestMethod]
			public void Then_assure_teamPicture_is_refreshed_from_database()
			{
				repositoryMock.Verify(r => r.Get(It.IsAny<Specification<DomainModel.TeamPicture.TeamPicture>>()), Times.Exactly(2));
			}

			[TestMethod]
			public void Then_assure_teamPicture_from_database_is_loaded()
			{
				Assert.AreEqual(MESSAGE, viewModel.Message);
				Assert.AreEqual(HEIGHT, viewModel.Snapshot.PixelHeight);
				Assert.AreEqual(WIDTH, viewModel.Snapshot.PixelWidth);
				Assert.AreEqual(WIDTH * HEIGHT, viewModel.Snapshot.Pixels.Length);
			}

			[TestMethod]
			public void Then_assure_teamPicture_from_database_added_to_list_of_snapshots_and_selected()
			{
				Assert.AreEqual(1, viewModel.Snapshots.Count);
				Assert.AreEqual(WIDTH * HEIGHT, viewModel.SelectedSnapshot.Pixels.Length);
			}

    		[TestMethod]
    		public void Then_assure_teamPicture_from_database_is_set_as_CaptureBrush()
    		{
    			Assert.IsNotNull(viewModel.CaptureBrush);
    			var imageBrush = viewModel.CaptureBrush as ImageBrush;
				Assert.AreEqual(viewModel.Snapshot, imageBrush.ImageSource);
    		}
    	}

    	[TestClass]
    	public class When_Timer_Elapse_and_fails_to_load_data : Shared
    	{
			public override void Before()
			{
				RepositoryIsNotWorking();
				ViewModelIsToldToRefresh();
			}

			[TestMethod]
			public void Then_assure_failure_should_is_logged()
			{
				logggerMock.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Once());
			}
    	}

    	[TestClass]
    	public class When_Toggle_Webcam_On : Shared
    	{
			public override void Before()
			{
				User_has_given_access_to_the_Webcam();

				viewModel.ToggleWebcamOnOff.Execute(null);
			}

    		[TestMethod]
    		public void Then_assure_capture_is_started()
    		{
				Assert.IsTrue(viewModel.IsWebcamOn);
    		}
    	}

    	[TestClass]
    	public class When_Toggle_Webcam_Off : Shared
    	{
			public override void Before()
			{
				User_has_given_access_to_the_Webcam();
				Pictures_has_been_taken();

				Camera_is_turned_on();

				Camera_is_turned_off();
			}

    		protected void Camera_is_turned_on()
    		{
    			viewModel.ToggleWebcamOnOff.Execute(null);
    		}

			protected void Camera_is_turned_off()
			{
				viewModel.ToggleWebcamOnOff.Execute(null);
			}

    		[TestMethod]
    		public void Then_assure_last_Snapshot_is_set_as_CaptureBrush()
    		{
				Assert.AreEqual(typeof(ImageBrush), viewModel.CaptureBrush.GetType());
				var imageBrush = viewModel.CaptureBrush as ImageBrush;
				Assert.AreEqual(viewModel.Snapshots.Last(), imageBrush.ImageSource);
    		}

    		[TestMethod]
    		public void Then_assure_last_Snapshot_is_set_as_SelectedSnapshot()
    		{
    			Assert.AreEqual(viewModel.Snapshots.Last(), viewModel.SelectedSnapshot);

                
    		}
    	}

    	[TestClass]
    	public class When_Delete : Shared
    	{
    		private WriteableBitmap deletedPicture;
    		private int deletedPictureIndex;

			public override void Before()
			{
				Pictures_has_been_taken();

				viewModel.SelectedSnapshot = viewModel.Snapshots.Last();
				deletedPicture = viewModel.SelectedSnapshot;
				deletedPictureIndex = viewModel.Snapshots.IndexOf(deletedPicture);

				viewModel.Delete.Execute(null);
			}

    		[TestMethod]
    		public void Then_assure_Picture_has_been_removed_from_Snapshots()
    		{
    			Assert.IsFalse(viewModel.Snapshots.Contains(deletedPicture));
    		}

			[TestMethod]
			public void Then_previous_Picture_has_been_set_as_SelectedSnapshot()
			{
				Assert.AreEqual(viewModel.Snapshots[deletedPictureIndex - 1], viewModel.SelectedSnapshot);
			}
    	}

        [TestClass]
        public class When_loading_and_saving_data : Shared
        {
            public override void Before()
            {
                Given_ViewModel_contains_valid_data();
                RepositoryContainsTeamPicture();
            }

            [TestMethod]
            public void Then_assure_progressbar_is_shown_while_loading_data()
            {
                timerMock.Raise(n => n.Elapsed += null, new EventArgs());
                
                progressbarMock.Verify(r => r.ShowInBothViews(It.IsAny<string>()), Times.AtLeastOnce());
            }

            [TestMethod]
            public void Then_assure_progressbar_is_hidden_after_loading_data()
            {
                timerMock.Raise(n => n.Elapsed += null, new EventArgs());

                progressbarMock.Verify(l => l.HideInBothViews(), Times.AtLeastOnce());
                Assert.IsFalse(viewModel.IsLoading);
            }

            [TestMethod]
            public void Then_assure_progressbar_is_shown_while_saving_picture()
            {
                viewModel.Save.Execute(null);

                progressbarMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()), Times.AtLeastOnce());
                Assert.IsTrue(viewModel.IsSaving);
            }

            [TestMethod]
            public void Then_assure_progressbar_is_hidden_after_saving_settings()
            {
                ConfigPersisterMock_is_setup_to_return_savecomplete();
                viewModel.Save.Execute(null);

                progressbarMock.Verify(l => l.HideInSettingsView(), Times.AtLeastOnce());
                Assert.IsFalse(viewModel.IsSaving);
            }
        }

        [TestClass]
        public class When_picture_scale_is_changed : Shared
        {
            public override void Before()
            {
                Given_ViewModel_contains_valid_data();
                RepositoryContainsTeamPicture();
            }

            [TestMethod]
            public void Then_assure_property_is_set()
            {
                viewModel.IsMaximized = true;
                Assert.AreEqual(MAXIMIZED, viewModel.PictureScaling);
            }

            [TestMethod]
            public void Then_assure_inverse_property_is_false()
            {
                viewModel.IsMaximized = true;
                Assert.IsFalse(viewModel.IsFitToScreen);
            }

        }
    }
}