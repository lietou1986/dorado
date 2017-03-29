using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Json
{
    public class JsonBodyWriter : BodyWriter
    {
        private object body;
        private XmlObjectSerializer serializer;

        public JsonBodyWriter(object body, XmlObjectSerializer serializer)
            : base(false)
        {
            if (body != null && serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.body = body;
            this.serializer = serializer;
        }

        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {
            if (body != null)
            {
                this.serializer.WriteObject(writer, body);
            }
        }

        public static BodyWriter GetBodyWriter(object body)
        {
            return new JsonBodyWriter(body, JsonObjectSerializer.Instance);
        }
    }
}