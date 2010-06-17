using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Tests.ViewModel
{
    class WidgetTests
    {
        [TestFixture]
        public class When_spawned : WidgetTestContext
        {
            public override void Context()
            {
                Given_Widget_is_created();
            }

            [Test]
            public void assure_it_has_a_ErrorInfo()
            {
                viewModel.ErrorInfo.ShouldNotBeNull();
            }

        }

        [TestFixture]
        public class When_reporting_a_failure : WidgetTestContext
        {
            private FailingTraybarWidget failingTraybarWidget;


            public override void Context()
            {
                Given_failingTraybarWidget_is_created();

                report_failure("something went wrong");
            }

            private void Given_failingTraybarWidget_is_created()
            {
                failingTraybarWidget = new FailingTraybarWidget();
            }

            private void report_failure(string msg)
            {
                failingTraybarWidget.Report(msg);
            }

            [Test]
            public void assure_failure_is_reported()
            {
                failingTraybarWidget.ErrorInfo.HasError.ShouldBeTrue();
            }

            [Test]
            public void assure_error_msg_is_provided_in_error_report()
            {
                failingTraybarWidget.ErrorInfo.ErrorMessage.ShouldBe("something went wrong");
            }

            [Test]
            public void assure_failure_report_can_be_removed()
            {
                failingTraybarWidget.RemoveReport();

                failingTraybarWidget.ErrorInfo.HasError.ShouldBeFalse();
                failingTraybarWidget.ErrorInfo.ErrorMessage.ShouldBe(string.Empty);
            }

        }

        class FailingTraybarWidget : Widget
        {
            public FailingTraybarWidget()
            {
            }

            public void Report(string msg)
            {
                ReportFailure(msg);
            }

            public void RemoveReport()
            {
                NoFailure();
            }
        }

    }
}
