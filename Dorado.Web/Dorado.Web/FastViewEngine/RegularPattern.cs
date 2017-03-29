using Dorado.Core.Data;
using Dorado.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.Web.FastViewEngine
{
    internal enum LoopStatEnum
    {
        无,
        分组子循环之前,
        分组子循环中,
        分组子循环之后
    }

    public class RegularPattern
    {
        public static string Replace(string Content, DataArrayList DataList)
        {
            int Index = 0;
            LoopStatEnum LoopStat = LoopStatEnum.无;
            return Regex.Replace(Read(ref Content, ref Index, null, null, null, ref LoopStat, DataList), @"\s+\r\n", "\r\n");
        }

        private static string Read(ref string Content, int Index, string LoopArray, string LoopGroup, object LoopValue, ref LoopStatEnum LoopStat, DataArrayList DataList)
        {
            return Read(ref Content, ref Index, LoopArray, LoopGroup, LoopValue, ref LoopStat, DataList);
        }

        /// <summary>
        /// 模板分析读取
        /// </summary>
        /// <param name="Content">文本内容</param>
        /// <param name="Index">文本下标</param>
        /// <param name="LoopArray">循环DataArray名称</param>
        /// <param name="LoopGroup">分组列的序号</param>
        /// <param name="LoopValue">分组列的值</param>
        /// <param name="LoopStat">分组循环的状态</param>
        /// <param name="DataList">DataArrayList数据集</param>
        /// <returns></returns>
        private static string Read(ref string Content, ref int Index, string LoopArray, string LoopGroup, object LoopValue, ref LoopStatEnum LoopStat, DataArrayList DataList)
        {
            StringBuilder sb = new StringBuilder();
            while (Index < Content.Length)
            {
                int pos;
                if ((pos = Content.IndexOf('[', Index)) == -1) //如果没有找到 [
                {
                    sb.Append(Content.Substring(Index));
                    Index = Content.Length;
                    continue;
                }

                sb.Append(Content.Substring(Index, pos - Index));
                Index = pos;

                if ((pos = Content.IndexOf(']', pos)) == -1) //如果没有找到 ]
                {
                    sb.Append(Content.Substring(Index));
                    Index = Content.Length;
                    continue;
                }

                string tag = Content.Substring(Index, pos - Index + 1);
                while ((pos = tag.IndexOf('[', 1)) > -1)
                {
                    sb.Append(tag.Substring(0, pos));
                    tag = tag.Substring(pos);
                    Index += pos;
                }

                if (tag.IndexOf("if ", 1) == 1 && tag.IndexOf(" then", tag.Length - 6) > -1)   //如果是if
                {
                    int exist = 0;
                    int reverse = 0;

                    pos = tag.IndexOf('.', 4);
                    string name = tag.Substring(4, pos - 4).Trim();
                    string col = tag.Substring(pos + 1, tag.Length - 15).Trim();

                    if (name[0] == '!')
                    {
                        reverse = 1;
                        name = name.Substring(1);
                    }
                    if (DataList.Contains(name) && DataList[name].Contains(col)) exist = 1;

                    pos = Content.IndexOf("[end if]", Index);
                    if (pos == -1)
                    {
                        sb.Append(Content.Substring(Index));
                        Index = Content.Length;
                        continue;
                    }
                    int mid = Content.IndexOf("[else]", Index);

                    if (mid == -1 || mid > pos)	//没有找到[else]
                    {
                        if (exist + reverse == 1)
                        {
                            string tmp_str = Content.Substring(Index + tag.Length, pos - Index - tag.Length);
                            sb.Append(Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref LoopStat, DataList));
                        }
                    }
                    else
                    {
                        string tmp_str;
                        if (exist + reverse == 1)
                        {
                            tmp_str = Content.Substring(Index + tag.Length, mid - Index - tag.Length);
                            sb.Append(Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref LoopStat, DataList));
                        }
                        else
                        {
                            tmp_str = Content.Substring(mid + 6, pos - mid - 6);
                            sb.Append(Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref LoopStat, DataList));
                        }
                    }
                    Index = pos + 8;
                }
                else if (tag.IndexOf("loop", 1) == 1)	  //如果找到loop
                {
                    string[] attr;
                    string endloop;
                    LoopStatEnum stat = LoopStatEnum.分组子循环之前;
                    if (tag[5] == '.')	//说明是主循环
                    {
                        attr = tag.Substring(6, tag.Length - 7).Split(' ');
                        LoopArray = attr[0];
                        LoopGroup = null;
                        LoopValue = null;
                        endloop = "[/loop." + LoopArray + ']';
                    }
                    else if (tag[5] == ' ')	//说明是子循环
                    {
                        attr = tag.Substring(6, tag.Length - 7).Split(' ');
                        if (LoopGroup != null) stat = LoopStatEnum.分组子循环中;
                        endloop = "[/loop]";
                    }
                    else
                    {
                        sb.Append(Content.Substring(Index));
                        Index = Content.Length;
                        continue;
                    }
                    int min = 0;
                    int max = 0;
                    for (int i = 0; i < attr.Length; i++)
                    {
                        if (attr[i].IndexOf("min=") > -1)
                            min = DataTypeExtensions.ToInt(attr[i].Substring(4));
                        else if (attr[i].IndexOf("max=") > -1)
                            max = DataTypeExtensions.ToInt(attr[i].Substring(4));
                        else if (attr[i].IndexOf("group=") > -1)
                            LoopGroup = attr[i].Substring(6);
                    }
                    pos = Content.IndexOf(endloop, Index + tag.Length);
                    if (pos == -1)
                    {
                        sb.Append(Content.Substring(Index));
                        Index = Content.Length;
                        continue;
                    }

                    Index += tag.Length;
                    string tmp_str = Content.Substring(Index, pos - Index);

                    DataArray arr = DataList[LoopArray];

                    int ii = 0;
                    if (LoopGroup != null)
                    {
                        if (stat == LoopStatEnum.分组子循环中)
                        {
                            if (arr != null)
                            {
                                while (!arr.Eof && arr[LoopGroup].Equals(LoopValue)) { arr.Cursor++; }
                                if (!arr.Eof)
                                {
                                    LoopValue = arr[LoopGroup];

                                    for (; ii < arr.Count; ii++)
                                    {
                                        if (max > 0 && ii >= max || min > 0 && ii >= min || arr.Eof) break;
                                        sb.Append(Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref stat, DataList));
                                    }
                                }
                                while (!arr.Eof && arr[LoopGroup].Equals(LoopValue)) { arr.Cursor++; }
                            }
                            if (min > 0 && ii < min)
                            {
                                string tmp = Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref stat, DataList);
                                for (; ii < min; ii++) sb.Append(tmp);
                            }
                            if (LoopStat == LoopStatEnum.分组子循环之前)
                                LoopStat = LoopStatEnum.分组子循环之后;
                            else
                                LoopStat = LoopStatEnum.分组子循环之前;
                        }
                        else
                        {
                            if (arr == null || !arr.Contains(LoopGroup))
                            {
                                LoopGroup = null;
                            }
                            else
                            {
                                LoopValue = null;
                                for (; ii < arr.Count; ii++)
                                {
                                    if (max > 0 && ii >= max || min > 0 && ii >= min || arr.Eof) break;
                                    sb.Append(Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref stat, DataList));
                                }
                                LoopGroup = null;
                            }
                            if (min > 0 && ii < min)
                            {
                                string tmp = Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref stat, DataList);
                                for (; ii < min; ii++) sb.Append(tmp);
                            }
                        }
                    }
                    else
                    {
                        if (arr != null)
                        {
                            for (; ii < arr.Count; ii++)
                            {
                                if (max > 0 && ii >= max || min > 0 && ii >= min || arr.Eof) break;
                                sb.Append(Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref stat, DataList));
                            }
                        }
                        if (min > 0 && ii < min)
                        {
                            string tmp = Read(ref tmp_str, 0, LoopArray, LoopGroup, LoopValue, ref stat, DataList);
                            for (; ii < min; ii++) sb.Append(tmp);
                        }
                    }
                    Index = pos + endloop.Length;
                }
                else if (tag.IndexOf("global", 1) == 1)  //global替换
                {
                    bool Single = true;
                    string col;
                    DataArray arr = DataList["global"];
                    if (tag[tag.Length - 2] == '/')
                        col = tag.Substring(8, tag.Length - 10);
                    else
                    {
                        Single = false;
                        col = tag.Substring(8, tag.Length - 9);
                    }
                    if (Single)
                    {
                        if (arr != null && !arr.Eof && arr.Contains(col)) sb.Append(arr[col].ToString());
                        Index += tag.Length;
                        continue;
                    }
                    string endtag = "[/global." + col + ']';

                    pos = Content.IndexOf(endtag, Index + tag.Length);
                    if (pos == -1)
                    {
                        sb.Append(Content.Substring(Index));
                        Index = Content.Length;
                        continue;
                    }
                    Index += tag.Length;
                    if (arr != null && !arr.Eof && arr.Contains(col)) sb.Append(arr[col].ToString());
                    else sb.Append(Content.Substring(Index, pos - Index));
                    Index = pos + endtag.Length;
                }
                else if (tag.IndexOf("av.", 1) == 1)   //广告
                {
                    int block = 0;
                    string endtag;
                    DataArray arr = DataList["av"];
                    if (tag[4] == 'b') //[av.block]
                    {
                        block = DataTypeExtensions.ToInt(tag.Substring(10, tag.Length - 11));
                        endtag = "[/" + tag.Substring(1);
                        pos = Content.IndexOf(endtag, Index + tag.Length);
                        if (pos == -1)
                        {
                            sb.Append(Content.Substring(Index));
                            Index = Content.Length;
                            continue;
                        }
                        Index += tag.Length;
                        bool find = false;
                        if (block > 0)
                        {
                            for (int i = 0; i < arr.Count; i++)
                            {
                                arr.Cursor = i;
                                if (arr["block"].Equals(block))
                                {
                                    if (!arr["link"].Equals(string.Empty))
                                        sb.Append("<a href=\"" + arr["link"].ToString() + "\" target=\"_blank\">" + arr["pic"].ToString() + "</a>");
                                    else
                                        sb.Append(arr["pic"].ToString());
                                    find = true;
                                    break;
                                }
                            }
                        }
                        if (!find) sb.Append(Content.Substring(Index, pos - Index));
                        Index = pos + endtag.Length;
                    }
                    else //[av.1]
                    {
                        block = DataTypeExtensions.ToInt(tag.Substring(4, tag.Length - 5));
                        endtag = "[/" + tag.Substring(1);
                        pos = Content.IndexOf(endtag, Index + tag.Length);
                        if (pos == -1)
                        {
                            sb.Append(Content.Substring(Index));
                            Index = Content.Length;
                            continue;
                        }
                        Index += tag.Length;
                        string tmp = Content.Substring(Index, pos - Index);

                        bool find = false;
                        for (int i = 0; i < arr.Count; i++)
                        {
                            if (arr["block", i].Equals(block))
                            {
                                find = true;
                                arr.Cursor = i;
                                break;
                            }
                        }
                        int posi = 0;
                        while (posi < tmp.Length)
                        {
                            int ii = tmp.IndexOf('[', posi); //查找结点的起始标志 比如 [
                            if (ii == -1)
                            {
                                sb.Append(tmp.Substring(posi));
                                posi = tmp.Length;
                                continue;
                            }

                            sb.Append(tmp.Substring(posi, ii - posi));
                            posi = ii;

                            ii = tmp.IndexOf(']', ii);	//查找结点的结束标志，比如 ]
                            if (ii == -1)
                            {
                                sb.Append(tmp.Substring(posi));
                                posi = tmp.Length;
                                continue;
                            }

                            string avtag = tmp.Substring(posi, ii - posi + 1);
                            while ((ii = tag.IndexOf('[', 1)) > -1)
                            {
                                sb.Append(tag.Substring(0, ii));
                                avtag = avtag.Substring(ii);
                                posi += ii;
                            }

                            string col;

                            if (avtag.IndexOf("text", 1) == 1) col = "text";
                            else if (avtag.IndexOf("pic", 1) == 1) col = "pic";
                            else
                            {
                                sb.Append(avtag);
                                posi += avtag.Length;
                                continue;
                            }
                            posi += avtag.Length;

                            int len = avtag.Length;
                            if (avtag[len - 2] == '/')  	//如果是单结点
                            {
                                if (find && !arr.Eof && arr["block"].Equals(block))
                                {
                                    sb.Append("<a href=\"" + arr["link"].ToString() + "\" target=\"_blank\">" + arr[col].ToString() + "</a>");
                                    if (avtag[len - 3] == '/') arr.Cursor++;
                                }
                            }
                            else //如果是嵌套结点
                            {
                                int endpos = tmp.IndexOf('/' + col + ']', posi);
                                if (endpos == -1)
                                {
                                    sb.Append(tmp.Substring(posi - avtag.Length));
                                    break;
                                }
                                if (find && !arr.Eof && arr["block"].Equals(block))
                                {
                                    sb.Append("<a href=\"" + arr["link"].ToString() + "\" target=\"_blank\">" + arr[col].ToString() + "</a>");
                                    if (tmp[endpos - 1] == '/') arr.Cursor++;
                                }
                                else
                                {
                                    if (tmp[endpos - 1] == '/')
                                        sb.Append(tmp.Substring(posi, endpos - posi - 2));
                                    else
                                        sb.Append(tmp.Substring(posi, endpos - posi - 1));
                                }
                                posi = endpos + col.Length + 2;
                            }
                        }
                        Index = pos + endtag.Length;
                    }
                }
                else //如果是普通结点
                {
                    string[] namelist;
                    bool Endline = false;
                    bool Single = true;
                    int max = 0;

                    int len = tag.Length;
                    if (tag[len - 3] == '/')	//如果是单结点带换行 [...//]
                    {
                        Endline = true;
                        if ((pos = tag.IndexOf(" max=")) > -1)
                        {
                            max = DataTypeExtensions.ToInt(tag.Substring(pos + 5, len - 4 - pos - 4));
                            namelist = CheckNode(tag.Substring(1, pos - 1));
                        }
                        else
                            namelist = CheckNode(tag.Substring(1, len - 4));
                    }
                    else if (tag[len - 2] == '/')	//如果是单结点 [.../]
                    {
                        if ((pos = tag.IndexOf(" max=")) > -1)
                        {
                            max = DataTypeExtensions.ToInt(tag.Substring(pos + 5, len - 3 - pos - 4));
                            namelist = CheckNode(tag.Substring(1, pos - 1));
                        }
                        else
                            namelist = CheckNode(tag.Substring(1, len - 3));
                    }
                    else 	//其它
                    {
                        Single = false;
                        if ((pos = tag.IndexOf(" max=")) > -1)
                        {
                            max = DataTypeExtensions.ToInt(tag.Substring(pos + 5, len - 2 - pos - 4));
                            namelist = CheckNode(tag.Substring(1, pos - 1));
                        }
                        else
                            namelist = CheckNode(tag.Substring(1, len - 2));
                    }
                    if (namelist == null)
                    {
                        sb.Append(tag);
                        Index += tag.Length;
                        continue;
                    }

                    string name = namelist[0];
                    string col = namelist[1];

                    DataArray arr = DataList[name];

                    if (Single) //如果是单结点
                    {
                        if (arr != null)
                        {
                            if (LoopGroup != null && name.Equals(LoopArray))
                            {
                                switch (LoopStat)
                                {
                                    case LoopStatEnum.分组子循环之前:
                                        if (Endline) LoopStat = LoopStatEnum.分组子循环之后;
                                        if (!arr.Eof && arr.Contains(col))
                                            sb.Append(DataTypeExtensions.Trim(arr[col].ToString(), max));
                                        break;

                                    case LoopStatEnum.分组子循环中:
                                        if (!arr.Eof && arr[LoopGroup].Equals(LoopValue) && arr.Contains(col))
                                            sb.Append(DataTypeExtensions.Trim(arr[col].ToString(), max));
                                        if (Endline) arr.Cursor++;
                                        break;

                                    case LoopStatEnum.分组子循环之后:
                                        if (Endline) LoopStat = LoopStatEnum.分组子循环之前;
                                        if (arr.Contains(col) && arr.Cursor <= arr.Count)
                                            sb.Append(DataTypeExtensions.Trim(arr[col, (arr.Cursor > 0 ? arr.Cursor - 1 : arr.Cursor)].ToString(), max));
                                        break;
                                }
                            }
                            else
                            {
                                if (!arr.Eof && arr.Contains(col)) sb.Append(DataTypeExtensions.Trim(arr[col].ToString(), max));
                                if (Endline) arr.Cursor++;
                            }
                        }
                        Index += tag.Length;
                        continue;
                    }
                    string end1 = '[' + "/" + name + "." + col + ']';
                    int pos1 = Content.IndexOf(end1, Index + tag.Length);
                    string end2 = '[' + "//" + name + "." + col + ']';
                    int pos2 = Content.IndexOf(end2, Index + tag.Length);

                    string tmp;
                    if (pos1 > -1 && (pos1 < pos2 || pos2 == -1))
                    {
                        Index += tag.Length;
                        tmp = Content.Substring(Index, pos1 - Index);
                        Index = pos1 + end1.Length;
                    }
                    else if (pos2 > -1 && (pos2 > pos1 || pos1 == -1))
                    {
                        Endline = true;
                        Index += tag.Length;
                        tmp = Content.Substring(Index, pos2 - Index);
                        Index = pos2 + end2.Length;
                    }
                    else
                    {
                        sb.Append(Content.Substring(Index));
                        Index = Content.Length;
                        continue;
                    }

                    if (arr != null)
                    {
                        if (LoopGroup != null && name.Equals(LoopArray))
                        {
                            switch (LoopStat)
                            {
                                case LoopStatEnum.分组子循环之前:
                                    if (Endline) LoopStat = LoopStatEnum.分组子循环之后;
                                    if (!arr.Eof && arr.Contains(col))
                                        sb.Append(DataTypeExtensions.Trim(arr[col].ToString(), max));
                                    else
                                        sb.Append(tmp);
                                    break;

                                case LoopStatEnum.分组子循环中:
                                    if (!arr.Eof && arr[LoopGroup].Equals(LoopValue) && arr.Contains(col))
                                        sb.Append(DataTypeExtensions.Trim(arr[col].ToString(), max));
                                    else
                                        sb.Append(tmp);
                                    if (Endline) arr.Cursor++;
                                    break;

                                case LoopStatEnum.分组子循环之后:
                                    if (Endline) LoopStat = LoopStatEnum.分组子循环之前;
                                    if (arr.Contains(col))
                                        sb.Append(DataTypeExtensions.Trim(arr[col, (arr.Cursor > 0 ? arr.Cursor - 1 : arr.Cursor)].ToString(), max));
                                    else
                                        sb.Append(tmp);
                                    break;
                            }
                        }
                        else
                        {
                            if (!arr.Eof && arr.Contains(col))
                                sb.Append(DataTypeExtensions.Trim(arr[col].ToString(), max));
                            else
                                sb.Append(tmp);
                            if (Endline) arr.Cursor++;
                        }
                    }
                    else sb.Append(tmp);
                }

                //end while
            }
            return sb.ToString();
        }

        private unsafe static string[] CheckNode(string Tag)
        {
            fixed (char* p = Tag)
            {
                for (int i = 0; i < Tag.Length; i++)
                {
                    char c = p[i];
                    if (c == '.')
                    {
                        return new string[] { Tag.Substring(0, i), Tag.Substring(i + 1) };
                    }
                    else if (!DataTypeExtensions.IsWord(c)) return null;
                }
            }
            return null;
        }
    }
}