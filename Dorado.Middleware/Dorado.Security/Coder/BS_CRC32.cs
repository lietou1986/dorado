namespace Dorado.Security.Coder
{
    public class BS_CRC32 : CRC32
    {
        public int GetByteCrc32Int(byte[] buffer, int start, int length)
        {
            this.Initialize();
            this.HashCore(buffer, start, length);
            this.HashFinal();
            return base.CrcValueInt;
        }

        public uint GetByteCrc32Uint(byte[] buffer, int start, int length)
        {
            this.Initialize();
            this.HashCore(buffer, start, length);
            this.HashFinal();
            return base.CrcValueUInt;
        }

        public int GetStringToCrc32Int(string str)
        {
            this.Initialize();
            return base.Crc32HashCodeInt(str);
        }

        public uint GetStringToCrc32Uint(string str)
        {
            this.Initialize();
            return base.GetCrc32HashCodeUInt(str);
        }
    }
}