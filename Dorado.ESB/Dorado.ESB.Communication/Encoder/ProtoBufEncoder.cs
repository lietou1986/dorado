using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class ProtoBufEncoder : CustomEncoder
    {
        public override string ContentType
        {
            get { return "application/protobuf; charset=utf-8"; }
        }

        public override string MediaType
        {
            get { return "application/protobuf"; }
        }

        public override MessageVersion MessageVersion
        {
            get { return MessageVersion.None; }
        }

        private static List<string> supportedContentType = new List<string>() {
                "application/protobuf", "text/javascript", "application/x-www-form-urlencoded", "text/html"
            };

        public override bool IsContentTypeSupported(string contentType)
        {
            contentType = contentType.ToLower();
            foreach (string type in supportedContentType)
            {
                if (contentType.IndexOf(type) == 0)
                    return true;
            }
            return false;
        }

        public ProtoBufEncoder()
            : base(new ProtoBufSerializer())
        {
        }
    }
}