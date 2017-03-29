namespace Dorado.Security.Crypto
{
    public class BS_Rsa
    {
        public static string RSAEncrypt(string content, ProvidesKey Pkey, string PublicKeyNew = null)
        {
            string EncryptStr = string.Empty;
            string PublicKey = string.Empty;
            string PublicKeyDefault = string.Empty;
            PublicKeyDefault = "<RSAKeyValue><Modulus>3KbYenM8/yYU7keZTNxPLOW/snTlZ0DEC2WgN8X40I1NL4VnBe9i6qCaKmXWhx/KxCOpIWlmqxmGbwNqVorsbAJpeLnWCMWh3eH3rrGCINIQIfzV46vxlV9kePEPR/jKwMQwGrtfyEvy2iOlpyDKU+3A34AMBxzDYosf0jt0n6GuUZqgY36CJCGwbg1hoaYyH4uh37MgVc9hYQ2qF6c6YGKY2WHvHbH6QCStNKXZehGTAwVdJrtDn5C/QwL20V76fr8JKgCHAdjHYamkqVzaDHKvb40AucpCtjWUmV+GCNmEV0Q3Y5CAYckmZsOFlNIf+pnZDo0sePpkn8t8tActZQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        PublicKey = PublicKeyDefault;
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        PublicKey = PublicKeyNew;
                        if (PublicKeyNew == "")
                        {
                            PublicKey = PublicKeyDefault;
                        }
                        break;
                    }
            }
            if (content != null)
            {
                EncryptStr = BS_Rsa.RSA_Encrypt(PublicKey, content);
            }
            return EncryptStr;
        }

        public static string RSADecrypt(string content, ProvidesKey Pkey, string PrivateKeyNew = null)
        {
            string DecryptStr = string.Empty;
            string PrivateKey = string.Empty;
            string PrivateKeyDefault = string.Empty;
            PrivateKeyDefault = "<RSAKeyValue><Modulus>3KbYenM8/yYU7keZTNxPLOW/snTlZ0DEC2WgN8X40I1NL4VnBe9i6qCaKmXWhx/KxCOpIWlmqxmGbwNqVorsbAJpeLnWCMWh3eH3rrGCINIQIfzV46vxlV9kePEPR/jKwMQwGrtfyEvy2iOlpyDKU+3A34AMBxzDYosf0jt0n6GuUZqgY36CJCGwbg1hoaYyH4uh37MgVc9hYQ2qF6c6YGKY2WHvHbH6QCStNKXZehGTAwVdJrtDn5C/QwL20V76fr8JKgCHAdjHYamkqVzaDHKvb40AucpCtjWUmV+GCNmEV0Q3Y5CAYckmZsOFlNIf+pnZDo0sePpkn8t8tActZQ==</Modulus><Exponent>AQAB</Exponent><P>9UeliTfjo4SW/Yqsv540ue7/v2vCHzOhjCpuOuhdO1C3XRzCSkJXSfWOxP2sS2yOyYXN4eZ9Wv3c4DPsbtp81S0JEqxbG5OQ8NcCyK9s1bqBQcMn3dTeNjaGuzmVXZq/CsXWQdg6Ht1KYsDr9Ku3moQp7w5k0LhYrAuESYP2yWE=</P><Q>5kukqNfL7tugDiLip4GJedGjeD9W/g2NGwc9PeOJIiV+eU9UqhDO/PZtFQj09JNgIrUhEL4QisfLetL4upwzflQ3901Zl5x2/XErFsgGHM2/kh++t22rHjoB3BgKRnmCb/Qu20ZjOZX6lrGBxDz7gL6JWopJoQteax/cdX6VToU=</Q><DP>0Q4Z5xR9rzcpQjxZ9JAxPHCaBlsf/1wQStk3TnoSTKYFQxVp+8UDhQQIhZn4qeiC4vxqeCntmgpZobZfxDPwxfk2jHXeyWhC1yNLOiVay+fEl3pwlkACjY5300fHKRrlzZbFgSV8lCXkarsn4Ugim04eN6S7BLK60RiXzdjuFME=</DP><DQ>k5rGFeYCcrzX0FexuDUBfU/Nm6PVSQ48c6dnLESrP3SSpIQhOsN9N4tmjQXpotzW0EgRwl5f0eOO2tFf1JOLOZwXJu2vz8ncRkMDoB1iNZH2CF1KatziwYPib0QFSfhVjxYN9kMeE+m57F4nUzTW6kS1Wo+p84uK39RTBAe3ANU=</DQ><InverseQ>jwTOcpN6PVAm2/XH5TP4Q4RhUAtGJvJP5sumnZludG50wsO5HKPb5lj762EIV6JTsq8JPGQNNKW9iyVd1ACZdqaoOewkyqwQAON9wjkMVXBhSECISSIeTO6rB18+8TmpGlKLs33hIMPJ/PHs3vlDWPfiAFUGkIt+0aPlWg4Lipw=</InverseQ><D>h/dH7iyUqaUNwMbd8D7RixkHxy/BLJ78LwmwcK3NZLWtmlrZ4Q5iJbJRTU7zotA0YiiXT79jRIu2CzJPlISGqwfRdlMNUNp3pAJaXYuEZDUNO+RQ6LpLY4AkqWHhyLfGOIKsIczUeNhUDZdpvFK4Wn9nGlINF1TiERcpxKJeev5BHoTxdzNNC6zLWNJIS0RpupH83+0HeO6RzCD1v3hYqPRr2m0ZnTX4iyOc/f5XlKtFmoY1m4HZEy3glirHH9n/jLTXSytumxvWq51ZJii8QPl4EHN8RT/9k3HFoTDcS5xkUEFSLqqnMJLCIM2+EVIL1kZ8oGVfJ9MRJ2ue+H6gAQ==</D></RSAKeyValue>";
            switch (Pkey)
            {
                case ProvidesKey.NeedKey:
                    {
                        PrivateKey = PrivateKeyDefault;
                        break;
                    }
                case ProvidesKey.NoNeedKey:
                    {
                        PrivateKey = PrivateKeyNew;
                        if (PrivateKeyNew == "")
                        {
                            PrivateKey = PrivateKeyDefault;
                        }
                        break;
                    }
            }
            if (content != null)
            {
                DecryptStr = BS_Rsa.RSA_Decrypt(PrivateKey, content);
            }
            return DecryptStr;
        }

        private static string RSA_Encrypt(string publickey, string content)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
            rsa.FromXmlString(publickey);
            byte[] cipherbytes = rsa.Encrypt(System.Text.Encoding.UTF8.GetBytes(content), false);
            return System.Convert.ToBase64String(cipherbytes);
        }

        private static string RSA_Decrypt(string privatekey, string content)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
            rsa.FromXmlString(privatekey);
            byte[] cipherbytes = rsa.Decrypt(System.Convert.FromBase64String(content), false);
            return System.Text.Encoding.UTF8.GetString(cipherbytes);
        }

        public void RSAKey(string PrivateKeyPath, string PublicKeyPath)
        {
            try
            {
                System.Security.Cryptography.RSACryptoServiceProvider provider = new System.Security.Cryptography.RSACryptoServiceProvider(2048);
                this.CreatePrivateKeyXML(PrivateKeyPath, provider.ToXmlString(true));
                this.CreatePublicKeyXML(PublicKeyPath, provider.ToXmlString(false));
            }
            catch (System.Exception arg_29_0)
            {
                System.Exception exception = arg_29_0;
                throw exception;
            }
        }

        private void CreatePrivateKeyXML(string path, string privatekey)
        {
            try
            {
                System.IO.FileStream privatekeyxml = new System.IO.FileStream(path, System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(privatekeyxml);
                sw.WriteLine(privatekey);
                sw.Close();
                privatekeyxml.Close();
            }
            catch
            {
                throw;
            }
        }

        private void CreatePublicKeyXML(string path, string publickey)
        {
            try
            {
                System.IO.FileStream publickeyxml = new System.IO.FileStream(path, System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(publickeyxml);
                sw.WriteLine(publickey);
                sw.Close();
                publickeyxml.Close();
            }
            catch
            {
                throw;
            }
        }
    }
}