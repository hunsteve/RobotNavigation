using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MMCar_Finder
{
    /// <see cref="http://www.pinvoke.net/default.aspx/Constants.WM"/>
    public class Constants
    {
        public const uint WM_CAP = 0x400;
        public const uint WM_CAP_DRIVER_CONNECT = 0x40a;
        public const uint WM_CAP_DRIVER_DISCONNECT = 0x40b;
        public const uint WM_CAP_EDIT_COPY = 0x41e;
        public const uint WM_CAP_DLG_VIDEOFORMAT = 0x429;
        public const uint WM_CAP_SET_PREVIEW = 0x432;
        public const uint WM_CAP_SET_OVERLAY = 0x433;
        public const uint WM_CAP_SET_PREVIEWRATE = 0x434;
        public const uint WM_CAP_SET_SCALE = 0x435;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_VISIBLE = 0x10000000;        
    }

    /// <see cref="http://windowssdk.msdn.microsoft.com/en-us/library/ms713477(VS.80).aspx"/>
    public class Avicap32
    {
        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_capgetdriverdescription.asp"/>
        [DllImport("avicap32.dll")]
        public extern static IntPtr capGetDriverDescription(
            ushort index,
            StringBuilder name,
            int nameCapacity,
            StringBuilder description,
            int descriptionCapacity
        );

        /// <see cref="http://msdn.microsoft.com/library/en-us/multimed/htm/_win32_capcreatecapturewindow.asp?frame=true"/>
        [DllImport("avicap32.dll")]
        public extern static IntPtr capCreateCaptureWindow(
            string title,
            uint style,
            int x,
            int y,
            int width,
            int height,
            IntPtr window,
            int id
        );
    }

    /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/messagesandmessagequeues.asp"/>
    public class User32
    {
        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/messagesandmessagequeues/messagesandmessagequeuesreference/messagesandmessagequeuesfunctions/sendmessage.asp"/>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam
        );

        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowfunctions/setwindowpos.asp"/>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );

        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowfunctions/destroywindow.asp"/>
        [DllImport("user32")]
        public static extern IntPtr DestroyWindow(
            IntPtr hWnd
        );
    }
}
