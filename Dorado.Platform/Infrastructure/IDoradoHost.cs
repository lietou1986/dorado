namespace Dorado.Platform.Infrastructure
{
    public interface IDoradoHost
    {
        /// <summary>
        /// Called once on startup to configure app domain, and load/apply existing shell configuration
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called externally when there is explicit knowledge that the list of installed
        /// modules/extensions has changed, and they need to be reloaded.
        /// </summary>
        void ReloadExtensions();

        /// <summary>
        /// Called each time a request begins to offer a just-in-time reinitialization point
        /// </summary>
        void BeginRequest();

        /// <summary>
        /// Called each time a request ends to deterministically commit and dispose outstanding activity
        /// </summary>
        void EndRequest();
    }
}