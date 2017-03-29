namespace Dorado.Web.FastViewEngine
{
    public class FastViewFileResult : FastViewResult
    {
        public FastViewFileResult()
        {
            base.FastViewEngine = FastViewEngineFactory.FileEngine;
        }
    }
}