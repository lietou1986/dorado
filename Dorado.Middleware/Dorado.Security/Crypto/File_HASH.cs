namespace Dorado.Security.Crypto
{
    public class File_HASH
    {
        public static string MD5Stream(string filePath)
        {
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            md5.ComputeHash(fs);
            fs.Close();
            byte[] b = md5.Hash;
            md5.Clear();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X2"));
            }
            System.Console.WriteLine(sb.ToString());
            System.Console.ReadLine();
            return sb.ToString();
        }

        public static string MD5File(string filePath)
        {
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            int bufferSize = 1048576;
            byte[] buff = new byte[bufferSize];
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            md5.Initialize();
            for (long offset = 0L; offset < fs.Length; offset += (long)bufferSize)
            {
                long readSize = (long)bufferSize;
                if (offset + readSize > fs.Length)
                {
                    readSize = fs.Length - offset;
                }
                fs.Read(buff, 0, System.Convert.ToInt32(readSize));
                if (offset + readSize < fs.Length)
                {
                    md5.TransformBlock(buff, 0, System.Convert.ToInt32(readSize), buff, 0);
                }
                else
                {
                    md5.TransformFinalBlock(buff, 0, System.Convert.ToInt32(readSize));
                }
            }
            fs.Close();
            byte[] result = md5.Hash;
            md5.Clear();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}