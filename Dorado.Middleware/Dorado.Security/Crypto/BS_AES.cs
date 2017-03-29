using System.Security.Cryptography;

namespace Dorado.Security.Crypto
{
    public class BS_AES
    {
        private static AesCryptoServiceProvider AESProvider = new AesCryptoServiceProvider();

        private static string Key
        {
            get
            {
                return "M)H0[WE],9}-chena+{~GBaby9>Q'y6M";
            }
        }

        private static string IV
        {
            get
            {
                return "P-\\~9Z.II(B$)=Gh";
            }
        }

        public static byte[] AESEncrypt(string plainStr, ProvidesKey Pkey, string NewKey = null, string NewIV = null)
        {
            byte[] bKey = null;
            byte[] bIV = null;
            byte[] EncryptByte = null;
            if (NewKey == null || NewIV == null)
            {
                NewKey = BS_AES.Key;
                NewIV = BS_AES.IV;
            }
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(BS_AES.Key);
                        bIV = System.Text.Encoding.UTF8.GetBytes(BS_AES.IV);
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
                EncryptByte = BS_AES.encryptStringToBytes_AES(plainStr, bKey, bIV);
            }
            return EncryptByte;
        }

        public static string AESDecrypt(byte[] cipherText, ProvidesKey Pkey, string NewKey = null, string NewIV = null)
        {
            byte[] bKey = null;
            byte[] bIV = null;
            string DecryptStr = null;
            if (NewKey == null || NewIV == null)
            {
                NewKey = BS_AES.Key;
                NewIV = BS_AES.IV;
            }
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(BS_AES.Key);
                        bIV = System.Text.Encoding.UTF8.GetBytes(BS_AES.IV);
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
                DecryptStr = BS_AES.decryptStringFromBytes_AES(cipherText, bKey, bIV);
            }
            return DecryptStr;
        }

        public static string AesEncryptFile(string inputFile, string OutputFile, ProvidesKey Pkey, string NewKey = null, string NewIV = null)
        {
            byte[] bKey = null;
            byte[] bIV = null;
            string EncryptByte = null;
            if (NewKey == null || NewIV == null)
            {
                NewKey = BS_AES.Key;
                NewIV = BS_AES.IV;
            }
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(BS_AES.Key);
                        bIV = System.Text.Encoding.UTF8.GetBytes(BS_AES.IV);
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(NewKey);
                        bIV = System.Text.Encoding.UTF8.GetBytes(NewIV);
                        break;
                    }
            }
            if (inputFile != null && OutputFile != null)
            {
                EncryptByte = BS_AES.EncryptFile(inputFile, OutputFile, bKey, bIV, 0, 256);
            }
            return EncryptByte;
        }

        public static string AesDecryptFile(string inputFile, string OutputFile, ProvidesKey Pkey, string NewKey = null, string NewIV = null)
        {
            byte[] bKey = null;
            byte[] bIV = null;
            string DecryptByte = null;
            if (NewKey == null || NewIV == null)
            {
                NewKey = BS_AES.Key;
                NewIV = BS_AES.IV;
            }
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(BS_AES.Key);
                        bIV = System.Text.Encoding.UTF8.GetBytes(BS_AES.IV);
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        bKey = System.Text.Encoding.UTF8.GetBytes(NewKey);
                        bIV = System.Text.Encoding.UTF8.GetBytes(NewIV);
                        break;
                    }
            }
            if (inputFile != null && OutputFile != null)
            {
                DecryptByte = BS_AES.DecryptFile(inputFile, OutputFile, bKey, bIV, 0, 256);
            }
            return DecryptByte;
        }

        public static byte[] encryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new System.ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new System.ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new System.ArgumentNullException("Key");
            }
            System.IO.MemoryStream msEncrypt = null;
            System.Security.Cryptography.CryptoStream csEncrypt = null;
            System.IO.StreamWriter swEncrypt = null;
            Aes aesAlg = null;
            try
            {
                aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                System.Security.Cryptography.ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                msEncrypt = new System.IO.MemoryStream();
                csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write);
                swEncrypt = new System.IO.StreamWriter(csEncrypt);
                swEncrypt.Write(plainText);
            }
            finally
            {
                if (swEncrypt != null)
                {
                    swEncrypt.Close();
                }
                if (csEncrypt != null)
                {
                    csEncrypt.Close();
                }
                if (msEncrypt != null)
                {
                    msEncrypt.Close();
                }
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }
            return msEncrypt.ToArray();
        }

        public static string decryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new System.ArgumentNullException("cipherText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new System.ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new System.ArgumentNullException("Key");
            }
            System.IO.MemoryStream msDecrypt = null;
            System.Security.Cryptography.CryptoStream csDecrypt = null;
            System.IO.StreamReader srDecrypt = null;
            Aes aesAlg = null;
            string plaintext = null;
            try
            {
                aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                System.Security.Cryptography.ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                msDecrypt = new System.IO.MemoryStream(cipherText);
                csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
                srDecrypt = new System.IO.StreamReader(csDecrypt);
                plaintext = srDecrypt.ReadToEnd();
            }
            finally
            {
                if (srDecrypt != null)
                {
                    srDecrypt.Close();
                }
                if (csDecrypt != null)
                {
                    csDecrypt.Close();
                }
                if (msDecrypt != null)
                {
                    msDecrypt.Close();
                }
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }
            return plaintext;
        }

        public static string EncryptFile(string inputFile, string OutputFile, byte[] Key, byte[] IV, byte CryptoFlag, int KeySize)
        {
            System.IO.FileStream fsInput = null;
            System.IO.FileStream fsCiperText = null;
            string result;
            try
            {
                fsInput = new System.IO.FileStream(inputFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                fsCiperText = new System.IO.FileStream(OutputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                switch (CryptoFlag)
                {
                    case 0:
                        {
                            BS_AES.AESProvider.Mode = System.Security.Cryptography.CipherMode.CBC;
                            break;
                        }
                    case 1:
                        {
                            BS_AES.AESProvider.Mode = System.Security.Cryptography.CipherMode.ECB;
                            break;
                        }
                    default:
                        {
                            BS_AES.AESProvider.Mode = System.Security.Cryptography.CipherMode.CBC;
                            break;
                        }
                }
                BS_AES.AESProvider.KeySize = KeySize;
                BS_AES.AESProvider.Key = Key;
                BS_AES.AESProvider.IV = IV;
                System.Security.Cryptography.ICryptoTransform encrypt = BS_AES.AESProvider.CreateEncryptor(BS_AES.AESProvider.Key, BS_AES.AESProvider.IV);
                System.Security.Cryptography.CryptoStream cryptStream = new System.Security.Cryptography.CryptoStream(fsCiperText, encrypt, System.Security.Cryptography.CryptoStreamMode.Write);
                int lChunkSize = 100;
                byte[] byteArrayInput = new byte[lChunkSize];
                while (fsInput.Position < fsInput.Length)
                {
                    int lBytesRead = fsInput.Read(byteArrayInput, 0, (fsInput.Length - fsInput.Position < (long)lChunkSize) ? System.Convert.ToInt32(fsInput.Length - fsInput.Position) : lChunkSize);
                    cryptStream.Write(byteArrayInput, 0, lBytesRead);
                    cryptStream.Flush();
                }
                cryptStream.Close();
                result = "Complete!";
            }
            catch (System.Exception arg_10C_0)
            {
                System.Exception exc = arg_10C_0;
                result = "Error!: " + exc.Message;
            }
            finally
            {
                fsCiperText.Close();
                fsInput.Close();
            }
            return result;
        }

        public static string DecryptFile(string inputFile, string outputFile, byte[] Key, byte[] IV, byte CryptoFlag, int KeySize)
        {
            System.IO.FileStream fsCiperFile = null;
            System.IO.FileStream fsOutputFile = null;
            string result;
            try
            {
                fsCiperFile = new System.IO.FileStream(inputFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                fsOutputFile = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                BS_AES.AESProvider.KeySize = KeySize;
                BS_AES.AESProvider.Key = Key;
                BS_AES.AESProvider.IV = IV;
                switch (CryptoFlag)
                {
                    case 0:
                        {
                            BS_AES.AESProvider.Mode = System.Security.Cryptography.CipherMode.CBC;
                            break;
                        }
                    case 1:
                        {
                            BS_AES.AESProvider.Mode = System.Security.Cryptography.CipherMode.ECB;
                            break;
                        }
                    default:
                        {
                            BS_AES.AESProvider.Mode = System.Security.Cryptography.CipherMode.CBC;
                            break;
                        }
                }
                System.Security.Cryptography.ICryptoTransform decrypt = BS_AES.AESProvider.CreateDecryptor(BS_AES.AESProvider.Key, BS_AES.AESProvider.IV);
                int lChunkSize = 100;
                byte[] byteArrayInput = new byte[lChunkSize];
                System.Security.Cryptography.CryptoStream decryptStream = new System.Security.Cryptography.CryptoStream(fsCiperFile, decrypt, System.Security.Cryptography.CryptoStreamMode.Read);
                bool lDone = false;
                while (!lDone)
                {
                    int lBytesRead = decryptStream.Read(byteArrayInput, 0, lChunkSize);
                    fsOutputFile.Write(byteArrayInput, 0, lBytesRead);
                    fsOutputFile.Flush();
                    if (lBytesRead == 0)
                    {
                        lDone = true;
                    }
                }
                decryptStream.Close();
                result = "Complete!";
            }
            catch (System.Exception arg_E6_0)
            {
                System.Exception exc = arg_E6_0;
                result = "Error!: " + exc.Message;
            }
            finally
            {
                fsCiperFile.Close();
                fsOutputFile.Close();
            }
            return result;
        }
    }
}