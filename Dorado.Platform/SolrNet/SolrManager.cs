using Dorado.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dorado.Platform.SolrNet
{
    public class SolrManager
    {
        private readonly string _solrUrl;

        public SolrManager(string address)
        {
            _solrUrl = address;
        }

        public Result CreateOrUpdateSolrDocuments(List<SolrDocument> documents)
        {
            if (documents == null || documents.Count == 0)
                throw new Exception("Telligent.Solr : No documents have been added for creating/updating.");

            var sb = new StringBuilder();
            sb.Append("<add>");
            foreach (var doc in documents)
                sb.Append(doc.GetXml());
            sb.Append("</add>");
            return Post(sb.ToString());
        }

        public Result Commit()
        {
            return Post("<commit/>");
        }

        public Result Optimize()
        {
            return Post("<optimize/>");
        }

        public Result Rollback()
        {
            return Post("<rollback/>");
        }

        public Result DeleteByQuery(string condition)
        {
            var sb = new StringBuilder();
            sb.Append("<delete>");
            sb.Append("<query>" + condition + "</query>");
            sb.Append("</delete>");
            return Post(sb.ToString());
        }

        public Result DeleteById(params string[] ids)
        {
            var sb = new StringBuilder();
            sb.Append("<delete>");
            foreach (var uniqueId in ids)
            {
                sb.Append("<id>" + uniqueId + "</id>");
            }
            sb.Append("</delete>");
            return Post(sb.ToString());
        }

        public Result Clear()
        {
            var sb = new StringBuilder();
            sb.Append("<delete>");
            sb.Append("<query>*:*</query>");
            sb.Append("</delete>");
            return Post(sb.ToString());
        }

        private Result Post(string data)
        {
            RequestService dataRequest = new RequestService(_solrUrl);
            Result<ResponseHeader> result = dataRequest.Request<ResponseHeader>(MimeType.Xml, data, 60);

            if (result.IsFail())
                return new Result(ResultStatus.Error, result.Message);

            ResponseHeader responseHeader = result.Data;
            if (responseHeader.Error != null)
                return new Result(ResultStatus.Error, responseHeader.Error.Msg);

            return Result.OK;
        }
    }
}