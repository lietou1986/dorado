namespace Dorado.Web.Fileset
{
    public static class StaticFileExtension
    {
        public static bool IsNull(this StaticFile file)
        {
            return file == null;
        }

        public static bool NotNull(this StaticFile file)
        {
            return file != null;
        }

        public static string GetParttenCache(this StaticFile file)
        {
            if (file != null)
            {
                return file.ParttenCache;
            }
            return string.Empty;
        }
    }
}