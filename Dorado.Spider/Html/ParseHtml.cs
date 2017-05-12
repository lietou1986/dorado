using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dorado.Spider.Html
{
    /// <summary>
    /// This class implements an HTML parser.  This parser is used
    /// by the Heaton Research spider, but it can also be used as a
    /// stand alone HTML parser.
    /// </summary>
    public class ParseHtml
    {
        /// <summary>
        /// A mapping of certain HTML encoded values(i.e. &amp;nbsp;)
        /// to their actual character values.
        /// </summary>
        private static Dictionary<String, char> _charMap;

        /// <summary>
        /// The stream that we are parsing fro
        /// </summary>
        private readonly PeekableInputStream _source;

        /// <summary>
        /// The current HTML tag. Access this property if the read
        /// function returns 0.
        /// </summary>
        public HtmlTag Tag { get; set; }

        /// <summary>
        /// Is there an end tag we are "locked into", such as
        /// a comment tag, script tag or similar.
        /// </summary>
        private String _lockedEndTag;

        /// <summary>
        /// Construct the HTML parser based in the specified stream.
        /// </summary>
        /// <param name="istream">The stream that will be parsed.</param>
        public ParseHtml(Stream istream)
        {
            this._source = new PeekableInputStream(istream);
            Tag = new HtmlTag();

            if (_charMap != null) return;
            _charMap = new Dictionary<String, char>
            {
                {"nbsp", ' '},
                {"lt", '<'},
                {"gt", '>'},
                {"amp", '&'},
                {"quot", '\"'},
                {"bull", (char) 149},
                {"trade", (char) 129}
            };
        }

        /// <summary>
        /// Read a single character from the HTML source, if this function returns zero(0) then you should call getTag to see what tag was found. Otherwise the value returned is simply the next character found.
        /// </summary>
        /// <returns>The character read, or zero if there is an HTML tag. If zero is returned, then call getTag to get the next tag.</returns>
        public virtual int Read()
        {
            // handle locked end tag
            if (this._lockedEndTag != null)
            {
                if (PeekEndTag(this._lockedEndTag))
                {
                    this._lockedEndTag = null;
                }
                else
                {
                    return this._source.Read();
                }
            }

            // look for next tag
            if (this._source.Peek(0) == '<')
            {
                ParseTag();
                if (!this.Tag.Ending
                    && (String.Compare(this.Tag.Name, "script", true) == 0 || String.Compare(this.Tag.Name, "style", true) == 0))
                {
                    this._lockedEndTag = this.Tag.Name.ToLower();
                }
                return 0;
            }
            else if (this._source.Peek(0) == '&')
            {
                return ParseSpecialCharacter();
            }
            else
            {
                return (this._source.Read());
            }
        }

        /// <summary>
        /// Represent as a string.  Read all text and ignore tags.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();

            int ch = 0;
            StringBuilder text = new StringBuilder();
            do
            {
                ch = Read();
                if (ch == 0)
                {
                    if (text.Length > 0)
                    {
                        text.Length = 0;
                    }
                }
                else if (ch != -1)
                {
                    text.Append((char)ch);
                }
            } while (ch != -1);
            if (text.Length > 0)
            {
            }
            return result.ToString();
        }

        /// <summary>
        /// Parse any special characters(i.e. &amp;nbsp;).
        /// </summary>
        /// <returns>The character that was parsed.</returns>
        private char ParseSpecialCharacter()
        {
            char result = (char)this._source.Read();
            int advanceBy = 0;

            // is there a special character?
            if (result == '&')
            {
                int ch = 0;
                StringBuilder buffer = new StringBuilder();

                // Loop through and read special character.
                do
                {
                    ch = this._source.Peek(advanceBy++);
                    if ((ch != '&') && (ch != ';') && !char.IsWhiteSpace((char)ch))
                    {
                        buffer.Append((char)ch);
                    }
                } while ((ch != ';') && (ch != -1) && !char.IsWhiteSpace((char)ch));

                String b = buffer.ToString().Trim().ToLower();

                // did we find a special character?
                if (b.Length > 0)
                {
                    if (b[0] == '#')
                    {
                        try
                        {
                            result = (char)int.Parse(b.Substring(1));
                        }
                        catch (FormatException)
                        {
                            advanceBy = 0;
                        }
                    }
                    else
                    {
                        if (_charMap.ContainsKey(b))
                        {
                            result = _charMap[b];
                        }
                        else
                        {
                            advanceBy = 0;
                        }
                    }
                }
                else
                {
                    advanceBy = 0;
                }
            }

            while (advanceBy > 0)
            {
                Read();
                advanceBy--;
            }

            return result;
        }

        /// <summary>
        /// See if the next few characters are an end tag.
        /// </summary>
        /// <param name="name">The end tag we are looking for.</param>
        /// <returns></returns>
        private bool PeekEndTag(String name)
        {
            int i = 0;

            // pass any whitespace
            while ((this._source.Peek(i) != -1)
                && char.IsWhiteSpace((char)this._source.Peek(i)))
            {
                i++;
            }

            // is a tag beginning
            if (this._source.Peek(i) != '<')
            {
                return false;
            }
            else
            {
                i++;
            }

            // pass any whitespace
            while ((this._source.Peek(i) != -1)
                && char.IsWhiteSpace((char)this._source.Peek(i)))
            {
                i++;
            }

            // is it an end tag
            if (this._source.Peek(i) != '/')
            {
                return false;
            }
            else
            {
                i++;
            }

            // pass any whitespace
            while ((this._source.Peek(i) != -1)
                && char.IsWhiteSpace((char)this._source.Peek(i)))
            {
                i++;
            }

            // does the name match
            foreach (char t in name)
            {
                if (char.ToLower((char)this._source.Peek(i)) != char
                    .ToLower((char)t))
                {
                    return false;
                }
                i++;
            }

            return true;
        }

        /// <summary>
        /// Remove any whitespace characters that are next in the InputStream.
        /// </summary>
        protected void EatWhitespace()
        {
            while (char.IsWhiteSpace((char)this._source.Peek(0)))
            {
                this._source.Read();
            }
        }

        /// <summary>
        /// Parse an attribute name, if one is present.
        /// </summary>
        /// <returns>The attribute name parsed.</returns>
        protected String ParseAttributeName()
        {
            EatWhitespace();

            if ("\"\'".IndexOf((char)this._source.Peek(0)) == -1)
            {
                StringBuilder buffer = new StringBuilder();
                while (!char.IsWhiteSpace((char)this._source.Peek(0))
                    && (this._source.Peek(0) != '=') && (this._source.Peek(0) != '>')
                    && (this._source.Peek(0) != -1))
                {
                    int ch = ParseSpecialCharacter();
                    buffer.Append((char)ch);
                }
                return buffer.ToString();
            }
            else
            {
                return (ParseString());
            }
        }

        /// <summary>
        /// Called to parse a double or single quote string.
        /// </summary>
        /// <returns>The string parsed.</returns>
        protected String ParseString()
        {
            StringBuilder result = new StringBuilder();
            EatWhitespace();
            if ("\"\'".IndexOf((char)this._source.Peek(0)) != -1)
            {
                int delim = this._source.Read();
                while ((this._source.Peek(0) != delim) && (this._source.Peek(0) != -1))
                {
                    if (result.Length > 1000)
                    {
                        break;
                    }
                    int ch = ParseSpecialCharacter();
                    if ((ch == 13) || (ch == 10))
                    {
                        continue;
                    }
                    result.Append((char)ch);
                }
                if ("\"\'".IndexOf((char)this._source.Peek(0)) != -1)
                {
                    this._source.Read();
                }
            }
            else
            {
                while (!char.IsWhiteSpace((char)this._source.Peek(0))
                    && (this._source.Peek(0) != -1) && (this._source.Peek(0) != '>'))
                {
                    result.Append(ParseSpecialCharacter());
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Called when a tag is detected. This method will parse the tag.
        /// </summary>
        protected void ParseTag()
        {
            this.Tag.Clear();
            StringBuilder tagName = new StringBuilder();

            this._source.Read();

            // Is it a comment?
            if ((this._source.Peek(0) == '!') && (this._source.Peek(1) == '-')
                && (this._source.Peek(2) == '-'))
            {
                while (this._source.Peek(0) != -1)
                {
                    if ((this._source.Peek(0) == '-') && (this._source.Peek(1) == '-')
                        && (this._source.Peek(2) == '>'))
                    {
                        break;
                    }
                    if (this._source.Peek(0) != '\r')
                    {
                        tagName.Append((char)this._source.Peek(0));
                    }
                    this._source.Read();
                }
                tagName.Append("--");
                this._source.Read();
                this._source.Read();
                this._source.Read();
                return;
            }

            // Find the tag name
            while (this._source.Peek(0) != -1)
            {
                if (char.IsWhiteSpace((char)this._source.Peek(0))
                    || (this._source.Peek(0) == '>'))
                {
                    break;
                }
                tagName.Append((char)this._source.Read());
            }

            EatWhitespace();
            this.Tag.Name = tagName.ToString();

            // Get the attributes.

            while ((this._source.Peek(0) != '>') && (this._source.Peek(0) != -1))
            {
                String attributeName = ParseAttributeName();
                String attributeValue = null;

                if (attributeName.Equals("/"))
                {
                    EatWhitespace();
                    if (this._source.Peek(0) == '>')
                    {
                        this.Tag.Ending = true;
                        break;
                    }
                }

                // is there a value?
                EatWhitespace();
                if (this._source.Peek(0) == '=')
                {
                    this._source.Read();
                    attributeValue = ParseString();
                }

                this.Tag.SetAttribute(attributeName, attributeValue);
            }
            this._source.Read();
        }
    }
}