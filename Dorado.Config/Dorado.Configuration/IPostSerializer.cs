namespace Dorado.Configuration
{
    /// <summary>
    /// 用来在配置序列化完毕之后做一些特殊的业务逻辑处理
    /// 譬如一些特殊属性/字段在序列化后需要被计算
    /// 注意：不要用来处理静态字段
    /// </summary>
    public interface IPostSerializer
    {
        void PostSerializer();
    }
}