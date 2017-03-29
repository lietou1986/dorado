using Dorado.Core.Data;
using System.IO;

namespace Dorado.Web.FastViewEngine
{
    public interface IFastViewEngine
    {
        string Process(FastViewContext context, string templatePath);

        string Process(string templateContent, FastViewContext context);

        string Process(DataArrayList data, string templatePath);

        string Process(string templateContent, DataArrayList data);

        void Process(FastViewContext context, TextWriter writer, string templatePath);

        void Process(string templateContent, TextWriter writer, FastViewContext context);

        void Process(DataArrayList data, TextWriter writer, string templatePath);

        void Process(string templateContent, TextWriter writer, DataArrayList data);
    }
}