namespace Dorado.Platform.FileSystems.Media
{
    public interface IMimeTypeProvider
    {
        string GetMimeType(string path);
    }
}