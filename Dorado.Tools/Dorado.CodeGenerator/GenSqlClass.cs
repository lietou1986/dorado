using Dorado.Core.Data;
using Dorado.Tool.SqlSettings;
using System;
using System.Linq;

namespace Dorado.Tool
{
    public class GenSqlClass
    {
        private static SqlSettingsConfiguration Config
        {
            get { return SqlSettingsConfigurationManager.Config; }
        }

        public static string Gen(string db, string name, string space)
        {
            try
            {
                Sql sql = Config.Sqls.SqlList.FirstOrDefault(n => n.Name.ToLower() == name.ToLower());
                if (sql == null) throw new ApplicationException("未找到此数据对象");

                DataArray data = new DataArray();
                data.AddRow();

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

                foreach (var par in sql.Parameters)
                {
                    string ss = par.Name.Replace("@", "");
                    if (!par.IsOutput) //输入参数
                    {
                        infieldstr += "," + TranType.ConvertSqlTypeToNet(par.DataType) + " " + ss;
                        infieldlist += "cmd.Parameters[\"" + par.Name + "\"].Value =" + ss +
                                       " ;\r\n";
                    }
                    else
                    {
                        infieldstr += ", out " + TranType.ConvertSqlTypeToNet(par.DataType) + " " +
                                      ss;
                        outfieldlist += ss + "=" + TranType.Convert(par.DataType) +
                                        "cmd.Parameters[\"" + par.Name +
                                        "\"].Value);\r\n";

                        outfielddirlist += "cmd.Parameters[\"" + par.Name + "\"].Direction = ParameterDirection.Output;\r\n";
                    }

                    fieldstr0 += "cmd.Parameters.Add(\"" + par.Name + "\",\t"
                                 + TranType.ConvertType(par.DataType) + ");\r\n\t\t\t\t";
                }
                string txt = "using System;\r\nusing System.Data;\r\nusing System.Data.SqlClient;\r\n\r\nusing Dorado.Core.Data;\r\n\r\n\r\n\r\nnamespace " + space + ".Sql\r\n{\r\n\tpublic class SQL类名\r\n\t{\r\n\t\tpublic static 返回类型 Exec(输入参数列表)\r\n\t\t{\r\n\t\t\tConn conn = null;\r\n\t\t\tSqlCommand cmd=null;\r\n\t\t\t声明Reader\r\n\t\t\ttry\r\n\t\t\t{\r\n\t\t\t\t实例化数据库;\r\n\t\t\t\tconn.Open();\r\n\t\t\t\tcmd=new SqlCommand();\r\n\t\t\t\tcmd.CommandText = @\"SQL内容\";\r\n\t\t\t\tcmd.CommandType = CommandType.Text;\r\n\t\t\t\tcmd.Connection=conn.Connection;\r\n\t\t\t\tcmd.CommandTimeout=Setting.CommandTimeOut;\t\r\n\t\t\t\tif(conn.Transaction!=null) cmd.Transaction=conn.Transaction;\r\n\r\n\t\t\t\t\t\t\tcmd.Parameters.Add(\"@RETURN_VALUE\",\tSqlDbType.Int,\t4);\r\n\t\t\t\tcmd.Parameters[\"@RETURN_VALUE\"].Direction= ParameterDirection.ReturnValue;\r\n\t\t\t        设置参数类型\r\n\t\t\t        设置参数值\r\n           设置参数指向\r\n                     返回类型 ret;\r\n\t\t\t        执行存储过程\r\n\t\t\t        获取输出参数\r\n                                return ret;\r\n\t\t\t}\r\n\t\t\tfinally\r\n\t\t\t{\r\n\t\t\t        if(reader!=null) reader.Close();\r\n\t\t\t\tif(cmd!=null) cmd.Dispose();\r\n\t\t\tif(conn!=null) conn.Close();\r\n\t\t\t}\r\n\t\t}\r\n\t}\r\n}";

                txt = txt.Replace("实例化数据库", string.Format("conn=new Conn(\"{0}\")", sql.UseDB ?? Config.Sqls.UseDB));
                txt = txt.Replace("SQL类名", sql.Name);
                txt = txt.Replace("SQL内容", sql.Body);
                txt = txt.Replace("设置参数类型", fieldstr0);
                txt = txt.Replace("输入参数列表", infieldstr.StartsWith(",") ? infieldstr.Remove(0, 1) : infieldstr);
                txt = txt.Replace("获取输出参数", outfieldlist);
                txt = txt.Replace("设置参数值", infieldlist);
                txt = txt.Replace("设置参数指向", outfielddirlist);
                txt = txt.Replace("输出参数设置", outfieldlist);
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
        }
    }
}