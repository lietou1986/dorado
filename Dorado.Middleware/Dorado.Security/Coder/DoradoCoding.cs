namespace Dorado.Security.Coder
{
    public class DoradoCoding
    {
        private const string book = "DoradoH M)H0[WE],9}-chena+{~GBaby9>Q'y6MP-/~9Z.II(B$)=Gh;AZ.FS'~/|@/.,]A/Z/IUASABJFSLKJ29023sajsklfjklsa09ffs2;l(_)+_=-;<>si32-0-0((_++=;lafak;si32;faka;s-203afask43//,897oj3lialjf|9279qsdes&^&#asd//w.z^iwaql}-_=+)_(l;2jaj290aslkjflkasjas320ueywe@;sdbced-+";

        public static string Encrypt(string Source)
        {
            if (Source.Length == 0)
            {
                return "";
            }
            string result;
            try
            {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(Source);
                byte[] books = System.Text.Encoding.Default.GetBytes("DoradoH M)H0[WE],9}-chena+{~GBaby9>Q'y6MP-/~9Z.II(B$)=Gh;AZ.FS'~/|@/.,]A/Z/IUASABJFSLKJ29023sajsklfjklsa09ffs2;l(_)+_=-;<>si32-0-0((_++=;lafak;si32;faka;s-203afask43//,897oj3lialjf|9279qsdes&^&#asd//w.z^iwaql}-_=+)_(l;2jaj290aslkjflkasjas320ueywe@;sdbced-+");
                byte[] Arry2 = new byte[bytes.Length + 9];
                byte pXY = 0;
                for (int i = 0; i < 8; i++)
                {
                    Arry2[i] = System.Convert.ToByte(new System.Random().Next(0, 255));
                }
                for (int j = 8; j < Arry2.Length - 1; j++)
                {
                    Arry2[j] = System.Convert.ToByte((int)(bytes[j - 8] ^ books[(int)Arry2[j % 8]]));
                    pXY = System.Convert.ToByte((int)(pXY + bytes[j - 8] & 255));
                }
                Arry2[Arry2.Length - 1] = System.Convert.ToByte((int)(pXY ^ books[(int)Arry2[(Arry2.Length - 1) % 8]]));
                string rtnString = System.Convert.ToBase64String(Arry2);
                rtnString = rtnString.Replace("+", "-");
                result = rtnString;
            }
            catch
            {
                result = "";
            }
            return result;
        }

        public static string Decrypt(string Source)
        {
            if (Source.Length == 0)
            {
                return "";
            }
            string decString = Source.Replace("-", "+");
            string result;
            try
            {
                byte[] bytes = System.Convert.FromBase64String(decString);
                byte[] books = System.Text.Encoding.Default.GetBytes("DoradoH M)H0[WE],9}-chena+{~GBaby9>Q'y6MP-/~9Z.II(B$)=Gh;AZ.FS'~/|@/.,]A/Z/IUASABJFSLKJ29023sajsklfjklsa09ffs2;l(_)+_=-;<>si32-0-0((_++=;lafak;si32;faka;s-203afask43//,897oj3lialjf|9279qsdes&^&#asd//w.z^iwaql}-_=+)_(l;2jaj290aslkjflkasjas320ueywe@;sdbced-+");
                byte[] ArryRnd = new byte[8];
                byte[] Arry2 = new byte[bytes.Length - 9];
                byte pXY = 0;
                for (int i = 0; i < 8; i++)
                {
                    ArryRnd[i] = bytes[i];
                }
                for (int j = 8; j < bytes.Length - 1; j++)
                {
                    Arry2[j - 8] = System.Convert.ToByte((int)(bytes[j] ^ books[(int)ArryRnd[j % 8]]));
                    pXY = System.Convert.ToByte((int)(pXY + Arry2[j - 8] & 255));
                }
                byte pCRC = System.Convert.ToByte((int)(bytes[bytes.Length - 1] ^ books[(int)ArryRnd[(bytes.Length - 1) % 8]]));
                if (pXY == pCRC)
                {
                    result = System.Text.Encoding.Default.GetString(Arry2);
                }
                else
                {
                    result = "";
                }
            }
            catch
            {
                result = "";
            }
            return result;
        }
    }
}