using System;

namespace Dorado.Web.FastViewEngine
{
    public static class FastViewEngineFactory
    {
        public static IFastViewEngine FileEngine
        {
            get;
            private set;
        }

        public static IFastViewEngine MemoryEngine
        {
            get;
            private set;
        }

        static FastViewEngineFactory()
        {
            FileEngine = CreateFileEngine(AppDomain.CurrentDomain.BaseDirectory, true);
            MemoryEngine = CreateMemoryEngine(true);
        }

        public static IFastViewEngine CreateFileEngine(string templateDirectory, bool cacheTemplate)
        {
            return new FastViewFileEngine(templateDirectory, cacheTemplate);
        }

        public static IFastViewEngine CreateFileEngine(bool cacheTemplate)
        {
            return CreateFileEngine(AppDomain.CurrentDomain.BaseDirectory, cacheTemplate);
        }

        public static IFastViewEngine CreateFileEngine()
        {
            return CreateFileEngine(true);
        }

        public static IFastViewEngine CreateMemoryEngine(bool cacheTemplate)
        {
            return null;
        }

        public static IFastViewEngine CreateMemoryEngine()
        {
            return CreateMemoryEngine(true);
        }
    }
}