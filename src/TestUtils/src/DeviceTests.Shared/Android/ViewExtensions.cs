#if ANDROID
using Android.OS;
using System;
using System.Threading.Tasks;
using PlatformView = Android.Views.View;

namespace Microsoft.Maui.DeviceTests;

static class ViewExtensions {

	internal static Task OnUnloadedAsync(this PlatformView platformView, TimeSpan? timeOut = null)
	{
		timeOut = timeOut ?? TimeSpan.FromSeconds(2);
		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		IDisposable token = null;
		token = platformView.OnUnloaded(() =>
		{
			taskCompletionSource.SetResult(true);
			token?.Dispose();
			token = null;
		});

		return taskCompletionSource.Task.WaitAsync(timeOut.Value);
	}

	internal static Task OnLoadedAsync(this PlatformView platformView, TimeSpan? timeOut = null)
	{
		timeOut = timeOut ?? TimeSpan.FromSeconds(2);
		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		IDisposable token = null;
		token = platformView.OnLoaded(() =>
		{
			taskCompletionSource.SetResult(true);
			token?.Dispose();
			token = null;
		});
		return taskCompletionSource.Task.WaitAsync(timeOut.Value);
	}

	internal static bool IsLoaded(this PlatformView frameworkElement)
	{
		if (frameworkElement == null)
        {
			return false;
        }

		if (frameworkElement.IsDisposed())
        {
			return false;
        }

		return frameworkElement.IsAttachedToWindow;
	}

	internal static IDisposable OnLoaded(this PlatformView view, Action action)
	{
		if (view.IsLoaded())
		{
			action();
			return new ActionDisposable(() => { });
		}

		EventHandler<PlatformView.ViewAttachedToWindowEventArgs> routedEventHandler = null;
		ActionDisposable disposable = new ActionDisposable(() =>
		{
			if (routedEventHandler != null)
            {
				view.ViewAttachedToWindow -= routedEventHandler;
            }
		});

		routedEventHandler = (_, __) =>
		{
			if (!view.IsLoaded() && Looper.MyLooper() is Looper q)
			{
				new Handler(q).Post(() =>
				{
					if (disposable is not null)
                    {
						action.Invoke();
                    }

					disposable?.Dispose();
					disposable = null;
				});

				return;
			}

			disposable?.Dispose();
			disposable = null;
			action();
		};

		view.ViewAttachedToWindow += routedEventHandler;
		return disposable;
	}

	internal static IDisposable OnUnloaded(this PlatformView view, Action action)
	{
		if (!view.IsLoaded())
		{
			action();
			return new ActionDisposable(() => { });
		}

		EventHandler<PlatformView.ViewDetachedFromWindowEventArgs> routedEventHandler = null;
		ActionDisposable disposable = new ActionDisposable(() =>
		{
			if (routedEventHandler != null)
            {
				view.ViewDetachedFromWindow -= routedEventHandler;
            }
		});

		routedEventHandler = (_, __) =>
		{
			// This event seems to fire prior to the view actually being
			// detached from the window
			if (view.IsLoaded() && Looper.MyLooper() is Looper q)
			{
				new Handler(q).Post(() =>
				{
					if (disposable is not null)
                    {
						action.Invoke();
                    }

					disposable?.Dispose();
					disposable = null;
				});

				return;
			}

			disposable?.Dispose();
			disposable = null;
			action();
		};

		view.ViewDetachedFromWindow += routedEventHandler;
		return disposable;
	}
}
#endif