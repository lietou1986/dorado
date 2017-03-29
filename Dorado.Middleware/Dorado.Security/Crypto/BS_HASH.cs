namespace Dorado.Security.Crypto
{
    public class BS_HASH
    {
        public enum HashType
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        private static string byteArrayToString(byte[] inputArray)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder("");
            for (int i = 0; i < inputArray.Length; i++)
            {
                output.Append(inputArray[i].ToString("X2"));
            }
            return output.ToString();
        }

        public static string Hash(string aPasswordSalt, BS_HASH.HashType aHashType)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            string Hash = string.Empty;
            switch (aHashType)
            {
                case BS_HASH.HashType.MD5:
                    {
                        System.Security.Cryptography.MD5CryptoServiceProvider md5hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
                        byte[] hashedDataBytes = md5hasher.ComputeHash(encoder.GetBytes(aPasswordSalt));
                        Hash = BS_HASH.byteArrayToString(hashedDataBytes);
                        break;
                    }
                case BS_HASH.HashType.SHA1:
                    {
                        System.Security.Cryptography.SHA1CryptoServiceProvider sha1hasher = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                        byte[] hashedDataBytes = sha1hasher.ComputeHash(encoder.GetBytes(aPasswordSalt));
                        Hash = BS_HASH.byteArrayToString(hashedDataBytes);
                        break;
                    }
                case BS_HASH.HashType.SHA256:
                    {
                        System.Security.Cryptography.SHA256Managed sha256hasher = new System.Security.Cryptography.SHA256Managed();
                        byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(aPasswordSalt));
                        Hash = BS_HASH.byteArrayToString(hashedDataBytes);
                        break;
                    }
                case BS_HASH.HashType.SHA384:
                    {
                        System.Security.Cryptography.SHA384Managed sha384hasher = new System.Security.Cryptography.SHA384Managed();
                        byte[] hashedDataBytes = sha384hasher.ComputeHash(encoder.GetBytes(aPasswordSalt));
                        Hash = BS_HASH.byteArrayToString(hashedDataBytes);
                        break;
                    }
                case BS_HASH.HashType.SHA512:
                    {
                        System.Security.Cryptography.SHA512Managed sha512hasher = new System.Security.Cryptography.SHA512Managed();
                        byte[] hashedDataBytes = sha512hasher.ComputeHash(encoder.GetBytes(aPasswordSalt));
                        Hash = BS_HASH.byteArrayToString(hashedDataBytes);
                        break;
                    }
            }
            return Hash;
        }
    }
}