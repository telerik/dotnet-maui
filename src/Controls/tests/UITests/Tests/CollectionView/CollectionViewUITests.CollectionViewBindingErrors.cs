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

		[Test]
		public void NoBindingErrors()
		{
			App.ScrollTo("CollectionViewBindingErrors");
			App.Click("CollectionViewBindingErrors");

			App.WaitForElement("WaitForStubControl");
			App.WaitForNoElement("Binding Errors: 0");
		}
	}
}