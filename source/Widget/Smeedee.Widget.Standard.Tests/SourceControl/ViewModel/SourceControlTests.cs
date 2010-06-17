using System;
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
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Client.Web.Services;

namespace Smeedee.Widget.Standard.Tests.SourceControl.ViewModel
{
    public class SourceControlTests
    {
        [Export(typeof(SourceControlContext))]
        public SourceControlContext SourceControlContextMock;

        public SourceControlTests()
        {
            SourceControlContextMock = new SourceControlContext();
        }

        [TestClass]
        public class When_spawned : SourceControlViewModelContext
        {
            public override void Context()
            {
                Given_SourceControlViewModel_is_created();
            }

            [TestMethod]
            public void assure_it_has_Changesets()
            {
                Assert.IsNotNull(viewModel.Changesets);
            }
        }


    }
}