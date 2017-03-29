using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Dorado.Wcf.MiniEncoder
{
    public class Compressor
    {
        private CompressAlgorithm _algorithm;

        public Compressor(CompressAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        //压缩数组
        public ArraySegment<byte> Compress(ArraySegment<byte> data)
        {
            MemoryStream ms = new MemoryStream();

            if (_algorithm == CompressAlgorithm.GZip)
            {
                Stream compressStream = new GZipStream(ms, CompressionMode.Compress, true);
                compressStream.Write(data.Array, 0, data.Count);
                compressStream.Close();
            }
            else
            {
                Stream compressStream = new DeflateStream(ms, CompressionMode.Compress, true);
                compressStream.Write(data.Array, 0, data.Count);
                compressStream.Close();
            }
            byte[] newByteArray = new byte[ms.Length];

            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(newByteArray, 0, newByteArray.Length);

            ArraySegment<byte> bytes = new ArraySegment<byte>(newByteArray);
            return bytes;
        }

        //压缩流
        public Stream Compress(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            if (_algorithm == CompressAlgorithm.GZip)
            {
                Stream compressStream = new GZipStream(ms, CompressionMode.Compress, true);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                compressStream.Write(buffer, 0, buffer.Length);
                compressStream.Close();
            }
            else
            {
                Stream compressStream = new DeflateStream(ms, CompressionMode.Compress, true);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                compressStream.Write(buffer, 0, buffer.Length);
                compressStream.Close();
            }
            return ms;
        }

        //解压缩数组
        public ArraySegment<byte> DeCompress(ArraySegment<byte> data)
        {
            MemoryStream ms = new MemoryStream();

            ms.Write(data.Array, 0, data.Count);
            ms.Seek(0, SeekOrigin.Begin);
            if (_algorithm == CompressAlgorithm.GZip)
            {
                Stream compressStream = new GZipStream(ms, CompressionMode.Decompress, false);
                byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                compressStream.Close();
                return new ArraySegment<byte>(newByteArray);
            }
            else
            {
                Stream compressStream = new DeflateStream(ms, CompressionMode.Decompress, false);
                byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                compressStream.Close();
                return new ArraySegment<byte>(newByteArray);
            }
        }

        //解压缩数组
        public Stream DeCompress(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            if (_algorithm == CompressAlgorithm.GZip)
            {
                Stream compressStream = new GZipStream(stream, CompressionMode.Decompress, false);
                byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                compressStream.Close();
                return new MemoryStream(newByteArray);
            }
            else
            {
                Stream compressStream = new DeflateStream(stream, CompressionMode.Decompress, false);
                byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                compressStream.Close();
                return new MemoryStream(newByteArray);
            }
        }

        public static byte[] RetrieveBytesFromStream(Stream stream, int bytesblock)
        {
            List<byte> lst = new List<byte>();
            byte[] data = new byte[1024];
            int totalCount = 0;
            while (true)
            {
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                byte[] buffers = new byte[bytesRead];
                Array.Copy(data, buffers, bytesRead);
                lst.AddRange(buffers);
                totalCount += bytesRead;
            }
            return lst.ToArray();
        }
    }
}