using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dorado.Platform.SolrNet
{
    public class SolrDocument : IEnumerable<KeyValuePair<SchemaField, string>>
    {
        private readonly Dictionary<string, string> _dict;
        public static readonly List<SchemaField> DefaultFieldList;
        private readonly List<SchemaField> _bidderFieldList = new List<SchemaField>();

        static SolrDocument()
        {
            DefaultFieldList = new List<SchemaField>();
            DefaultFieldList.AddRange(SchemaConfigProvider.Instance.Fields);
            DefaultFieldList.Add(new SchemaField() { Name = "score" });
        }

        public bool Contains(string name)
        {
            return _bidderFieldList.Any(f => f.Name == name) || DefaultFieldList.Any(f => f.Name == name);
        }

        public SolrDocument AddField(string name)
        {
            if (!Contains(name))
                _bidderFieldList.Add(new SchemaField() { Name = name });
            return this;
        }

        public SolrDocument AddCdataField(string name)
        {
            if (!Contains(name))
                _bidderFieldList.Add(new SchemaField() { Name = name, IsCdata = true });
            return this;
        }

        private void MargeField()
        {
            _bidderFieldList.InsertRange(0, DefaultFieldList);
        }

        public SolrDocument()
        {
            _dict = new Dictionary<string, string>();
        }

        public string GetXml()
        {
            MargeField();

            var sb = new StringBuilder();

            sb.Append("<doc>");
            foreach (var field in _bidderFieldList)
            {
                if (!_dict.ContainsKey(field.Name)) continue;
                var nodeValue = _dict[field.Name] ?? "";
                if (field.MultiValued)
                {
                    var values = nodeValue.Split(new[] { ',', '，', ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
                    foreach (var val in values)
                    {
                        AddElement(field, val.Trim(), sb);
                    }
                }
                else
                {
                    AddElement(field, nodeValue, sb);
                }
            }
            sb.Append("</doc>");
            return sb.ToString();
        }

        public void AddElement(SchemaField field, string value, StringBuilder sb)
        {
            sb.Append("<field name=\"");
            sb.Append(field.Name);
            if (field.Boost > 0)
            {
                sb.Append("\" Boost=\"");
                sb.Append(field.Boost);
            }
            sb.Append("\">");
            if (field.IsCdata && !string.IsNullOrEmpty(value))
            {
                sb.Append("<![CDATA[");
                sb.Append(value);
                sb.Append("]]>");
            }
            else
            {
                sb.Append(value);
            }
            sb.Append("</field>");
        }

        public string this[string name]
        {
            get
            {
                return _dict.ContainsKey(name) ? _dict[name] : AddField(name)[name];
            }
            set
            {
                _dict[name] = value;
            }
        }

        public IEnumerator<KeyValuePair<SchemaField, string>> GetEnumerator()
        {
            return _bidderFieldList.Select(field => new KeyValuePair<SchemaField, string>(field, this[field.Name])).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Dictionary<string, string> GetResultDict()
        {
            return _dict.ToDictionary(c => c.Key, c => c.Value);
        }
    }
}