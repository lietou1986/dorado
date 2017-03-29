using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Dorado.Core.FastData.DataType;
using Dorado.Extensions;

namespace Dorado.Core.FastData
{
    [Serializable]
    public class DataArrayList : IXmlSerializable, IEnumerable, IEnumerator, ICloneable
    {
        private TreeName _tree = new TreeName();

        public int Count
        {
            get { return _tree.Count; }
        }

        public DataArray Add(string name, DataArray arr)
        {
            arr.Name = name;
            return Add(arr);
        }

        public DataArray Add(DataArray arr)
        {
            if (arr == null) return null;
            arr.Cursor = 0;
            TreeNameNode node = _tree.Add(new TreeNameNode(arr.Name, arr));
            if (node == null) return this[arr.Name];
            return (DataArray)node.Data;
        }

        public DataArrayList Add(DataArrayList list)
        {
            foreach (DataArray arr in list)
            {
                Add(arr);
            }
            return list;
        }

        public bool Contains(string name)
        {
            if (name == null) return false;
            return this[name] != null ? true : false;
        }

        public DataArray this[object value]
        {
            get
            {
                return this[value.ToString()];
            }
        }

        public DataArray this[string name]
        {
            get
            {
                TreeNameNode node = _tree[name];
                if (node == null) return null;
                return (DataArray)node.Data;
            }
        }

        public DataArray Delete(string name)
        {
            TreeNameNode node = _tree.Delete(name);
            if (node == null) return null;
            return (DataArray)node.Data;
        }

        public DataArray Rename(string old, string name)
        {
            TreeNameNode node = _tree.Rename(old, name);
            if (node == null) throw new ApplicationException("从" + old + "改名" + name + "没有成功！");
            return (DataArray)node.Data;
        }

        public void Union(DataArrayList list)
        {
            foreach (DataArray data in this)
            {
                if (list.Contains(data.Name)) data.Union(list[data.Name]);
            }
        }

        #region IXmlSerializable   成员

        public void WriteXml(XmlWriter writer)
        {
            foreach (DataArray data in this)
            {
                data.WriteXml(writer, false);
            }
            writer.Close();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            DataArray tmp = null;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name != "row")
                        {
                            if (reader.Name != "none" && reader.Name != string.Empty)
                            {
                                tmp = new DataArray(reader.Name);
                                Add(tmp);
                            }
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name.Equals("page")) tmp.Page = DataTypeExtensions.ToInt(reader.Value);
                                else if (reader.Name.Equals("pagesize")) tmp.PageSize = DataTypeExtensions.ToInt(reader.Value);
                                else if (reader.Name.Equals("maxcount")) tmp.MaxCount = DataTypeExtensions.ToInt(reader.Value);
                            }
                        }
                        else
                        {
                            tmp.AddRow();
                            while (reader.MoveToNextAttribute())
                            {
                                if (!tmp.Columns.Contains(reader.Name)) tmp.Columns.Add(reader.Name);
                                tmp[reader.Name].Set(reader.Value.Replace("\\r", "\r").Replace("\\n", "\n"));
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        reader.ReadEndElement();
                        tmp.Cursor = 0;
                        return;
                }
            }
        }

        public void ReadXml(string xmlstr)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xmlstr));
            ReadXml(reader);
        }

        #endregion IXmlSerializable   成员

        #region ICloneable 成员

        public object Clone()
        {
            DataArrayList cols = new DataArrayList();
            cols._tree = (TreeName)_tree.Clone();
            return cols;
        }

        #endregion ICloneable 成员

        #region IEnumerator 成员

        public void Reset()
        {
            _tree.Reset();
        }

        public object Current
        {
            get
            {
                TreeNameNode node = (TreeNameNode)_tree.Current;
                if (node == null) return null;
                return (DataArray)node.Data;
            }
        }

        public bool MoveNext()
        {
            return _tree.MoveNext();
        }

        #endregion IEnumerator 成员

        #region IEnumerable 成员

        public IEnumerator GetEnumerator()
        {
            Reset();
            return this;
        }

        #endregion IEnumerable 成员
    }
}