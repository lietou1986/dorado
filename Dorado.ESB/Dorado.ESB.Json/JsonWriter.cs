using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace Dorado.ESB.Json
{
    public class JsonWriter : XmlDictionaryWriter, IXmlJsonWriterInitializer
    {
        #region fields

        private WriteState writeState;
        private JsonNodeWriter nodeWriter;

        #endregion fields

        #region IXmlJsonWriterInitializer Members

        public void SetOutput(Stream stream, Encoding encoding, bool ownsStream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding != null && encoding.WebName != Encoding.UTF8.WebName)
            {
                string errString = string.Format("Unexpected encoding: {0}, Only UTF8 encoding is supported", encoding.EncodingName);
                throw new ArgumentException(errString);
            }

            if (nodeWriter == null)
            {
                nodeWriter = new JsonNodeWriter();
            }
            nodeWriter.SetOutput(stream, ownsStream);

            InitializeWriter();
        }

        private void InitializeWriter()
        {
            writeState = WriteState.Start;
        }

        #endregion IXmlJsonWriterInitializer Members

        #region XmlDictionaryWriter implementation code

        public override void WriteStartDocument()
        {
            CheckCloseState();
            RequireState(WriteState.Start);
        }

        public override void WriteEndDocument()
        {
            CheckCloseState();
        }

        public override void WriteRaw(string data)
        {
            nodeWriter.WriteJsonString(data);
        }

        public override void Close()
        {
            CheckCloseState();

            try
            {
                nodeWriter.Flush();
                nodeWriter.Close();
            }
            finally
            {
                writeState = WriteState.Closed;
            }
        }

        public override void Flush()
        {
            CheckCloseState();
            nodeWriter.Flush();
        }

        #endregion XmlDictionaryWriter implementation code

        #region helper

        private void CheckCloseState()
        {
            if (writeState == WriteState.Closed)
            {
                throw new ApplicationException("JsonWriter closed");
            }
        }

        private void RequireState(WriteState requiredState)
        {
            if (writeState != requiredState)
            {
                string errString = string.Format("JsonWriter is in an invalid write state: {0}, required: {1}",
                    writeState, requiredState);
                throw new ApplicationException(errString);
            }
        }

        #endregion helper

        #region not supported XmlDictionaryWriter members

        public override string LookupPrefix(string ns)
        {
            throw new NotSupportedException();
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteCData(string text)
        {
            throw new NotSupportedException();
        }

        public override void WriteCharEntity(char ch)
        {
            throw new NotSupportedException();
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteComment(string text)
        {
            throw new NotSupportedException();
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            throw new NotSupportedException();
        }

        public override void WriteEndAttribute()
        {
            throw new NotSupportedException();
        }

        public override void WriteEntityRef(string name)
        {
            throw new NotSupportedException();
        }

        public override void WriteFullEndElement()
        {
            throw new NotSupportedException();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            throw new NotSupportedException();
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            throw new NotSupportedException();
        }

        public override void WriteStartDocument(bool standalone)
        {
            throw new NotSupportedException();
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            throw new NotSupportedException();
        }

        public override void WriteEndElement()
        {
            throw new NotSupportedException();
        }

        public override WriteState WriteState
        {
            get { throw new NotSupportedException(); }
        }

        public override void WriteString(string text)
        {
            throw new NotSupportedException();
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotSupportedException();
        }

        public override void WriteWhitespace(string ws)
        {
            throw new NotSupportedException();
        }

        #endregion not supported XmlDictionaryWriter members

        #region JsonNodeWriter

        private class JsonNodeWriter
        {
            #region fields

            private Stream stream;
            private bool ownsStream;

            #endregion fields

            public void SetOutput(Stream stream, bool ownsStream)
            {
                this.stream = stream;
                this.ownsStream = ownsStream;
            }

            public void WriteJsonString(string jsonString)
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                stream.Write(data, 0, data.Length);
            }

            public void Close()
            {
                if (stream != null)
                {
                    if (ownsStream) stream.Close();
                    stream = null;
                }
            }

            public void Flush()
            {
                stream.Flush();
            }
        }

        #endregion JsonNodeWriter
    }
}