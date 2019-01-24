using Dorado.Core;
using Dorado.Core.Logger;
using NuGet;
using System;

namespace Dorado.Platform.Packaging
{
    internal class NugetLogger : NuGet.ILogger
    {
        public void Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Debug:
                    //LoggerWrapper.Logger.Debug(String.Format(message, args));
                    break;

                case MessageLevel.Error:
                    LoggerWrapper.Logger.Error(String.Format(message, args));
                    break;

                case MessageLevel.Info:
                    LoggerWrapper.Logger.Info(String.Format(message, args));
                    break;

                case MessageLevel.Warning:
                    LoggerWrapper.Logger.Warn(String.Format(message, args));
                    break;
            }
        }

        public FileConflictResolution ResolveFileConflict(string message)
        {
            return FileConflictResolution.OverwriteAll;
        }
    }
}