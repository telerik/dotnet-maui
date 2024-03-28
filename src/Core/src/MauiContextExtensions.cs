using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.ApplicationModel;
using System;


#if WINDOWS
using NativeApplication = Microsoft.UI.Xaml.Application;
using NativeWindow = Microsoft.UI.Xaml.Window;
#elif __IOS__ || __MACCATALYST__
using NativeApplication = UIKit.IUIApplicationDelegate;
using NativeWindow = UIKit.UIWindow;
#elif __ANDROID__
using NativeApplication = Android.App.Application;
using NativeWindow = Android.App.Activity;
#elif TIZEN
using NativeApplication = Tizen.Applications.CoreApplication;
using NativeWindow = Tizen.NUI.Window;
#else
using NativeApplication = System.Object;
using NativeWindow = System.Object;
#endif

namespace Microsoft.Maui
{
	internal static partial class MauiContextExtensions
	{
		public static IAnimationManager GetAnimationManager(this IMauiContext mauiContext) =>
			mauiContext.Services.GetRequiredService<IAnimationManager>();

		public static IDispatcher GetDispatcher(this IMauiContext mauiContext) =>
			mauiContext.Services.GetRequiredService<IDispatcher>();

		public static IDispatcher GetOptionalDispatcher(this IMauiContext mauiContext) =>
			mauiContext.Services.GetService<IDispatcher>();

		// Reflection shortcut
		static void AddWeakSpecific<TService>(IServiceProvider services, TService instance)
			where TService : class
		{
			var type = services.GetType();
			var addSpecific = type.GetMethod("AddSpecific", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			addSpecific.Invoke(services, new object[] { typeof(TService), static (object state) => (object)((WeakReference)state).Target, new WeakReference(instance) });
		}

		// Reflection shortcut
		static void AddSpecific<TService>(IServiceProvider services, TService instance)
			where TService : class
		{
			var type = services.GetType();
			var addSpecific = type.GetMethod("AddSpecific", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			addSpecific.Invoke(services, new object[] { typeof(TService), static (object state) => (object)state, instance });
		}

		public static IMauiContext MakeApplicationScope(this IMauiContext mauiContext, NativeApplication platformApplication)
		{
			var scopedContext = new MauiContext(mauiContext.Services);

			AddSpecific(scopedContext.Services, platformApplication);

			return scopedContext;
		}

		public static IMauiContext MakeWindowScope(this IMauiContext mauiContext, NativeWindow platformWindow, out IServiceScope scope)
		{
			// Create the window-level scopes that will only be used for the lifetime of the window
			// TODO: We need to dispose of these services once the window closes
			scope = mauiContext.Services.CreateScope();

#if ANDROID
			var scopedContext = new MauiContext(scope.ServiceProvider, platformWindow);
#else
			var scopedContext = new MauiContext(scope.ServiceProvider);
#endif

			AddWeakSpecific(scopedContext.Services, platformWindow);

#if ANDROID
			AddSpecific(scopedContext.Services, new NavigationRootManager(scopedContext));
#endif
#if WINDOWS
			AddSpecific(scopedContext.Services, new NavigationRootManager(platformWindow));
#endif

			// Initialize any window-scoped services, for example the window dispatchers and animation tickers
			scopedContext.InitializeScopedServices();

			return scopedContext;
		}

		public static void InitializeAppServices(this MauiApp mauiApp)
		{
			var initServices = mauiApp.Services.GetServices<IMauiInitializeService>();
			if (initServices is null)
				return;
			
			foreach (var instance in initServices)
				instance.Initialize(mauiApp.Services);
		}

		public static void InitializeScopedServices(this IMauiContext scopedContext)
		{
			var scopedServices = scopedContext.Services.GetServices<IMauiInitializeScopedService>();
			if (scopedServices is null)
				return;

			foreach (var service in scopedServices)
				service.Initialize(scopedContext.Services);
		}
	}
}
