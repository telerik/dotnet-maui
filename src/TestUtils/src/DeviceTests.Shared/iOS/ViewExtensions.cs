#if IOS
using System;
using System.Collections.Generic;
using UIKit;

namespace Microsoft.Maui.Platform;

// NOTE: There are ViewExtensions in the MAUI repo, but the codebase is HUGE
// pulls too much internal APIs in use,
// while in the DeviceTests is used just a small subset.

public static partial class ViewExtensions2
{
    internal static T FindResponder<T>(this UIView view) where T : UIResponder
    {
        var nextResponder = view as UIResponder;
        while (nextResponder is not null)
        {
            nextResponder = nextResponder.NextResponder;

            if (nextResponder is T responder)
            {
                return responder;
            }
        }
        return null;
    }

    internal static T GetChildAt<T>(this UIView view, int index) where T : UIView
    {
        if (index < view.Subviews.Length)
        {
            return (T)view.Subviews[index];
        }

        return null;
    }

    public static T FindDescendantView<T>(this UIView view) where T : UIView =>
        FindDescendantView<T>(view, (_) => true);

    public static T FindDescendantView<T>(UIView view, Func<UIView, bool> predicate) where T : UIView
    {
        var queue = new Queue<UIView>();
        queue.Enqueue(view);

        while (queue.Count > 0)
        {
            var descendantView = queue.Dequeue();

            if (descendantView is T result && predicate.Invoke(result))
            {
                return result;
            }

            int i = 0;
            UIView child;
            while ((child = descendantView?.GetChildAt<UIView>(i)) is not null)
            {
#if TIZEN
                // I had to add this check for Tizen to compile.
                // I think Tizen isn't accounting for the null check
                // in the while loop correctly
                if (child is null)
                    break;
#endif
                queue.Enqueue(child);
                i++;
            }
        }

        return null;
    }
}
#endif