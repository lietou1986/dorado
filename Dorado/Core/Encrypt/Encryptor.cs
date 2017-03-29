using System;
using System.IO;
using System.Security.Cryptography;

namespace Dorado.Core.Encrypt
{
    public class Encryptor
    {
        private EncryptEngine engin;
        public byte[] IV;

        public EncryptEngine EncryptEngine
        {
            get
            {
                return this.engin;
            }
            set
            {
                this.engin = value;
            }
        }

        public Encryptor(EncryptionAlgorithm algID, string key)
        {
            this.engin = new EncryptEngine(algID, key);
        }

        public string Encrypt(string MainString)
        {
            MemoryStream memory = new MemoryStream();
            CryptoStream stream = new CryptoStream(memory, this.engin.GetCryptTransform(), CryptoStreamMode.Write);
            StreamWriter streamwriter = new StreamWriter(stream);
            streamwriter.WriteLine(MainString);
            streamwriter.Close();
            stream.Close();
            this.IV = this.engin.Vector;
            byte[] buffer = memory.ToArray();
            memory.Close();
            return Convert.ToBase64String(buffer);
        }
    }
}