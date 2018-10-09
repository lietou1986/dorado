using Dorado.Core.Data;

namespace Dorado.Tool
{
    public class TranType
    {
        public static string ConvertSqlTypeToNet(string sourceTypeName)
        {
            string retrunValue = "";
            switch (sourceTypeName)
            {
                case "int identity": retrunValue = "int"; break;
                case "bit": retrunValue = "int"; break;
                case "bigint": retrunValue = "bigInt"; break;
                case "char": retrunValue = "string"; break;
                case "datetime": retrunValue = "DateTime"; break;
                case "decimal": retrunValue = "decimal"; break;
                case "float": retrunValue = "double"; break;
                case "image": retrunValue = "byte[]"; break;
                case "int": retrunValue = "int"; break;
                case "money": retrunValue = "float"; break;
                case "nchar": retrunValue = "string"; break;
                case "ntext": retrunValue = "string"; break;
                case "nvarchar": retrunValue = "string"; break;
                case "numeric": retrunValue = "float"; break;
                case "real": retrunValue = "double"; break;
                case "smalldatetime": retrunValue = "DateTime"; break;
                case "smallint": retrunValue = "short"; break;
                case "smallmoney": retrunValue = "decimal"; break;
                case "text": retrunValue = "string"; break;
                case "timestamp": retrunValue = "string"; break;
                case "tinyint": retrunValue = "byte"; break;
                case "binary": retrunValue = "long"; break;
                case "varbinary": retrunValue = "long"; break;
                case "varchar": retrunValue = "string"; break;
                case "uniqueidentifier": retrunValue = "Guid"; break;
            };
            return retrunValue;
        }

        //转换SQL数据类型到system.Data.SqlDbType
        public static string ConvertType(DataArrayRow dr)
        {
            string retrunValue = "";
            string fieldTypeName;
            fieldTypeName = "type_name";

            switch (dr[fieldTypeName][0].ToString())
            {
                case "int identity": retrunValue = "System.Data.SqlDbType.Int"; break;
                case "int": retrunValue = "System.Data.SqlDbType.Int"; break;
                case "bit": retrunValue = "System.Data.SqlDbType.Bit"; break;
                case "bigint": retrunValue = "System.Data.SqlDbType.BigInt"; break;
                case "char": retrunValue = "System.Data.SqlDbType.Char"; break;
                case "datetime": retrunValue = "System.Data.SqlDbType.DateTime "; break;

                //case "decimal":
                //    High = (System.Int32)(dr["prec"]);
                //    retrunValue = High.ToString();
                //    if (retrunValue == "2")
                //    { retrunValue = "System.Data.SqlDbType.Float"; }
                //    else
                //    { retrunValue = "System.Data.SqlDbType.Decimal"; }
                //    break;
                case "float": retrunValue = "System.Data.SqlDbType.Float"; break;
                case "image": retrunValue = "System.Data.SqlDbType.Image"; break;
                case "money": retrunValue = "System.Data.SqlDbType.Money"; break;
                case "nchar": retrunValue = "System.Data.SqlDbType.NChar"; break;
                case "ntext": retrunValue = "System.Data.SqlDbType.NText"; break;
                case "nvarchar": retrunValue = "System.Data.SqlDbType.NVarChar"; break;
                case "numeric": retrunValue = "System.Data.SqlDbType.Decimal"; break;
                case "real": retrunValue = "System.Data.SqlDbType.Real"; break;
                case "smalldatetime": retrunValue = "System.Data.SqlDbType.SmallDateTime"; break;
                case "smallint": retrunValue = "System.Data.SqlDbType.SmallInt"; break;
                case "smallmoney": retrunValue = "System.Data.SqlDbType.SmallMoney"; break;
                case "text": retrunValue = "System.Data.SqlDbType.Text"; break;
                case "timestamp": retrunValue = "System.Data.SqlDbType.Timestamp"; break;
                case "tinyint": retrunValue = "System.Data.SqlDbType.TinyInt"; break;
                case "varbinary": retrunValue = "System.Data.SqlDbType.VarBinary"; break;
                case "varchar": retrunValue = "System.Data.SqlDbType.VarChar"; break;
                case "uniqueidentifier": retrunValue = "System.Data.SqlDbType.UniqueIdentifier"; break;
            };
            return retrunValue;
        }

        //转换SQL数据类型到system.Data.SqlDbType
        public static string ConvertType(string sourceTypeName)
        {
            string retrunValue = "";
            switch (sourceTypeName)
            {
                case "int identity": retrunValue = "System.Data.SqlDbType.Int"; break;
                case "int": retrunValue = "System.Data.SqlDbType.Int"; break;
                case "bit": retrunValue = "System.Data.SqlDbType.Bit"; break;
                case "bigint": retrunValue = "System.Data.SqlDbType.BigInt"; break;
                case "char": retrunValue = "System.Data.SqlDbType.Char"; break;
                case "datetime": retrunValue = "System.Data.SqlDbType.DateTime "; break;

                //case "decimal":
                //    High = (System.Int32)(dr["prec"]);
                //    retrunValue = High.ToString();
                //    if (retrunValue == "2")
                //    { retrunValue = "System.Data.SqlDbType.Float"; }
                //    else
                //    { retrunValue = "System.Data.SqlDbType.Decimal"; }
                //    break;
                case "float": retrunValue = "System.Data.SqlDbType.Float"; break;
                case "image": retrunValue = "System.Data.SqlDbType.Image"; break;
                case "money": retrunValue = "System.Data.SqlDbType.Money"; break;
                case "nchar": retrunValue = "System.Data.SqlDbType.NChar"; break;
                case "ntext": retrunValue = "System.Data.SqlDbType.NText"; break;
                case "nvarchar": retrunValue = "System.Data.SqlDbType.NVarChar"; break;
                case "numeric": retrunValue = "System.Data.SqlDbType.Decimal"; break;
                case "real": retrunValue = "System.Data.SqlDbType.Real"; break;
                case "smalldatetime": retrunValue = "System.Data.SqlDbType.SmallDateTime"; break;
                case "smallint": retrunValue = "System.Data.SqlDbType.SmallInt"; break;
                case "smallmoney": retrunValue = "System.Data.SqlDbType.SmallMoney"; break;
                case "text": retrunValue = "System.Data.SqlDbType.Text"; break;
                case "timestamp": retrunValue = "System.Data.SqlDbType.Timestamp"; break;
                case "tinyint": retrunValue = "System.Data.SqlDbType.TinyInt"; break;
                case "varbinary": retrunValue = "System.Data.SqlDbType.VarBinary"; break;
                case "varchar": retrunValue = "System.Data.SqlDbType.VarChar"; break;
                case "uniqueidentifier": retrunValue = "System.Data.SqlDbType.UniqueIdentifier"; break;
            };
            return retrunValue;
        }

        /// <summary>
        /// 转换SQL数据类型到typeof(
        /// </summary>
        /// <param name="sourceTypeName"></param>
        /// <returns></returns>
        public static string ConvertSqlTypeToTypeOf(string sourceTypeName)
        {
            string retrunValue = "";
            switch (sourceTypeName)
            {
                case "int identity": retrunValue = "typeof(System.Int32)"; break;
                case "bit": retrunValue = "typeof(System.Boolean)"; break;
                case "bigint": retrunValue = "typeof(System.Int64)"; break;
                case "char": retrunValue = "typeof(System.String)"; break;
                case "datetime": retrunValue = "typeof(System.DateTime) "; break;
                case "decimal": retrunValue = "typeof(System.Decimal)"; break;
                case "float": retrunValue = "typeof(System.Decimal)"; break;
                case "int": retrunValue = "typeof(System.Int32)"; break;
                case "money": retrunValue = "typeof(System.Decimal)"; break;
                case "nchar": retrunValue = "typeof(System.String)"; break;
                case "nvarchar": retrunValue = "typeof(System.String)"; break;
                case "numeric": retrunValue = "typeof(System.Decimal)"; break;
                case "smalldatetime": retrunValue = "typeof(System.DateTime)"; break;
                case "smallint": retrunValue = "typeof(System.Int16)"; break;
                case "smallmoney": retrunValue = "typeof(System.Decimal)"; break;
                case "tinyint": retrunValue = "typeof(System.Int16)"; break;
                case "varchar": retrunValue = "typeof(System.String)"; break;
                case "text": retrunValue = "typeof(System.String)"; break;
                case "image": retrunValue = "typeof(System.Data.SqlTypes.SqlBinary)"; break;
            };
            return retrunValue;
        }

        public static string Transfer(string typeName)
        {
            switch (typeName)
            {
                case "bit": return ".ToByte().ToString()";
                case "bigint": return ".ToLong().ToString()";
                case "char": return ".ToSQL()";
                case "datetime": return ".ToSQL()";
                case "decimal": return ".ToDecimal().ToString()";
                case "float": return ".ToDouble().ToString()";
                case "int": return ".ToInt().ToString()";
                case "money": return ".ToDecimal().ToString()";
                case "nchar": return ".ToSQL()";
                case "nvarchar": return ".ToSQL()";
                case "numeric": return ".ToDecimal().ToString()";
                case "smalldatetime": return ".ToSQL()";
                case "smallint": return ".ToShort().ToString()";
                case "smallmoney": return ".ToDecimal().ToString()";
                case "tinyint": return ".ToByte().ToString()";
                case "varchar": return ".ToSQL()";
                case "text": return ".ToSQL()";
                case "binary": return ".ToBinary()";
                case "varbinary": return ".ToBinary()";
                case "uniqueidentifier": return ".ToSQL()";
                default: return ".ToSQL()";
            };
        }

        public static string Convert(string typeName)
        {
            switch (typeName)
            {
                case "bit": return "Valid.ToBool(";
                case "bigint": return "Valid.ToLong(";
                case "char": return "Valid.ToString(";
                case "datetime": return "Valid.ToDateTime(";
                case "decimal": return "Valid.ToDecimal(";
                case "float": return "Valid.ToDouble(";
                case "int": return "Valid.ToInt(";
                case "money": return "Valid.ToDecimal(";
                case "nchar": return "Valid.ToString(";
                case "nvarchar": return "Valid.ToString(";
                case "numeric": return "Valid.ToDecimal(";
                case "smalldatetime": return "Valid.ToDateTime(";
                case "smallint": return "Valid.ToShort(";
                case "smallmoney": return "Valid.ToDecimal(";
                case "tinyint": return "Valid.ToByte(";
                case "varchar": return "Valid.ToString(";
                case "text": return "Valid.ToString(";
                case "binary": return "Valid.ToLong(";
                case "varbinary": return "Valid.ToLong(";
                case "uniqueidentifier": return "Valid.ToGuid(";
                default: return ".ToString(";
            };
        }
    }
}