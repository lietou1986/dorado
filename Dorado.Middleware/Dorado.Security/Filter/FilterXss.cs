using Microsoft.Security.Application;

namespace Dorado.Security.Filter
{
    public class FilterXss
    {
        public static string FilterXSS(string input)
        {
            return Sanitizer.GetSafeHtmlFragment(input);
        }

        public static string Outputcoding(string input, OutputCode Opcoding)
        {
            string OutputString = string.Empty;
            if (input == null || input.Trim() == string.Empty)
            {
                return string.Empty;
            }
            switch (Opcoding)
            {
                case OutputCode.HtmlEncode:
                    {
                        OutputString = Encoder.HtmlEncode(input);
                        break;
                    }
                case OutputCode.HtmlAttributeEncode:
                    {
                        OutputString = Encoder.HtmlAttributeEncode(input);
                        break;
                    }
                case OutputCode.JavaScriptEncode:
                    {
                        OutputString = Encoder.JavaScriptEncode(input);
                        break;
                    }
                case OutputCode.UrlEncode:
                    {
                        OutputString = Encoder.UrlEncode(input);
                        break;
                    }
                case OutputCode.XmlAttributeEncode:
                    {
                        OutputString = Encoder.XmlAttributeEncode(input);
                        break;
                    }
                case OutputCode.XmlEncode:
                    {
                        OutputString = Encoder.XmlEncode(input);
                        break;
                    }
            }
            return OutputString;
        }
    }
}