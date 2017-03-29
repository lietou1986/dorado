namespace Dorado.Web.FastViewEngine
{
    public class FastViewMemoryResult : FastViewResult
    {
        public FastViewMemoryResult()
        {
            base.FastViewEngine = FastViewEngineFactory.MemoryEngine;
        }
    }
}