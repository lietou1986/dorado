using Dorado.Core.Data;
using Dorado.Extensions;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.Web.FastViewEngine
{
    public class PatReader
    {
        internal class NodeInfo
        {
            internal NodeInfo Next;

            internal virtual string ToString(DataArrayList list)
            {
                return string.Empty;
            }
        }

        internal sealed class TextInfo : NodeInfo
        {
            internal string Text;

            internal TextInfo(string Content)
            {
                Text = Content;
            }

            internal override string ToString(DataArrayList list)
            {
                return Text;
            }
        }

        internal sealed class ElementInfo : NodeInfo
        {
            internal string Name;
            internal string Column;
            internal int Max;
            internal string Text = string.Empty;
            internal bool NewLine;

            internal override string ToString(DataArrayList list)
            {
                if (list == null) return Text;
                DataArray data = list[Name];
                if (data == null || data.Eof) return Text;

                DataArrayColumn col = data[Column];

                string content = (col == null ? Text : col.ToString(Max));

                if (NewLine && !Name.Equals("root")) data.Cursor++;

                return content;
            }
        }

        internal sealed class CaseInfo : NodeInfo
        {
            internal string Name;
            internal string Column;
            internal string Op;
            internal bool IsInt = true;
            internal string Value;
            internal NodeInfo Inner = new NodeInfo();
            internal NodeInfo Other = null;

            internal override string ToString(DataArrayList list)
            {
                if (Op == null)
                {
                    StringBuilder sb = new StringBuilder();
                    NodeInfo info = Inner;
                    while (info != null)
                    {
                        sb.Append(info.ToString(list));
                        info = info.Next;
                    }
                    return sb.ToString();
                }
                DataArray data = list[Name];
                if (data == null || data.Eof)
                {
                    if (Other != null) return Other.ToString(list);
                    else return string.Empty;
                }

                DataArrayColumn col = data[Column];
                if (col == null)
                {
                    if (Other != null) return Other.ToString(list);
                    else return string.Empty;
                }

                bool fit = false;
                if (IsInt)
                {
                    int value1 = col.ToInt();
                    int value2 = DataTypeExtensions.ToInt(Value);
                    switch (Op)
                    {
                        case "=":
                        case "==":
                            fit = (value1 == value2);
                            break;

                        case "<>":
                        case "!=":
                            fit = (value1 != value2);
                            break;

                        case ">":
                            fit = (value1 > value2);
                            break;

                        case "<":
                            fit = (value1 < value2);
                            break;

                        case ">=":
                            fit = (value1 >= value2);
                            break;

                        case "<=":
                            fit = (value1 <= value2);
                            break;
                    }
                }
                else
                {
                    string value1 = col.ToString().Trim();
                    string value2 = Value.Trim();
                    switch (Op)
                    {
                        case "=":
                        case "==":
                            fit = (value1 == value2);
                            break;

                        case "<>":
                        case "!=":
                            fit = (value1 != value2);
                            break;

                        case ">":
                            fit = (value1.CompareTo(value2) > 0);
                            break;

                        case "<":
                            fit = (value1.CompareTo(value2) < 0);
                            break;

                        case ">=":
                            fit = (value1.CompareTo(value2) >= 0);
                            break;

                        case "<=":
                            fit = (value1.CompareTo(value2) <= 0);
                            break;
                    }
                }
                if (fit)
                {
                    StringBuilder sb = new StringBuilder();
                    NodeInfo info = Inner;
                    while (info != null)
                    {
                        sb.Append(info.ToString(list));
                        info = info.Next;
                    }
                    return sb.ToString();
                }
                if (Other != null) return Other.ToString(list);
                else return string.Empty;
            }
        }

        internal sealed class LoopInfo : NodeInfo
        {
            internal string Name;
            internal string Key;
            internal string Group;
            internal string Column;
            internal bool StepBack = false;
            internal int Line;
            internal NodeInfo Inner = new NodeInfo();

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                NodeInfo info = Inner;
                while (info != null)
                {
                    sb.Append(info.ToString());
                    info = info.Next;
                }
                return sb.ToString();
            }

            internal override string ToString(DataArrayList list)
            {
                DataArray data = list[Name];
                StringBuilder sb = new StringBuilder();
                if (Group != null)
                {
                    object pv = null;
                    DataArray pdata = list[Group];
                    if (pdata != null)
                    {
                        DataArrayColumn pcol = null;
                        if (StepBack)
                        {
                            if (pdata.Cursor - 1 < pdata.Count) pcol = pdata[Column, pdata.Cursor - 1];
                        }
                        else
                        {
                            if (!pdata.Eof) pcol = pdata[Column];
                        }
                        if (pcol != null) pv = pcol.ToObject();
                    }
                    int count = 0;
                    if (data != null)
                    {
                        while (!data.Eof && ((Line == 0 && count < 100) || count < Line))
                        {
                            DataArrayColumn col = data[Key];
                            if (col == null) break;
                            if (!col.ToObject().Equals(pv)) break;

                            NodeInfo info = Inner;
                            while (info != null)
                            {
                                if (info is ElementInfo)
                                {
                                    ElementInfo einfo = (ElementInfo)info;
                                    if (einfo.Name == Name)
                                    {
                                        col = data[Column];
                                        if (col == null) sb.Append(einfo.Text);
                                        else sb.Append(col.ToString(einfo.Max));
                                        if (einfo.NewLine) data.Cursor++;
                                    }
                                    else
                                    {
                                        sb.Append(info.ToString(list));
                                    }
                                }
                                else
                                {
                                    sb.Append(info.ToString(list));
                                }
                                info = info.Next;
                            }
                            count++;
                        }
                    }
                    if (count < Line)
                    {
                        StringBuilder tmp = new StringBuilder();
                        NodeInfo info = Inner;
                        while (info != null)
                        {
                            if (info is ElementInfo && ((ElementInfo)info).Name == Name)
                            {
                                tmp.Append(((ElementInfo)info).Text);
                            }
                            else
                            {
                                tmp.Append(info.ToString(list));
                            }
                            info = info.Next;
                        }
                        while (count < Line)
                        {
                            sb.Append(tmp.ToString());
                            count++;
                        }
                    }
                }
                else
                {
                    int count = 0;
                    if (data != null)
                    {
                        while (!data.Eof && ((Line == 0 && count < 100) || count < Line))
                        {
                            NodeInfo info = Inner;
                            while (info != null)
                            {
                                sb.Append(info.ToString(list));
                                info = info.Next;
                            }
                            count++;
                        }
                    }
                    if (count < Line)
                    {
                        StringBuilder tmp = new StringBuilder();
                        NodeInfo info = Inner;
                        while (info != null)
                        {
                            tmp.Append(info.ToString(list));
                            info = info.Next;
                        }
                        while (count < Line)
                        {
                            sb.Append(tmp.ToString());
                            count++;
                        }
                    }
                }
                return sb.ToString();
            }
        }

        internal sealed class AvNodeInfo : NodeInfo
        {
            internal int Block;
            internal string Column;
            internal int Max;
            internal string Text = string.Empty;
            internal bool NewLine;

            internal override string ToString(DataArrayList list)
            {
                if (list == null) return Text;
                DataArray data = list["av"];
                if (data == null || data.Eof || data["block"].ToInt() != Block) return Text;

                DataArrayColumn col = data[Column];

                string content = col.ToString(Max);

                if (NewLine) data.Cursor++;

                return content;
            }
        }

        internal sealed class AvInfo : NodeInfo
        {
            internal int Block;
            private NodeInfo head;
            private NodeInfo tail;

            internal void Add(NodeInfo info)
            {
                if (head == null) tail = head = info;
                else tail = tail.Next = info;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                NodeInfo info = head;
                while (info != null)
                {
                    sb.Append(info.ToString(null));
                    info = info.Next;
                }
                return sb.ToString();
            }

            internal override string ToString(DataArrayList list)
            {
                DataArray data = list["av"];
                if (data == null) return ToString();

                if (data.Eof || data["block"].ToInt() != Block)
                {
                    int i = 0;
                    for (; i < data.Count; i++)
                    {
                        if (data["block", i].ToInt() == Block)
                        {
                            data.Cursor = i;
                            break;
                        }
                    }
                    if (i >= data.Count) return ToString();
                }
                StringBuilder sb = new StringBuilder();
                NodeInfo info = head;
                while (info != null)
                {
                    sb.Append(info.ToString(list));
                    info = info.Next;
                }
                return sb.ToString();
            }
        }

        private NodeInfo head = new NodeInfo();

        public PatReader(System.IO.StreamReader reader)
        {
            Parse(head, reader.ReadToEnd());
        }

        public PatReader(System.IO.TextReader reader)
        {
            Parse(head, reader.ReadToEnd());
        }

        public PatReader(string url)
        {
            Parse(head, EasyFile.Read(url));
        }

        public override string ToString()
        {
            return ToString(new DataArrayList());
        }

        public string ToString(DataArrayList list)
        {
            StringBuilder sb = new StringBuilder();
            NodeInfo info = head;
            while (info != null)
            {
                sb.Append(info.ToString(list));
                info = info.Next;
            }
            return sb.ToString();
        }

        public void Output(TextWriter writer, DataArrayList list)
        {
            NodeInfo info = head;
            while (info != null)
            {
                writer.Write(info.ToString(list));
                info = info.Next;
            }
            writer.Flush();
            list = null;
        }

        private void Parse(NodeInfo info, string Content)
        {
            while (Content.Length > 0)
            {
                int pos = Content.IndexOf('[');	//查找[
                if (pos == -1)
                {
                    info.Next = new TextInfo(Content);
                    return;
                }

                int begin = pos;

                pos = Content.IndexOf(']', pos);	//查找]
                if (pos == -1)
                {
                    info.Next = new TextInfo(Content);
                    return;
                }

                int end = pos + 1;

                string Tag = Content.Substring(begin, end - begin);
                while ((pos = Tag.IndexOf('[', 1)) > -1)
                {
                    Tag = Tag.Substring(pos);
                    begin += pos;
                }
                while (end < Content.Length && (Content[end] == '\r' || Content[end] == '\n'))
                {
                    end++;
                }

                if (begin > 0) info = info.Next = new TextInfo(Content.Substring(0, begin));

                if (Tag.IndexOf("case", 1) == 1)   //如果是Case
                {
                    Match ma = Regex.Match(Tag, @"case(\.(\w+))?\s*((\w+)\.(\w+)\s*([!=><]+)\s*("")?(\w*))?");
                    string name = ma.Groups[1].Value;
                    int posend = Content.IndexOf("[/case" + name + "]", end);
                    CaseInfo tmp = new CaseInfo();
                    info = info.Next = tmp;
                    while (true)
                    {
                        if (ma.Groups[4].Value != string.Empty) tmp.Name = ma.Groups[4].Value;
                        if (ma.Groups[5].Value != string.Empty) tmp.Column = ma.Groups[5].Value;
                        if (ma.Groups[6].Value != string.Empty) tmp.Op = ma.Groups[6].Value;
                        if (ma.Groups[7].Value != string.Empty) tmp.IsInt = false;
                        tmp.Value = ma.Groups[8].Value;
                        pos = Content.IndexOf("[case" + name, end);
                        if (pos == -1 || pos >= posend)
                        {
                            Parse(tmp.Inner, Content.Substring(end, posend - end));
                            break;
                        }
                        Parse(tmp.Inner, Content.Substring(end, pos - end));
                        tmp.Other = new CaseInfo();
                        tmp = (CaseInfo)tmp.Other;
                        end = Content.IndexOf("]", pos) + 1;
                        ma = Regex.Match(Content.Substring(pos, end - pos),
                            @"case(\.(\w+))?\s*((\w+)\.(\w+)\s*([!=><]+)\s*("")?(\w*))?");
                    }
                    end = posend + ("[/case" + name + "]").Length;
                }
                else if (Tag.IndexOf("loop.", 1) == 1)	  //如果是Loop
                {
                    string[] list = Regex.Replace(Tag.Substring(1, Tag.Length - 2), @"\s*=\s*", "=").Split(' ');
                    LoopInfo tmp = new LoopInfo();
                    info = info.Next = tmp;
                    foreach (string s in list)
                    {
                        if (s == string.Empty) continue;
                        if (s.IndexOf("loop.") > -1) tmp.Name = s.Substring(5);
                        else if (s == "stepback") tmp.StepBack = true;
                        else if (s.IndexOf("line=") > -1) tmp.Line = DataTypeExtensions.ToInt(s.Substring(5));
                        else if (s.IndexOf("=") > -1)
                        {
                            int pp = s.IndexOf("=");
                            tmp.Key = s.Substring(0, pp);
                            pos = s.IndexOf(".", pp);
                            tmp.Group = s.Substring(pp + 1, pos - pp - 1);
                            tmp.Column = s.Substring(pos + 1);
                        }
                    }
                    pos = Content.IndexOf("[/loop." + tmp.Name + "]", end);
                    Parse(tmp.Inner, Content.Substring(end, pos - end));
                    end = pos + ("[/loop." + tmp.Name + "]").Length;
                }
                else if (Tag.IndexOf("av.", 1) == 1)   //广告
                {
                    string[] list = Tag.Substring(1, Tag.Length - 2).Split('.');
                    AvInfo tmp = new AvInfo();
                    info = info.Next = tmp;
                    tmp.Block = DataTypeExtensions.ToInt(list[1]);
                    pos = Content.IndexOf("[/av." + tmp.Block.ToString() + "]", end);
                    ParseAV(tmp, Content.Substring(end, pos - end));
                    end = pos + ("[/av." + tmp.Block.ToString() + "]").Length;
                }
                else //普通结点
                {
                    Match ma = Regex.Match(Tag, @"(\w+)\.(\w+)\s*(max\s*=\s*(\d+))?\s*(/)?(/)?");
                    if (ma.Groups[0].Value == string.Empty)
                    {
                        info = info.Next = new TextInfo(Tag);
                    }
                    else
                    {
                        ElementInfo tmp = new ElementInfo();
                        tmp.Name = ma.Groups[1].Value;
                        tmp.Column = ma.Groups[2].Value;
                        bool contain = true;
                        tmp.Max = DataTypeExtensions.ToInt(ma.Groups[4].Value);
                        if (ma.Groups[5].Value != string.Empty) contain = false;
                        if (ma.Groups[6].Value != string.Empty) tmp.NewLine = true;

                        if (contain)
                        {
                            pos = Content.IndexOf("/" + tmp.Name + "." + tmp.Column + "]", end);
                            if (pos == -1)
                            {
                                info = info.Next = new TextInfo(Tag);
                            }
                            else
                            {
                                if (Content[pos - 1] == '/')
                                {
                                    tmp.NewLine = true;
                                    tmp.Text = Content.Substring(end, pos - end - 2);
                                }
                                else tmp.Text = Content.Substring(end, pos - end - 1);
                                info = info.Next = tmp;
                                end = pos + ("/" + tmp.Name + "." + tmp.Column + "]").Length;
                            }
                        }
                        else
                        {
                            info = info.Next = tmp;
                        }
                    }
                }
                Content = Content.Substring(end);
            }
        }

        private void ParseAV(AvInfo info, string Content)
        {
            while (Content.Length > 0)
            {
                int pos = Content.IndexOf('[');	//查找[
                if (pos == -1)
                {
                    info.Add(new TextInfo(Content));
                    return;
                }

                int begin = pos;

                pos = Content.IndexOf(']', pos);	//查找]
                if (pos == -1)
                {
                    info.Add(new TextInfo(Content));
                    return;
                }

                int end = pos + 1;

                string Tag = Content.Substring(begin, end - begin);
                while ((pos = Tag.IndexOf('[', 1)) > -1)
                {
                    Tag = Tag.Substring(pos);
                    begin += pos;
                }

                if (begin > 0) info.Add(new TextInfo(Content.Substring(0, begin)));

                Match ma = Regex.Match(Tag, @"(text|pic|url)\s*(max\s*=\s*(\d+))?\s*(/)?(/)?");
                string name = ma.Groups[1].Value;
                if (name == string.Empty)
                {
                    info.Add(new TextInfo(Tag));
                }
                else
                {
                    AvNodeInfo tmp = new AvNodeInfo();
                    tmp.Block = info.Block;
                    tmp.Column = ma.Groups[1].Value;
                    bool contain = true;
                    tmp.Max = DataTypeExtensions.ToInt(ma.Groups[3].Value);
                    if (ma.Groups[4].Value != string.Empty) contain = false;
                    if (ma.Groups[5].Value != string.Empty) tmp.NewLine = true;

                    if (contain)
                    {
                        pos = Content.IndexOf("/" + tmp.Column + "]", end);
                        if (pos == -1)
                        {
                            info.Add(new TextInfo(Tag));
                        }
                        else
                        {
                            if (Content[pos - 1] == '/')
                            {
                                tmp.NewLine = true;
                                tmp.Text = Content.Substring(end, pos - end - 2);
                            }
                            else tmp.Text = Content.Substring(end, pos - end - 1);
                            info.Add(tmp);
                            end = pos + ("/" + tmp.Column + "]").Length;
                        }
                    }
                    else
                    {
                        info.Add(tmp);
                    }
                }
                Content = Content.Substring(end);
            }
        }
    }
}