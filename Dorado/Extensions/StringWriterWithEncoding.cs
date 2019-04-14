using System;
using System.IO;
using System.Text;

namespace Dorado.Extensions
{
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public override Encoding Encoding
        {
            get
            {
                if (this._encoding != null)
                {
                    return this._encoding;
                }
                return base.Encoding;
            }
        }

        public StringWriterWithEncoding()
        {
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public StringWriterWithEncoding(StringBuilder stringBuilder) : base(stringBuilder)
        {
        }

        public StringWriterWithEncoding(StringBuilder stringBuilder, IFormatProvider formatProvider) : base(stringBuilder, formatProvider)
        {
        }

        public StringWriterWithEncoding(Encoding encoding)
        {
            this._encoding = encoding;
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding) : base(formatProvider)
        {
            this._encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder stringBuilder, Encoding encoding) : base(stringBuilder)
        {
            this._encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder stringBuilder, IFormatProvider formatProvider, Encoding encoding) : base(stringBuilder, formatProvider)
        {
            this._encoding = encoding;
        }
    }
}