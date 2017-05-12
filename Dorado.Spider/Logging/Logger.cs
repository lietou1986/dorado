using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Dorado.Spider.Logging
{
    /// <summary>
    /// Logger: Provides basic logging for the spider.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The logging levels.
        /// </summary>
        public enum Level
        {
            /// <summary>
            /// Debugging information.
            /// </summary>
            Debug = 0,

            /// <summary>
            /// General information about the spider's progress.
            /// </summary>
            Info = 1,

            /// <summary>
            /// Errors that were encountered, but will not halt the
            /// spider.
            /// </summary>
            Error = 2,

            /// <summary>
            /// Critical errors.
            /// </summary>
            Critical = 3
        };

        /// <summary>
        /// If the filename is null, then the log is not recorded to a file.
        /// If a filename is specified then the log will be recorded to that file.
        /// </summary>
        public String Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
            }
        }

        /// <summary>
        /// Give a value of true to record logging to the console.
        /// </summary>
        public bool Console
        {
            get
            {
                return _console;
            }
            set
            {
                _console = value;
            }
        }

        /// <summary>
        /// The level at which logging should be recorded, anything below this level will not be logged.
        /// </summary>
        public Level LogLevel
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        private String _filename;
        private bool _console = false;
        private Level _level = Level.Info;

        /// <summary>
        /// Delete the log file.
        /// </summary>
        public void Clear()
        {
            if (_filename != null)
                File.Delete(_filename);
        }

        /// <summary>
        /// Record a line of text into the log.
        /// </summary>
        /// <param name="level">The level for this entry.</param>
        /// <param name="str">The string to record.</param>
        public void Log(Level level, String str)
        {
            Monitor.Enter(this);
            try
            {
                if (level < this._level)
                    return;

                StringBuilder builder = new StringBuilder();
                DateTime now = DateTime.Now;
                builder.Append(now.ToString());
                builder.Append(':');
                if (level == Level.Critical)
                    builder.Append("CRITICAL");
                else if (level == Level.Debug)
                    builder.Append("DEBUG");
                else if (level == Level.Error)
                    builder.Append("ERROR");
                else if (level == Level.Info)
                    builder.Append("INFO");
                builder.Append(':');
                builder.Append(Thread.CurrentThread.ManagedThreadId);
                builder.Append(':');
                builder.Append(str);

                if (_filename != null)
                {
                    FileStream fs = new FileStream(_filename, FileMode.OpenOrCreate | FileMode.Append);
                    StreamWriter writer = new StreamWriter(fs);

                    writer.WriteLine(builder.ToString());
                    writer.Close();
                    fs.Close();
                }
                if (_console)
                    System.Console.WriteLine(builder.ToString());
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Record an exception into the log.
        /// </summary>
        /// <param name="level">The level for this entry.</param>
        /// <param name="str">A string to describe the entry.</param>
        /// <param name="e">The exception.</param>
        public void Log(Level level, String str, Exception e)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(str);
            builder.Append(Environment.NewLine);
            builder.Append(e.ToString());
            builder.Append(Environment.NewLine);
            builder.Append(e.StackTrace);
            Log(level, builder.ToString());
        }
    }
}