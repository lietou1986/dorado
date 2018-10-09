
using Dorado.Core.Data;
using System;
using System.Text;
using Dorado.Configuration;
using Dorado.Extensions;

namespace Dorado.Platform.CodeGenerator
{
    public class GenTableClass
    {
        public static string Gen(string db,string objectId, string space)
        {
            Conn conn = null;
            try
            {
                conn = new Conn(ConnectionStringProvider.Get(db));
                DataArray tmp = conn.Select(@"select name,xtype from sysobjects where id={0}", objectId);
                if (tmp.IsEmpty) throw new ApplicationException("未找到此数据对象");

                //库名，用户
                DataArray datadb = conn.Select("select DB_NAME() dbname,user_name() username");
                string dbname = datadb["dbname"].ToString();
                string username = datadb["username"].ToString();

                string tableName = tmp["name"].ToString();
                DataArray datatb = conn.Select(@"select (case when (SELECT count(*) FROM sysobjects WHERE (name in
(SELECT name  FROM sysindexes  WHERE (id = a.id) AND (indid in
(SELECT indid  FROM sysindexkeys WHERE (id = a.id) AND (colid in
(SELECT colid  FROM syscolumns  WHERE (id = a.id) AND (name = a.name))))))) AND
(xtype = 'PK'))>0 then '主键' else '' end) + (case when g.fkey>0 then '外键' else '' end) +
(case when a.iscomputed=1 then '计算列' else '' end) 键值,
a.name 字段名,
b.name 字段类型,
isnull(h.name,'') 主键字段,
isnull(i.name,'') 主键表,
case when b.name='nvarchar' or b.name='ntext' then a.length/2 else a.length end 长度,
case when d.text is null then '' else substring(d.text,2,len(d.text)-2) end [默认值],
case when a.isnullable=1 then 'true' else 'false' end [允许空],
c.value 备注,
e.text 计算公式,
COLUMNPROPERTY(a.id,a.name,'IsRowGuidCol') 唯一标识,
COLUMNPROPERTY( a.id,a.name,'IsIdentity') 标识,
case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then IDENT_SEED(object_name(a.id)) else 0 end 种子,
case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then IDENT_INCR(object_name(a.id)) else 0 end 增量,
a.prec 精度,
a.xscale 小数位,
a.colorder 列序号,
'' 引用名
from syscolumns a
inner join systypes b on a.xusertype=b.xusertype
left join sys.extended_properties  c on a.id=c.major_id and a.colid = c.minor_id and c.name='MS_Description'
left join syscomments d on a.cdefault=d.id
left join syscomments e on a.id=e.id and a.colorder=e.number
left join sysforeignkeys g on a.id=g.fkeyid and g.fkey=a.colid
left join syscolumns h on g.rkeyid=h.id and g.rkey=h.colid
left join sysobjects i on g.rkeyid=i.id
where objectproperty(a.id,'IsUserTable')=1 and object_name(a.id)='{0}' order by a.colorder", tableName);

                string str = "using System;\r\nusing System.Text;\r\nusing System.Data.SqlClient;\r\n\r\nusing Dorado.Core.Data;\r\n\r\n\r\nnamespace " + space + ".Table\r\n{\r\n\t/// <summary>\r\n\t/// 表 表名 的自动生成类。\r\n\t/// </summary>\r\n\tpublic partial class T_类名\r\n\t{\r\n\r\n\t\tpublic static TableDelegate Insert_Before=null;\r\n\t\tpublic static TableDelegate Insert_After=null;\r\n\t\tpublic static TableDelegate Update_Before=null;\r\n\t\tpublic static TableDelegate Update_After=null;\r\n\t\tpublic static TableDelegate Delete_Before=null;\r\n\t\tpublic static TableDelegate Delete_After=null;\r\n\r\n\t\tpublic struct Field\r\n\t\t{\r\n\t\t字段成员列表\r\n\t\t}\r\n\r\n\t\tpublic const string TableName = \"表名\";\r\n\r\n\t\tprivate static string _insert_sql(DataArray data, bool isStrict)\r\n\t\t{\r\n\t\t\tStringBuilder fld = new StringBuilder();\r\n\t\t\tStringBuilder var = new StringBuilder();\r\n\t\t\tDataArrayColumn field=null;\r\n\r\n\t\t\t生成插入语句\r\n\r\n\t\t\tif(fld.Length==0) return string.Empty;\r\n\t\t\treturn \"insert into \" + TableName + \" with (rowlock) (\" + \r\n\t\t\t\tfld.ToString(0,fld.Length-1) + \") values (\" + \r\n\t\t\t\tvar.ToString(0,var.Length-1) +\");\\n\";\r\n\t\t}\r\n\r\n\t\tprivate static string _update_sql(DataArray data)\r\n\t\t{\r\n\t\t\tif(!data.Contains(主键字段)) return string.Empty;\r\n\t\t\tStringBuilder var = new StringBuilder();\r\n\t\t\tDataArrayColumn field=null;\r\n\r\n\t\t\t生成更新语句\r\n\t\t\tif(var.Length==0) return string.Empty;\t\t\r\n\t\t\treturn var.ToString(0,var.Length-1) + \" where \" + 主键字段 + \"=\" + data[主键字段].ToSQL();\r\n\t\t}\r\n\t\t\t\t\r\n\t\tpublic static 插入返回类型 Insert(Conn conn , DataArray data, bool isStrict = true)\r\n\t\t{\r\n\t\t\tif(data.Count==0) throw new ApplicationException(\"对不起，数据不能为空！\");\r\n                        string sql=_insert_sql(data,isStrict) 插入查询语句;\r\n                        if(sql==string.Empty) return 0;\r\n\t\t\tif(Insert_Before!=null) { Insert_Before(data); }\r\n\t\t\tSqlCommand cmd=null;\r\n\t\t\ttry\r\n\t\t\t{\r\n\t\t\t\tconn.Open();\r\n\t\t\t\tcmd=new SqlCommand(sql,conn.Connection);\r\n\t\t\t\tcmd.CommandTimeout=conn.CommandTimeOut;\t\r\n\t\t\t\tif(conn.Transaction!=null) cmd.Transaction=conn.Transaction;\r\n\t\t\t\t插入返回类型 ret=插入执行命令;\r\n\t\t\t\tif(Insert_After!=null) { Insert_After.BeginInvoke(data,null,null); }\r\n\t\t\t\treturn ret;\r\n\t\t\t}\r\n\t\t\tfinally\r\n\t\t\t{\r\n\t\t\t\tif(cmd!=null) cmd.Dispose();\r\n\t\t\t}\r\n\t\t}\r\n\t\t\r\n\t\tpublic static int Update(Conn conn,DataArray data)\r\n\t\t{\r\n\t\t\tif(data.Count==0) throw new ApplicationException(\"对不起，数据不能为空！\");\r\n                        string sql=_update_sql(data);\r\n                        if(sql==string.Empty) return 0;\r\n\t\t\tif(Update_Before!=null) { Update_Before(data); }\r\n\t\t\tint ret= Update(conn,sql);\r\n\t\t\tif(Update_After!=null) { Update_After.BeginInvoke(data,null,null); }\r\n\t\t\treturn ret;\r\n\t\t}\r\n\r\n\t\tpublic static int Update(Conn conn,string SQL,params object[] para)\r\n\t\t{\r\n\t\t\tfor (int i=0; i<para.Length;i++)\r\n\t\t\t{\r\n\t\t\t     if(para[i] is string) para[i]=para[i].ToString().Replace(\"'\",\"''\");\r\n\t\t\t}\r\n\t\t\treturn Update(conn,string.Format(SQL,para));\r\n\t\t}\r\n\r\n\t\tpublic static int Update(Conn conn,string SQL)\r\n\t\t{\r\n\t\t\tstring sql = \"update \" + TableName + \" with (rowlock) set \" + SQL;\r\n\t\t\tSqlCommand cmd=null;\r\n\t\t\ttry\r\n\t\t\t{\r\n\t\t\t\tconn.Open();\r\n\t\t\t\tcmd=new SqlCommand(sql,conn.Connection);\r\n\t\t\t\tcmd.CommandTimeout=conn.CommandTimeOut;\t\r\n\t\t\t\tif(conn.Transaction!=null) cmd.Transaction=conn.Transaction;\r\n\t\t\t\treturn cmd.ExecuteNonQuery();\r\n\t\t\t}\r\n\t\t\tfinally\r\n\t\t\t{\r\n\t\t\t\tif(cmd!=null) cmd.Dispose();\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tpublic static int Delete(Conn conn,DataArray data)\r\n\t\t{\r\n\t\t\tif(data.Count==0) throw new ApplicationException(\"对不起，数据不能为空！\");\r\n\t\t\tif(Delete_Before!=null) { Delete_Before(data); }\r\n\t\t\tint ret=Delete(conn,主键字段+ \" in (\" + data[主键字段].ToList() + \")\");\r\n\t\t\tif(Delete_After!=null) { Delete_After.BeginInvoke(data,null,null); }\r\n\t\t\treturn ret;\r\n\t\t}\r\n\r\n\t\tpublic static int Delete(Conn conn,string SQL, params object[] para)\r\n\t\t{\r\n\t\t\tfor (int i=0; i<para.Length;i++)\r\n\t\t\t{\r\n\t\t\t     if(para[i] is string) para[i]=para[i].ToString().Replace(\"'\",\"''\");\r\n\t\t\t}\r\n\t\t\treturn Delete(conn,string.Format(SQL,para));\r\n\t\t}\r\n\r\n\t\tpublic static int Delete(Conn conn,string SQL)\r\n\t\t{\r\n\t\t\tstring sql=\"delete \" + TableName + \" with (rowlock) where \" +SQL;\r\n\t\t\tSqlCommand cmd=null;\r\n\t\t\ttry\r\n\t\t\t{\r\n\t\t\t\tconn.Open();\r\n\t\t\t\tcmd=new SqlCommand(sql,conn.Connection);\r\n\t\t\t\tcmd.CommandTimeout=conn.CommandTimeOut;\t\r\n\t\t\t\tif(conn.Transaction!=null) cmd.Transaction=conn.Transaction;\r\n\t\t\t\treturn cmd.ExecuteNonQuery();\r\n\t\t\t}\r\n\t\t\tfinally\r\n\t\t\t{\r\n\t\t\t\tif(cmd!=null) cmd.Dispose();\r\n\t\t\t}\r\n\t\t}\r\n\t}\r\n}";
                StringBuilder code = new StringBuilder();

                //生成注解
                code.Append("/*************************************************************************************************************************\r\n");
                code.Append("* 表 " + tableName + " 的自动生成类\r\n");
                code.Append("* 生成时间： " + DateTime.Now.ToString() + "\r\n");
                code.Append("* \r\n");
                code.Append("* 键值  |     字段名                  | 字段类型   | 长度  | 小数 | 空 | 标识 | 种子 | 增量 | 默认值           | 备注\r\n");
                code.Append("*-------------------------------------------------------------------------------------------------------------------------\r\n");

                string fieldstr = "";
                while (datatb.Read())
                {
                    fieldstr += "\t/// <summary>" + datatb["备注"].ToString().Replace("\r\n", "") + "</summary>\r\n\t";
                    datatb.Columns["引用名"].Set(datatb["字段名"].ToString().Abbr());
                    fieldstr += "\t\tpublic const string " + datatb["引用名"].ToString() + " = \"" + datatb["字段名"].ToString().ToLower() + "\";\r\n\t\t";
                    code.Append("* ");
                    if (datatb["键值"].ToString() != "")
                        code.Append("  √   ");
                    else
                        code.Append("      ");
                    code.Append("| ");
                    code.Append(datatb["字段名"].ToString().PadRight(28));
                    code.Append("| ");
                    code.Append(datatb["字段类型"].ToString().PadRight(11));
                    code.Append("| ");
                    code.Append(datatb["长度"].ToString().PadRight(7));
                    code.Append("| ");
                    if (datatb["小数位"].ToString() == "0")
                        code.Append("    ");
                    else
                        code.Append(datatb["小数位"].ToString().PadRight(4));
                    code.Append("| ");
                    if (datatb["允许空"].ToString() == "true")
                        code.Append(" √ ");
                    else
                        code.Append("   ");
                    code.Append("| ");
                    if (datatb["标识"].ToString() == "1")
                        code.Append(" √   ");
                    else
                        code.Append("     ");
                    code.Append("| ");

                    if (datatb["标识"].ToString() == "1")
                        code.Append(datatb["种子"].ToString().PadRight(5));
                    else
                        code.Append("     ");
                    code.Append("| ");

                    if (datatb["标识"].ToString() == "1")
                        code.Append(datatb["增量"].ToString().PadRight(5));
                    else
                        code.Append("     ");

                    code.Append("| ");
                    code.Append(datatb["默认值"].ToString().PadRight(17));
                    code.Append("| ");
                    code.Append(datatb["备注"].ToString().Replace("\r\n", ""));
                    code.Append("\r\n");

                    if (datatb["键值"].ToString().IndexOf("外键") >= 0)
                    {
                        code.Append("*      ( ");
                        code.Append(datatb["主键表"].ToString() + "." + datatb["主键字段"].ToString() + " )\r\n");
                    }
                }
                datatb.MoveFirst();

                code.Append("* \r\n");
                code.Append("*************************************************************************************************************************/\r\n");
                code.Append(str);
                code.Append("\r\n\n");

                str = code.ToString();
                str = str.Replace("字段成员列表", fieldstr);
                str = str.Replace("类名", tableName.UpFirst());
                //str = str.Replace("表名", dbname + ".dbo." + tableName);
                str = str.Replace("表名", "dbo." + tableName);
                StringBuilder sb = new StringBuilder();

                //生成插入语句---------------------------------------------------------------------------------------------------------

                string keyname = null;
                bool keyinc = false;
                int rowi = 0;
                while (datatb.Read())
                {
                    string key = datatb["键值"].ToString();
                    string name = datatb["引用名"].ToString();
                    string inc = datatb["标识"].ToString();
                    string type = datatb["字段类型"].ToString();
                    string len = datatb["长度"].ToString();
                    string memo = datatb["备注"].ToString().Replace("\r\n", "");
                    string rawMemo = memo;
                    if (memo.IndexOf("||") > -1)
                        rawMemo = memo.Substring(0, memo.IndexOf("||"));
                    string origin = datatb["默认值"].ToString();
                    string none = datatb["允许空"].ToString();
                    switch (type)
                    {
                        case "ntext":
                            len = "1073741823";
                            break;

                        case "text":
                            len = "2147483647";
                            break;
                    }
                    if (rowi == 0 && key.IndexOf("主键") >= 0)
                    {
                        keyname = name;
                        if (datatb["增量"].ToString() == "1") keyinc = true;
                    }
                    rowi++;
                    if (memo.IndexOf("即时更新") >= 0 || inc == "1")
                    {
                        continue;
                    }
                    else
                    {
                        sb.Append("\r\n");
                        if (none == "false")
                        {
                            if (origin == "") sb.Append("\t\t\t//【 " + rawMemo + " 】必填字段\r\n");
                            else sb.Append("\t\t\t//【 " + rawMemo + " 】默认值为：" + origin + "\r\n");
                        }
                        else
                        {
                            sb.Append("\t\t\t//【 " + rawMemo + " 】\r\n");
                        }

                        sb.Append("\t\t\tfield=data[Field." + name + "];\r\n");
                        if (none == "false" && origin == "")
                        {
                            sb.Append("\t\t\tif (field == null || (isStrict && string.IsNullOrWhiteSpace(field.ToString())))\r\n");
                            sb.Append("\t\t\t\tthrow new ApplicationException(\"请您填写" + rawMemo + "！\");\r\n\r\n");

                            sb.Append("\t\t\tfld.Append(Field." + name + " + \",\" );\r\n");
                            sb.Append("\t\t\tvar.Append(field" + TranType.Transfer(type) + " + \",\" );\r\n");
                        }
                        else
                        {
                          
                            sb.Append("\t\t\tif (field!=null)\r\n");
                            sb.Append("\t\t\t{\r\n");
                            sb.Append("\t\t\t\tfld.Append(Field." + name + " + \",\" );\r\n");
                            sb.Append("\t\t\t\tvar.Append(field" + TranType.Transfer(type) + " + \",\" );\r\n");
                            sb.Append("\t\t\t}\r\n");
                        }
                       
                    }
                }
                datatb.MoveFirst();
                str = str.Replace("生成插入语句", sb.ToString());
                str = str.Replace("主键字段", "Field." + keyname);
                if (keyinc)
                {
                    str = str.Replace("插入返回类型", "object");
                    str = str.Replace("插入查询语句", "+\"select @@identity\"");
                    str = str.Replace("插入执行命令", "cmd.ExecuteScalar()");
                }
                else
                {
                    str = str.Replace("插入返回类型", "int");
                    str = str.Replace("插入查询语句", "");
                    str = str.Replace("插入执行命令", "cmd.ExecuteNonQuery()");
                }

                //生成更新语句---------------------------------------------------------------------------------------------------------
                sb.Remove(0, sb.Length);
                while (datatb.Read())
                {
                    string key = datatb["键值"].ToString();
                    string name = datatb["引用名"].ToString();
                    string inc = datatb["标识"].ToString();
                    string type = datatb["字段类型"].ToString();
                    string len = datatb["长度"].ToString();
                    string memo = datatb["备注"].ToString().Replace("\r\n", "");
                    string rawMemo = memo;
                    if (memo.IndexOf("||") > -1)
                        rawMemo = memo.Substring(0, memo.IndexOf("||"));
                    string none = datatb["允许空"].ToString();
                    string origin = datatb["默认值"].ToString();

                    switch (type)
                    {
                        case "ntext":
                            len = "1073741823";
                            break;

                        case "text":
                            len = "2147483647";
                            break;
                    }
                    sb.Append("\r\n");
                    if (key.IndexOf("主键") < 0)
                    {
                        if (none == "false")
                        {
                            if (origin == "") sb.Append("\t\t\t//【 " + rawMemo + " 】必填字段\r\n");
                            else sb.Append("\t\t\t//【 " + rawMemo + " 】默认值为：" + origin + "\r\n");
                        }
                        else
                        {
                            sb.Append("\t\t\t//【 " + rawMemo + " 】\r\n");
                        }

                        if (memo.IndexOf("即时更新") >= 0)
                        {
                            sb.Append("\t\t\tfield=data[Field." + name + "];\r\n");
                            sb.Append("\t\t\tif (field!=null)\r\n");
                            sb.Append("\t\t\t{\r\n");
                            sb.Append("\t\t\t\tvar.Append(Field." + name + " + \"=\" +data[Field." + name + "]" + TranType.Transfer(type) + "+ \",\");\r\n");
                            sb.Append("\t\t\t}\r\n");
                            sb.Append("\t\t\telse var.Append(Field." + name + " + \"=getdate(),\");\r\n");
                        }
                        else
                        {
                            sb.Append("\t\t\tfield=data[Field." + name + "];\r\n");
                            sb.Append("\t\t\tif (field!=null)\r\n");
                            sb.Append("\t\t\t{\r\n");
                            sb.Append("\t\t\t\tvar.Append(Field." + name + " + \"=\" +data[Field." + name + "]" + TranType.Transfer(type) + "+ \",\");\r\n");
                            sb.Append("\t\t\t}\r\n");
                        }
                    }
                }

                return str.Replace("生成更新语句", sb.ToString());
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }
    }
}