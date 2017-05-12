using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Dorado.Spider.Rss
{
    /// <summary>
    /// RSS: This is the class that actually parses the
    /// RSS and builds a collection of RSSItems.  To make use
    /// of this class call the load method with a URL that
    /// points to RSS.
    /// </summary>
    public class Rss
    {
        /// <summary>
        /// All of the attributes for this RSS document.
        /// </summary>
        public Dictionary<String, String> Attributes
        {
            get
            {
                return _attributes;
            }
        }

        /// <summary>
        /// All RSS items, or stories, found.
        /// </summary>
        public List<RssItem> Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// All of the attributes for this RSS document.
        /// </summary>
        private readonly Dictionary<String, String> _attributes = new Dictionary<String, String>();

        /// <summary>
        /// All RSS items, or stories, found.
        /// </summary>
        private readonly List<RssItem> _items = new List<RssItem>();

        /// <summary>
        /// Simple utility function that converts a RSS formatted date
        /// into a C# date.
        /// </summary>
        /// <param name="datestr">A date</param>
        /// <returns>A C# DateTime object.</returns>
        public static DateTime ParseDate(String datestr)
        {
            DateTime date = DateTime.Parse(datestr);
            return date;
        }

        /// <summary>
        /// Load the specified RSS item, or story.
        /// </summary>
        /// <param name="item">A XML node that contains a RSS item.</param>
        private void LoadItem(XmlNode item)
        {
            RssItem rssItem = new RssItem();
            rssItem.Load(item);
            _items.Add(rssItem);
        }

        /// <summary>
        /// Load the channle node.
        /// </summary>
        /// <param name="channel">A node that contains a channel.</param>
        private void LoadChannel(XmlNode channel)
        {
            foreach (XmlNode node in channel.ChildNodes)
            {
                String nodename = node.Name;
                if (String.Compare(nodename, "item", true) == 0)
                {
                    LoadItem(node);
                }
                else
                {
                    _attributes.Remove(nodename);
                    _attributes.Add(nodename, channel.InnerText);
                }
            }
        }

        /// <summary>
        /// Load all RSS data from the specified URL.
        /// </summary>
        /// <param name="url">URL that contains XML data.</param>
        public void Load(Uri url)
        {
            WebRequest http = HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)http.GetResponse();
            Stream istream = response.GetResponseStream();

            XmlDocument d = new XmlDocument();
            d.Load(istream);

            foreach (XmlNode node in d.DocumentElement.ChildNodes)
            {
                String nodename = node.Name;

                // RSS 2.0
                if (String.Compare(nodename, "channel", true) == 0)
                {
                    LoadChannel(node);
                }

                // RSS 1.0
                else if (String.Compare(nodename, "item", true) == 0)
                {
                    LoadItem(node);
                }
            }
        }

        /// <summary>
        /// Convert the object to a String.
        /// </summary>
        /// <returns>The object as a String.</returns>
        public override String ToString()
        {
            StringBuilder str = new StringBuilder();

            foreach (String item in _attributes.Keys)
            {
                str.Append(item);
                str.Append('=');
                str.Append(_attributes[item]);
                str.Append('\n');
            }
            str.Append("Items:\n");
            foreach (RssItem item in _items)
            {
                str.Append(item.ToString());
                str.Append('\n');
            }
            return str.ToString();
        }
    }
}