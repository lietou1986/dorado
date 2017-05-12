using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Dorado.Spider.Filter
{
    /// <summary>
    /// This filter causes the spider so skip URL's from a robots.txt file.
    /// </summary>
    internal class RobotsFilter : ISpiderFilter
    {
        /// <summary>
        /// Returns a list of URL's to be excluded.
        /// </summary>
        public List<String> Exclude
        {
            get
            {
                return _exclude;
            }
        }

        /// <summary>
        /// The full URL of the robots.txt file.
        /// </summary>
        public Uri RobotURL
        {
            get
            {
                return _robotUrl;
            }
        }

        /// <summary>
        /// The full URL of the robots.txt file.
        /// </summary>
        private Uri _robotUrl;

        /// <summary>
        /// A list of full URL's to exclude.
        /// </summary>
        private readonly List<String> _exclude = new List<String>();

        /// <summary>
        /// Is the parser active? It can become inactive when
        /// parsing sections of the file for other user agents.
        /// </summary>
        private bool _active;

        /// <summary>
        /// The user agent string we are to use. null for default.
        /// </summary>
        private String _userAgent;

        /// <summary>
        /// Check to see if the specified URL is to be excluded.
        /// </summary>
        /// <param name="url">The URL to be checked.</param>
        /// <returns>Returns true if the URL should be excluded.</returns>
        public bool IsExcluded(Uri url)
        {
            return this._exclude.Any(str => url.PathAndQuery.StartsWith(str));
        }

        /// <summary>
        /// Called when a new host is to be processed. SpiderFilter
        /// classes can not be shared among hosts.
        /// </summary>
        /// <param name="host">The new host.</param>
        /// <param name="userAgent">The user agent being used by the spider. Leave
        /// null for default.</param>
        public void NewHost(String host, String userAgent)
        {
            try
            {
                this._active = false;
                this._userAgent = userAgent;

                StringBuilder robotStr = new StringBuilder();
                robotStr.Append("http://");
                robotStr.Append(host);
                robotStr.Append("/robots.txt");
                this._robotUrl = new Uri(robotStr.ToString());

                WebRequest http = HttpWebRequest.Create(this._robotUrl);

                if (userAgent != null)
                {
                    http.Headers.Set("User-Agent", userAgent);
                }

                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);

                _exclude.Clear();

                try
                {
                    String str;
                    while ((str = reader.ReadLine()) != null)
                    {
                        LoadLine(str);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception)
            {
                // Site does not have a robots.txt file
                // this is common.
            }
        }

        /// <summary>
        /// Add the specified string to the exclude list.
        /// </summary>
        /// <param name="str">This string to add.  This is the path part of a URL.</param>
        private void Add(String str)
        {
            if (!this._exclude.Contains(str))
            {
                this._exclude.Add(str);
            }
        }

        /// <summary>
        /// Called internally to process each line of the
        /// robots.txt file.
        /// </summary>
        /// <param name="str">The line that was read in.</param>
        private void LoadLine(String str)
        {
            str = str.Trim();
            int i = str.IndexOf(':');

            if ((str.Length == 0) || (str[0] == '#') || (i == -1))
            {
                return;
            }

            String command = str.Substring(0, i);
            String rest = str.Substring(i + 1).Trim();
            if (String.Compare(command, "User-agent", true) == 0)
            {
                this._active = false;
                if (rest.Equals("*"))
                {
                    this._active = true;
                }
                else
                {
                    if ((this._userAgent != null) && String.Compare(rest, this._userAgent, true) == 0)
                    {
                        this._active = true;
                    }
                }
            }
            if (!this._active) return;
            if (String.Compare(command, "disallow", true) != 0) return;
            if (rest.Trim().Length <= 0) return;
            Uri url = new Uri(this._robotUrl, rest);
            Add(url.PathAndQuery);
        }
    }
}