using System.Text.RegularExpressions;

namespace Dorado.Package.ServiceImp.Utility
{
    public class UtilityHelper
    {
        public static string GetPackagePartPath(string address)
        {
            const string patternToMatch = @"^*\d{4}(\-|\/|\.)\d{1,2}\1\d{1,2}";
            int index = 0;
            Regex regex = new Regex(patternToMatch, RegexOptions.Compiled | RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(address);
            foreach (Match match in matches)
            {
                index = match.Index;
                break;
            }
            return address.Substring(index).Replace(@"\", "/");
        }

        public static bool IsGuid(string guid)
        {
            Match m = Regex.Match(guid, @"^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$|^[0-9a-f]{32}$", RegexOptions.IgnoreCase);
            if (m.Success)
                return true;
            else
                return false;
        }
    }
}