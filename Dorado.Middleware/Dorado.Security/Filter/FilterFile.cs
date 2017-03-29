using System.Web;

namespace Dorado.Security.Filter
{
    public class FilterFile
    {
        public enum ProvidesType
        {
            text,
            image,
            audio,
            video,
            application
        }

        public static bool IsType(HttpPostedFileBase Fileupload, FilterFile.ProvidesType Ptype)
        {
            bool ret = false;
            string FilterType = string.Empty;
            switch (Ptype)
            {
                case FilterFile.ProvidesType.text:
                    {
                        FilterType = "text";
                        break;
                    }
                case FilterFile.ProvidesType.image:
                    {
                        FilterType = "image";
                        break;
                    }
                case FilterFile.ProvidesType.audio:
                    {
                        FilterType = "audio";
                        break;
                    }
                case FilterFile.ProvidesType.video:
                    {
                        FilterType = "video";
                        break;
                    }
                case FilterFile.ProvidesType.application:
                    {
                        FilterType = "application";
                        break;
                    }
            }
            string type = Fileupload.ContentType.ToLower();
            if (type.Contains(FilterType))
            {
                ret = true;
            }
            return ret;
        }

        public static bool IsAllowedPicture(HttpPostedFileBase hifile)
        {
            bool ret = false;
            byte[] fileData = new byte[2];
            hifile.InputStream.Read(fileData, 0, 2);
            string fileclass = "";
            bool result;
            try
            {
                fileclass = fileData[0].ToString();
                fileclass += fileData[1].ToString();
            }
            catch
            {
                result = false;
                return result;
            }
            hifile.InputStream.Position = 0L;
            string[] fileType = new string[]
			{
				"255216",
				"7173",
				"6677",
				"13780"
			};
            for (int i = 0; i < fileType.Length; i++)
            {
                if (fileclass == fileType[i])
                {
                    ret = true;
                    break;
                }
            }
            return ret;
            return result;
        }

        public static bool CheckIsTextFile(string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            bool isTextFile = true;
            bool result;
            try
            {
                int i = 0;
                int length = (int)fs.Length;
                while (i < length && isTextFile)
                {
                    byte data = (byte)fs.ReadByte();
                    isTextFile = (data != 0);
                    i++;
                }
                result = isTextFile;
            }
            catch (System.Exception arg_39_0)
            {
                System.Exception ex = arg_39_0;
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return result;
        }
    }
}