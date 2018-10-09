using Dorado.Configuration;
using Dorado.Core.Data;
using Dorado.Extensions;
using System;

namespace Dorado.Tool
{
    public class GenProcClass
    {
        public static string Gen(string db, string objectId, string space)
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

                DataArray data = new DataArray();
                data.AddRow();

                string procname = tmp["name"].ToString();

                //搜索该过程的参数
                string fieldstr0 = "";		//设置参数类型
                string infieldstr = "";	    //输入参数
                string infieldlist = "";	//参数列表
                string outfieldlist = "";	//输出参数列表
                string outfielddirlist = "";	//输出参数指向
                string execstr = "";		//执行方式
                string backstr = "DataArrayList";        //返回类型
                string read1 = "SqlDataReader reader=null;";
                string read2 = "";

                execstr = "ret=new DataArrayList();\r\n\t\t\t\treader=cmd.ExecuteReader();\r\n\t\t\t\tint fldcount=reader.FieldCount;\r\n\t\t\t\tstring[] columns = new string[fldcount];\r\n\t\t\t\twhile(reader.Read())\r\n\t\t\t\t{\r\n\t\t\t\t\tfor (int i=0; i<fldcount; i++)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tcolumns[i]=reader.GetString(i);\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\r\n\t\t\t\tint count=0;\r\n\t\t\t\twhile(reader.NextResult()){\r\n\t\t\t\t\tret.Add(columns[count],ConnBase.Exec(reader,null,false));\r\n\t\t\t\t\tcount++;\r\n\t\t\t\t}\r\n\t\t\t\treader.Close();\r\n";

                DataArray tmp0 = conn.Select("sp_sproc_columns {0}", procname);

                while (tmp0.Read())
                {
                    if (tmp0["column_name"].ToString() != "@RETURN_VALUE")
                    {
                        string ss = tmp0["column_name"].ToString().Replace("@", "");
                        if (tmp0["column_type"].ToString() == "1")  //输入参数
                        {
                            infieldstr += "," + TranType.ConvertSqlTypeToNet(tmp0["type_name"].ToString()) + " " + ss;
                            infieldlist += "cmd.Parameters[\"" + tmp0["column_name"].ToString() + "\"].Value =" + ss + " ;\r\n";
                        }

                        if (tmp0["column_type"].ToString() == "2" || tmp0["column_type"].ToString() == "4") //输出参数
                        {
                            infieldstr += ", out " + TranType.ConvertSqlTypeToNet(tmp0["type_name"].ToString()) + " " + ss;
                            outfieldlist += ss + "=" + TranType.Convert(tmp0["type_name"].ToString()) + "cmd.Parameters[\"" + tmp0["column_name"].ToString() + "\"].Value);\r\n\t\t\t\t";
                        }

                        fieldstr0 += "cmd.Parameters.Add(\"" + tmp0["column_name"].ToString() + "\",\t"
                            + TranType.ConvertType((DataArrayRow)tmp0.Rows.Current) + ",\t"
                            + (tmp0["type_name"].ToString()[0] == 'n' ? tmp0["precision"].ToString() : tmp0["length"].ToString()) + ");\r\n\t\t\t\t";
                    }
                }

                string txt = "using System;\r\nusing System.Data;\r\nusing System.Data.SqlClient;\r\n\r\nusing Dorado.Core.Data;\r\n\r\n\r\n\r\nnamespace " + space + ".Procedure\r\n{\r\n\tpublic class 存储过程类名\r\n\t{\r\n\t\tpublic static 返回类型 Exec(Conn conn 输入参数列表)\r\n\t\t{\r\n\t\t\tSqlCommand cmd=null;\r\n\t\t\t声明Reader\r\n\t\t\ttry\r\n\t\t\t{\r\n\t\t\t\tconn.Open();\r\n\t\t\t\tcmd=new SqlCommand();\r\n\t\t\t\tcmd.CommandText = \"数据库名.dbo.存储过程名称\";\r\n\t\t\t\tcmd.CommandType = CommandType.StoredProcedure;\r\n\t\t\t\tcmd.Connection=conn.Connection;\r\n\t\t\t\tcmd.CommandTimeout=Setting.CommandTimeOut;\t\r\n\t\t\t\tif(conn.Transaction!=null) cmd.Transaction=conn.Transaction;\r\n\r\n\t\t\t\t\t\t\tcmd.Parameters.Add(\"@RETURN_VALUE\",\tSqlDbType.Int,\t4);\r\n\t\t\t\tcmd.Parameters[\"@RETURN_VALUE\"].Direction= ParameterDirection.ReturnValue;\r\n\t\t\t        设置参数类型\r\n\t\t\t        设置参数值\r\n             设置参数指向\r\n                   返回类型 ret;\r\n\t\t\t        执行存储过程\r\n\t\t\t        获取输出参数\r\n                                return ret;\r\n\t\t\t}\r\n\t\t\tfinally\r\n\t\t\t{\r\n\t\t\t        if(reader!=null) reader.Close();\r\n\t\t\t\tif(cmd!=null) cmd.Dispose();\r\n\t\t\t}\r\n\t\t}\r\n\t}\r\n}";

                txt = txt.Replace("数据库名", dbname);
                txt = txt.Replace("存储过程类名", procname.Replace("zd_", "").UpFirst());
                txt = txt.Replace("存储过程名称", procname);
                txt = txt.Replace("设置参数类型", fieldstr0);
                txt = txt.Replace("输入参数列表", infieldstr);
                txt = txt.Replace("获取输出参数", outfieldlist);
                txt = txt.Replace("设置参数值", infieldlist);
                txt = txt.Replace("输出参数设置", outfieldlist);
                txt = txt.Replace("设置参数指向", outfielddirlist);
                txt = txt.Replace("执行存储过程", execstr);
                txt = txt.Replace("返回类型", backstr);
                txt = txt.Replace("声明Reader", read1);
                txt = txt.Replace("关闭Reader", read2);

                return txt;
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