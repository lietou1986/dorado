namespace Dorado.Security.Coder
{
    public class RondomSN
    {
        private static string[] RandSend = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"a",
			"b",
			"c",
			"d",
			"e",
			"f",
			"g",
			"h",
			"i",
			"j",
			"k",
			"l",
			"m",
			"n",
			"o",
			"p",
			"q",
			"r",
			"s",
			"t",
			"u",
			"v",
			"w",
			"x",
			"y",
			"z",
			"A",
			"B",
			"C",
			"D",
			"E",
			"F",
			"G",
			"H",
			"I",
			"J",
			"K",
			"L",
			"M",
			"N",
			"O",
			"P",
			"Q",
			"R",
			"S",
			"T",
			"U",
			"V",
			"W",
			"X",
			"Y",
			"Z",
			"@",
			"#",
			"$",
			"%",
			"&",
			"*",
			".",
			","
		};

        public static string GetRandom(int Length)
        {
            if (Length <= 0 || Length > 255)
            {
                Length = 10;
            }
            string Result = string.Empty;
            for (int i = 0; i < Length; i++)
            {
                System.Security.Cryptography.RandomNumberGenerator random = System.Security.Cryptography.RandomNumberGenerator.Create();
                byte[] randb = new byte[1];
                random.GetBytes(randb);
                int index = int.Parse(randb[0].ToString()) % 70;
                Result += RondomSN.RandSend[index];
            }
            return Result;
        }
    }
}