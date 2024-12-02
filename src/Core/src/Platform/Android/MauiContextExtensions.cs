// The code is linked in our project under LinkBase=".../Platform/Android"
// but the compiler magic won't work and will fail to find the using Android.* namespaces
// unless wrapped in #if ANDROID...
#if ANDROID
using System;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Java.Util.Zip;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;

namespace Microsoft.Maui.Platform
{
	internal static partial class MauiContextExtensions
	{
		public static NavigationRootManager GetNavigationRootManager(this IMauiContext mauiContext) =>
			mauiContext.Services.GetRequiredService<NavigationRootManager>();

		public static LayoutInflater GetLayoutInflater(this IMauiContext mauiContext)
		{
			var layoutInflater = mauiContext.Services.GetService<LayoutInflater>();

			if (!layoutInflater.IsAlive() && mauiContext.Context != null)
			{
				var activity = mauiContext.Context.GetActivity();

				if (activity != null)
					layoutInflater = LayoutInflater.From(activity);
			}

			return layoutInflater ?? throw new InvalidOperationException("LayoutInflater Not Found");
		}

		public static FragmentManager GetFragmentManager(this IMauiContext mauiContext)
		{
			var fragmentManager = mauiContext.Services.GetService<FragmentManager>();

			return fragmentManager
				?? mauiContext.Context?.GetFragmentManager()
				?? throw new InvalidOperationException("FragmentManager Not Found");
		}

		public static AppCompatActivity GetActivity(this IMauiContext mauiContext) =>
			(mauiContext.Context?.GetActivity() as AppCompatActivity)
			?? throw new InvalidOperationException("AppCompatActivity Not Found");

		// Reflection shortcut
		static void AddWeakSpecific<TService>(IServiceProvider services, TService instance)
			where TService : class
		{
			var type = services.GetType();
			var addSpecific = type.GetMethod("AddSpecific", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			addSpecific.Invoke(services, new object[] { typeof(TService), static (object state) => (object)((WeakReference)state).Target, new WeakReference(instance) });
		}

		static void AddSpecific<TService>(IServiceProvider services, TService instance)
			where TService : class
		{
			var type = services.GetType();
			var addSpecific = type.GetMethod("AddSpecific", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			addSpecific.Invoke(services, new object[] { typeof(TService), static (object state) => (object)state, instance });
		}

		public static IMauiContext MakeScoped(this IMauiContext mauiContext,
			LayoutInflater layoutInflater = null,
			FragmentManager fragmentManager = null,
			Android.Content.Context context = null,
			bool registerNewNavigationRoot = false)
		{
			var scopedContext = new MauiContext(mauiContext.Services);

			if (layoutInflater != null)
				AddWeakSpecific(scopedContext.Services, layoutInflater);

			if (fragmentManager != null)
				AddWeakSpecific(scopedContext.Services, fragmentManager);

			if (context != null)
				AddWeakSpecific(scopedContext.Services, context);

			if (registerNewNavigationRoot)
			{
				if (fragmentManager == null)
					throw new InvalidOperationException("If you're creating a new Navigation Root you need to use a new Fragment Manager");

				AddSpecific(scopedContext.Services, new NavigationRootManager(scopedContext));
			}

			return scopedContext;
		}

		internal static View ToPlatform(
			this IView view,
			IMauiContext fragmentMauiContext,
			Android.Content.Context context,
			LayoutInflater layoutInflater,
			FragmentManager childFragmentManager)
		{
			if (view.Handler?.MauiContext is MauiContext scopedMauiContext)
			{
				// If this handler belongs to a different activity then we need to 
				// recreate the view.
				// If it's the same activity we just update the layout inflater
				// and the fragment manager so that the platform view doesn't recreate
				// underneath the users feet
				if (scopedMauiContext.GetActivity() == context.GetActivity() &&
					view.Handler.PlatformView is View platformView)
				{
					typeof(MauiContext)
						.GetMethod("AddWeakSpecific", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
						.Invoke(scopedMauiContext, new object[] { layoutInflater });

					typeof(MauiContext)
						.GetMethod("AddWeakSpecific", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
						.Invoke(scopedMauiContext, new object[] { childFragmentManager });

					return platformView;
				}
			}

			return (View)view.ToPlatform2(fragmentMauiContext.MakeScoped(layoutInflater: layoutInflater, fragmentManager: childFragmentManager));
		}

		internal static IServiceProvider GetApplicationServices(this IMauiContext mauiContext)
		{
			if (IPlatformApplication.Current?.Services is not null)
				return IPlatformApplication.Current.Services;

			throw new InvalidOperationException("Unable to find Application Services");
		}

		public static Android.App.Activity GetPlatformWindow(this IMauiContext mauiContext) =>
			mauiContext.Services.GetRequiredService<Android.App.Activity>();
	}
}
#endif