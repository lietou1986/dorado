using System;
using System.Linq;

namespace Dorado.Data
{
    public class SqlHelper
    {
        public static readonly string[] IllegalWords = new string[]
		{
			"'",
			"<",
			">",
			";",
			"(",
			")",
			"* ",
			"% ",
			"--",
			"and ",
			"or ",
			"select ",
			"update ",
			"delete ",
			"drop ",
			"create ",
			"union ",
			"insert ",
			"net ",
			"truncate ",
			"exec ",
			"declare ",
			"and ",
			"count ",
			"chr ",
			"mid ",
			"master ",
			"char "
		};

        public static bool CheckSqlParameter(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            string[] illegalWords = IllegalWords;
            return illegalWords.All(word => value.IndexOf(word, StringComparison.OrdinalIgnoreCase) < 0);
        }
    }
}