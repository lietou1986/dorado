using System.Net;

namespace Dorado.Security.Coder
{
    public class BS_Base64
    {
        public static string EncodingString(string SourceString, System.Text.Encoding Ens)
        {
            return System.Convert.ToBase64String(Ens.GetBytes(SourceString));
        }

        public static string EncodingString(string SourceString)
        {
            return BS_Base64.EncodingString(SourceString, System.Text.Encoding.Default);
        }

        public static string DecodingString(string Base64String, System.Text.Encoding Ens)
        {
            return Ens.GetString(System.Convert.FromBase64String(Base64String));
        }

        public static string DecodingString(string Base64String)
        {
            return BS_Base64.DecodingString(Base64String, System.Text.Encoding.Default);
        }

        public static string EncodingFileToString(string strFileName)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(strFileName);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            string Base64String = System.Convert.ToBase64String(br.ReadBytes((int)fs.Length));
            br.Close();
            fs.Close();
            return Base64String;
        }

        public static bool EncodingFileToFile(string strSourceFileName, string strSaveFileName)
        {
            string strBase64 = BS_Base64.EncodingFileToString(strSourceFileName);
            System.IO.StreamWriter fs = new System.IO.StreamWriter(strSaveFileName);
            fs.Write(strBase64);
            fs.Close();
            return true;
        }

        public static bool DecodingFileFromString(string Base64String, string strSaveFileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(strSaveFileName, System.IO.FileMode.Create);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
            bw.Write(System.Convert.FromBase64String(Base64String));
            bw.Close();
            fs.Close();
            return true;
        }

        public static bool DecodingFileFromFile(string strBase64FileName, string strSaveFileName)
        {
            System.IO.StreamReader fs = new System.IO.StreamReader(strBase64FileName, System.Text.Encoding.ASCII);
            char[] base64CharArray = new char[(int)((object)((System.IntPtr)fs.BaseStream.Length))];
            fs.Read(base64CharArray, 0, (int)fs.BaseStream.Length);
            string Base64String = new string(base64CharArray);
            fs.Close();
            return BS_Base64.DecodingFileFromString(Base64String, strSaveFileName);
        }

        public static string EncodingWebFile(string strURL, WebClient objWebClient)
        {
            return System.Convert.ToBase64String(objWebClient.DownloadData(strURL));
        }

        public static string EncodingWebFile(string strURL)
        {
            return BS_Base64.EncodingWebFile(strURL, new WebClient());
        }
    }
}