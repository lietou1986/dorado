using Dorado.Security.Coder;
using OpenSSL.Core;
using OpenSSL.Crypto;
using OpenSSL.X509;
using System.Xml;

namespace Dorado.Security.Certificate
{
    public class CertificateAuthority
    {
        private int KeyLength;

        public CertificateAuthority()
        {
            this.KeyLength = 2048;
        }

        public string GetP12PassWord()
        {
            string PassWord = string.Empty;
            try
            {
                System.Guid.NewGuid();
                PassWord = PassWord.Substring(0, 30);
            }
            catch (System.Exception)
            {
            }
            return PassWord;
        }

        public CryptoKey GetCryptoKey()
        {
            CryptoKey Key;
            try
            {
                int bits = 2048;
                RSA Rsa = new RSA();
                BigNumber bigNumber = OpenSSL.Core.Random.Next(20, 50, 5);
                Rsa.GenerateKeys(bits, bigNumber, null, null);
                Key = new CryptoKey(Rsa);
                Rsa.Dispose();
            }
            catch (System.Exception)
            {
                Key = null;
            }
            return Key;
        }

        public bool CreateX509Cert(XmlNode Node, CryptoKey pubKey, CryptoKey priKey, Stack<X509Certificate> X509Certs, string RootPath, ref X509Certificate X509Cert)
        {
            bool result = true;
            try
            {
                string Name = Node.Attributes["Name"].Value;
                RootPath += Node.Attributes["Path"].Value;
                int Serial = int.Parse(Node.SelectSingleNode("Serial").InnerText);
                string Versoin = Node.SelectSingleNode("Versoin").InnerText;
                string Issuer = Node.SelectSingleNode("Issuer").InnerText;
                string Subject = Node.SelectSingleNode("Subject").InnerText;
                int Period = int.Parse(Node.SelectSingleNode("Period").InnerText);
                X509Cert = new X509Certificate(Serial, Subject, Issuer, pubKey, System.DateTime.Now, System.DateTime.Now.AddYears(Period));
                X509Cert.Version = int.Parse(Versoin);
                X509Cert.Sign(priKey, MessageDigest.SHA1);
                string Path = RootPath + Name;
                if (!System.IO.Directory.Exists(RootPath))
                {
                    System.IO.Directory.CreateDirectory(RootPath);
                }
                System.IO.File.SetAttributes(RootPath, System.IO.FileAttributes.Normal);
                BIO X509Bio = BIO.File(Path + ".crt", "w");
                X509Cert.Write(X509Bio);
                string Password = RondomSN.GetRandom(30);
                PKCS12 p12 = new PKCS12(Password, pubKey, X509Cert, X509Certs);
                BIO p12Bio = BIO.File(Path + ".p12", "w");
                p12.Write(p12Bio);
                X509Bio.Dispose();
                p12Bio.Dispose();
                p12.Dispose();
                string p12PwdPath = RootPath + "P12口令.txt";
                System.IO.FileStream fileStream = new System.IO.FileStream(p12PwdPath, System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream, System.Text.Encoding.GetEncoding("gb2312"));
                sw.WriteLine(Password);
                sw.Flush();
                sw.Close();
                fileStream.Close();
            }
            catch (System.Exception)
            {
                X509Cert = null;
                result = false;
            }
            return result;
        }
    }
}