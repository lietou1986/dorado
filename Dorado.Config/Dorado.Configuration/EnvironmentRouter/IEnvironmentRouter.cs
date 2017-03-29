namespace Dorado.Configuration.EnvironmentRouter
{
    /// <summary>
    /// 环境路由接口
    /// </summary>
    public interface IEnvironmentRouter
    {
        NetworkEnvironment GetCurrentEnvironment();
    }

    public enum NetworkEnvironment
    {
        Prod,
        Dev,
        Test,
        Labs
    }
}