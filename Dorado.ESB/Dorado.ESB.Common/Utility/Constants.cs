namespace Dorado.ESB.Common
{
    public static class Constants
    {
        public static class Authentication
        {
            public const string Dorado_WS_APIKEY = "C50CF2B2-B4E3-420a-9839-7966192982A1",
                                Dorado_WS_SHAREDPWDKEY = "F279A8C1-DD28-47fb-B9E6-AA2205C0EF78",
                                HTTP_AUTHORIZATION_HEADER = "Authorization",
                                HTTP_DATE_HEADER = "Date",
                                Dorado_AUTHHEADER_SOURCE = "Auth-Header",
                                TOKEN_HEADER_NAME = "AuthenticationToken",
                                TOKEN_HEADER_NAMESPACE = "urn:Dorado.Services",
                                Dorado_AUTHQUERYSTRING_SOURCE = "Auth-Querystring",
                                Dorado_APIKEY_QUERYPARAM = "api_key",
                                Dorado_TOKEN_QUERYPARAM = "token",
                                Dorado_SIGNATURE_QUERYPARAM = "api_sig",
                                Dorado_DATE_QUERYPARAM = "date",
                                Dorado_AUTH_PREFIX = "MWS",
                                Dorado_HEADER_PREFIX = "x-Dorado-",
                                Dorado_DATE_HEADER = "x-Dorado-date",
                                Dorado_CREDENTIAL_HEADER = "x-Dorado-credential",
                                Dorado_USERTOKEN_HEADER = "x-Dorado-usertoken",
                                Dorado_USERNAME = "UserName",
                                Dorado_PASSWORD = "Password",
                                Dorado_AUTH_HEADER_DELIMITER = ":",
                                Dorado_QUERYSTRING_DELIMITER = "&",
                                Dorado_QUERYSTRING_EQUALS = "=",
                                Dorado_QUERYSTRING_RETURN_URL = "next",
                                Dorado_QUERYSTRING_TOKEN = "token",
                                Dorado_COOKIE = "x-Dorado-COOKIE";
        }
    }
}