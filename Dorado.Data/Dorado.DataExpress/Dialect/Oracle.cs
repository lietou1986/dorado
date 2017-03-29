using System;
using System.Text;

namespace Dorado.DataExpress.Dialect
{
    public class Oracle : BaseDialect
    {
        private const string OracleNewLine = "\n";
        private const string NameTempalte = "\"{0}\"";

        public override string NewLine
        {
            get
            {
                return "\n";
            }
        }

        public override string ParameterPrefix
        {
            get
            {
                return ":";
            }
        }

        public override string SystemNameTemplate
        {
            get
            {
                return "\"{0}\"";
            }
        }

        protected override string ConvertKeyword(string keyword)
        {
            if (keyword == null)
            {
                throw new ArgumentNullException("keyword");
            }
            return keyword.ToUpper();
        }

        public override string GetSystemName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Contains("."))
            {
                string[] names = name.Split(new char[]
				{
					'.'
				});
                StringBuilder sb = new StringBuilder(name.Length + names.Length * 4);
                string[] array = names;
                for (int i = 0; i < array.Length; i++)
                {
                    string s = array[i];
                    if (!string.IsNullOrEmpty(s))
                    {
                        sb.Append(base.GetSystemName(s.ToUpper())).Append(".");
                    }
                }
                return sb.ToString(0, sb.Length - 1);
            }
            return base.GetSystemName(name);
        }
    }
}