using Dorado.Core;
using Dorado.Core.Logger;
using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Text;

namespace Dorado.Platform.Utils
{
    public class PinYin
    {
        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string ConvertToAllSpell(string strChinese)
        {
            if (string.IsNullOrWhiteSpace(strChinese)) return "";
            try
            {
                StringBuilder fullSpell = new StringBuilder();
                for (int i = 0; i < strChinese.Length; i++)
                {
                    var chr = strChinese[i];
                    fullSpell.Append(GetSpell(chr));
                }
                return fullSpell.ToString().ToUpper();
            }
            catch (Exception e)
            {
                LoggerWrapper.Logger.Error("全拼转化出错！", e);
                return string.Empty;
            }
        }

        /// <summary>
        /// 汉字转首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetFirstSpell(string strChinese)
        {
            if (string.IsNullOrWhiteSpace(strChinese)) return "";
            try
            {
                StringBuilder fullSpell = new StringBuilder();
                for (int i = 0; i < strChinese.Length; i++)
                {
                    var chr = strChinese[i];
                    fullSpell.Append(GetSpell(chr)[0]);
                }

                return fullSpell.ToString().ToUpper();
            }
            catch (Exception e)
            {
                LoggerWrapper.Logger.Error("首字母转化出错！", e);
                return string.Empty;
            }
        }

        private static string GetSpell(char chr)
        {
            var coverchr = NPinyin.Pinyin.GetPinyin(chr);

            bool isChineses = ChineseChar.IsValidChar(coverchr[0]);
            if (isChineses)
            {
                ChineseChar chineseChar = new ChineseChar(coverchr[0]);
                foreach (string value in chineseChar.Pinyins)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value.Remove(value.Length - 1, 1);
                    }
                }
            }

            return coverchr;
        }
    }
}