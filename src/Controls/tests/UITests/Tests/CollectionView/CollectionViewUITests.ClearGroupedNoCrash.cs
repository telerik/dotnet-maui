using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class ClearGroupedNoCrashUITests : CollectionViewUITests
	{
		const string Go = "Go";
		const string Success = "Success";

		public ClearGroupedNoCrashUITests(TestDevice device)
			: base(device)
		{
		}

		// ClearingGroupedCollectionViewShouldNotCrash (src\Compatibility\ControlGallery\src\Issues.Shared\Issue8899.cs)
		[Test]
		[Description("Clearing CollectionView IsGrouped=\"True\" no crashes application")]
		public void ClearingGroupedNoCrash()
		{
			// Navigate to the Grouping galleries
			VisitInitialGallery("Grouping");

			// Navigate to the specific sample inside Grouping galleries
			VisitSubGallery("ClearGroupingNoCrash");

			App.WaitForElement(Go);
			App.Click(Go);

			App.WaitForNoElement(Success);
		}
	}
}