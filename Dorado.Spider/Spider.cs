using Dorado.Spider.Filter;
using Dorado.Spider.Logging;
using Dorado.Spider.Workload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Dorado.Spider
{
    /// <summary>
    /// Spider: This is the main class
    /// </summary>
    public class Spider
    {
        /// <summary>
        /// The workload manager for the spider.
        /// </summary>
        public IWorkloadManager Workload
        {
            get
            {
                return _workloadManager;
            }
        }

        /// <summary>
        /// A list of URL filters to use.
        /// </summary>
        public List<ISpiderFilter> Filters
        {
            get
            {
                return _filters;
            }
        }

        /// <summary>
        /// The SpiderReportable object for the spider.  The spider
        /// will report all information to this class.
        /// </summary>
        public ISpiderReportable Report
        {
            get
            {
                return _report;
            }
        }

        /// <summary>
        /// Used to log spider events.  Using this object
        /// you can configure how the spider logs information.
        /// </summary>
        public Logger Logging
        {
            get
            {
                return _logging;
            }
        }

        /// <summary>
        /// The configuration options for the spider.
        /// </summary>
        public SpiderOptions Options
        {
            get
            {
                return _options;
            }
        }

        /// <summary>
        /// The object that the spider reports its findings to.
        /// </summary>
        private readonly ISpiderReportable _report;

        /**
         * A flag that indicates if this process should be
         * canceled.
         */
        private bool _cancel;

        /// <summary>
        /// The workload manager, the spider can use any of several
        /// different workload managers. The workload manager
        /// tracks all URL's found.
        /// </summary>
        private readonly IWorkloadManager _workloadManager;

        /// <summary>
        /// The options for the spider.
        /// </summary>
        private readonly SpiderOptions _options;

        /// <summary>
        /// Filters used to block specific URL's.
        /// </summary>
        private readonly List<ISpiderFilter> _filters = new List<ISpiderFilter>();

        /// <summary>
        /// The time that the spider began.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// The time that the spider ended.
        /// </summary>
        private DateTime _stopTime;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Logger _logging = new Logger();

        /// <summary>
        /// The types of link that can be encountered.
        /// </summary>
        public enum UrlType
        {
            /// <summary>
            /// Hyperlinks from the &lt;A&gt; tag.
            /// </summary>
            Hyperlink,

            /// <summary>
            /// Images from the &lt;IMG&gt; tag.
            /// </summary>
            Image,

            /// <summary>
            /// External scripts from the &lt;SCRIPT&gt; tag.
            /// </summary>
            Script,

            /// <summary>
            /// External styles from the &lt;STYLE&gt; tag.
            /// </summary>
            Style
        }

        /// <summary>
        /// Construct a spider object. The options parameter
        /// specifies the options for this spider. The report
        /// parameter specifies the class that the spider is to
        /// report progress to.
        /// </summary>
        /// <param name="options">The configuration options for this spider.</param>
        /// <param name="report">A SpiderReportable class to report progress to</param>
        public Spider(SpiderOptions options, ISpiderReportable report)
        {
            this._options = options;
            this._report = report;

            this._workloadManager = (IWorkloadManager)Assembly.GetExecutingAssembly().CreateInstance(this._options.WorkloadManager);

            this._workloadManager.Init(this);
            report.Init(this);

            // add filters
            if (options.Filter != null)
            {
                foreach (String name in options.Filter)
                {
                    ISpiderFilter filter = (ISpiderFilter)Assembly.GetExecutingAssembly().CreateInstance(name);
                    if (filter == null)
                        throw new SpiderException("Invalid filter specified: " + name);
                    this._filters.Add(filter);
                }
            }

            // perform startup
            if (String.Compare(options.Startup, SpiderOptions.StartupResume) == 0)
            {
                this._workloadManager.Resume();
            }
            else
            {
                this._workloadManager.Clear();
            }
        }

        /// <summary>
        /// Add a URL for processing. Accepts a SpiderURL.
        /// </summary>
        /// <param name="url">The URL to add.</param>
        /// <param name="source">Where this URL was found.</param>
        /// <param name="depth">The depth of this URL.</param>
        public void AddUrl(Uri url, Uri source, int depth)
        {
            // Check the depth.
            if ((this._options.MaxDepth != -1) && (depth > this._options.MaxDepth))
            {
                return;
            }

            // Check to see if it does not pass any of the filters.
            if (this._filters.Any(filter => filter.IsExcluded(url)))
            {
                return;
            }

            // Add the item.
            if (this._workloadManager.Add(url, source, depth))
            {
                StringBuilder str = new StringBuilder();
                str.Append("Adding to workload: ");
                str.Append(url);
                str.Append("(depth=");
                str.Append(depth);
                str.Append(")");
                _logging.Log(Logger.Level.Info, str.ToString());
            }
        }

        /// <summary>
        /// This will halt the spider.
        /// </summary>
        public void Cancel()
        {
            this._cancel = true;
        }

        /// <summary>
        /// Generate basic status information about the spider.
        /// </summary>
        public String Status
        {
            get
            {
                StringBuilder result = new StringBuilder();
                TimeSpan duration = _stopTime - _startTime;
                result.Append("Start time:");
                result.Append(this._startTime.ToString());
                result.Append('\n');
                result.Append("Stop time:");
                result.Append(this._stopTime.ToString());
                result.Append('\n');
                result.Append("Minutes Elapsed:");
                result.Append(duration);
                result.Append('\n');

                return result.ToString();
            }
        }

        /// <summary>
        /// Called to start the spider.
        /// </summary>
        public void Process()
        {
            this._cancel = false;
            this._startTime = DateTime.Now;

            // Process all hosts/
            do
            {
                ProcessHost();
            } while (this._workloadManager.NextHost() != null);

            this._stopTime = DateTime.Now;
        }

        /// <summary>
        /// Process one individual host.
        /// </summary>
        private void ProcessHost()
        {
            String host = this._workloadManager.GetCurrentHost();

            // First, notify the manager.
            if (!this._report.BeginHost(host))
            {
                return;
            }

            // Second, notify any filters of a new host/
            foreach (ISpiderFilter filter in this._filters)
            {
                try
                {
                    filter.NewHost(host, this._options.UserAgent);
                }
                catch (IOException e)
                {
                    _logging.Log(Logger.Level.Info, "Error while reading robots.txt file:"
                        + e.Message);
                }
            }

            // Now process this host.
            do
            {
                var url = this._workloadManager.GetWork();
                if (url != null)
                {
                    WaitCallback w = new WaitCallback(SpiderWorkerProc);
                    ThreadPool.QueueUserWorkItem(w, url);
                }
                else
                {
                    this._workloadManager.WaitForWork(60);
                }
            } while (!this._cancel && !_workloadManager.WorkloadEmpty());
        }

        /// <summary>
        /// This method is called by the thread pool to process one
        /// single URL.
        /// </summary>
        /// <param name="stateInfo">Not used.</param>
        private void SpiderWorkerProc(Object stateInfo)
        {
            Stream istream = null;
            HttpWebResponse response;
            Uri url = null;
            try
            {
                url = (Uri)stateInfo;
                _logging.Log(Logger.Level.Info, "Processing: " + url);

                // Get the URL's contents.
                var http = HttpWebRequest.Create(url);
                http.Timeout = this._options.Timeout;
                if (this._options.UserAgent != null)
                {
                    http.Headers["User-Agent"] = this._options.UserAgent;
                }
                response = (HttpWebResponse)http.GetResponse();

                // Read the URL.
                istream = response.GetResponseStream();

                // Parse the URL.
                if (String.Compare(response.ContentType, "text/html") == 0)
                {
                    SpiderParseHtml parse = new SpiderParseHtml(response.ResponseUri,
                        new SpiderInputStream(istream, null), this);
                    this._report.SpiderProcessUrl(url, parse);
                }
                else
                {
                    this._report.SpiderProcessUrl(url, istream);
                }
            }
            catch (IOException e)
            {
                _logging.Log(Logger.Level.Info, "I/O error on URL:" + url);
                try
                {
                    this._workloadManager.MarkError(url);
                }
                catch (WorkloadException)
                {
                    _logging.Log(Logger.Level.Error, "Error marking workload(1).", e);
                }
                this._report.SpiderUrlError(url);
                return;
            }
            catch (WebException e)
            {
                _logging.Log(Logger.Level.Info, "Web error on URL:" + url);
                try
                {
                    this._workloadManager.MarkError(url);
                }
                catch (WorkloadException)
                {
                    _logging.Log(Logger.Level.Error, "Error marking workload(2).", e);
                }
                this._report.SpiderUrlError(url);
                return;
            }
            catch (Exception e)
            {
                try
                {
                    this._workloadManager.MarkError(url);
                }
                catch (WorkloadException)
                {
                    _logging.Log(Logger.Level.Error, "Error marking workload(3).", e);
                }

                _logging.Log(Logger.Level.Error, "Caught exception at URL:" + url.ToString(), e);
                this._report.SpiderUrlError(url);
                return;
            }
            finally
            {
                if (istream != null)
                {
                    istream.Close();
                }
            }

            try
            {
                // Mark URL as complete.
                this._workloadManager.MarkProcessed(url);
                _logging.Log(Logger.Level.Info, "Complete: " + url);
                if (!url.Equals(response.ResponseUri))
                {
                    // save the URL(for redirect's)
                    this._workloadManager.Add(response.ResponseUri, url,
                        this._workloadManager.GetDepth(response.ResponseUri));
                    this._workloadManager.MarkProcessed(response.ResponseUri);
                }
            }
            catch (WorkloadException e)
            {
                _logging.Log(Logger.Level.Error, "Error marking workload(3).", e);
            }
        }
    }
}