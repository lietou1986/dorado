namespace Dorado.Core.Encrypt
{
    public class WebFrameworkSymEncrypt
    {
        private class WebFrameworkSymEncryptConfig
        {
            public static string Key
            {
                get
                {
                    return string.Empty;
                }
            }

            public static string IV
            {
                get
                {
                    return string.Empty;
                }
            }

            public static SymEncryptionAlgorithm Algorithm
            {
                get
                {
                    return SymEncryptionAlgorithm.Rijndael;
                }
            }
        }

        private static readonly SymEncrypt instance = new SymEncrypt(WebFrameworkSymEncrypt.WebFrameworkSymEncryptConfig.Key, WebFrameworkSymEncrypt.WebFrameworkSymEncryptConfig.IV, WebFrameworkSymEncrypt.WebFrameworkSymEncryptConfig.Algorithm);

        public static SymEncrypt SymEncryptor
        {
            get
            {
                return WebFrameworkSymEncrypt.instance;
            }
        }
    }
}