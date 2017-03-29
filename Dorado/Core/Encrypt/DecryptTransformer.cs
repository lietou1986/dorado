using System.Security.Cryptography;
using System.Text;

namespace Dorado.Core.Encrypt
{
    public class DecryptTransformer
    {
        private EncryptionAlgorithm algorithmID;
        private string SecurityKey = "";
        private byte[] IV;
        private bool bHasIV;

        public EncryptionAlgorithm EncryptionAlgorithm
        {
            get
            {
                return this.algorithmID;
            }
            set
            {
                this.algorithmID = value;
            }
        }

        public DecryptTransformer(EncryptionAlgorithm algID)
        {
            this.algorithmID = algID;
        }

        public DecryptTransformer(EncryptionAlgorithm algID, byte[] iv)
        {
            this.algorithmID = algID;
            this.IV = iv;
            this.bHasIV = true;
        }

        public void SetSecurityKey(string Key)
        {
            this.SecurityKey = Key;
        }

        public ICryptoTransform GetCryptoTransform()
        {
            bool bHasSecuityKey = false;
            if (this.SecurityKey.Length != 0)
            {
                bHasSecuityKey = true;
            }
            byte[] key = Encoding.ASCII.GetBytes(this.SecurityKey);
            switch (this.algorithmID)
            {
                case EncryptionAlgorithm.DES:
                    {
                        DES des = new DESCryptoServiceProvider();
                        if (bHasSecuityKey)
                        {
                            des.Key = key;
                        }
                        if (this.bHasIV)
                        {
                            des.IV = this.IV;
                        }
                        return des.CreateDecryptor();
                    }
                case EncryptionAlgorithm.Rc2:
                    {
                        RC2 rc = new RC2CryptoServiceProvider();
                        if (bHasSecuityKey)
                        {
                            rc.Key = key;
                        }
                        if (this.bHasIV)
                        {
                            rc.IV = this.IV;
                        }
                        return rc.CreateDecryptor();
                    }
                case EncryptionAlgorithm.Rijndael:
                    {
                        Rijndael rj = new RijndaelManaged();
                        if (bHasSecuityKey)
                        {
                            rj.Key = key;
                        }
                        if (this.bHasIV)
                        {
                            rj.IV = this.IV;
                        }
                        return rj.CreateDecryptor();
                    }
                case EncryptionAlgorithm.TripleDes:
                    {
                        TripleDES tDes = new TripleDESCryptoServiceProvider();
                        if (bHasSecuityKey)
                        {
                            tDes.Key = key;
                        }
                        if (this.bHasIV)
                        {
                            tDes.IV = this.IV;
                        }
                        return tDes.CreateDecryptor();
                    }
                default:
                    {
                        throw new CryptographicException("Algorithm ID '" + this.algorithmID + "' not supported.");
                    }
            }
        }
    }
}