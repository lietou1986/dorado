using System.Configuration;

namespace Dorado.ESB.Common.Utility
{
    public class APIUri
    {
        public static string serverBaseURL = ConfigurationManager.AppSettings["ServerBaseURL"];

        public static APIUri CurrentUserUri = new APIUri("currentuser");
        public static APIUri UserUri = new APIUri("users/{0}");
        public static APIUri UserProfileUri = new APIUri("users/{0}/profile");
        public static APIUri FriendsUri = new APIUri("users/{0}/friends");
        public static APIUri DetailsUri = new APIUri("users/{0}/details");
        public static APIUri InterestsUri = new APIUri("users/{0}/interests");
        public static APIUri BlogUri = new APIUri("users/{0}/blog");
        public static APIUri CommentsUri = new APIUri("users/{0}/comments");
        public static APIUri CommentsPagingUri = new APIUri("users/{0}/comments?page={1}&page_size={2}");
        public static APIUri GroupsUri = new APIUri("users/{0}/groups");
        public static APIUri SchoolsUri = new APIUri("users/{0}/schools");
        public static APIUri CompaniesUri = new APIUri("users/{0}/companies");
        public static APIUri AlbumsUri = new APIUri("users/{0}/albums");
        public static APIUri AlbumsPagingUri = new APIUri("users/{0}/albums?page={1}&page_size={2}");
        public static APIUri FriendsPagingUri = new APIUri("users/{0}/friends?page={1}&page_size={2}");
        public static APIUri TopFriendsUri = new APIUri("users/{0}/friends/top");
        public static APIUri AlbumUri = new APIUri("users/{0}/albums/{1}");
        public static APIUri PhotosUri = new APIUri("users/{0}/albums/{1}/photos");
        public static APIUri PhotosPagingUri = new APIUri("users/{0}/albums/{1}/photos?page={2}&page_size={3}");
        public static APIUri PhotoUri = new APIUri("users/{0}/albums/{1}/photos/{2}");
        public static APIUri PhotoCommentsUri = new APIUri("users/{0}/albums/{1}/photos/{2}/comments");
        public static APIUri PhotoCommentUri = new APIUri("users/{0}/albums/{1}/photos/{2}/comments/{3}");
        public static APIUri PhotoCommentsPagingUri = new APIUri("users/{0}/albums/{1}/photos/{2}/comments?page={3}&page_size={4}");
        public static APIUri PostBulletinUri = new APIUri("bulletins");
        public static readonly char URI_DELIMITER = '/';
        public static readonly string QUERY_DELIMITER = "?";
        public static readonly string FORMAT_OUTPUT_TYPE = ".{0}";

        /// <summary>
        /// response output types enum
        /// </summary>
        public enum ResultOuputType
        {
            XML
            , JSON
        }

        private string template;

        /// <summary>
        /// original uri of the resource
        /// </summary>
        private string _URI;

        /// <summary>
        /// which output types to response
        /// </summary>
        private ResultOuputType _OutputType;

        public string Template
        {
            get { return template; }
            set { template = value; }
        }

        /// <summary>
        /// the original URI passed to the constructor for the resource
        /// </summary>
        public string URI
        {
            get { return _URI; }
            set { _URI = value; }
        }

        /// <summary>
        /// the desired output type of the response
        /// </summary>
        public ResultOuputType OutputType
        {
            get { return _OutputType; }
            set { _OutputType = value; }
        }

        /// <summary>
        /// Private so no one can create instances of this class.
        /// </summary>
        /// <param name="template"></param>
        private APIUri(string template)
        {
            //added for output of json
            this.URI = template;
            this.Template = string.Format("{0}/{1}", serverBaseURL, template);
        }

        public string Build(params object[] values)
        {
            return string.Format(this.Template, values);
        }

        /// <summary>
        /// build the URI with the specified response output result
        /// </summary>
        /// <param name="outputType"></param>
        /// <param name="serverBase"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string Build(ResultOuputType outputType, string serverBase, params object[] values)
        {
            string output = string.Format(FORMAT_OUTPUT_TYPE, outputType.ToString().ToLower());
            string templateString;

            if (this.URI.Contains(QUERY_DELIMITER))
            {
                templateString = this.URI.Substring(0, this.URI.IndexOf(QUERY_DELIMITER)) + output + this.URI.Substring(this.URI.IndexOf(QUERY_DELIMITER));
            }
            else
            {
                templateString = this.URI + output;
            }

            string serverUrl = serverBaseURL;

            if (!string.IsNullOrEmpty(serverBase))
            {
                serverUrl = serverBase;
            }

            return serverUrl + URI_DELIMITER + string.Format(templateString, values);
        }
    }
}