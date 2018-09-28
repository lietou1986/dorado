using System.ComponentModel;

namespace Dorado
{
    public enum ResultStatus
    {
        [Description("Ok")]
        OK = 200,

        [Description("Bad Request")]
        BadRequest = 400,

        [Description("Unauthorized")]
        Unauthorized = 401,

        [Description("Forbidden")]
        Forbidden = 403,

        [Description("Data Not Found")]
        NotFound = 404,

        [Description("Internal Error")]
        Error = 500,

        [Description("Not Supported")]
        NotSupported = 505,

        [Description("Config Error")]
        ConfigError = 601,

        [Description("Third Party Api Error")]
        ThirdPartyApiError = 9000
    }
}