using System;
using System.Runtime.InteropServices;

namespace Reactor.Platform.Windows
{
    public enum DWM_USE_IMMERSIVE_DARK_MODE
    {
        FALSE = 0,
        TRUE = 1
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    // Copied from dwmapi.h
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }
    
    public class Win32
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
            DWMWINDOWATTRIBUTE attribute,
            ref int pvAttribute,
            uint cbAttribute);

        public static void WindowDressing(IntPtr handle)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 10)
            {
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                Win32.DwmSetWindowAttribute(handle, attribute, ref preference, sizeof(int));
                attribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;
                var dark = 1;
                Win32.DwmSetWindowAttribute(handle, attribute, ref dark, sizeof(int));
            }
        }

    }
}