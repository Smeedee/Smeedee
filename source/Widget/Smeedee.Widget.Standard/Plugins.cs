using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Widgets.SL.CI;
using Smeedee.Client.Widgets.SL.ProjectInfo;
using Smeedee.Client.Widgets.SL.SourceControl;
using Smeedee.Client.Widgets.SL.Summary;
using Smeedee.Widget.Standard.ProjectInfo;
using Smeedee.Widget.Standard.SourceControl;

namespace Smeedee.Widget.Standard
{
    public class Plugins
    {
        [Export(typeof(Slide))]
        public TeamMembersSlide TeamMemberSlide = new TeamMembersSlide();

        [Export(typeof(Slide))]
        public WorkingDaysLeftSlide WorkingDaysLeftSlide = new WorkingDaysLeftSlide();

        [Export(typeof(Slide))]
        public BuildStatusSlide BuildStatusSlide = new BuildStatusSlide();
    
        [Export(typeof(Slide))]
        public LatestCommitSlide LatestCommitsSlide = new LatestCommitSlide();
    
        [Export(typeof(Slide))]
        public SummarySlide SummarySlide = new SummarySlide();
    

        [Export(typeof(TraybarWidget))]
        public BuildStatusTraybarWidget BuildStatusWidget = new BuildStatusTraybarWidget();
    
        [Export(typeof(TraybarWidget))]
        public WorkingDaysLeftTraybarWidget WorkingDaysLeftTraybarWidget = new WorkingDaysLeftTraybarWidget();
    
        [Export(typeof(TraybarWidget))]
        public ChangesTodayWidget ChangesTodayTraybarWidget = new ChangesTodayWidget();
    }
}
