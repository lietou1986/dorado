using System;
using System.Collections.Generic;
using System.Text;

namespace Dorado.Spider.Html
{
    /// <summary>
    /// HTMLTag: This class holds a single HTML tag. This class
    /// subclasses the AttributeList class. This allows the
    /// HTMLTag class to hold a collection of attributes, just as
    /// an actual HTML tag does.
    /// </summary>
    public class HtmlTag
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Is this tag both a beginning and an
        /// ending tag.
        /// </summary>
        public Boolean Ending { get; set; }

        /// <summary>
        /// The attributes of this tag.
        /// </summary>
        private readonly Dictionary<String, String> _attributes = new Dictionary<String, String>();

        /// <summary>
        /// Clear out this tag.
        /// </summary>
        public void Clear()
        {
            this._attributes.Clear();
            this.Name = "";
            this.Ending = false;
        }

        /// <summary>
        /// Access the individual attributes by name.
        /// </summary>
        public String this[string key]
        {
            get
            {
                if (_attributes.ContainsKey(key.ToLower()))
                    return this._attributes[key.ToLower()];
                else
                    return null;
            }
            set
            {
                this._attributes.Add(key.ToLower(), value);
            }
        }

        /// <summary>
        /// Convert this tag back into string form, with the
        /// beginning &lt; and ending &gt;.
        /// </summary>
        /// <returns>The attribute value that was found.</returns>
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder("<");
            buffer.Append(this.Name);

            foreach (String key in _attributes.Keys)
            {
                String value = this._attributes[key];
                buffer.Append(' ');

                if (value == null)
                {
                    buffer.Append("\"");
                    buffer.Append(key);
                    buffer.Append("\"");
                }
                else
                {
                    buffer.Append(key);
                    buffer.Append("=\"");
                    buffer.Append(value);
                    buffer.Append("\"");
                }
            }

            if (this.Ending)
            {
                buffer.Append('/');
            }
            buffer.Append(">");
            return buffer.ToString();
        }

        /// <summary>
        /// Set the specified attribute.
        /// </summary>
        /// <param name="key">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public void SetAttribute(String key, String value)
        {
            _attributes.Remove(key.ToLower());
            _attributes.Add(key.ToLower(), value);
        }
    }
}