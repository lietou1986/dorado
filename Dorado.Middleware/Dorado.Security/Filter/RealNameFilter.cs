using System.Text.RegularExpressions;

namespace Dorado.Security.Filter
{
    public class RealNameFilter
    {
        private static System.Collections.Generic.Dictionary<char, object> _badCharInName;
        private static System.Collections.Generic.List<Regex> _badWordInName;

        static RealNameFilter()
        {
            RealNameFilter._badCharInName = new System.Collections.Generic.Dictionary<char, object>();
            RealNameFilter._badCharInName.Add('!', null);
            RealNameFilter._badCharInName.Add('@', null);
            RealNameFilter._badCharInName.Add('#', null);
            RealNameFilter._badCharInName.Add('$', null);
            RealNameFilter._badCharInName.Add('%', null);
            RealNameFilter._badCharInName.Add('^', null);
            RealNameFilter._badCharInName.Add('&', null);
            RealNameFilter._badCharInName.Add('*', null);
            RealNameFilter._badCharInName.Add('(', null);
            RealNameFilter._badCharInName.Add(')', null);
            RealNameFilter._badCharInName.Add('<', null);
            RealNameFilter._badCharInName.Add('>', null);
            RealNameFilter._badCharInName.Add('/', null);
            RealNameFilter._badCharInName.Add('\\', null);
            RealNameFilter._badCharInName.Add('?', null);
            RealNameFilter._badCharInName.Add('0', null);
            RealNameFilter._badCharInName.Add('1', null);
            RealNameFilter._badCharInName.Add('2', null);
            RealNameFilter._badCharInName.Add('3', null);
            RealNameFilter._badCharInName.Add('4', null);
            RealNameFilter._badCharInName.Add('5', null);
            RealNameFilter._badCharInName.Add('6', null);
            RealNameFilter._badCharInName.Add('7', null);
            RealNameFilter._badCharInName.Add('8', null);
            RealNameFilter._badCharInName.Add('9', null);
            RealNameFilter._badWordInName = new System.Collections.Generic.List<Regex>();
            string[] words = new string[]
			{
				"bastard",
				"comment",
				"faggot",
				"whore",
				"bitch",
				"dildo",
				"check",
				"fuck",
				"sexy",
				"cool",
				"cock",
				"shit",
				"anal",
				"cunt",
				"slut",
				"twat",
				"ass",
				"fag",
				"sex",
				"hot",
				"out",
				"the",
				"my",
				"of"
			};
            string[] array = words;
            for (int j = 0; j < array.Length; j++)
            {
                string word = array[j];
                System.Text.StringBuilder strB = new System.Text.StringBuilder();
                strB.Append("(?:\\b|_)(");
                int len = word.Length;
                for (int i = 0; i < len; i++)
                {
                    strB.Append(word[i]);
                    if (i < len - 1)
                    {
                        strB.Append("\\s*");
                    }
                }
                strB.Append(")(?:\\b|_)");
                RealNameFilter._badWordInName.Add(new Regex(strB.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled));
            }
        }

        public static string ValidateRealName(string input)
        {
            if (input == null)
            {
                return null;
            }
            input = input.Trim();
            System.Text.StringBuilder strBld = new System.Text.StringBuilder();
            char repeatChar = ' ';
            int repeatCount = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char current = input[i];
                if (!RealNameFilter._badCharInName.ContainsKey(current))
                {
                    if (char.ToLower(current) == repeatChar)
                    {
                        if (repeatCount > 0)
                        {
                            goto IL_5A;
                        }
                        repeatCount++;
                    }
                    else
                    {
                        repeatChar = char.ToLower(current);
                        repeatCount = 0;
                    }
                    strBld.Append(current);
                }
            IL_5A: ;
            }
            string temp = strBld.ToString();
            bool hasBadWord = true;
            while (hasBadWord)
            {
                hasBadWord = false;
                foreach (Regex regex in RealNameFilter._badWordInName)
                {
                    Match match = regex.Match(temp);
                    if (match.Success)
                    {
                        Group group = match.Groups[1];
                        temp = temp.Substring(0, group.Index) + temp.Substring(group.Index + group.Length);
                        hasBadWord = true;
                        break;
                    }
                }
            }
            System.Text.StringBuilder strBld2 = new System.Text.StringBuilder(temp);
            int upperCount = 0;
            for (int j = 0; j < strBld2.Length; j++)
            {
                if (char.IsLetter(strBld2[j]))
                {
                    if (upperCount == 0)
                    {
                        strBld2[j] = char.ToUpper(strBld2[j]);
                        upperCount = 1;
                    }
                    else
                    {
                        if (char.IsUpper(strBld2[j]))
                        {
                            if (upperCount < 2)
                            {
                                upperCount++;
                            }
                            else
                            {
                                strBld2[j] = char.ToLower(strBld2[j]);
                            }
                        }
                    }
                }
            }
            return strBld2.ToString().Trim();
        }
    }
}