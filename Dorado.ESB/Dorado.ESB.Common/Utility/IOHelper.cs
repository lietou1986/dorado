using System.IO;
using System.Text;

namespace Dorado.ESB.Common.Utility
{
    /// <summary>IO Helper Class</summary>
    public static class IOHelper
    {
        /// <summary>
        /// Checks if the file exists at the specified path
        /// </summary>
        /// <param name="filename"></param>
        public static bool CheckFileExists(string filename)
        {
            Check.NotEmpty(filename, "filename");
            FileInfo file = new FileInfo(filename);
            return file.Exists;
        }

        /// <summary>
        /// Writes the data to the file at filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        public static void WriteFile(string filename, byte[] data)
        {
            Check.NotEmpty(filename, "filename");
            Check.NotNull(data, "file data");

            EnsureDirectoryStructureExists(filename);

            using (BufferedStream bs = new BufferedStream(new FileInfo(filename).Open(FileMode.OpenOrCreate, FileAccess.Write)))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    bs.WriteByte(data[i]);
                }
            }
        }

        /// <summary>
        /// Ensures the directory structure exists for the filename.
        /// </summary>
        /// <param name="filename"></param>
        public static void EnsureDirectoryStructureExists(string filename)
        {
            Check.NotEmpty(filename, "filename");

            FileInfo fileInfo = new FileInfo(filename);
            DirectoryInfo directoryInfo = fileInfo.Directory;

            Directory.CreateDirectory(directoryInfo.FullName);
        }

        /// <summary>
        /// Reads the contents of the file at filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ReadFile(string filename)
        {
            Check.NotEmpty(filename, "filename");

            string str;

            using (StreamReader reader = new StreamReader(filename))
            {
                str = reader.ReadToEnd();
            }

            return str;
        }

        /// <summary>
        /// Create Memory Stream
        /// </summary>
        /// <param name="contents">Contents of stream</param>
        /// <param name="encoding">Encoding of stream</param>
        /// <returns>
        /// Non-resizable Memory Stream loaded with contents
        /// </returns>
        public static MemoryStream CreateMemoryStream(string contents, Encoding encoding)
        {
            Check.NotNull(contents, "contents");

            byte[] bytes = encoding.GetBytes(contents);
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// Read Contents From Stream
        /// </summary>
        /// <param name="stream">Stream object</param>
        /// <returns>string contents of stream</returns>
        public static string ReadContentsFromStream(Stream stream)
        {
            Check.NotNull(stream, "stream");
            Check.True(stream.CanRead, "stream.CanRead", "The stream must be readable.");

            string str;

            try
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                StreamReader streamReader = new StreamReader(stream, true);
                str = streamReader.ReadToEnd();
                streamReader.Close();
            }
            finally
            {
                stream.Close();
            }

            return str;
        }

        /// <summary>
        /// Append To File
        /// </summary>
        /// <param name="fileName">name of File to append</param>
        /// <param name="contents">data to append to file</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AppendToFile(string fileName, string contents)
        {
            Check.NotEmpty(fileName, "file name");

            // Check for any invalid characters in the filename.
            if (fileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                return false;
            }

            StreamWriter writer = new StreamWriter(fileName, true);

            try
            {
                writer.Write(contents);
                writer.Flush();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            return true;
        }
    }
}