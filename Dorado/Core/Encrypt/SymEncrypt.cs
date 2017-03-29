using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dorado.Core.Encrypt
{
    public class SymEncrypt : ISymEncrypt
    {
        private string _key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
        private string _iv = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
        private SymEncryptionAlgorithm _algorithm = SymEncryptionAlgorithm.Rijndael;
        private SymmetricAlgorithm _cryptAlgorithm;

        private ICryptoTransform Encryptor
        {
            get
            {
                return this._cryptAlgorithm.CreateEncryptor();
            }
        }

        private ICryptoTransform Decryptor
        {
            get
            {
                return this._cryptAlgorithm.CreateDecryptor();
            }
        }

        public SymEncrypt(string key, string iv, SymEncryptionAlgorithm algorithm)
        {
            if (!string.IsNullOrEmpty(key))
            {
                this._key = key;
            }
            if (!string.IsNullOrEmpty(iv))
            {
                this._iv = iv;
            }
            this._algorithm = algorithm;
            this._cryptAlgorithm = this.GetSymAlgorithm();
            this._cryptAlgorithm.Key = this.GetLegalKey();
            this._cryptAlgorithm.IV = this.GetLegalIV();
        }

        private SymmetricAlgorithm GetSymAlgorithm()
        {
            switch (this._algorithm)
            {
                case SymEncryptionAlgorithm.DES:
                    {
                        return new DESCryptoServiceProvider();
                    }
                case SymEncryptionAlgorithm.RC2:
                    {
                        return new RC2CryptoServiceProvider();
                    }
                case SymEncryptionAlgorithm.Rijndael:
                    {
                        return new RijndaelManaged();
                    }
                case SymEncryptionAlgorithm.TripleDes:
                    {
                        return new TripleDESCryptoServiceProvider();
                    }
                default:
                    {
                        return new RijndaelManaged();
                    }
            }
        }

        private byte[] GetLegalKey()
        {
            try
            {
                this._cryptAlgorithm.GenerateKey();
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace);
            }
            byte[] tempKey = this._cryptAlgorithm.Key;
            int keyLength = tempKey.Length;
            string key = this._key;
            if (this._key.Length > keyLength)
            {
                key = key.Substring(0, keyLength);
            }
            else
            {
                key = key.PadRight(keyLength, ' ');
            }
            return Encoding.ASCII.GetBytes(key);
        }

        private byte[] GetLegalIV()
        {
            string iv = this._iv;
            this._cryptAlgorithm.GenerateIV();
            byte[] bytTemp = this._cryptAlgorithm.IV;
            int ivLength = bytTemp.Length;
            if (iv.Length > ivLength)
            {
                iv = iv.Substring(0, ivLength);
            }
            else
            {
                iv = iv.PadRight(ivLength, ' ');
            }
            return Encoding.ASCII.GetBytes(iv);
        }

        public string Encrypt(string source)
        {
            byte[] byteIn = Encoding.UTF8.GetBytes(source);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cs = new CryptoStream(memoryStream, this.Encryptor, CryptoStreamMode.Write);
            cs.Write(byteIn, 0, byteIn.Length);
            cs.FlushFinalBlock();
            memoryStream.Close();
            byte[] bytOut = memoryStream.ToArray();
            memoryStream.Close();
            return Convert.ToBase64String(bytOut);
        }

        public string Decrypt(string source)
        {
            byte[] byteIn = Convert.FromBase64String(source);
            MemoryStream ms = new MemoryStream(byteIn, 0, byteIn.Length);
            CryptoStream cs = new CryptoStream(ms, this.Decryptor, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            string result = sr.ReadToEnd();
            cs.Close();
            sr.Close();
            return result;
        }
    }
}