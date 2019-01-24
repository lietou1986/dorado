using System.Web;

namespace Dorado.Platform.Infrastructure
{
    public interface IBackgroundHttpContextFactory
    {
        HttpContext CreateHttpContext(string url);

        void InitializeHttpContext(string url);
    }
}