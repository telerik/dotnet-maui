using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Maui.Controls.Sample.CollectionViewGalleries.EmptyViewGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmptyViewNoCrash : ContentPage
	{

		public EmptyViewNoCrash()
		{
			InitializeComponent();

			BindingContext = new EmptyViewNoCrashViewModel();
		}
	}

	public class EmptyViewNoCrashViewModel
	{
		public EmptyViewNoCrashViewModel()
		{
			ReceiptsList = new List<string>();
		}

		public List<string> ReceiptsList { get; set; }
	}
}