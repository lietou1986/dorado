using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Dorado.Extensions;

namespace Dorado.Core.FastData
{
    public delegate void TableDelegate(DataArray data);

    public delegate object TransDelegate(object value);

    public delegate object TransLineDelegate(DataArray data);

    [Serializable]
    public class DataArray : IXmlSerializable, IEnumerable, ICloneable
    {
        private int _rowsize;//当前定义的行数
        private int _count;
        private string _name;//名称
        private int _cursor;//当前下标
        private int _page = 1;//页数
        private int _pagesize;	//每页显示数据
        private int _maxcount;	//总数
        private DataArrayColumns _cols;
        private bool _reading;

        public DataArray()
            : this(null, 20)
        {
        }

        public DataArray(string name)
            : this(name, 20)
        {
        }

        public DataArray(string name, int rows)
        {
            _name = name;
            _rowsize = rows;
            _cols = new DataArrayColumns(this);
        }

        public void Union(DataArray arr)
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
        }

        public void Join(DataArray arr, string field)
        {
            if (!arr.Contains(field)) return;

            if (RowSize < arr.RowSize) RowSize = arr.RowSize;
            else if (RowSize > arr.RowSize) arr.RowSize = RowSize;

            DataArrayColumn col = arr[field];

            if (!Contains(field)) Columns.Add(field);

            DataArrayColumn tmp = this[field];
            tmp._type = col._type;
            tmp._data = col._data;
        }

        public void Join(DataArray arr)
        {
            Join(arr, false);
        }

        public void Join(DataArray arr, bool overWrite)
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
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Cursor
        {
            get { return _cursor; }
            set
            {
                _cursor = value;
            }
        }

        public int Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public int PageSize
        {
            get { return _pagesize; }
            set
            {
                _pagesize = value;
                RowSize = value;
            }
        }

        public int MaxCount
        {
            get { return _maxcount; }
            set { _maxcount = value; }
        }

        public int MaxPage
        {
            get
            {
                if (_maxcount == 0) return 0;
                int size = (_pagesize == 0 ? 20 : _pagesize);
                return (_maxcount - 1) / size + 1;
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

        public DataArrayColumns Columns
        {
            get
            {
                return _cols;
            }
        }

        public DataArrayRows Rows
        {
            get { return new DataArrayRows(this); }
        }

        public DataArrayColumn this[string name]
        {
            get
            {
                return _cols[name];
            }
        }

        public DataArrayColumn this[string name, int row]
        {
            get
            {
                return _cols[name, row];
            }
        }

        public bool Contains(string name)
        {
            return _cols.Contains(name);
        }

        public bool IsDataEmpty(string name)
        {
        	return !Contains(name)||string.IsNullOrWhiteSpace(this[name].ToString());
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
                foreach (DataArrayColumn col in _cols)
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
            _cols.Rename(old, name);
        }

        public bool ChangeType(string name, Type type)
        {
            DataArrayColumn col = _cols[name];
            if (col != null) return col.ChangeType(type);
            return false;
        }

        public bool Eof
        {
            get
            {
                if (_cursor >= _count || _count == 0) return true;
                return false;
            }
        }

        public bool Read()
        {
            if (_reading)
            {
                if (_cursor >= _count - 1)
                {
                    _reading = false;
                    return false;
                }
                _cursor++;
            }
            else
            {
                if (_count == 0) return false;
                _cursor = 0;
                _reading = true;
            }
            return true;
        }

        public void MoveFirst()
        {
            _cursor = 0;
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
            _cursor = _count;
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

        public void Replace(string name, string field, TransDelegate tran)
        {
            Replace(name, field, typeof(string), tran);
        }

        public void Replace(string name, string field, Type type, TransDelegate tran)
        {
            if (!Contains(field)) Columns.Add(field, type);
            for (int i = 0; i < Count; i++)
            {
                this[field, i].Set(tran(this[name, i].ToObject()));
            }
        }

        public void Replace(string name, TransDelegate tran)
        {
            this[name].Replace(tran);
        }

        public void ReplaceLine(string name, TransLineDelegate tran)
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
            return ToString(_name);
        }

        public string ToString(string name)
        {
            if (_count == 0) return "T" + (name == null ? "" : "." + name) + "={};";
            StringBuilder ret = new StringBuilder((name == null ? "T" : "T." + name) + "=");
            StringBuilder tmp = new StringBuilder();

            if (_pagesize == 0)
            {
                ret.Append("{");
                foreach (DataArrayColumn col in _cols)
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
                foreach (DataArrayColumn col in _cols)
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
                ret.Append("T.page=" + _page.ToString() + ";");
                ret.Append("T.pagesize=" + _pagesize.ToString() + ";");
                ret.Append("T.maxcount=" + _maxcount.ToString() + ";");
                int maxpage = _maxcount == 0 ? 0 : (_maxcount - 1) / _pagesize + 1;
                ret.Append("T.maxpage=" + maxpage.ToString() + ";");
                ret.Append("T.pageturn=\"" + _page.ToString() + "|" + maxpage.ToString() + "\";");
            }

            return ret.ToString();
        }

        public string ToCsv()
        {
            if (_count == 0) return string.Empty;
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < _count; i++)
            {
                if (i > 0) ret.Append(",");
                foreach (DataArrayColumn col in _cols)
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
            if (_name != null)
                writer.WriteStartElement(_name);
            else
                writer.WriteStartElement("none");

            if (_pagesize > 0)
            {
                if (_page > 1) writer.WriteAttributeString("page", _page.ToString());
                if (_pagesize > 0) writer.WriteAttributeString("pagesize", _pagesize.ToString());
                if (_maxcount > 0) writer.WriteAttributeString("maxcount", _maxcount.ToString());
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
                                if (!_cols.Contains(reader.Name)) _cols.Add(reader.Name);
                                this[reader.Name].Set(reader.Value.Replace("\\r", "\r").Replace("\\n", "\n"));
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        reader.ReadEndElement();
                        _cursor = 0;
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
                    if (!_cols.Contains(nodeName)) _cols.Add(nodeName);
                    this[nodeName].Set(nodeValue);
                }
            }
            _cursor = 0;
        }

        #endregion IXmlSerializable   成员

        #region ICloneable 成员

        public object Clone()
        {
            DataArray data = new DataArray(_name, RowSize)
            {
                _name = _name,
                _cols = (DataArrayColumns) _cols.Clone(),
                _page = _page,
                _pagesize = _pagesize,
                _maxcount = _maxcount,
                _count = _count,
                _cursor = 0
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