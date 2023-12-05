using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class CollectionViewDynamicallyLoadUITests : CollectionViewUITests
	{
		const string DynamicallyLoad = "CollectionViewDynamicallyLoad";
		const string Success = "Success";

		public CollectionViewDynamicallyLoadUITests(TestDevice device)
			: base(device)
		{
		}

		// CollectionViewShouldSourceShouldUpdateWhileInvisible (src\Compatibility\ControlGallery\src\Issues.Shared\Issue13126.cs)
		[Test]
		public void DynamicallyLoadCollectionView()
		{
			App.WaitForElement(DynamicallyLoad);
			App.Click(DynamicallyLoad);

			App.WaitForNoElement(Success);
		}
	}
}