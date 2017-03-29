using System;
using System.Diagnostics;

namespace Dorado.Windows.SingleInstancing
{
    /// <summary>
    /// Provides methods which a single instance application can use to in order to be respond to any new instance of it.
    /// </summary>
    [Serializable]
    public class SingleInstanceEnforcer : ISingleInstanceEnforcer
    {
        #region ISingleInstanceEnforcer 成员

        public virtual void OnMessageReceived(MessageEventArgs e)
        {
        }

        public virtual void OnNewInstanceCreated(EventArgs e)
        {
            Process ps = Process.GetCurrentProcess();
            WindowUtility.SetActiveWindow(ps.MainWindowHandle);
            WindowUtility.ShowWindow(ps.MainWindowHandle, WindowUtility.SW_RESTORE);
            WindowUtility.SetForegroundWindow(ps.MainWindowHandle);
        }

        #endregion ISingleInstanceEnforcer 成员
    }
}