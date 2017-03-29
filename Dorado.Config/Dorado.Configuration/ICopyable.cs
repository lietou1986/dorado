namespace Dorado.Configuration
{
    /// <summary>
    /// 用于拷贝一个配置对象到一个单例的配置类
    /// 默认配置处理程序只拷贝类的公共属性/字段, 但如果配置类具有一些特殊的逻辑需要复制就需要使用此接口
    /// </summary>
    public interface ICopyable
    {
        /// <summary>
        ///拷贝属性到目标对象
        /// </summary>
        /// <param name="destObject">目标对象</param>
        void CopyTo(object destObject);
    }
}