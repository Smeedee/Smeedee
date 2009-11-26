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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;

using APD.Client;
using APD.Client.Framework;
using APD.Client.Framework.Commands;
using APD.Client.Services;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;

using Moq;
using System.Linq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using System.Threading;


namespace APD.ClientTests.ShellPresenterSpecs
{

    public class Shared
    {
        protected Mock<IShell> shellMock;

        protected Mock<IRegionManager> regionManagerMock;
        protected Mock<ICheckIfAdminUIShouldBeDisplayed> adminDisplayCheckMock;
        protected Mock<IVisibleModule> module1Mock;
        protected Mock<IVisibleModule> module2Mock;
        protected Mock<IVisibleModule> module3Mock;
        protected Mock<IVisibleModule> module4Mock;
        protected Mock<IVisibleModule> moduleAdminMock;
        protected Mock<ICheckIfAdminUIShouldBeDisplayed> displayAdminUICheckerMock;

        protected FreezeViewCommandNotifier freezeCmdMock;
        protected UnFreezeViewCommandNotifier unFreezeCmdMock;
        protected NextSlideCommandNotifier nextSlideCmdMock;
        protected PreviousSlideCommandNotifier prevSlideCmdMock;
        protected TogglePauseSlideShowCommandNotifier togglePauseCmdMock;
        protected ToggleAdminModeCommandNotifier toggleAdminCmdMock;

        protected static EventAggregator eventAggregator;
        protected static ShellPresenter presenter;

        protected Slide slide1;
        protected Slide slide2;
        protected Slide slide3;
        protected Slide slide4;
        protected Slide adminSlide;

        protected UserControl view1;
        protected UserControl view2;
        protected UserControl view3;
        protected UserControl view4;
        protected UserControl adminView;


        //protected string contentRegionString = "MainContent";
        protected SlideRegion contentRegion = SlideRegion.MainContent;
        protected Mock<IRegion> contentRegionMock;
        protected List<object> contentRegionObjects;
        protected object contentRegionView;

        protected string savedShellTitle;
        protected string savedShellInfoString;


        protected void setupMocks()
        {
            displayAdminUICheckerMock = new Mock<ICheckIfAdminUIShouldBeDisplayed>();

            savedShellTitle = "";
            savedShellInfoString = "";

            shellMock = new Mock<IShell>();
            shellMock.Setup(sm => sm.SetTitle(It.IsAny<string>())).Callback((string s) => savedShellTitle = s);
            shellMock.Setup(sm => sm.SetInfoString(It.IsAny<string>())).Callback((string s) =>
            savedShellInfoString = s);

            adminDisplayCheckMock = new Mock<ICheckIfAdminUIShouldBeDisplayed>();

            SetupRegionManagerMock();

            SetupSlideMocks();

            eventAggregator = new EventAggregator();
            freezeCmdMock = new FreezeViewCommandNotifier(eventAggregator);
            unFreezeCmdMock = new UnFreezeViewCommandNotifier(eventAggregator);
            nextSlideCmdMock = new NextSlideCommandNotifier(eventAggregator);
            prevSlideCmdMock = new PreviousSlideCommandNotifier(eventAggregator);
            togglePauseCmdMock = new TogglePauseSlideShowCommandNotifier(eventAggregator);
            toggleAdminCmdMock = new ToggleAdminModeCommandNotifier(eventAggregator);
        }


        private void SetupRegionManagerMock()
        {
            contentRegionObjects = new List<object>();
            contentRegionView = new object();
            contentRegionMock = new Mock<IRegion>();
            //contentRegionMock.SetupGet(cr => cr.Views).Returns( new ViewCollection ... );
            contentRegionMock.Setup(cr => cr.Add(It.IsAny<object>()))
                .Callback((object o) => contentRegionObjects.Add(o));
            contentRegionMock.Setup(cr => cr.Remove(It.IsAny<object>()))
                .Callback((object o) => contentRegionObjects.Remove(o));

            regionManagerMock = new Mock<IRegionManager>();
            regionManagerMock.SetupGet(k => k.Regions[contentRegion.ToString()]).Returns(contentRegionMock.Object);
        }

        private void SetupSlideMocks()
        {
            //view1 = new UserControl();
            //view2 = new UserControl();

            module1Mock = new Mock<IVisibleModule>();
            module2Mock = new Mock<IVisibleModule>();
            module3Mock = new Mock<IVisibleModule>();
            module4Mock = new Mock<IVisibleModule>();
            moduleAdminMock = new Mock<IVisibleModule>();

            module1Mock.SetupGet(m => m.View).Returns(view1);
            module2Mock.SetupGet(m => m.View).Returns(view2);
            module3Mock.SetupGet(m => m.View).Returns(view3);
            module4Mock.SetupGet(m => m.View).Returns(view4);
            moduleAdminMock.SetupGet(m => m.View).Returns(adminView);

            slide1 = new Slide("First Slide", 10);
            slide1.RegionMappings.Add(contentRegion, module1Mock.Object);


            slide2 = new Slide("Second Slide", 15);
            slide2.RegionMappings.Add(contentRegion, module2Mock.Object);

            slide3 = new Slide("Third Slide", 15);
            slide3.RegionMappings.Add(contentRegion, module3Mock.Object);
            
            slide4 = new Slide("Build Status", 15);
            slide4.RegionMappings.Add(contentRegion, module3Mock.Object);

            adminSlide = new Slide("AdminSlide", 0);
            adminSlide.RegionMappings.Add(contentRegion, moduleAdminMock.Object);

        }

        protected void CreateShellPrestenter()
        {
            presenter = new ShellPresenter(
                shellMock.Object,
                new NoUIInvocation(),
                regionManagerMock.Object,
                freezeCmdMock,
                unFreezeCmdMock,
                nextSlideCmdMock,
                prevSlideCmdMock,
                togglePauseCmdMock,
                toggleAdminCmdMock,
                displayAdminUICheckerMock.Object);
            presenter.Slides = new List<Slide> {slide1, slide2, slide3, slide4};
            presenter.AdminSlide = adminSlide;
        }

        protected bool IsSlideOnShell(Slide slide)
        {
            bool result = ( savedShellTitle == slide.Title );

            //var contentRegion = regionManagerMock.Object.Regions[contentRegionString];
            //result &= contentRegion.Views.Contains(view1);
            //incomplete. Need instances of UserControls so view1 is not null


            return result;
        }

        protected void fireSecondTicker()
        {
            Type shellPresenterType = presenter.GetType();
            MethodInfo second_Tick = shellPresenterType.GetMethod("second_Tick",
                                                              BindingFlags.Instance | BindingFlags.NonPublic);
            second_Tick.Invoke(presenter, new object[1] {null});
        }
    }

    [TestFixture]
    public class when_spawned : Shared
    {

        [SetUp]
        public void Setup()
        {
            setupMocks();
        }

        [Test]
        public void assure_instance_is_created()
        {
            CreateShellPrestenter();
            presenter.ShouldNotBeNull();
        }

        [Test]
        public void assure_starts_unpaused()
        {
            CreateShellPrestenter();
            presenter.IsPaused.ShouldBeFalse();
        }

    }

    [TestFixture]
    public class rotation_is_started : Shared
    {
        [SetUp]
        public void Setup()
        {
            setupMocks();
        }

        [Test]
        public void assure_switches_to_first_slide_when_starting_rotation()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.StartsWith("Slide 1").ShouldBeTrue();
        }


        //when commands are published
        [Test]
        public void assure_pause_state_toggles_when_Pause_command_is_published()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            var p = new TogglePauseSlideShowCommandPublisher(eventAggregator);
            p.Notify();
            presenter.IsPaused.ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.Equals("Paused").ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.Equals("").ShouldBeTrue();
            p.Notify();
            presenter.IsPaused.ShouldBeFalse();
            fireSecondTicker();
            savedShellInfoString.StartsWith("Slide 1").ShouldBeTrue();
        }


        [Test]
        public void assure_switches_slide_when_Next_slide_command_is_published()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            fireSecondTicker();
            IsSlideOnShell(slide1).ShouldBeTrue();
            savedShellInfoString.StartsWith("Slide 1").ShouldBeTrue();
            var p = new NextSlideCommandPublisher(eventAggregator);
            p.Notify();
            fireSecondTicker();
            IsSlideOnShell(slide2).ShouldBeTrue();
            savedShellInfoString.StartsWith("Slide 2").ShouldBeTrue();
        }

        [Test]
        public void assure_switches_slide_when_Previous_slide_command_is_published()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.StartsWith("Slide 1").ShouldBeTrue();
            var p = new PreviousSlideCommandPublisher(eventAggregator);
            p.Notify();
            fireSecondTicker();
            IsSlideOnShell(slide4).ShouldBeTrue();
            savedShellInfoString.StartsWith("Slide 4").ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_build_breaks : Shared
    {

        [SetUp]
        public void Setup()
        {
            setupMocks();
        }

        [Test]
        public void assure_frozen_and_broken_build_state_is_turned_on_when_build_is_broken()
        {
            CreateShellPrestenter();
            var p = new FreezeViewCommandPublisher(eventAggregator);
            p.Notify();
            presenter.IsBrokenBuild.ShouldBeTrue();
            presenter.IsFrozen.ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.Equals("ATTENTION!").ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.Equals("").ShouldBeTrue();
        }

        [Test]
        public void assure_slide_is_not_changed_when_build_breaks_and_paused_is_on()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            var p = new TogglePauseSlideShowCommandPublisher(eventAggregator);
            p.Notify();
            var q = new FreezeViewCommandPublisher(eventAggregator);
            q.Notify();
            presenter.IsBrokenBuild.ShouldBeTrue();
            presenter.IsFrozen.ShouldBeTrue();
            IsSlideOnShell(slide1).ShouldBeTrue();
        }


        [Test]
        public void assure_slide_is_changed_when_build_breaks_and_paused_is_off()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            var q = new FreezeViewCommandPublisher(eventAggregator);
            q.Notify();
            presenter.IsBrokenBuild.ShouldBeTrue();
            presenter.IsFrozen.ShouldBeTrue();
            IsSlideOnShell(slide4).ShouldBeTrue();
        }

        [Test]
        public void assure_slide_is_unpaused_when_build_breaks_and_paused_is_toggled()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            var p = new FreezeViewCommandPublisher(eventAggregator);
            p.Notify();
            var q = new TogglePauseSlideShowCommandPublisher(eventAggregator);
            q.Notify();
            presenter.IsBrokenBuild.ShouldBeTrue();
            presenter.IsFrozen.ShouldBeFalse();
            presenter.IsPaused.ShouldBeFalse();
            IsSlideOnShell(slide4).ShouldBeTrue();
        }
    }
    
    [TestFixture]
    public class when_build_unbreaks : Shared
    {

        [SetUp]
        public void Setup()
        {
            setupMocks();
        }

        [Test]
        public void assure_slide_is_unfrozen_when_build_unbreaks()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            var p = new FreezeViewCommandPublisher(eventAggregator);
            p.Notify();
            var q = new UnFreezeViewCommandPublisher(eventAggregator);
            q.Notify();
            presenter.IsBrokenBuild.ShouldBeFalse();
            presenter.IsFrozen.ShouldBeFalse();
            presenter.IsPaused.ShouldBeFalse();
            IsSlideOnShell(slide4).ShouldBeTrue();
        }

        [Test]
        public void assure_slide_is_still_paused_when_build_unbreaks()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            IsSlideOnShell(slide1).ShouldBeTrue();
            var p = new TogglePauseSlideShowCommandPublisher(eventAggregator);
            p.Notify();
            var q = new FreezeViewCommandPublisher(eventAggregator);
            q.Notify();
            var r = new UnFreezeViewCommandPublisher(eventAggregator);
            r.Notify();
            presenter.IsBrokenBuild.ShouldBeFalse();
            presenter.IsFrozen.ShouldBeFalse();
            presenter.IsPaused.ShouldBeTrue();
            IsSlideOnShell(slide1).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_starting_up_in_admin_mode : Shared
    {
        [SetUp]
        public void Setup()
        {
            setupMocks();
        }

        [Test]
        public void Assure_admin_mode_is_toggeled()
        {
            displayAdminUICheckerMock.Setup(d => d.DisplayAdminUI()).Returns(true);

            CreateShellPrestenter();
            presenter.StartSlideRotation();

            presenter.IsAdminMode.ShouldBeTrue();
            IsSlideOnShell(presenter.AdminSlide).ShouldBeTrue();
            savedShellInfoString.ShouldBe("Admin Mode");
        }

        [Test]
        public void Assure_admin_mode_is_frozen()
        {
            displayAdminUICheckerMock.Setup(d => d.DisplayAdminUI()).Returns(true);

            CreateShellPrestenter();
            presenter.StartSlideRotation();
            presenter.IsAdminMode.ShouldBeTrue();

            for (int i = 0; i < 10; i++ )
            {
                fireSecondTicker();
                presenter.IsAdminMode.ShouldBeTrue();
                IsSlideOnShell(presenter.AdminSlide).ShouldBeTrue();
                savedShellInfoString.ShouldBe("Admin Mode");
            }
            
        }
    }

    [TestFixture]
    public class when_admin_mode_is_toggled : Shared
    {

        [SetUp]
        public void Setup()
        {
            setupMocks();
        }

        [Test]
        public void assure_admin_mode_is_toggled()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            presenter.IsAdminMode.ShouldBeFalse();
            var p = new ToggleAdminModeCommandPublisher(eventAggregator);
            p.Notify();
            presenter.IsAdminMode.ShouldBeTrue();
            fireSecondTicker();
            savedShellInfoString.Equals("Admin Mode").ShouldBeTrue();
            p.Notify();
            presenter.IsAdminMode.ShouldBeFalse();
            fireSecondTicker();
            savedShellInfoString.StartsWith("Slide 1").ShouldBeTrue();
        }

        [Test]
        public void Assure_rotation_is_frozen()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            var p = new ToggleAdminModeCommandPublisher(eventAggregator);
            p.Notify();
            presenter.IsAdminMode.ShouldBeTrue();

            for (int i = 0; i < 10; i++)
            {
                fireSecondTicker();
                presenter.IsAdminMode.ShouldBeTrue();
            }
        }

        [Test]
        public void assure_admin_view_is_shown()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            var p = new ToggleAdminModeCommandPublisher(eventAggregator);
            p.Notify();
            IsSlideOnShell(adminSlide).ShouldBeTrue();
        }

        [Test]
        public void assure_status_text_is_changed()
        {
            CreateShellPrestenter();
            presenter.StartSlideRotation();
            fireSecondTicker();
            savedShellInfoString.StartsWith("Slide 1").ShouldBeTrue();
            var p = new ToggleAdminModeCommandPublisher(eventAggregator);
            p.Notify();
            fireSecondTicker();
            savedShellInfoString.Equals("Admin Mode").ShouldBeTrue();
        }
    }

}