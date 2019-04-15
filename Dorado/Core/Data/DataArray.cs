using Dorado.Extensions;
using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Core.Data
{
    [Serializable]
    public class DataArray : IXmlSerializable, IEnumerable, ICloneable
    {
        private int _rowsize;//当前定义的行数
        private int _count;
        private int _pagesize;  //每页显示数据
        private bool _reading;

        public DataArray()
            : this("mlist", 20)
        {
        }

        public DataArray(string name)
            : this(name, 20)
        {
        }

        public DataArray(string name, int rows)
        {
            Name = name;
            _rowsize = rows;
            Columns = new DataArrayColumns(this);
        }

        public DataArray Union(DataArray arr)
        {
            RowSize = _count + arr._count;
            foreach (DataArrayColumn col in Columns)
            {
                if (!arr.Contains(col.Name)) continue;
                for (int i = 0; i < arr._count; i++)
                {
                    col[i + _count].Set(arr[col.Name, i].ToObject());
                }
            }
            _count += arr._count;
            return this;
        }

        public DataArray Join(DataArray arr, string field)
        {
            if (!arr.Contains(field)) return this;

            if (RowSize < arr.RowSize) RowSize = arr.RowSize;
            else if (RowSize > arr.RowSize) arr.RowSize = RowSize;

            DataArrayColumn col = arr[field];

            if (!Contains(field)) Columns.Add(field);

            DataArrayColumn tmp = this[field];
            tmp._type = col._type;
            tmp._data = col._data;
            return this;
        }

        public DataArray Join(DataArray arr)
        {
            Join(arr, false);
            return this;
        }

        public DataArray Join(DataArray arr, bool overWrite)
        {
            if (RowSize < arr.RowSize) RowSize = arr.RowSize;
            else if (RowSize > arr.RowSize) arr.RowSize = RowSize;

            foreach (DataArrayColumn col in arr.Columns)
            {
                if (!Contains(col.Name)) Columns.Add(col);
                else if (!overWrite) continue;

                DataArrayColumn tmp = this[col.Name];
                tmp._type = col._type;
                tmp._data = col._data;
            }
            return this;
        }

        public string Name { get; set; }

        public int Cursor { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get { return _pagesize; }
            set
            {
                _pagesize = value;
                RowSize = value;
            }
        }

        public int MaxCount { get; set; }

        public int MaxPage
        {
            get
            {
                if (MaxCount == 0) return 0;
                int size = (_pagesize == 0 ? 20 : _pagesize);
                return (MaxCount - 1) / size + 1;
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                if (value >= 0 && _count >= value) _count = value;
            }
        }

        public DataArrayColumns Columns { get; private set; }

        public DataArrayRows Rows
        {
            get { return new DataArrayRows(this); }
        }

        public DataArrayColumn this[string name]
        {
            get
            {
                return Columns[name];
            }
        }

        public DataArrayColumn this[string name, int row]
        {
            get
            {
                return Columns[name, row];
            }
        }

        public bool Contains(string name)
        {
            return Columns.Contains(name);
        }

        public bool IsDataEmpty(string name)
        {
            return !Contains(name) || string.IsNullOrWhiteSpace(this[name].ToString());
        }

        public bool IsDataEqual(string name, string value)
        {
            return this[name].ToString() == value;
        }

        public bool IsMobile(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsMobile.IsMatch(this[name].ToString());
        }

        public bool IsEmail(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsEmail.IsMatch(this[name].ToString());
        }

        public bool IsWebUrl(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsWebUrl.IsMatch(this[name].ToString());
        }

        public bool IsNumeric(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsNumeric.IsMatch(this[name].ToString());
        }

        public bool IsAlpha(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsAlpha.IsMatch(this[name].ToString());
        }

        public bool IsAlphaNumeric(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsAlphaNumeric.IsMatch(this[name].ToString());
        }

        public bool IsNotNumber(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsNotNumber.IsMatch(this[name].ToString());
        }

        public bool IsGuid(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsGuid.IsMatch(this[name].ToString());
        }

        public bool IsIDCard(string name)
        {
            return !IsDataEmpty(name) && RegularExpressions.IsIDCard.IsMatch(this[name].ToString());
        }

        internal int RowSize
        {
            get
            {
                return _rowsize;
            }
            set
            {
                if (value <= _rowsize) return;
                _rowsize = value > 2 * _rowsize ? value : 2 * _rowsize;
                foreach (DataArrayColumn col in Columns)
                {
                    Array tmp = Array.CreateInstance(col.Type, _rowsize);
                    ((Array)col.Data).CopyTo(tmp, 0);
                    col.Data = tmp;
                }
            }
        }

        public void AddPara(object para)
        {
            FieldInfo[] info = para.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo fld in info)
            {
                string name = fld.Name.ToLower();
                if (!Contains(name)) Columns.Add(name, fld.FieldType);
                if (fld.FieldType == typeof(string))
                    this[name].Set(fld.GetValue(para).ToString().Replace("\"", "&quot;"));
                else
                    this[name].Set(fld.GetValue(para));
            }
        }

        public string PageTurn(string head)
        {
            return PageTurn(head, 5, null);
        }

        public string PageTurn(string head, int scale)
        {
            return PageTurn(head, scale, null);
        }

        public string PageTurn(string head, object para)
        {
            return PageTurn(head, 5, para);
        }

        public string PageTurn(string head, int scale, object para)
        {
            return PageTurn(head, scale, para, null);
        }

        public string PageTurn(string head, int scale, object para, string key)
        {
            if (MaxCount <= 0) return "没有找到相关记录";
            int MaxPage = (MaxCount - 1) / PageSize + 1;
            if (MaxPage == 1) return "<font color=#ff0000>共 1 页</font>";

            string url = string.Empty;
            string query = string.Empty;

            if (key == null) key = "page";
            else query = key.Substring(0, 1);

            if (para != null) url = WebExtensions.UrlTail(para, key);

            if (Page <= 0) Page = 1;
            else if (Page > MaxPage) Page = MaxPage;

            int prev = Page - scale < 1 ? 1 : Page - scale;
            int next = Page + scale > MaxPage ? MaxPage : Page + scale;

            StringBuilder sb = new StringBuilder();

            if (Page > 1) sb.Append("<a href=\"" + head + "-" + query + (Page - 1).ToString() + url + "\">上一页</a> ");
            for (int i = prev; i < Page; i++)
            {
                sb.Append("<a href=\"" + head + "-" + query + i.ToString() + url + "\">[" + i.ToString() + "]</a> ");
            }
            sb.Append("<span style='color:red;font-weight:bold'>" + Page.ToString() + "</span> ");
            for (int i = Page + 1; i <= next; i++)
            {
                sb.Append("<a href=\"" + head + "-" + query + i.ToString() + url + "\">[" + i.ToString() + "]</a> ");
            }
            if (Page < MaxPage) sb.Append("<a href=\"" + head + "-" + query + (Page + 1).ToString() + url + "\">下一页</a> ");

            return sb.ToString();
        }

        public string PageTurn(string head, int page, int maxPage, int scale, object para)
        {
            if (maxPage <= 0) return "没有找到相关记录";
            if (maxPage == 1) return "<font color=#ff0000>共 1 页</font>";

            string url = "";

            if (para != null) url = WebExtensions.UrlQuery(para, "page");
            if (url != "") url += "&";

            if (page <= 0) page = 1;
            else if (page > maxPage) page = maxPage;

            int prev = page - scale < 1 ? 1 : page - scale;
            int next = page + scale > maxPage ? maxPage : page + scale;

            string addr = head + "?";
            StringBuilder sb = new StringBuilder();

            if (page > 1) sb.Append("<a href=\"" + addr + url + "page=" + (page - 1).ToString() + "\">上一页</a> ");
            for (int i = prev; i < page; i++)
            {
                sb.Append("<a href=\"" + addr + url + "page=" + i.ToString() + "\">[" + i.ToString() + "]</a> ");
            }
            sb.Append("<span style='color:red;font-weight:bold'>" + page.ToString() + "</span> ");
            for (int i = page + 1; i <= next; i++)
            {
                sb.Append("<a href=\"" + addr + url + "page=" + i.ToString() + "\">[" + i.ToString() + "]</a> ");
            }
            if (page < maxPage) sb.Append("<a href=\"" + addr + url + "page=" + (page + 1).ToString() + "\">下一页</a> ");

            return sb.ToString();
        }

        public void Rename(string old, string name)
        {
            Columns.Rename(old, name);
        }

        public bool ChangeType(string name, Type type)
        {
            DataArrayColumn col = Columns[name];
            if (col != null) return col.ChangeType(type);
            return false;
        }

        public bool Eof
        {
            get
            {
                if (Cursor >= _count || _count == 0) return true;
                return false;
            }
        }

        public bool Read()
        {
            if (_reading)
            {
                if (Cursor >= _count - 1)
                {
                    _reading = false;
                    return false;
                }
                Cursor++;
            }
            else
            {
                if (_count == 0) return false;
                Cursor = 0;
                _reading = true;
            }
            return true;
        }

        public void MoveFirst()
        {
            Cursor = 0;
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool IsEmpty
        {
            get
            {
                return _count <= 0;
            }
        }

        public void AddRow()
        {
            Cursor = _count;
            RowSize = ++_count;
        }

        public void Set(string name, bool value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, sbyte value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, byte value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, ushort value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, short value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, uint value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, int value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, ulong value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, long value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, double value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, float value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, DateTime value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Set(string name, object value)
        {
            for (int i = 0; i < _count; i++)
            {
                this[name, i].Set(value);
            }
        }

        public void Replace(string name, string old, string value)
        {
            this[name].Replace(old, value);
        }

        public void Replace(string name, string field, Func<object, object> tran)
        {
            Replace(name, field, typeof(string), tran);
        }

        public void Replace(string name, string field, Type type, Func<object, object> tran)
        {
            if (!Contains(field)) Columns.Add(field, type);
            for (int i = 0; i < Count; i++)
            {
                this[field, i].Set(tran(this[name, i].ToObject()));
            }
        }

        public void Replace(string name, Func<object, object> tran)
        {
            this[name].Replace(tran);
        }

        public void ReplaceLine(string name, Func<DataArray, object> tran)
        {
            this[name].Replace(tran);
        }

        public void ReplaceReg(string name, string regStr, string value)
        {
            this[name].ReplaceReg(regStr, value);
        }

        public void ReplaceReg(string name, string regStr, MatchEvaluator func)
        {
            this[name].ReplaceReg(regStr, func);
        }

        public void HighLight(string name, string value)
        {
            this[name].HighLight(value);
        }

        public new string ToString()
        {
            return ToString(Name);
        }

        public string ToString(string name)
        {
            if (_count == 0) return "T" + (name == null ? ".data" : "." + name) + "={};";
            StringBuilder ret = new StringBuilder((name == null ? "T.data" : "T." + name) + "=");
            StringBuilder tmp = new StringBuilder();

            if (_pagesize == 0)
            {
                ret.Append("{");
                foreach (DataArrayColumn col in Columns)
                {
                    if (tmp.Length > 0) tmp.Append(",");
                    tmp.Append(col.Name + ":" + col.ToSafeString());
                }
                ret.Append(tmp);
                ret.Append("};");
            }
            else
            {
                ret.Append("{");
                foreach (DataArrayColumn col in Columns)
                {
                    if (tmp.Length > 0) tmp.Append(",");
                    tmp.Append(col.Name + ":[");
                    for (int i = 0; i < _count; i++)
                    {
                        if (i > 0) tmp.Append(",");
                        tmp.Append(col[i].ToSafeString());
                    }
                    tmp.Append("]");
                }
                ret.Append(tmp);
                ret.Append("};");
                ret.Append("T.page=" + Page.ToString() + ";");
                ret.Append("T.pagesize=" + _pagesize.ToString() + ";");
                ret.Append("T.maxcount=" + MaxCount.ToString() + ";");
                int maxpage = MaxCount == 0 ? 0 : (MaxCount - 1) / _pagesize + 1;
                ret.Append("T.maxpage=" + maxpage.ToString() + ";");
                ret.Append("T.pageturn=\"" + Page.ToString() + "|" + maxpage.ToString() + "\";");
            }

            return ret.ToString();
        }

        public string ToJson()
        {
            return ToJson(Name);
        }

        public string ToJson(string name)
        {
            if (_count == 0) return "T" + (name == null ? ".data" : "." + name) + "=[];";
            StringBuilder ret = new StringBuilder((name == null ? "T.data" : "T." + name) + "=[");

            for (int i = 0; i < _count; i++)
            {
                StringBuilder tmp = new StringBuilder();
                ret.Append("{");
                foreach (DataArrayColumn col in Columns)
                {
                    if (tmp.Length > 0) tmp.Append(",");
                    tmp.Append("\"" + col.Name + "\"" + ":" + col[i].ToSafeString());
                }
                ret.Append(tmp);
                ret.Append("}");
                if (i < _count - 1)
                    ret.Append(",");
            }
            ret.Append("]");
            return ret.ToString();
        }

        public string ToCsv()
        {
            if (_count == 0) return string.Empty;
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < _count; i++)
            {
                if (i > 0) ret.Append(",");
                foreach (DataArrayColumn col in Columns)
                {
                    ret.Append("\"" + col[i].ToString().Replace("\"", "\"\"") + "\"");
                }
                ret.Append("\r\n");
            }

            return ret.ToString();
        }

        #region IXmlSerializable   成员

        public void WriteXml(XmlWriter writer)
        {
            WriteXml(writer, true);
        }

        public void WriteXml(XmlWriter writer, bool close)
        {
            if (Name != null)
                writer.WriteStartElement(Name);
            else
                writer.WriteStartElement("none");

            if (_pagesize > 0)
            {
                if (Page > 1) writer.WriteAttributeString("page", Page.ToString());
                if (_pagesize > 0) writer.WriteAttributeString("pagesize", _pagesize.ToString());
                if (MaxCount > 0) writer.WriteAttributeString("maxcount", MaxCount.ToString());
            }

            while (Read())
            {
                writer.WriteStartElement("row");
                foreach (DataArrayColumn col in Columns)
                {
                    writer.WriteAttributeString(col.Name, col.ToString());
                }
                writer.WriteEndElement();
            }
            if (close) writer.Close();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name != "row")
                        {
                            if (reader.Name != "none" && reader.Name != string.Empty) Name = reader.Name;
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name.Equals("page")) Page = DataTypeExtensions.ToInt(reader.Value);
                                else if (reader.Name.Equals("pagesize")) PageSize = DataTypeExtensions.ToInt(reader.Value);
                                else if (reader.Name.Equals("maxcount")) MaxCount = DataTypeExtensions.ToInt(reader.Value);
                            }
                        }
                        else
                        {
                            AddRow();
                            while (reader.MoveToNextAttribute())
                            {
                                if (!Columns.Contains(reader.Name)) Columns.Add(reader.Name);
                                this[reader.Name].Set(reader.Value.Replace("\\r", "\r").Replace("\\n", "\n"));
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        reader.ReadEndElement();
                        Cursor = 0;
                        return;
                }
            }
        }

        public void ReadXml(string xmlStr)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace;
            int pos = xmlStr.IndexOf(">");
            if (pos == -1) return;
            string pre = xmlStr.Substring(0, pos);
            string name = Regex.Match(pre, @"<(\w+)\s", options).Groups[1].Value;
            if (name != "none" && name != string.Empty) Name = name;
            MatchCollection ma = Regex.Matches(pre, @"(\w+)\s*=""([^""]*)""", options);
            foreach (Match m in ma)
            {
                if (m.Groups[1].Value.Equals("page")) Page = DataTypeExtensions.ToInt(m.Groups[2].Value);
                else if (m.Groups[1].Value.Equals("pagesize")) PageSize = DataTypeExtensions.ToInt(m.Groups[2].Value);
                else if (m.Groups[1].Value.Equals("maxcount")) MaxCount = DataTypeExtensions.ToInt(m.Groups[2].Value);
            }
            ma = Regex.Matches(xmlStr.Substring(pos + 1), @"<row[^>]*?\/>", options);
            foreach (Match m in ma)
            {
                AddRow();
                MatchCollection mb = Regex.Matches(m.Value, @"(\w+)\s*=""([^""]*)""", options);
                foreach (Match n in mb)
                {
                    string nodeName = n.Groups[1].Value;
                    string nodeValue = XmlExtensions.Outbox(n.Groups[2].Value);
                    if (!Columns.Contains(nodeName)) Columns.Add(nodeName);
                    this[nodeName].Set(nodeValue);
                }
            }
            Cursor = 0;
        }

        #endregion IXmlSerializable   成员

        #region ICloneable 成员

        public object Clone()
        {
            DataArray data = new DataArray(Name, RowSize)
            {
                Name = Name,
                Columns = (DataArrayColumns)Columns.Clone(),
                Page = Page,
                _pagesize = _pagesize,
                MaxCount = MaxCount,
                _count = _count,
                Cursor = 0
            };
            return data;
        }

        #endregion ICloneable 成员

        #region IEnumerable 成员

        public IEnumerator GetEnumerator()
        {
            return new DataArrayRows(this);
        }

        #endregion IEnumerable 成员
    }
}