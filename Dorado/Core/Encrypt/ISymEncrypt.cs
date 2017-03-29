namespace Dorado.Core.Encrypt
{
    public interface ISymEncrypt
    {
        string Encrypt(string source);

        string Decrypt(string source);
    }
}