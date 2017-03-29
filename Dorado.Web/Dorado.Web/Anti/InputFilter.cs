using System;

namespace Dorado.Web.Filter
{
    internal static class InputFilter
    {
        public static readonly string[] IllegalWords = new string[]
		{
			"'",
			"<",
			">",
			";",
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
			"exec(",
			"declare ",
			"and ",
			"count ",
			"chr ",
			"mid ",
			"master ",
			"char",
			"char(",
			"nchar",
			"nchar(",
			"sp_executesql",
			"sp_execute",
			"execute("
		};

        public static bool Validate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] illegalWords = InputFilter.IllegalWords;
                for (int i = 0; i < illegalWords.Length; i++)
                {
                    string word = illegalWords[i];
                    if (value.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}