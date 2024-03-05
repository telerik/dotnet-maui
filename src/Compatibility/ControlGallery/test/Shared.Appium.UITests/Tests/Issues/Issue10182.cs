﻿using NUnit.Framework;
using UITest.Appium;

namespace UITests
{
	public class Issue10182 : IssuesUITest
	{
		public Issue10182(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "[Bug] Exception Ancestor must be provided for all pushes except first";

		[Test]
		[Category(UITestCategories.LifeCycle)]
		public void AppDoesntCrashWhenResettingPage()
		{
			this.IgnoreIfPlatforms([TestDevice.iOS, TestDevice.Mac, TestDevice.Windows]);

			App.WaitForElement("Success");
		}
	}
}