namespace Dorado.Security.Crypto
{
    public class BS_Rc4
    {
        public static byte[] Rc4Encrypt(byte[] Encrypt, ProvidesKey Pkey, string NewKey = null)
        {
            RC4Crypto rc4Cryto = new RC4Crypto();
            string Password = string.Empty;
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        Password = "F969DD3C9G0A886622H6";
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        Password = NewKey;
                        break;
                    }
            }
            if (NewKey == null)
            {
                Password = "F969DD3C9G0A886622H6";
            }
            return rc4Cryto.EncryptEx(Encrypt, Password);
        }

        public static byte[] Rc4Decrypt(byte[] Decrypt, ProvidesKey Pkey, string NewKey = null)
        {
            RC4Crypto rc4Cryoto = new RC4Crypto();
            string Password = string.Empty;
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        Password = "F969DD3C9G0A886622H6";
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        Password = NewKey;
                        break;
                    }
            }
            if (NewKey == null)
            {
                Password = "F969DD3C9G0A886622H6";
            }
            return rc4Cryoto.DecryptEx(Decrypt, Password);
        }
    }
}