using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dorado.DataExpress.Utility
{
    public class Des
    {
        private static readonly byte[] DefaultIv = new byte[]
		{
			25,
			120,
			17,
			9,
			25,
			129,
			4,
			20
		};

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] IV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] result;
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream encStream = new CryptoStream(stream, des.CreateEncryptor(key, IV), CryptoStreamMode.Write))
                {
                    encStream.Write(data, 0, data.Length);
                }
                result = stream.ToArray();
            }
            return result;
        }

        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            return Des.Encrypt(data, key, Des.DefaultIv);
        }

        public static byte[] Encrypt(byte[] data, DesKey key)
        {
            return Des.Encrypt(data, key.Key, key.IV);
        }

        public static string Encrypt(string source, string password)
        {
            DesKey key = Des.PasswordToKey(password);
            return ToolLite.BytesToHex(Des.Encrypt(Encoding.Unicode.GetBytes(source), key.Key, key.IV));
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] IV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] result;
            using (MemoryStream oms = new MemoryStream())
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (CryptoStream encStream = new CryptoStream(stream, des.CreateDecryptor(key, IV), CryptoStreamMode.Read))
                    {
                        for (int readData = encStream.ReadByte(); readData != -1; readData = encStream.ReadByte())
                        {
                            oms.WriteByte((byte)readData);
                        }
                    }
                }
                result = oms.ToArray();
            }
            return result;
        }

        public static byte[] Decrypt(byte[] data, DesKey key)
        {
            return Des.Decrypt(data, key.Key, key.IV);
        }

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            return Des.Decrypt(data, key, Des.DefaultIv);
        }

        public static string Decrypt(string source, string password)
        {
            DesKey key = Des.PasswordToKey(password);
            byte[] buffer = ToolLite.HexToBytes(source);
            byte[] sbuff = Des.Decrypt(buffer, key.Key, key.IV);
            return Encoding.Unicode.GetString(sbuff);
        }

        public static DesKey PasswordToKey(string password)
        {
            DesKey key;
            key.Key = new byte[8];
            key.IV = new byte[8];
            byte[] sbuff = Encoding.ASCII.GetBytes(password);
            if (sbuff.Length == 8)
            {
                Array.Copy(sbuff, key.Key, 8);
                Array.Copy(sbuff, key.IV, 8);
            }
            if (sbuff.Length > 8)
            {
                Array.Copy(sbuff, key.Key, 8);
                int idx = sbuff.Length - 8;
                Array.Copy(sbuff, idx, key.IV, 0, 8);
            }
            if (sbuff.Length < 8)
            {
                Array.Copy(sbuff, key.Key, sbuff.Length);
                Array.Copy(sbuff, key.IV, sbuff.Length);
            }
            return key;
        }
    }
}