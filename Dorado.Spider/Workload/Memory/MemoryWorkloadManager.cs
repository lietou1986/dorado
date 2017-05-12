using System;
using System.Collections.Generic;
using System.Threading;

namespace Dorado.Spider.Workload.Memory
{
    /// <summary>
    /// MemoryWorkloadManager: This class implements a workload
    /// manager that stores the list of URL's in memory. This
    /// workload manager only supports spidering against a single
    /// host.
    /// </summary>
    public class MemoryWorkloadManager : IWorkloadManager
    {
        /// <summary>
        /// The current workload, a map between URL and URLStatus
        /// objects.
        /// </summary>
        private readonly Dictionary<Uri, UrlStatus> _workload = new Dictionary<Uri, UrlStatus>();

        /// <summary>
        /// The list of those items, which are already in the
        /// workload, that are waiting for processing.
        /// </summary>
        private readonly Queue<Uri> _waiting = new Queue<Uri>();

        /// <summary>
        /// How many URL's are currently being processed.
        /// </summary>
        private int _workingCount = 0;

        /// <summary>
        /// Because the MemoryWorkloadManager only supports a
        /// single host, the currentHost is set to the host of the
        /// first URL added.
        /// </summary>
        private String _currentHost;

        /// <summary>
        /// Allows other threads to wait for the status of the
        /// workload to change.
        /// </summary>
        private readonly AutoResetEvent _workloadEvent = new AutoResetEvent(true);

        /// <summary>
        /// Add the specified URL to the workload.
        /// </summary>
        /// <param name="url">The URL to be added.</param>
        /// <param name="source">The page that contains this URL.</param>
        /// <param name="depth">The depth of this URL.</param>
        /// <returns>True if the URL was added, false otherwise.</returns>
        public bool Add(Uri url, Uri source, int depth)
        {
            if (Contains(url)) return false;
            this._waiting.Enqueue(url);
            SetStatus(url, source, UrlStatus.Status.Waiting, depth);
            if (this._currentHost == null)
            {
                this._currentHost = url.Host.ToLower();
            }
            this._workloadEvent.Set();
            return true;
        }

        /// <summary>
        /// Clear the workload.
        /// </summary>
        public void Clear()
        {
            this._workload.Clear();
            this._waiting.Clear();
            this._workingCount = 0;
            this._workloadEvent.Set();
        }

        /// <summary>
        /// Determine if the workload contains the specified URL.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>Returns true if the specified URL is contained in the workload</returns>
        public bool Contains(Uri url)
        {
            return (this._workload.ContainsKey(url));
        }

        /// <summary>
        /// Convert the specified String to a URL. If the string is
        /// too long or has other issues, throw a
        /// WorkloadException.
        /// </summary>
        /// <param name="url">A String to convert into a URL.</param>
        /// <returns>The URL converted.</returns>
        public Uri ConvertUrl(String url)
        {
            try
            {
                return new Uri(url);
            }
            catch (UriFormatException e)
            {
                throw new WorkloadException(e);
            }
        }

        /// <summary>
        /// Get the current host.
        /// </summary>
        /// <returns>The current host.</returns>
        public String GetCurrentHost()
        {
            return this._currentHost;
        }

        /// <summary>
        /// Get the depth of the specified URL.
        /// </summary>
        /// <param name="url">The URL to get the depth of.</param>
        /// <returns>The depth of the specified URL.</returns>
        public int GetDepth(Uri url)
        {
            UrlStatus s = this._workload[url];
            return s != null ? s.Depth : 1;
        }

        /// <summary>
        /// Get the source page that contains the specified URL.
        /// </summary>
        /// <param name="url">The URL to seek the source for.</param>
        /// <returns>The source of the specified URL.</returns>
        public Uri GetSource(Uri url)
        {
            UrlStatus s = this._workload[url];
            return s == null ? null : s.Source;
        }

        /// <summary>
        /// Get a new URL to work on. Wait if there are no URL's
        /// currently available. Return null if done with the
        /// current host. The URL being returned will be marked as
        /// in progress.
        /// </summary>
        /// <returns>The next URL to work on.</returns>
        public Uri GetWork()
        {
            if (this._waiting.Count <= 0) return null;
            Uri url = this._waiting.Dequeue();
            SetStatus(url, null, UrlStatus.Status.Working, -1);
            this._workingCount++;
            return url;
        }

        /// <summary>
        /// Setup this workload manager for the specified spider.
        /// This method is not used by the MemoryWorkloadManager.
        /// </summary>
        /// <param name="spider">The spider using this workload manager.</param>
        public void Init(Spider spider)
        {
        }

        /// <summary>
        /// Mark the specified URL as error.
        /// </summary>
        /// <param name="url">The URL that had an error.</param>
        public void MarkError(Uri url)
        {
            this._workingCount--;
            SetStatus(url, null, UrlStatus.Status.Error, -1);
        }

        /// <summary>
        /// Mark the specified URL as successfully processed.
        /// </summary>
        /// <param name="url">The URL to mark as processed.</param>
        public void MarkProcessed(Uri url)
        {
            this._workingCount--;
            SetStatus(url, null, UrlStatus.Status.Processed, -1);
        }

        /// <summary>
        /// Move on to process the next host. This should only be
        /// called after getWork returns null. Because the
        /// MemoryWorkloadManager is single host only, this
        /// function simply returns null.
        /// </summary>
        /// <returns>The name of the next host.</returns>
        public String NextHost()
        {
            return null;
        }

        /// <summary>
        /// Setup the workload so that it can be resumed from where
        /// the last spider left the workload.
        /// </summary>
        public void Resume()
        {
            throw (new WorkloadException(
                "Memory based workload managers can not resume."));
        }

        /// <summary>
        /// If there is currently no work available, then wait
        /// until a new URL has been added to the workload. Because
        /// the MemoryWorkloadManager uses a blocking queue, this
        /// method is not needed. It is implemented to support the
        /// interface.
        /// </summary>
        /// <param name="time">The amount of time to wait.</param>
        public void WaitForWork(int time)
        {
            DateTime start = DateTime.Now;
            while (!WorkloadEmpty() && this._workingCount > 0)
            {
                if (_workloadEvent.WaitOne(1000, false)) continue;
                TimeSpan span = DateTime.Now - start;
                if (span.TotalSeconds > time)
                    return;
            }
        }

        /// <summary>
        /// Return true if there are no more workload units.
        /// </summary>
        /// <returns>Returns true if there are no more workload units.</returns>
        public bool WorkloadEmpty()
        {
            try
            {
                Monitor.Enter(this);

                if (this._waiting.Count != 0)
                {
                    return false;
                }

                if (this._workingCount < 1)
                    return true;
                else
                    return false;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Set the source, status and depth for the specified URL.
        /// </summary>
        /// <param name="url">The URL to set.</param>
        /// <param name="source">The source of this URL.</param>
        /// <param name="status"> The status of this URL.</param>
        /// <param name="depth">The depth of this URL.</param>
        private void SetStatus(Uri url, Uri source, UrlStatus.Status status, int depth)
        {
            UrlStatus s;
            if (!this._workload.ContainsKey(url))
            {
                s = new UrlStatus();
                this._workload.Add(url, s);
            }
            else
                s = this._workload[url];

            s.CurrentStatus = status;

            if (source != null)
            {
                s.Source = source;
            }

            if (depth != -1)
            {
                s.Depth = depth;
            }

            _workloadEvent.Set();
        }
    }
}