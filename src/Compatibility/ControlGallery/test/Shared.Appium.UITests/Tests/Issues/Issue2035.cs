﻿using NUnit.Framework;
using UITest.Appium;

namespace UITests
{
	public class Issue2035 : IssuesUITest
	{
		const string Success = "Success";

		public Issue2035(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "App crashes when setting CurrentPage on TabbedPage in ctor in 2.5.1pre1";
		
		[Test]
		[Category(UITestCategories.TabbedPage)]
		public void Issue2035Test()
		{
			this.IgnoreIfPlatforms([TestDevice.iOS, TestDevice.Mac, TestDevice.Windows]);

			App.WaitForElement(Success);
			//if it doesn't crash, we're good.
		}
	}
}