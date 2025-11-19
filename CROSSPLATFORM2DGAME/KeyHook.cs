#if WINDOWS
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace CROSSPLATFORM2DGAME
{
    public static class KeyHook
    {
        public static void Attach(Microsoft.Maui.Controls.Window mauiWindow, KeyHandler keyHandler)
        {
            // Get the native WinUI window from the Maui window handler
            if (mauiWindow.Handler.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
            {
                // Get the root UIElement (usually a Grid)
                if (nativeWindow.Content is UIElement rootElement)
                {
                    // Make sure the root element can get keyboard focus
                    rootElement.IsTabStop = true;
                    rootElement.Focus(FocusState.Programmatic);

                    // Subscribe to key events
                    rootElement.KeyDown += (s, e) =>
                    {
                        keyHandler.SetKeyDown(e.Key.ToString());
                    };

                    rootElement.KeyUp += (s, e) =>
                    {
                        keyHandler.SetKeyUp(e.Key.ToString());
                    };
                }
            }
        }
    }
}
#endif