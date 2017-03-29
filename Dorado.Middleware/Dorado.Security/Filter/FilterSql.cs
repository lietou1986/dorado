namespace Dorado.Security.Filter
{
    public class FilterSql
    {
        private const string StrKeyWord = "and|or|select|update|delete|drop|create|union|insert|net|truncate|exec|declare|count|chr|mid|master|char";
        private const string StrRegex = "'|--|<|>|;|(|)|*|%";

        public static string KeyWord
        {
            get
            {
                return "and|or|select|update|delete|drop|create|union|insert|net|truncate|exec|declare|count|chr|mid|master|char";
            }
        }

        public static string RegexString
        {
            get
            {
                return "'|--|<|>|;|(|)|*|%";
            }
        }

        public static bool DataCheck(string Input)
        {
            if (Input.Length < 1)
            {
                return true;
            }
            string strIllegal = "'|--|<|>|;|(|)|*|%|and|or|select|update|delete|drop|create|union|insert|net|truncate|exec|declare|count|chr|mid|master|char";
            string[] myArray = strIllegal.Split(new char[]
			{
				'|'
			});
            string sInput = Input.ToLower();
            string[] array = myArray;
            for (int i = 0; i < array.Length; i++)
            {
                string strItem = array[i];
                if (sInput.IndexOf(strItem) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckKeyWord(string _sWord)
        {
            string word = (!string.IsNullOrEmpty(_sWord)) ? _sWord.ToLower() : string.Empty;
            string[] patten = "and|or|select|update|delete|drop|create|union|insert|net|truncate|exec|declare|count|chr|mid|master|char".Split(new char[]
			{
				'|'
			});
            string[] patten2 = "'|--|<|>|;|(|)|*|%".Split(new char[]
			{
				'|'
			});
            string[] array = patten;
            for (int k = 0; k < array.Length; k++)
            {
                string i = array[k];
                if (word.Contains(" " + i) || word.Contains(i + " "))
                {
                    bool result = true;
                    return result;
                }
            }
            string[] array2 = patten2;
            for (int l = 0; l < array2.Length; l++)
            {
                string j = array2[l];
                if (word.Contains(j))
                {
                    bool result = true;
                    return result;
                }
            }
            return false;
        }

        public static string EncodeInputString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string tempStr = str.ToLower();
            tempStr = tempStr.Replace("'", "＇");
            tempStr = tempStr.Replace("--", "－－");
            tempStr = tempStr.Replace("<", "＜");
            tempStr = tempStr.Replace(">", "＞");
            tempStr = tempStr.Replace(";", "；");
            tempStr = tempStr.Replace("(", "（");
            tempStr = tempStr.Replace(")", "）");
            tempStr = tempStr.Replace("*", "＊");
            tempStr = tempStr.Replace("%", "％");
            tempStr = tempStr.Replace("and", "ａｎｄ");
            tempStr = tempStr.Replace("or", "ｏｒ");
            tempStr = tempStr.Replace("select", "ｓｅｌｅｃｔ");
            tempStr = tempStr.Replace("update", "ｕｐｄａｔｅ");
            tempStr = tempStr.Replace("delete", "ｄｅｌｅｔｅ");
            tempStr = tempStr.Replace("drop", "ｄｒｏｐ");
            tempStr = tempStr.Replace("create", "ｃｒｅａｔｅ");
            tempStr = tempStr.Replace("union", "ｕｎｉｏｎ");
            tempStr = tempStr.Replace("insert", "ｉｎｓｅｒｔ");
            tempStr = tempStr.Replace("net", "ｎｅｔ");
            tempStr = tempStr.Replace("truncate", "ｔｒｕｎｃａｔｅ");
            tempStr = tempStr.Replace("exec", "ｅｘｅｃ");
            tempStr = tempStr.Replace("declare", "ｄｅｃｌａｒｅ");
            tempStr = tempStr.Replace("count", "ｃｏｕｎｔ");
            tempStr = tempStr.Replace("chr", "ｃｈｒ");
            tempStr = tempStr.Replace("mid", "ｍｉｄ");
            tempStr = tempStr.Replace("master", "ｍａｓｔｅｒ");
            return tempStr.Replace("char", "ｃｈａｒ");
        }

        public static string DecodeOutputString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string tempStr = str.Replace("＇", "'");
            tempStr = tempStr.Replace("－－", "--");
            tempStr = tempStr.Replace("＜", "<");
            tempStr = tempStr.Replace("＞", ">");
            tempStr = tempStr.Replace("；", ";");
            tempStr = tempStr.Replace("（", "(");
            tempStr = tempStr.Replace("）", ")");
            tempStr = tempStr.Replace("＊", "*");
            tempStr = tempStr.Replace("％", "%");
            tempStr = tempStr.Replace("ａｎｄ", "and");
            tempStr = tempStr.Replace("ｏｒ", "or");
            tempStr = tempStr.Replace("ｓｅｌｅｃｔ", "select");
            tempStr = tempStr.Replace("ｕｐｄａｔｅ", "update");
            tempStr = tempStr.Replace("ｄｅｌｅｔｅ", "delete");
            tempStr = tempStr.Replace("ｄｒｏｐ", "drop");
            tempStr = tempStr.Replace("ｃｒｅａｔｅ", "create");
            tempStr = tempStr.Replace("ｕｎｉｏｎ", "union");
            tempStr = tempStr.Replace("ｉｎｓｅｒｔ", "insert");
            tempStr = tempStr.Replace("ｎｅｔ", "net");
            tempStr = tempStr.Replace("ｔｒｕｎｃａｔｅ", "truncate");
            tempStr = tempStr.Replace("ｅｘｅｃ", "exec");
            tempStr = tempStr.Replace("ｄｅｃｌａｒｅ", "declare");
            tempStr = tempStr.Replace("ｃｏｕｎｔ", "count");
            tempStr = tempStr.Replace("ｃｈｒ", "chr");
            tempStr = tempStr.Replace("ｍｉｄ", "mid");
            tempStr = tempStr.Replace("ｍａｓｔｅｒ", "master");
            return tempStr.Replace("ｃｈａｒ", "char");
        }

        public static string EncodeStr(string str)
        {
            str = (str ?? "");
            str = str.Replace("&nbsp", "&amp;nbsp");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("'", "’");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br/>");
            return str;
        }

        public static string DecodeStr(string str)
        {
            str = (str ?? "");
            str = str.Replace("&amp;nbsp", "&nbsp");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("’", "'");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("<br/>", "\n");
            return str;
        }
    }
}