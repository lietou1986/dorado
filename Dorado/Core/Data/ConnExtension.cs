using Dorado.Core.Logger;
using Dorado.Extensions;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Dorado.Core.Data
{
    internal struct ParaInfo
    {
        public int Count;
        public int[] Pos;
        public SqlParameter[] Para;
        public string Sql;
        public string Ret;

        public ParaInfo(int length)
        {
            Count = 0;
            Pos = new int[length];
            Para = new SqlParameter[length];
            Sql = String.Empty;
            Ret = String.Empty;
        }
    }

    /// <summary>
    /// Conn连接基础类
    /// </summary>
    public static class ConnExtension
    {
        public static DataArray Exec(this IDataReader reader, DataArray data, bool back)
        {
            if (back) return reader.ExecBack(data);
            if (data == null) data = new DataArray();
            if (reader == null) return data;

            int fldcount = reader.FieldCount;
            DataArrayColumn[] columns = new DataArrayColumn[fldcount];

            for (int i = 0; i < fldcount; i++)
            {
                columns[i] = data.Columns.Add(reader.GetName(i).ToLower(), reader.GetFieldType(i));
            }
            while (reader.Read())
            {
                data.AddRow();
                for (int i = 0; i < fldcount; i++)
                {
                    columns[i].Set(reader.GetValue(i), data.Count - 1);
                }
            }
            data.Cursor = 0;
            return data;
        }

        private static DataArray ExecBack(this IDataReader reader, DataArray data)
        {
            if (data == null) data = new DataArray();
            if (reader == null) return data;

            int fldcount = reader.FieldCount;
            DataArrayColumn[] columns = new DataArrayColumn[fldcount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[i] = data.Columns.Add(reader.GetName(i).ToLower(), reader.GetFieldType(i));
            }

            ArrayList list = new ArrayList(data.RowSize);
            while (reader.Read())
            {
                object[] temp = new object[fldcount];
                reader.GetValues(temp);
                list.Add(temp);
            }
            reader.Close();
            for (int r = list.Count - 1; r >= 0; r--)
            {
                object[] temp = (object[])list[r];
                data.AddRow();
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i].Set(temp[i], data.Count - 1);
                }
            }
            data.Cursor = 0;
            return data;
        }

        public static DataArray Exec(this DataTable table, DataArray data, bool back)
        {
            if (back) return ExecBack(table, data);
            if (data == null) data = new DataArray();
            if (table == null) return data;

            int fldcount = table.Columns.Count;
            DataArrayColumn[] columns = new DataArrayColumn[fldcount];

            for (int i = 0; i < fldcount; i++)
            {
                columns[i] = data.Columns.Add(table.Columns[i].ColumnName.ToLower(), table.Columns[i].DataType);
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                data.AddRow();
                for (int j = 0; j < fldcount; j++)
                {
                    columns[j].Set(table.Rows[i][j], data.Count - 1);
                }
            }

            data.Cursor = 0;
            return data;
        }

        private static DataArray ExecBack(this DataTable table, DataArray data)
        {
            if (data == null) data = new DataArray();
            if (table == null) return data;

            int fldcount = table.Columns.Count; ;
            DataArrayColumn[] columns = new DataArrayColumn[fldcount];
            for (int i = 0; i < fldcount; i++)
            {
                columns[i] = data.Columns.Add(table.Columns[i].ColumnName.ToLower(), table.Columns[i].DataType);
            }

            ArrayList list = new ArrayList(data.RowSize);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                list.Add(table.Rows[i].ItemArray);
            }
            for (int r = list.Count - 1; r >= 0; r--)
            {
                object[] temp = (object[])list[r];
                data.AddRow();
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i].Set(temp[i], data.Count - 1);
                }
            }
            data.Cursor = 0;
            return data;
        }
    }
}