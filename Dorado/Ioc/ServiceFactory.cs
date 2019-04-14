using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dorado.Ioc
{
    public class ServiceFactoryWrapper : IServiceFactory
    {
        static ServiceFactoryWrapper ( )
        {
            Instance = new ServiceFactoryWrapper( );
        }

        public static IServiceFactory Instance { get; private set; }
        #region IServiceFactory 成员
        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <returns></returns>
        public object CreateInstance ( Type type )
        {
            MethodInfo mi = ServiceCache.Get( type );
            try
            {
                object srv = mi.Invoke( null , null );
                return srv;
            }
            catch ( Exception error )
            {
                throw new ServiceNotFountException( type , error );
            }
        }
        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public T CreateInstance<T> ( )
        {
            return ( T ) CreateInstance( typeof( T ) );
        }

        #endregion
    }
    /// <summary>
    /// 接口服务的缓存
    /// </summary>
    public static class ServiceCache
    {
        static ServiceCache ( )
        {
            SyncRoot = new object( );
            _cache = new Dictionary<Type , MethodInfo>( 33 );
            _baseType = typeof( ServiceCache );
        }
        /// <summary>
        /// 常量定义
        /// </summary>
        public static class Consts
        {
            /// <summary>
            /// 创建实例
            /// </summary>
            public const string CreateInstance = "CreateInstance";
        }
        public static object SyncRoot { get; private set; }
        static Type _baseType;
        static Dictionary<Type , MethodInfo> _cache;
        public static MethodInfo Get ( Type type )
        {
            lock ( SyncRoot )
            {
                MethodInfo mi = null;
                if ( !_cache.TryGetValue( type , out mi ) )
                {
                    Type gtype = _baseType.MakeGenericType( new Type[ ] { type } );
                    mi = gtype.GetMethod( Consts.CreateInstance , BindingFlags.Static | BindingFlags.Public );
                    if ( mi == null )
                    {
                        throw new NullReferenceException( "未能获取服务实例" );
                    }
                    _cache.Add( type , mi );
                }
                return mi;
            }
        }
    }
}
