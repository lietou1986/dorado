using System;
using System.IO;
using System.Security.Cryptography;

namespace Dorado.Core.Encrypt
{
    public class Decryptor
    {
        private EncryptionAlgorithm AlgoritmID;
        private byte[] IV;

        public DecryptTransformer DecryptTransformer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
            }
        }

        public Decryptor(EncryptionAlgorithm algID)
        {
            this.AlgoritmID = algID;
        }

        public Decryptor(EncryptionAlgorithm algID, byte[] iv)
        {
            this.AlgoritmID = algID;
            this.IV = iv;
        }

        public string Decrypt(string mainString, string key)
        {
            DecryptTransformer dt = new DecryptTransformer(this.AlgoritmID, this.IV);
            dt.SetSecurityKey(key);
            byte[] buffer = Convert.FromBase64String(mainString.Trim());
            MemoryStream ms = new MemoryStream(buffer);
            CryptoStream encStream = new CryptoStream(ms, dt.GetCryptoTransform(), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(encStream);
            string val = sr.ReadLine();
            sr.Close();
            encStream.Close();
            ms.Close();
            return val;
        }
    }
}