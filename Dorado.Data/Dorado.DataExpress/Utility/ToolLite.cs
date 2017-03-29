using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dorado.DataExpress.Utility
{
    public class ToolLite
    {
        private static readonly char[] HexDigest = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

        public static string BytesToHex(byte[] buffer)
        {
            char[] result = new char[buffer.Length * 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                result[i * 2] = ToolLite.HexDigest[buffer[i] >> 4];
                result[i * 2 + 1] = ToolLite.HexDigest[(int)(buffer[i] & 15)];
            }
            return new string(result);
        }

        public static string BytesToHexWithLeader(byte[] buffer)
        {
            char[] result = new char[buffer.Length * 2 + 2];
            result[0] = '0';
            result[1] = 'x';
            for (int i = 1; i < buffer.Length + 1; i++)
            {
                result[i * 2] = ToolLite.HexDigest[buffer[i - 1] >> 4];
                result[i * 2 + 1] = ToolLite.HexDigest[(int)(buffer[i - 1] & 15)];
            }
            return new string(result);
        }

        public static byte[] HexToBytes(string s)
        {
            if (s.StartsWith("0x"))
            {
                s = s.Substring(2);
            }
            byte[] bytes = new byte[s.Length / 2];
            try
            {
                for (int i = 0; i < s.Length; i += 2)
                {
                    bytes[i / 2] = byte.Parse(s.Substring(i, 2), NumberStyles.AllowHexSpecifier);
                }
            }
            catch
            {
                return null;
            }
            return bytes;
        }

        public static byte[] HexToBytes(object o)
        {
            return ToolLite.HexToBytes(o.ToString());
        }

        public static string HashString(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.Initialize();
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            byte[] hash = md5.ComputeHash(bytes);
            return ToolLite.BytesToHex(hash);
        }

        public static string Hash(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.Initialize();
            byte[] hashVal = md5.ComputeHash(stream);
            return ToolLite.BytesToHex(hashVal);
        }

        public static string Hash(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            return ToolLite.Hash(ms);
        }

        public static bool IsInheritedForm(Type type, Type parent)
        {
            return type.BaseType != null && (type.BaseType == parent || ToolLite.IsInheritedForm(type.BaseType, parent));
        }
    }
}