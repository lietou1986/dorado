using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.ESB.Json
{
    public static class JsonFormatHelper
    {
        #region fields

        private const string UriTemplateMatchKey = "UriTemplateMatchResults";
        private const string UseDefaultEncoderPropName = "UseDefaultEncoder";
        private static readonly WebBodyFormatMessageProperty JsonMessageProperty;

        private const string pattern1 = "\\[({\"Key\")";
        private const string pattern2 = "(\"Value\"\\s*:\\s*([^}]*)})\\]";
        private const string pattern3 = "{\"Key\"\\s*:\\s*(?'key'[^,]*)\\s*,\\s*\"Value\"\\s*:\\s*(?'value'[^}]*)}";
        private static readonly Regex regex1;
        private static readonly Regex regex2;
        private static readonly Regex regex3;

        #endregion fields

        #region cctor

        static JsonFormatHelper()
        {
            JsonMessageProperty = new WebBodyFormatMessageProperty(WebContentFormat.Json);

            regex1 = new Regex(pattern1, RegexOptions.Compiled);
            regex2 = new Regex(pattern2, RegexOptions.Compiled);
            regex3 = new Regex(pattern3, RegexOptions.Compiled);
        }

        #endregion cctor

        public static UriTemplateMatch GetUriTemplateMatch(Message message, UriTemplate uriTemplate, Uri baseAddress)
        {
            UriTemplateMatch result = null;

            if (message.Properties.ContainsKey(UriTemplateMatchKey))
            {
                result = message.Properties[UriTemplateMatchKey] as UriTemplateMatch;
            }
            else if ((message.Headers.To != null) && message.Headers.To.IsAbsoluteUri)
            {
                result = uriTemplate.Match(baseAddress, message.Headers.To);
            }

            return result;
        }

        public static void SuppressReplyEntityBody(Message message)
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
            }
            else
            {
                object obj = null;
                message.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj);
                HttpResponseMessageProperty property = obj as HttpResponseMessageProperty;
                if (property == null)
                {
                    property = new HttpResponseMessageProperty();
                    message.Properties[HttpResponseMessageProperty.Name] = property;
                }
                property.SuppressEntityBody = true;
            }
        }

        public static void AttachProperty(Message message, string name, object property)
        {
            message.Properties.Add(name, property);
        }

        public static void AttachBodyFormatProperty(Message message, bool isRequest)
        {
            AttachProperty(message, "WebBodyFormatMessageProperty", JsonMessageProperty);
        }

        public static void AttachEncoderProperty(Message message, string operation)
        {
            JsonFormatHelper.AttachProperty(message, UseDefaultEncoderPropName, true);
        }

        public static UriTemplate GetUriTemplate(OperationDescription operationDescription)
        {
            string utSring = GetUriTemplateString(operationDescription);
            return new UriTemplate(utSring);
        }

        public static void Populate(OperationDescription operationDescription, UriTemplate uriTemplate, QueryStringConverter qsc,
            out int utVarCount, out Dictionary<int, string> pathMapping, out Dictionary<int, KeyValuePair<string, Type>> queryMapping)
        {
            utVarCount = uriTemplate.PathSegmentVariableNames.Count + uriTemplate.QueryValueVariableNames.Count;

            pathMapping = new Dictionary<int, string>();
            queryMapping = new Dictionary<int, KeyValuePair<string, Type>>();

            List<string> pathVarNames = new List<string>(uriTemplate.PathSegmentVariableNames);
            List<string> queryVarNames = new List<string>(uriTemplate.QueryValueVariableNames);
            Dictionary<string, byte> alreadyFoundNames = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < operationDescription.Messages[0].Body.Parts.Count; ++i)
            {
                MessagePartDescription description = operationDescription.Messages[0].Body.Parts[i];
                string name = description.Name;
                if (alreadyFoundNames.ContainsKey(name))
                {
                    throw new InvalidOperationException("Duplicate Uri template var name found: " + name);
                }

                List<string> list = new List<string>(pathVarNames);
                foreach (string str in list)
                {
                    if (string.Compare(name, str, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (description.Type != typeof(string))
                        {
                            throw new InvalidOperationException("Uri template path var must be string");
                        }
                        pathMapping.Add(i, name);
                        alreadyFoundNames.Add(name, 0);
                        pathVarNames.Remove(str);
                    }
                }

                list = new List<string>(queryVarNames);
                foreach (string str in list)
                {
                    if (string.Compare(name, str, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (!qsc.CanConvert(description.Type))
                        {
                            throw new InvalidOperationException("Unexpected type " + description.Type + ", Uri template query var must be convertible");
                        }
                        queryMapping.Add(i, new KeyValuePair<string, Type>(name, description.Type));
                        alreadyFoundNames.Add(name, 0);
                        queryVarNames.Remove(str);
                    }
                }
            }

            if (pathVarNames.Count != 0 || queryVarNames.Count != 0)
            {
                throw new InvalidOperationException("Uri template missing var");
            }
        }

        public static bool IsJsonMethod(OperationDescription operationDescription)
        {
            WebGetAttribute webGet = operationDescription.Behaviors.Find<WebGetAttribute>();
            if (webGet != null && webGet.ResponseFormat == WebMessageFormat.Json)
            {
                return true;
            }

            WebInvokeAttribute webInvoke = operationDescription.Behaviors.Find<WebInvokeAttribute>();
            if (webInvoke != null && webInvoke.ResponseFormat == WebMessageFormat.Json)
            {
                return true;
            }

            return false;
        }

        public static bool IsWebGetMethod(OperationDescription description)
        {
            WebGetAttribute webGet = description.Behaviors.Find<WebGetAttribute>();
            if (webGet != null) return true;

            return false;
        }

        public static string GetStandardJsonString(string json)
        {
            string result = regex1.Replace(json, "{$1");
            result = regex2.Replace(result, "$1}");
            result = regex3.Replace(result, "${key}:${value}");
            result = result.Replace('[', '{');
            result = result.Replace(']', '}');
            return result;
        }

        public static bool ShouldUseDefaultEncoder(Message message)
        {
            object o = null;
            if (message.Properties.TryGetValue(UseDefaultEncoderPropName, out o))
            {
                return (bool)o;
            }
            else
            {
                return false;
            }
        }

        #region helper

        private static string GetUriTemplateString(OperationDescription operationDescription)
        {
            string webUriTemplate = GetWebUriTemplate(operationDescription);

            if ((webUriTemplate == null) && (GetWebMethod(operationDescription) == "GET"))
            {
                webUriTemplate = GetDefaultUTString(operationDescription);
            }

            if (webUriTemplate == null)
            {
                webUriTemplate = operationDescription.Name;
            }

            return webUriTemplate;
        }

        private static string GetWebUriTemplate(OperationDescription operationDescription)
        {
            WebGetAttribute webGet = operationDescription.Behaviors.Find<WebGetAttribute>();
            if (webGet != null) return webGet.UriTemplate;

            WebInvokeAttribute webInvoke = operationDescription.Behaviors.Find<WebInvokeAttribute>();
            if (webInvoke != null) return webInvoke.UriTemplate;

            return null;
        }

        private static string GetDefaultUTString(OperationDescription operationDescription)
        {
            StringBuilder builder = new StringBuilder(operationDescription.Name);

            if (!IsUntypedMessage(operationDescription.Messages[0]))
            {
                builder.Append("?");
                foreach (MessagePartDescription description in operationDescription.Messages[0].Body.Parts)
                {
                    string name = description.Name;
                    builder.Append(name);
                    builder.Append("={");
                    builder.Append(name);
                    builder.Append("}&");
                }
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }

        private static bool IsUntypedMessage(MessageDescription message)
        {
            if (message == null) return false;

            if (message.Body.ReturnValue != null)
            {
                return ((message.Body.Parts.Count == 0) && (message.Body.ReturnValue.Type == typeof(Message)));
            }
            else
            {
                return ((message.Body.Parts.Count == 1) && (message.Body.Parts[0].Type == typeof(Message)));
            }
        }

        private static string GetWebMethod(OperationDescription operationDescription)
        {
            WebGetAttribute webGet = operationDescription.Behaviors.Find<WebGetAttribute>();
            if (webGet != null) return "GET";

            WebInvokeAttribute webInvoke = operationDescription.Behaviors.Find<WebInvokeAttribute>();
            if (webInvoke == null) return "POST";

            return webInvoke.Method ?? "POST";
        }

        #endregion helper
    }
}