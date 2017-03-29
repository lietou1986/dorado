using Dorado.Configuration.EnvironmentRouter;

namespace Dorado.Configuration
{
    public interface IConfiguration
    {
        /// <summary>
        /// 应用程序名称
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// 运行环境
        /// </summary>
        NetworkEnvironment Environment { get; }
    }
}