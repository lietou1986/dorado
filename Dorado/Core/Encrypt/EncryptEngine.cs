using System.Security.Cryptography;
using System.Text;

namespace Dorado.Core.Encrypt
{
    public class EncryptEngine
    {
        private bool bWithKey;
        private EncryptionAlgorithm AlgoritmID;
        private string keyword = "";
        public byte[] Vector;

        public EncryptionAlgorithm EncryptionAlgorithm
        {
            get
            {
                return this.AlgoritmID;
            }
            set
            {
                this.AlgoritmID = value;
            }
        }

        public EncryptEngine(EncryptionAlgorithm AlgID, string Key)
        {
            if (Key.Length == 0)
            {
                this.bWithKey = false;
            }
            else
            {
                this.bWithKey = true;
            }
            this.keyword = Key;
            this.AlgoritmID = AlgID;
        }

        public ICryptoTransform GetCryptTransform()
        {
            byte[] key = Encoding.ASCII.GetBytes(this.keyword);
            switch (this.AlgoritmID)
            {
                case EncryptionAlgorithm.DES:
                    {
                        DES des = new DESCryptoServiceProvider();
                        des.Mode = CipherMode.CBC;
                        if (this.bWithKey)
                        {
                            des.Key = key;
                        }
                        this.Vector = des.IV;
                        return des.CreateEncryptor();
                    }
                case EncryptionAlgorithm.Rc2:
                    {
                        RC2 rc = new RC2CryptoServiceProvider();
                        rc.Mode = CipherMode.CBC;
                        if (this.bWithKey)
                        {
                            rc.Key = key;
                        }
                        this.Vector = rc.IV;
                        return rc.CreateEncryptor();
                    }
                case EncryptionAlgorithm.Rijndael:
                    {
                        Rijndael rj = new RijndaelManaged();
                        rj.Mode = CipherMode.CBC;
                        if (this.bWithKey)
                        {
                            rj.Key = key;
                        }
                        this.Vector = rj.IV;
                        return rj.CreateEncryptor();
                    }
                case EncryptionAlgorithm.TripleDes:
                    {
                        TripleDES tDes = new TripleDESCryptoServiceProvider();
                        tDes.Mode = CipherMode.CBC;
                        if (this.bWithKey)
                        {
                            tDes.Key = key;
                        }
                        this.Vector = tDes.IV;
                        return tDes.CreateEncryptor();
                    }
                default:
                    {
                        throw new CryptographicException("Algorithm " + this.AlgoritmID + " Not Supported!");
                    }
            }
        }

        public static bool ValidateKeySize(EncryptionAlgorithm algID, int Lenght)
        {
            switch (algID)
            {
                case EncryptionAlgorithm.DES:
                    {
                        DES des = new DESCryptoServiceProvider();
                        return des.ValidKeySize(Lenght);
                    }
                case EncryptionAlgorithm.Rc2:
                    {
                        RC2 rc = new RC2CryptoServiceProvider();
                        return rc.ValidKeySize(Lenght);
                    }
                case EncryptionAlgorithm.Rijndael:
                    {
                        Rijndael rj = new RijndaelManaged();
                        return rj.ValidKeySize(Lenght);
                    }
                case EncryptionAlgorithm.TripleDes:
                    {
                        TripleDES tDes = new TripleDESCryptoServiceProvider();
                        return tDes.ValidKeySize(Lenght);
                    }
                default:
                    {
                        throw new CryptographicException("Algorithm " + algID + " Not Supported!");
                    }
            }
        }
    }
}