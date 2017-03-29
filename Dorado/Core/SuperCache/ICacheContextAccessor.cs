namespace Dorado.Core.SuperCache
{
    public interface ICacheContextAccessor
    {
        IAcquireContext Current { get; set; }
    }
}