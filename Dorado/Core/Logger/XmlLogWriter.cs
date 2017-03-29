using System.IO;
using System.Xml.Linq;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 基于Xml文件的日志记录策略
    /// </summary>
    /// <typeparam name="TLogItem">日志类型</typeparam>
    public class XmlLogWriter<TLogItem> : FileLogWriter<TLogItem>
        where TLogItem : ILogItem
    {
        public XmlLogWriter(string directory, int maxFileSize = DefaultMaxFileSize)
            : base(new TimeFileLogWriterPathStrategy(directory))
        {
            MaxFileSize = maxFileSize;
        }

        protected override void WriteLogItems()
        {
            if (LogItems.Count == 0)
                return;

            try
            {
                lock (ThisLock)
                {
                    if (Path == null || _GetFileSize(Path) >= MaxFileSize)
                        Path = PathStrategy.GetNewPath(".xml");

                    string directory = System.IO.Path.GetDirectoryName(Path);
                    if (!File.Exists(directory))
                        Directory.CreateDirectory(directory);
                }

                if (!File.Exists(Path))
                {
                    new XElement("Logs",
                        new XAttribute(XNamespace.Xmlns + "namespace", "www.Dorado.com")).Save(Path);
                }

                lock (LogItems)
                {
                    XElement log = XElement.Load(Path);
                    LogItems.ForEach(n =>
                    log.Add(new XElement("Log", new XAttribute("Title", n.Title), new XAttribute("Type", n.Type.ToString()), new XAttribute("Time", n.Time.ToString()), new XElement("Message", n.Message))));
                    log.Save(Path);
                    LogItems.Clear();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}