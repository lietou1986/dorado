using Microsoft.Ajax.Utilities;
using System.Collections.Generic;

namespace Dorado.Web
{
    public static class MicrosoftAjaxMinifer
    {
        public static Minifier Minifier
        {
            get;
            set;
        }

        public static CodeSettings JavascriptSetting
        {
            get;
            set;
        }

        public static CssSettings CssSetting
        {
            get;
            set;
        }

        public static ICollection<ContextError> Errors
        {
            get
            {
                return Minifier.ErrorList ?? new List<ContextError>();
            }
        }

        public static int WarningLevel
        {
            get
            {
                return Minifier.WarningLevel;
            }
            set
            {
                Minifier.WarningLevel = value;
            }
        }

        static MicrosoftAjaxMinifer()
        {
            Minifier = new Minifier();
            Minifier.WarningLevel = 2;
            JavascriptSetting = new CodeSettings
            {
                LocalRenaming = LocalRenaming.KeepAll,
                MinifyCode = true,
                StripDebugStatements = true
            };
            CssSetting = new CssSettings();
        }

        public static string MinifyJavaScript(string javascript)
        {
            return Minifier.MinifyJavaScript(javascript, JavascriptSetting);
        }

        public static string MinifyCss(string css)
        {
            return Minifier.MinifyStyleSheet(css, CssSetting);
        }
    }
}