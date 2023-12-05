using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class CollectionViewTabbedPageUITests : CollectionViewUITests
	{
		const string CollectionViewTabbedPage = "CollectionViewInsideTabbedPage";
		const string Add1 = "Add1";
		const string Add2 = "Add2";
		const string Success = "Success";
		const string Tab2 = "Tab2";
		const string Tab3 = "Tab3";

		public CollectionViewTabbedPageUITests(TestDevice device)
			: base(device)
		{
		}

		// AddingGroupToUnviewedGroupedCollectionViewShouldNotCrash (src\Compatibility\ControlGallery\src\Issues.Shared\Issue7700.cs)
		[Test]
		public void AddingItemToUnviewedCollectionViewShouldNotCrash()
		{
			this.IgnoreIfPlatforms(new TestDevice[] { TestDevice.Android, TestDevice.iOS, TestDevice.Mac, TestDevice.Windows },
				"Click does not find Tab elements");

			App.WaitForElement(CollectionViewTabbedPage);
			App.Click(CollectionViewTabbedPage);

			App.WaitForElement(Add1);
			App.Click(Add1);
			App.Click(Tab2);

			App.WaitForElement(Success);
		}

		// AddingGroupToUnviewedGroupedCollectionViewShouldNotCrash (src\Compatibility\ControlGallery\src\Issues.Shared\Issue7700.cs)
		[Test]
		public void AddingGroupToUnviewedGroupedCollectionViewShouldNotCrash()
		{
			this.IgnoreIfPlatforms(new TestDevice[] { TestDevice.Android, TestDevice.iOS, TestDevice.Mac, TestDevice.Windows },
				"Click does not find Tab elements");

			App.WaitForElement(CollectionViewTabbedPage);
			App.Click(CollectionViewTabbedPage);

			App.WaitForElement(Add2);
			App.Click(Add2);
			App.Click(Tab3);

			App.WaitForElement(Success);
		}
	}
}