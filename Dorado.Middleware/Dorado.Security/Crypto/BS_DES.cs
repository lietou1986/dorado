namespace Dorado.Security.Crypto
{
    public class BS_DES
    {
        private static string Key
        {
            get
            {
                return "{G^a'b]#";
            }
        }

        private static string IV
        {
            get
            {
                return "B$)!M(]P";
            }
        }

        public static byte[] DesEncrypt(string plainStr, ProvidesKey Pkey, string NewKey = null, string NewIV = null)
        {
            byte[] bKey = null;
            byte[] bIV = null;
            byte[] EncryptByte = null;
            if (NewKey == null || NewIV == null)
            {
                NewKey = BS_DES.Key;
                NewIV = BS_DES.IV;
            }
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(BS_DES.Key);
                        bIV = System.Text.Encoding.UTF8.GetBytes(BS_DES.IV);
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(NewKey);
                        bIV = System.Text.Encoding.UTF8.GetBytes(NewIV);
                        break;
                    }
            }
            if (plainStr != null)
            {
                EncryptByte = BS_DES.EncryptTextToMemory(plainStr, bKey, bIV);
            }
            return EncryptByte;
        }

        public static string DesDecrypt(byte[] cipherText, ProvidesKey Pkey, string NewKey = null, string NewIV = null)
        {
            byte[] bKey = null;
            byte[] bIV = null;
            string DecryptStr = null;
            if (NewKey == null || NewIV == null)
            {
                NewKey = BS_DES.Key;
                NewIV = BS_DES.IV;
            }
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(BS_DES.Key);
                        bIV = System.Text.Encoding.UTF8.GetBytes(BS_DES.IV);
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(NewKey);
                        bIV = System.Text.Encoding.UTF8.GetBytes(NewIV);
                        break;
                    }
            }
            if (cipherText != null)
            {
                DecryptStr = BS_DES.DecryptTextFromMemory(cipherText, bKey, bIV);
            }
            return DecryptStr;
        }

        public static byte[] EncryptTextToMemory(string Data, byte[] Key, byte[] IV)
        {
            byte[] result;
            try
            {
                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                System.Security.Cryptography.DES DESalg = System.Security.Cryptography.DES.Create();
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(mStream, DESalg.CreateEncryptor(Key, IV), System.Security.Cryptography.CryptoStreamMode.Write);
                byte[] toEncrypt = new System.Text.ASCIIEncoding().GetBytes(Data);
                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();
                byte[] ret = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                result = ret;
            }
            catch (System.Security.Cryptography.CryptographicException arg_53_0)
            {
                System.Security.Cryptography.CryptographicException e = arg_53_0;
                System.Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                result = null;
            }
            return result;
        }

        public static string DecryptTextFromMemory(byte[] Data, byte[] Key, byte[] IV)
        {
            string result;
            try
            {
                System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(Data);
                System.Security.Cryptography.DES DESalg = System.Security.Cryptography.DES.Create();
                System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, DESalg.CreateDecryptor(Key, IV), System.Security.Cryptography.CryptoStreamMode.Read);
                byte[] fromEncrypt = new byte[Data.Length];
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                result = new System.Text.ASCIIEncoding().GetString(fromEncrypt);
            }
            catch (System.Security.Cryptography.CryptographicException arg_41_0)
            {
                System.Security.Cryptography.CryptographicException e = arg_41_0;
                System.Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                result = null;
            }
            return result;
        }

        public static void EncryptTextToFile(string Data, string FileName, byte[] Key, byte[] IV)
        {
            try
            {
                System.IO.FileStream fStream = System.IO.File.Open(FileName, System.IO.FileMode.OpenOrCreate);
                System.Security.Cryptography.DES DESalg = System.Security.Cryptography.DES.Create();
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(fStream, DESalg.CreateEncryptor(Key, IV), System.Security.Cryptography.CryptoStreamMode.Write);
                System.IO.StreamWriter sWriter = new System.IO.StreamWriter(cStream);
                sWriter.WriteLine(Data);
                sWriter.Close();
                cStream.Close();
                fStream.Close();
            }
            catch (System.Security.Cryptography.CryptographicException arg_40_0)
            {
                System.Security.Cryptography.CryptographicException e = arg_40_0;
                System.Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            }
            catch (System.UnauthorizedAccessException arg_55_0)
            {
                System.UnauthorizedAccessException e2 = arg_55_0;
                System.Console.WriteLine("A file error occurred: {0}", e2.Message);
            }
        }

        public static string DecryptTextFromFile(string FileName, byte[] Key, byte[] IV)
        {
            string result;
            try
            {
                System.IO.FileStream fStream = System.IO.File.Open(FileName, System.IO.FileMode.OpenOrCreate);
                System.Security.Cryptography.DES DESalg = System.Security.Cryptography.DES.Create();
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(fStream, DESalg.CreateDecryptor(Key, IV), System.Security.Cryptography.CryptoStreamMode.Read);
                System.IO.StreamReader sReader = new System.IO.StreamReader(cStream);
                string val = sReader.ReadLine();
                sReader.Close();
                cStream.Close();
                fStream.Close();
                result = val;
            }
            catch (System.Security.Cryptography.CryptographicException arg_45_0)
            {
                System.Security.Cryptography.CryptographicException e = arg_45_0;
                System.Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                result = null;
            }
            catch (System.UnauthorizedAccessException arg_5D_0)
            {
                System.UnauthorizedAccessException e2 = arg_5D_0;
                System.Console.WriteLine("A file error occurred: {0}", e2.Message);
                result = null;
            }
            return result;
        }
    }
}