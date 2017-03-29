using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dorado.DataExpress.Utility
{
    public class TripleDes
    {
        private static readonly byte[] defaultIV = new byte[]
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

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (TripleDES.IsWeakKey(key))
            {
                throw new Exception("key为已知弱密钥,请更换强度更高的密钥!");
            }
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.Flush();
                }
                result = ms.ToArray();
            }
            return result;
        }

        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            return TripleDes.Encrypt(data, key, TripleDes.defaultIV);
        }

        public static byte[] Encrypt(byte[] data, DesKey key)
        {
            return TripleDes.Encrypt(data, key.Key, key.IV);
        }

        public static string Encrypt(string data, string password)
        {
            DesKey pkey = TripleDes.PasswordToKey(password);
            byte[] buff = Encoding.Unicode.GetBytes(data);
            return ToolLite.BytesToHex(TripleDes.Encrypt(buff, pkey));
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            byte[] result;
            using (MemoryStream oms = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        for (int i = cs.ReadByte(); i != -1; i = cs.ReadByte())
                        {
                            oms.WriteByte((byte)i);
                        }
                    }
                }
                result = oms.ToArray();
            }
            return result;
        }

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            return TripleDes.Decrypt(data, key, TripleDes.defaultIV);
        }

        public static byte[] Decrypt(byte[] data, DesKey key)
        {
            return TripleDes.Decrypt(data, key.Key, key.IV);
        }

        public static string Decrypt(string data, string password)
        {
            DesKey key = TripleDes.PasswordToKey(password);
            byte[] buff = ToolLite.HexToBytes(data);
            return Encoding.Unicode.GetString(TripleDes.Decrypt(buff, key));
        }

        public static DesKey PasswordToKey(string password)
        {
            DesKey key;
            key.Key = new byte[24];
            key.IV = new byte[8];
            byte[] sbuff = Encoding.ASCII.GetBytes(password);
            if (sbuff.Length == 24)
            {
                Array.Copy(sbuff, key.Key, 24);
                Array.Copy(sbuff, key.IV, 8);
            }
            if (sbuff.Length > 24)
            {
                Array.Copy(sbuff, key.Key, 24);
                int idx = sbuff.Length - 8;
                Array.Copy(sbuff, idx, key.IV, 0, 8);
            }
            if (sbuff.Length < 24)
            {
                Array.Copy(sbuff, key.Key, sbuff.Length);
                if (sbuff.Length < 8)
                {
                    Array.Copy(sbuff, key.IV, sbuff.Length);
                }
                else
                {
                    Array.Copy(sbuff, key.IV, 8);
                }
            }
            return key;
        }
    }
}