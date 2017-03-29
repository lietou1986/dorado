namespace Dorado.Security.Crypto
{
    public class CryptoBase
    {
        public enum EncoderMode
        {
            Base64Encoder,
            HexEncoder
        }

        public static System.Text.Encoding Encode = System.Text.Encoding.Default;

        public string Encrypt(string data, string pass, CryptoBase.EncoderMode em)
        {
            if (data == null || pass == null)
            {
                return null;
            }
            if (em == CryptoBase.EncoderMode.Base64Encoder)
            {
                return System.Convert.ToBase64String(this.EncryptEx(CryptoBase.Encode.GetBytes(data), pass));
            }
            return CryptoBase.ByteToHex(this.EncryptEx(CryptoBase.Encode.GetBytes(data), pass));
        }

        public string Decrypt(string data, string pass, CryptoBase.EncoderMode em)
        {
            if (data == null || pass == null)
            {
                return null;
            }
            if (em == CryptoBase.EncoderMode.Base64Encoder)
            {
                return CryptoBase.Encode.GetString(this.DecryptEx(System.Convert.FromBase64String(data), pass));
            }
            return CryptoBase.Encode.GetString(this.DecryptEx(CryptoBase.HexToByte(data), pass));
        }

        public string Encrypt(string data, string pass)
        {
            return this.Encrypt(data, pass, CryptoBase.EncoderMode.Base64Encoder);
        }

        public string Decrypt(string data, string pass)
        {
            return this.Decrypt(data, pass, CryptoBase.EncoderMode.Base64Encoder);
        }

        public virtual byte[] EncryptEx(byte[] data, string pass)
        {
            return null;
        }

        public virtual byte[] DecryptEx(byte[] data, string pass)
        {
            return null;
        }

        public static byte[] HexToByte(string szHex)
        {
            int iLen = szHex.Length;
            if (iLen <= 0 || iLen % 2 != 0)
            {
                return null;
            }
            int dwCount = iLen / 2;
            byte[] pbBuffer = new byte[dwCount];
            for (int i = 0; i < dwCount; i++)
            {
                uint tmp = (uint)(szHex[i * 2] - ((szHex[i * 2] >= 'A') ? '7' : '0'));
                if (tmp >= 16u)
                {
                    return null;
                }
                uint tmp2 = (uint)(szHex[i * 2 + 1] - ((szHex[i * 2 + 1] >= 'A') ? '7' : '0'));
                if (tmp2 >= 16u)
                {
                    return null;
                }
                pbBuffer[i] = (byte)(tmp * 16u + tmp2);
            }
            return pbBuffer;
        }

        public static string ByteToHex(byte[] vByte)
        {
            if (vByte == null || vByte.Length < 1)
            {
                return null;
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder(vByte.Length * 2);
            for (int i = 0; i < vByte.Length; i++)
            {
                if (vByte[i] < 0)
                {
                    return null;
                }
                uint j = (uint)(vByte[i] / 16);
                sb.Append((char)((ulong)j + (ulong)((j > 9u) ? 55L : 48L)));
                j = (uint)(vByte[i] % 16);
                sb.Append((char)((ulong)j + (ulong)((j > 9u) ? 55L : 48L)));
            }
            return sb.ToString();
        }
    }
}