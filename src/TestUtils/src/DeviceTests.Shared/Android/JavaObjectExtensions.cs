#if ANDROID
using System;

namespace Microsoft.Maui.DeviceTests;

static class JavaObjectExtensions
{
	public static bool IsDisposed(this global::Java.Lang.Object obj)
	{
		return obj.Handle == IntPtr.Zero;
	}

	public static bool IsDisposed(this global::Android.Runtime.IJavaObject obj)
	{
		return obj.Handle == IntPtr.Zero;
	}
}
#endif