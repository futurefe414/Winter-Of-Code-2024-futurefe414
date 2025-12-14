using Microsoft.UI.Xaml.Controls;
using System;

namespace SastImg.Client.Helpers;

/// <summary>
/// Helper class for navigation operations
/// </summary>
public static class NavigationHelper
{
    /// <summary>
    /// Navigate to a page with optional parameter
    /// </summary>
    public static bool NavigateTo(Frame frame, Type pageType, object? parameter = null)
    {
        if (frame == null || pageType == null)
            return false;

        return frame.Navigate(pageType, parameter);
    }

    /// <summary>
    /// Navigate back if possible
    /// </summary>
    public static bool GoBack(Frame frame)
    {
        if (frame == null || !frame.CanGoBack)
            return false;

        frame.GoBack();
        return true;
    }

    /// <summary>
    /// Check if navigation back is possible
    /// </summary>
    public static bool CanGoBack(Frame frame)
    {
        return frame != null && frame.CanGoBack;
    }

    /// <summary>
    /// Clear navigation history
    /// </summary>
    public static void ClearHistory(Frame frame)
    {
        if (frame == null)
            return;

        frame.BackStack.Clear();
    }
}
