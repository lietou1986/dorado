namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public interface IWcfClientFactory<T> where T : class
    {
        System.ServiceModel.ClientBase<T> CreateProxy();
    }
}