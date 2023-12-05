using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class EmptyViewNoCrashUITests : CollectionViewUITests
	{
		public EmptyViewNoCrashUITests(TestDevice device)
			: base(device)
		{
		}

		// EmptyViewShouldNotCrash (src\Compatibility\ControlGallery\src\Issues.Shared\Issue9196.xaml.cs)
		[Test]
		public void EmptyViewShouldNotCrash()
		{
			// Navigate to the EmptyView galleries
			VisitInitialGallery("EmptyView");

			// Navigate to the specific sample inside EmptyView galleries
			VisitSubGallery("EmptyViewNoCrash");

			App.WaitForNoElement("Success");
		}
	}
}