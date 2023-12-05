using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class CollectionViewBindingErrorsUITests : CollectionViewUITests
	{
		public CollectionViewBindingErrorsUITests(TestDevice device)
			: base(device)
		{
		}

		// CollectionViewBindingErrorsShouldBeZero (src\Compatibility\ControlGallery\src\Issues.Shared\CollectionViewBindingErrors.xaml.cs)
		[Test]
		public void NoBindingErrors()
		{
			App.WaitForElement("CollectionViewBindingErrors");
			App.Click("CollectionViewBindingErrors");

			App.WaitForElement("WaitForStubControl");
			App.WaitForNoElement("Binding Errors: 0");
		}
	}
}