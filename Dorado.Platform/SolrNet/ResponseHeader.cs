﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using Newtonsoft.Json;

namespace Dorado.Platform.SolrNet
{
    public class Header
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("QTime")]
        public int QTime { get; set; }
    }

    public class Error
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class ResponseHeader
    {
        [JsonProperty("responseHeader")]
        public Header Header { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }
}