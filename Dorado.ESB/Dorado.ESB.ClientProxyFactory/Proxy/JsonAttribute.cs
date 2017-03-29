using System;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public enum JsonMethod
    {
        Post,
        Get
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JsonAttribute : Attribute
    {
        private JsonMethod method = JsonMethod.Post;

        public JsonMethod Method
        {
            get
            {
                return method;
            }
            set
            {
                method = value;
            }
        }

        private string uriTemplate;

        public string UriTemplate
        {
            get
            {
                return uriTemplate;
            }
            set
            {
                uriTemplate = value;
            }
        }
    }
}