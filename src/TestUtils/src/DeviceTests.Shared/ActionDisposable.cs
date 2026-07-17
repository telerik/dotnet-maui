using System;
using System.Threading;

namespace Microsoft.Maui.DeviceTests;

class ActionDisposable : IDisposable
{
	volatile Action _action;

	public ActionDisposable(Action action)
	{
		this._action = action;
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref this._action, null)?.Invoke();
	}
}
