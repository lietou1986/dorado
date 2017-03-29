using System;
using System.IO;

namespace Dorado.Utils
{
    public class StreamUtility
    {
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[(int)((object)((IntPtr)stream.Length))];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0L, SeekOrigin.Begin);
            return bytes;
        }

        public static Stream BytesToStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static void StreamToFile(Stream stream, string fileName)
        {
            byte[] bytes = new byte[(int)((object)((IntPtr)stream.Length))];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0L, SeekOrigin.Begin);
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        public static Stream FileToStream(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[(int)((object)((IntPtr)fileStream.Length))];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            return new MemoryStream(bytes);
        }
    }
}