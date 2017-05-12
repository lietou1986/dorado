using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dorado.Spider
{
    /// <summary>
    /// SpiderOptions: This class contains options for the
    /// spider's execution.
    /// </summary>
    public class SpiderOptions
    {
        /// <summary>
        /// Specifies that when the spider starts up it should clear the workload.
        /// </summary>
        public const String StartupClear = "CLEAR";

        /// <summary>
        /// Specifies that the spider should resume processing its workload.
        /// </summary>
        public const String StartupResume = "RESUME";

        /// <summary>
        /// How many milliseconds to wait when downloading pages.
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        /// <summary>
        /// The maximum depth to search pages. -1 specifies no maximum depth.
        /// </summary>
        public int MaxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                _maxDepth = value;
            }
        }

        /// <summary>
        /// What user agent should be reported to the web site.  This allows the web site to determine what browser is being used.
        /// </summary>
        public String UserAgent { get; set; }

        /// <summary>
        /// The connection string for databases. Used to hold the workload.
        /// </summary>
        public String DbConnectionString { get; set; }

        /// <summary>
        /// What class to use as a workload manager.
        /// </summary>
        public String WorkloadManager { get; set; }

        /// <summary>
        /// How to startup the spider, either clear or resume.
        /// </summary>
        public String Startup
        {
            get
            {
                return _startup;
            }
            set
            {
                _startup = value;
            }
        }

        /// <summary>
        ///  Specifies a class to be used a filter.
        /// </summary>
        public List<String> Filter
        {
            get
            {
                return _filter;
            }
        }

        private int _timeout = 60000;
        private int _maxDepth = -1;
        private String _startup = StartupClear;
        private readonly List<String> _filter = new List<String>();

        public SpiderOptions()
        {
            UserAgent = null;
        }

        /// <summary>
        /// Load the spider settings from a configuration file.
        /// </summary>
        /// <param name="inputFile">The name of the configuration file.</param>
        public void Load(String inputFile)
        {
            StreamReader r = File.OpenText(inputFile);

            String line;
            while ((line = r.ReadLine()) != null)
            {
                ParseLine(line);
            }
            r.Close();
        }

        /// <summary>
        /// The line of text read from the configuration file.
        /// </summary>
        /// <param name="line">The line of text read from the configuration file.</param>
        private void ParseLine(String line)
        {
            int i = line.IndexOf(':');
            if (i == -1)
            {
                return;
            }
            var name = line.Substring(0, i).Trim();
            var value = line.Substring(i + 1).Trim();

            if (value.Trim().Length == 0)
            {
                value = null;
            }

            Type myType = typeof(SpiderOptions);
            FieldInfo field = myType.GetField(name,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new SpiderException("Unknown configuration file element: " + name + " .");
            }
            else if (field.FieldType == typeof(String))
            {
                field.SetValue(this, value);
            }
            else if (field.FieldType == typeof(List<String>))
            {
                List<String> list = (List<String>)field.GetValue(this);
                list.Add(value);
            }
            else
            {
                int x = int.Parse(value);
                field.SetValue(this, x);
            }
        }
    }
}