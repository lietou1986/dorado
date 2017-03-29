namespace Dorado.Core.SuperCache
{
    public interface IVolatileToken
    {
        bool IsCurrent { get; }
    }
}