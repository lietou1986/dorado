using System;

namespace Dorado.Security.Crypto
{
    public class RC4Crypto : CryptoBase
    {
        public override byte[] EncryptEx(byte[] data, string pass)
        {
            if ((data == null) || (pass == null))
            {
                return null;
            }
            byte[] output = new byte[data.Length];
            long i = 0L;
            long j = 0L;
            byte[] mBox = GetKey(CryptoBase.Encode.GetBytes(pass), 0x100);
            for (long offset = 0L; offset < data.Length; offset += 1L)
            {
                i = (i + 1L) % ((long)mBox.Length);
                j = (j + mBox[(int)((IntPtr)i)]) % ((long)mBox.Length);
                byte temp = mBox[(int)((IntPtr)i)];
                mBox[(int)((IntPtr)i)] = mBox[(int)((IntPtr)j)];
                mBox[(int)((IntPtr)j)] = temp;
                byte a = data[(int)((IntPtr)offset)];
                byte b = mBox[(mBox[(int)((IntPtr)i)] + mBox[(int)((IntPtr)j)]) % mBox.Length];
                output[(int)((IntPtr)offset)] = (byte)(a ^ b);
            }
            return output;
        }

        public override byte[] DecryptEx(byte[] data, string pass)
        {
            return this.EncryptEx(data, pass);
        }

        private static byte[] GetKey(byte[] pass, int kLen)
        {
            byte[] mBox = new byte[kLen];
            for (long i = 0L; i < (long)kLen; i += 1L)
            {
                mBox[(int)((object)((System.IntPtr)i))] = (byte)i;
            }
            long j = 0L;
            for (long k = 0L; k < (long)kLen; k += 1L)
            {
                j = (j + (long)((ulong)mBox[(int)((object)((System.IntPtr)k))]) + (long)((ulong)pass[(int)((object)((System.IntPtr)(k % (long)pass.Length)))])) % (long)kLen;
                byte temp = mBox[(int)((object)((System.IntPtr)k))];
                mBox[(int)((object)((System.IntPtr)k))] = mBox[(int)((object)((System.IntPtr)j))];
                mBox[(int)((object)((System.IntPtr)j))] = temp;
            }
            return mBox;
        }
    }
}