using NUnit.Framework;
using UITest.Appium;
using UITest.Core;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Maui.AppiumTests
{
	public class CollectionViewUITests : UITest
	{
		const string CollectionViewGallery = "CollectionView Gallery";

		readonly string _collectionViewId = "collectionview";
		readonly string _btnUpdate = "btnUpdate";
		readonly string _entryUpdate = "entryUpdate";
		readonly string _entryInsert = "entryInsert";
		readonly string _entryRemove = "entryRemove";
		readonly string _entryReplace = "entryReplace";
		readonly string _entryScrollTo = "entryScrollTo";
		readonly string _btnInsert = "btnInsert";
		readonly string _btnRemove = "btnRemove";
		readonly string _btnReplace = "btnReplace";
		readonly string _btnGo = "btnGo";
		readonly string _inserted = "Inserted";
		readonly string _replaced = "Replacement";
		readonly string _picker = "pickerSelectItem";
		readonly string _dialogAndroidFrame = "select_dialog_listview";

		public CollectionViewUITests(TestDevice device)
			: base(device)
		{
		}

		protected override void FixtureSetup()
		{
			base.FixtureSetup();
			App.NavigateToGallery(CollectionViewGallery);
		}

		protected override void FixtureTeardown()
		{
			base.FixtureTeardown();
			this.Back();
		}

		[TearDown]
		public void CollectionViewUITestBaseTearDown()
		{
			this.Back();
		}
	
		[TestCase("Observable Collection", "Add/RemoveItemsList", 19, 6)]
		[TestCase("Observable Collection", "Add/RemoveItemsGrid", 19, 6)]
		[TestCase("Default Text", "VerticalListCode", 101, 11)]
		[TestCase("Default Text", "HorizontalListCode", 101, 11)]
		[TestCase("Default Text", "VerticalGridCode", 101, 11)]
		[TestCase("Default Text", "HorizontalGridCode", 101, 11)]
		[TestCase("DataTemplate", "VerticalListCode", 19, 6)]
		[TestCase("DataTemplate", "HorizontalListCode", 19, 6)]
		[TestCase("DataTemplate", "VerticalGridCode", 19, 6)]
		[TestCase("DataTemplate", "HorizontalGridCode", 19, 6)]
		public void VisitAndUpdateItemsSource(string collectionTestName, string subGallery, int firstItem, int lastItem)
		{
			VisitInitialGallery(collectionTestName);
			VisitSubGallery(subGallery, !subGallery.Contains("Horizontal", StringComparison.OrdinalIgnoreCase), $"Item: {firstItem}", $"Item: {lastItem}", lastItem - 1, true, false);
			this.Back();
		}

		[TestCase("Observable Collection", new string[] { "Add/RemoveItemsList", "Add/RemoveItemsGrid" }, 1, 6)]
		public void AddRemoveItems(string collectionTestName, string[] subGalleries, int firstItem, int lastItem)
		{
			VisitInitialGallery(collectionTestName);

			foreach (var gallery in subGalleries)
			{
				if (gallery == "FilterItems")
					continue;

				VisitSubGallery(gallery, !gallery.Contains("Horizontal", StringComparison.OrdinalIgnoreCase), $"Item: {firstItem}", $"Item: {lastItem}", lastItem - 1, false, true);
				this.Back();
			}
		}

		[TestCase("Observable Collection", new string[] { "Add/RemoveItemsList", "Add/RemoveItemsGrid" }, 19, 6)]
		[TestCase("Default Text", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode" }, 101, 11)] //HorizontalGridCode
		[TestCase("DataTemplate", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode", "HorizontalGridCode" }, 19, 6)]
		public void VisitAndTestItemsPosition(string collectionTestName, string[] subGalleries, int firstItem, int lastItem)
		{
			VisitInitialGallery(collectionTestName);

			foreach (var gallery in subGalleries)
			{
				if (gallery == "FilterItems")
					continue;
				App.WaitForElement(gallery);
				App.Click(gallery);
				TestItemsPosition(gallery);
				this.Back();
			}
		}

		[TestCase("EmptyView", "EmptyView (load simulation)", "photo")]
		public void VisitAndCheckItem(string collectionTestName, string subgallery, string item)
		{
			VisitInitialGallery(collectionTestName);

			App.WaitForElement(subgallery);
			App.Click(subgallery);

			App.WaitForElement(item);
		}

		[TestCase("DataTemplate", "DataTemplateSelector")]
		public void VisitAndCheckForItems(string collectionTestName, string subGallery)
		{
			VisitInitialGallery(collectionTestName);

			App.WaitForElement(subGallery);
			App.Click(subGallery);

			App.WaitForElement("weekend");
			App.WaitForElement("weekday");
		}

		[TestCase("ScrollTo", new string[] {
		 	"ScrollToIndexCode,HorizontalList", "ScrollToIndexCode,VerticalList", "ScrollToIndexCode,HorizontalGrid", "ScrollToIndexCode,VerticalGrid",
		 	"ScrollToItemCode,HorizontalList", "ScrollToItemCode,VerticalList", "ScrollToItemCode,HorizontalGrid", "ScrollToItemCode,VerticalGrid", }, 1, 20)]
		public void ScrollTo(string collectionTestName, string[] subGalleries, int firstItem, int goToItem)
		{
			VisitInitialGallery(collectionTestName);

			foreach (var galleryName in subGalleries)
			{
				if (galleryName == "FilterItems")
					continue;

				var isList = !galleryName.Contains("Grid", StringComparison.OrdinalIgnoreCase);
				var isItem = !galleryName.Contains("Index", StringComparison.OrdinalIgnoreCase);

				if (isItem)
				{
					TestScrollToItem(firstItem, goToItem, galleryName, isList);
				}
				else
				{
					TestScrollToIndex(firstItem, goToItem, galleryName, isList);
				}

				this.Back();
			}
		}

		void TestScrollToItem(int firstItem, int goToItem, string galleryName, bool isList)
		{
			App.WaitForElement(galleryName);
			App.Click(galleryName);
			App.WaitForElement(_picker);
			App.Click(_picker);

			var firstItemMarked = $"Item: {firstItem}";
			var goToItemMarked = isList ? $"Item: {goToItem}" : $"Item: {goToItem - 1}";
			App.WaitForElement(firstItemMarked);

			var pickerDialogFrame = App.FindElement(_dialogAndroidFrame).GetRect();

			//App.ScrollForElement($"* marked:'{goToItemMarked}'", new Drag(pickerDialogFrame, Drag.Direction.BottomToTop, Drag.DragLength.Short));
			App.ScrollTo(goToItemMarked);

			App.Click(goToItemMarked);
			App.DismissKeyboard();
			App.Click(_btnGo);
			App.WaitForNoElement(firstItemMarked);
			App.WaitForElement(goToItemMarked);
		}

		void TestScrollToIndex(int firstItem, int goToItem, string galleryName, bool isList)
		{
			App.WaitForElement(galleryName);
			App.Click(galleryName);
			App.WaitForElement(_entryScrollTo);
			App.ClearText(_entryScrollTo);
			App.EnterText(_entryScrollTo, goToItem.ToString());
			App.DismissKeyboard();
			App.Click(_btnGo);
			App.WaitForNoElement($"Item: {firstItem}");
			var itemToCheck = isList ? $"Item: {goToItem}" : $"Item: {goToItem - 1}";
			App.WaitForElement(itemToCheck);
		}

		void VisitInitialGallery(string collectionTestName)
		{
			var galleryName = $"{collectionTestName} Galleries";
			string trimmedGalleryName = galleryName.Replace(" ", "", StringComparison.OrdinalIgnoreCase);
			App.WaitForElement(trimmedGalleryName);
			App.Click(trimmedGalleryName);
		}

		void VisitSubGallery(string galleryName, bool scrollDown, string lastItem, string firstPageItem, int updateItemsCount, bool testItemSource, bool testAddRemove)
		{
			App.WaitForElement(galleryName);
			App.Click(galleryName);

			// Let's test the update
			if (testItemSource)
			{
				var collectionViewFrame = TestItemsExist(scrollDown, lastItem);
				TestUpdateItemsWorks(scrollDown, firstPageItem, updateItemsCount.ToString(), collectionViewFrame);
			}

			if (testAddRemove)
			{
				TestAddRemoveReplaceWorks(lastItem);
			}
		}

		void TestAddRemoveReplaceWorks(string lastItem)
		{
			App.WaitForElement(_entryRemove);
			App.ClearText(_entryRemove);
			App.EnterText(_entryRemove, "1");
			App.DismissKeyboard();
			App.Click(_btnRemove);
			App.WaitForNoElement(lastItem);
			App.ClearText(_entryInsert);
			App.EnterText(_entryInsert, "1");
			App.DismissKeyboard();
			App.Click(_btnInsert);
			App.WaitForElement(_inserted);
			//TODO: enable replace
			App.ClearText(_entryReplace);
			App.EnterText(_entryReplace, "1");
			App.DismissKeyboard();
			App.Click(_btnReplace);
			App.WaitForElement(_replaced);
		}

		void TestUpdateItemsWorks(bool scrollDown, string itemMarked, string updateItemsCount, System.Drawing.Rectangle collectionViewFrame)
		{
			App.WaitForElement(_entryUpdate);

			//App.ScrollForElement($"* marked:'{itemMarked}'", new Drag(collectionViewFrame, scrollDown ? Drag.Direction.TopToBottom : Drag.Direction.LeftToRight, Drag.DragLength.Long), 50);
			//App.ScrollTo(itemMarked);

			App.ClearText(_entryUpdate);
			App.EnterText(_entryUpdate, updateItemsCount);
			App.DismissKeyboard();
			App.Click(_btnUpdate);
			App.WaitForNoElement(itemMarked);
		}

		System.Drawing.Rectangle TestItemsExist(bool scrollDown, string itemMarked)
		{
			App.WaitForElement(_btnUpdate);
			var collectionViewFrame = App.FindElement(_collectionViewId).GetRect();

			//App.ScrollForElement($"* marked:'{itemMarked}'", new Drag(collectionViewFrame, scrollDown ? Drag.Direction.BottomToTop : Drag.Direction.RightToLeft, Drag.DragLength.Long));
			//App.ScrollTo(itemMarked);

			return collectionViewFrame;
		}

		void TestItemsPosition(string gallery)
		{
			var firstItem = "Item: 0";
			var secondItem = "Item: 1";
			var fourthItem = "Item: 3";

			var isVertical = !gallery.Contains("Horizontal", StringComparison.OrdinalIgnoreCase);
			var isList = !gallery.Contains("Grid", StringComparison.OrdinalIgnoreCase);
			App.WaitForNoElement(gallery);

			var element1 = App.FindElement(firstItem);
			var element2 = App.FindElement(secondItem);

			if (isVertical)
			{
				if (isList)
				{
					Assert.AreEqual(element1.GetRect().X, element2.GetRect().X, message: $"{gallery} Elements are not align");
					Assert.Greater(element2.GetRect().Y, element1.GetRect().Y, message: $"{gallery} Element2.Y is not greater that Element1.Y");
				}
				else
				{
					var element3 = App.FindElement(fourthItem);
					Assert.AreEqual(element2.GetRect().Y, element1.GetRect().Y, message: $"{gallery} Elements are not align");
					Assert.Greater(element3.GetRect().Y, element1.GetRect().Y, message: $"{gallery} Element3.Y is not greater that Element1.Y");
					Assert.AreEqual(element3.GetRect().X, element1.GetRect().X, message: $"{gallery} Element3.X on second row is not below Element1X");
				}
			}
			else
			{
				if (isList)
				{
					Assert.AreEqual(element1.GetRect().Y, element2.GetRect().Y, message: $"{gallery} Elements are not align");
					Assert.Greater(element2.GetRect().X, element1.GetRect().X, message: $"{gallery} Element2.X is not greater that Element1.X");
				}
				else
				{
					var element3 = App.FindElement(fourthItem);
					Assert.AreEqual(element2.GetRect().X, element1.GetRect().X, message: $"{gallery} Elements are not align");
					Assert.Greater(element3.GetRect().X, element1.GetRect().X, message: $"{gallery} Element2.X is not greater that Element1.X");
					Assert.AreEqual(element3.GetRect().Y, element1.GetRect().Y, message: $"{gallery} Element3.Y is not in the same row as Element1.Y");
				}
			}
		}
	}
}