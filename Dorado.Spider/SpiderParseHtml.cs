using Dorado.Spider.Html;
using Dorado.Spider.Logging;
using Dorado.Spider.Workload;
using System;
using System.IO;

namespace Dorado.Spider
{
    /// <summary>
    /// SpiderParseHTML: This class layers on top of the
    /// ParseHTML class and allows the spider to extract what
    /// link information it needs. A SpiderParseHTML class can be
    /// used just like the ParseHTML class, with the spider
    /// gaining its information in the background.
    /// </summary>
    public class SpiderParseHtml : ParseHtml
    {
        /// <summary>
        /// The stream that the parser is reading from.
        /// </summary>
        public SpiderInputStream Stream
        {
            get
            {
                return _stream;
            }
        }

        /// <summary>
        /// The Spider that this page is being parsed for.
        /// </summary>
        private readonly Spider _spider;

        /// <summary>
        /// The URL that is being parsed.
        /// </summary>
        private Uri _baseUrl;

        /// <summary>
        /// The depth of the page being parsed.
        /// </summary>
        private readonly int _depth;

        /// <summary>
        /// The InputStream that is being parsed.
        /// </summary>
        private readonly SpiderInputStream _stream;

        /// <summary>
        /// Construct a SpiderParseHTML object. This object allows
        /// you to parse HTML, while the spider collects link
        /// information in the background.
        /// </summary>
        /// <param name="baseUrl">The URL that is being parsed, this is used for relative links.</param>
        /// <param name="istream">The InputStream being parsed.</param>
        /// <param name="spider">The Spider that is parsing.</param>
        public SpiderParseHtml(Uri baseUrl, SpiderInputStream istream, Spider spider)
            : base(istream)
        {
            this._stream = istream;
            this._spider = spider;
            this._baseUrl = baseUrl;
            this._depth = spider.Workload.GetDepth(baseUrl);
        }

        /// <summary>
        /// Read a single character. This function will process any
        /// tags that the spider needs for navigation, then pass
        /// the character on to the caller. This allows the spider
        /// to transparently gather its links.
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            int result = base.Read();
            if (result == 0)
            {
                HtmlTag tag = Tag;

                if (String.Compare(tag.Name, "a", true) == 0)
                {
                    String href = tag["href"];
                    HandleA(href);
                }
                else if (String.Compare(tag.Name, "img", true) == 0)
                {
                    String src = tag["src"];
                    AddUrl(src, Spider.UrlType.Image);
                }
                else if (String.Compare(tag.Name, "style", true) == 0)
                {
                    String src = tag["src"];
                    AddUrl(src, Spider.UrlType.Style);
                }
                else if (String.Compare(tag.Name, "link", true) == 0)
                {
                    String href = tag["href"];
                    AddUrl(href, Spider.UrlType.Script);
                }
                else if (String.Compare(tag.Name, "base", true) == 0)
                {
                    String href = tag["href"];
                    this._baseUrl = new Uri(this._baseUrl, href);
                }
            }
            return result;
        }

        /// <summary>
        /// Read all characters on the page. This will discard
        /// these characters, but allow the spider to examine the
        /// tags and find links.
        /// </summary>
        public void ReadAll()
        {
            while (Read() != -1)
            {
            }
        }

        /// <summary>
        /// Used internally, to add a URL to the spider's workload.
        /// </summary>
        /// <param name="u">The URL to add.</param>
        /// <param name="type">What type of link this is.</param>
        private void AddUrl(String u, Spider.UrlType type)
        {
            if (u == null)
            {
                return;
            }

            try
            {
                Uri url = UrlUtility.ConstructUrl(this._baseUrl, u, true);
                url = this._spider.Workload.ConvertUrl(url.ToString());

                if ((String.Compare(url.Scheme, "http", true) != 0) && (String.Compare(url.Scheme, "https", true) != 0))
                    return;
                if (!this._spider.Report.SpiderFoundUrl(url, this._baseUrl, type)) return;
                try
                {
                    this._spider.AddUrl(url, this._baseUrl, this._depth + 1);
                }
                catch (WorkloadException e)
                {
                    throw new IOException(e.Message);
                }
            }
            catch (UriFormatException)
            {
                _spider.Logging.Log(Logger.Level.Info, "Malformed URL found:" + u);
            }
            catch (WorkloadException)
            {
                _spider.Logging.Log(Logger.Level.Info, "Invalid URL found:" + u);
            }
        }

        /// <summary>
        /// This method is called when an anchor(A) tag is found.
        /// </summary>
        /// <param name="href">The link found.</param>
        private void HandleA(String href)
        {
            String cmp = null;
            if (href != null)
            {
                href = href.Trim();
                cmp = href.ToLower();
            }

            if ((cmp == null) || UrlUtility.ContainsInvalidUrlCharacters(href)) return;
            if (!cmp.StartsWith("javascript:")
                && !cmp.StartsWith("rstp:")
                && !cmp.StartsWith("rtsp:")
                && !cmp.StartsWith("news:")
                && !cmp.StartsWith("irc:")
                && !cmp.StartsWith("mailto:"))
            {
                AddUrl(href, Spider.UrlType.Hyperlink);
            }
        }
    }
}