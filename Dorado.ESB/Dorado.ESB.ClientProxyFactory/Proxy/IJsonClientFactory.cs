namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public interface IJsonClientFactory<T>
    {
        T GetJsonProtocolObject(string baseUrl);
    }
}