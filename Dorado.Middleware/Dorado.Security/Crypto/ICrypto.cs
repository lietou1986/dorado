namespace Dorado.Security.Crypto
{
    internal interface ICrypto
    {
        byte[] EncryptEx(byte[] data, string pass);

        byte[] DecryptEx(byte[] data, string pass);
    }
}