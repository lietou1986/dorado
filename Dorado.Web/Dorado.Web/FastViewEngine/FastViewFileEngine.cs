using Dorado.Core.Data;
using System.IO;

namespace Dorado.Web.FastViewEngine
{
    public class FastViewFileEngine : IFastViewEngine
    {
        internal FastViewFileEngine(string templatePathDirectory, bool cacheTemplate)
        {
        }

        public string Process(FastViewContext context, string templatePath)
        {
            return Process(EasyFile.Read(templatePath), context);
        }

        public string Process(string templateContent, FastViewContext context)
        {
            if (context.Data == null)
                return templateContent;

            return RegularPattern.Replace(templateContent, context.Data);
        }

        public string Process(DataArrayList data, string templatePath)
        {
            return Process(EasyFile.Read(templatePath), data);
        }

        public string Process(string templateContent, DataArrayList data)
        {
            if (data == null)
                return templateContent;

            return RegularPattern.Replace(templateContent, data);
        }

        public void Process(FastViewContext context, TextWriter writer, string templatePath)
        {
            writer.Write(Process(context, templatePath));
        }

        public void Process(string templateContent, TextWriter writer, FastViewContext context)
        {
            writer.Write(Process(templateContent, context));
        }

        public void Process(DataArrayList data, TextWriter writer, string templatePath)
        {
            writer.Write(Process(data, templatePath));
        }

        public void Process(string templateContent, TextWriter writer, DataArrayList data)
        {
            writer.Write(Process(templateContent, data));
        }
    }
}