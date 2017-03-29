using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;

namespace Dorado.ESB.Communication
{
    public static class UriTemplateHelper
    {
        private const string UriTemplateMatchKey = "UriTemplateMatchResults";

        public static UriTemplate GetUriTemplate(OperationDescription operation)
        {
            string template = GetUriTemplateString(operation);
            return new UriTemplate(template);
        }

        public static UriTemplateMatch GetUriTemplateMatch(Message message, UriTemplate template, Uri baseAddress)
        {
            object obj;
            if (message.Properties.TryGetValue(UriTemplateMatchKey, out obj))
                return obj as UriTemplateMatch;
            else if (message.Headers.To != null && message.Headers.To.IsAbsoluteUri)
                return template.Match(baseAddress, message.Headers.To);
            return null;
        }

        private static string GetUriTemplateString(OperationDescription operation)
        {
            string template = GetWebUriTemplate(operation);

            if ((template == null) && (GetWebMethod(operation) == "GET"))
                template = GetDefaultTemplate(operation);

            if (template == null)
                template = operation.Name;

            return template;
        }

        private static string GetWebUriTemplate(OperationDescription operation)
        {
            WebGetAttribute get = operation.Behaviors.Find<WebGetAttribute>();
            if (get != null) return get.UriTemplate;

            WebInvokeAttribute invoke = operation.Behaviors.Find<WebInvokeAttribute>();
            if (invoke != null) return invoke.UriTemplate;

            return null;
        }

        private static string GetDefaultTemplate(OperationDescription operation)
        {
            StringBuilder builder = new StringBuilder(operation.Name);
            if (!IsUntypedMessage(operation.Messages[0]))
            {
                builder.Append("?");
                foreach (MessagePartDescription part in operation.Messages[0].Body.Parts)
                    builder.AppendFormat("{0}={{0}}&", part.Name);
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        public static bool IsUntypedMessage(MessageDescription message)
        {
            if (message == null) return false;
            if (message.Body.ReturnValue != null)
                return (message.Body.Parts.Count == 0) && (message.Body.ReturnValue.Type == typeof(Message));
            else
                return (message.Body.Parts.Count == 1) && (message.Body.Parts[0].Type == typeof(Message));
        }

        private static string GetWebMethod(OperationDescription operation)
        {
            WebGetAttribute get = operation.Behaviors.Find<WebGetAttribute>();
            if (get != null) return "GET";

            WebInvokeAttribute invoke = operation.Behaviors.Find<WebInvokeAttribute>();
            if (invoke == null) return "POST";

            return invoke.Method ?? "POST";
        }
    }
}