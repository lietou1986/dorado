using System.Management;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.VWS.ClientHost
{
    public class W3wp
    {
        private W3wp()
        {
        }

        public static string GetAllW3wp(string appPoolName)
        {
            ObjectQuery oQuery = new ObjectQuery("select * from Win32_Process where Name='w3wp.exe'");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
            ManagementObjectCollection oReturnCollection = oSearcher.Get();

            string pid = "";
            string processId = "";
            string cmdLine;
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject oReturn in oReturnCollection)
            {
                pid = oReturn.GetPropertyValue("ProcessId").ToString();
                cmdLine = (string)oReturn.GetPropertyValue("CommandLine");

                string pattern = "-ap \"(.*)\"";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = regex.Match(cmdLine);
                //string appPoolName = match.Groups[1].ToString();
                //sb.AppendFormat("W3WP.exe PID: {0}   AppPoolId:{1}\r\n", pid, appPoolName);
                if (match.Groups != null && match.Groups[1] != null && match.Groups[1].ToString() == appPoolName)
                {
                    processId = pid;
                    return processId;
                }
            }

            return processId;
            //            return sb.ToString();
        }
    }
}