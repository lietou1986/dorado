using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Dorado.Utils
{
    public class DataTableUtility
    {
        public static DataTable AppendMapColumns(DataTable datatable, string fieldid, string fieldparentid, string defaultvalue)
        {
            datatable.Columns.Add("index", typeof(int));
            datatable.Columns.Add("mapid", typeof(int));
            int index = 0;
            AppendMapColumns(datatable.DefaultView, fieldid, fieldparentid, defaultvalue, ref index);
            return datatable;
        }

        public static void AppendMapColumns(DataView dv, string fieldid, string fieldparentid, string defaultvalue, ref int index)
        {
            dv.RowFilter = string.Format("{0}='{1}'", fieldid, defaultvalue);
            int mapid = 0;
            if (dv.Count > 0)
            {
                mapid = (int)dv[0]["index"];
            }
            dv.RowFilter = string.Format("{0}='{1}'", fieldparentid, defaultvalue);
            foreach (DataRowView drv in dv)
            {
                string parentid = drv[fieldid].ToString();
                drv.Row["index"] = ++index;
                drv.Row["mapid"] = mapid;
                AppendMapColumns(dv, fieldid, fieldparentid, parentid, ref index);
            }
        }

        public static DataTable GetDataTable<T>(List<T> list) where T : class
        {
            DataTable dt = new DataTable();
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo p in array)
            {
                dt.Columns.Add(p.Name);
            }
            foreach (T t in list)
            {
                DataRow dr = dt.NewRow();
                PropertyInfo[] array2 = properties;
                foreach (PropertyInfo p2 in array2)
                {
                    dr[p2.Name] = p2.GetValue(t, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}