using System;
using System.Runtime.InteropServices;

namespace Dorado.Windows.SingleInstancing
{
    public class WindowUtility
    {
        public static readonly int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImportAttribute("user32.dll")] //winuser.h
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hWnd);
    }
}