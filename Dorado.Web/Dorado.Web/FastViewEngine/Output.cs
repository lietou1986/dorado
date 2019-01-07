using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Extensions;
using System;
using System.ComponentModel;

namespace Dorado.Web.FastViewEngine
{
    /// <summary>
    /// Output 的摘要说明。
    /// </summary>
    public class Output
    {
        public static string Success(object value)
        {
            return Success(value.ToString());
        }

        public static string Success()
        {
            return Success("操作成功");
        }

        public static string Success(string value, params object[] args)
        {
            return Write(OutputType.Success, value, args);
        }

        public static string Error(Exception ex)
        {
            LoggerWrapper.Logger.Error(ex.Message, ex);
#if DEBUG
            return Write(OutputType.Alert, ex.ToString());

#else
            return Write(OutputType.Alert, ex.Message);

#endif
        }

        public static string Error(string message, Exception ex)
        {
            LoggerWrapper.Logger.Error(message, ex);
#if DEBUG
            return Write(OutputType.Alert, ex.ToString());

#else
            return Write(OutputType.Alert, message);

#endif
        }

        public static string Error(string ex)
        {
            LoggerWrapper.Logger.Error(ex);
            return Write(OutputType.Alert, ex);
        }

        public static string Error(string ex, params object[] args)
        {
            LoggerWrapper.Logger.Error(ex);
            return Write(OutputType.Alert, ex, args);
        }

        public static string Script(string value, params object[] args)
        {
            return Write(OutputType.Script, value, args);
        }

        public static string Alert(int value)
        {
            return Write(OutputType.Alert, value);
        }

        public static string Alert(long value)
        {
            return Write(OutputType.Alert, value);
        }

        public static string Alert(byte value)
        {
            return Write(OutputType.Alert, value);
        }

        public static string Alert(double value)
        {
            return Write(OutputType.Alert, value);
        }

        public static string Alert(float value)
        {
            return Write(OutputType.Alert, value);
        }

        public static string Alert(string value, params object[] args)
        {
            return Write(OutputType.Alert, string.Format(value, args));
        }

        public static string Alert(object value)
        {
            return Write(OutputType.Alert, value);
        }

        public static string Write(OutputType type, int value)
        {
            return string.Format("T.{0}={1};", type.GetDefaultDescription(), value);
        }

        public static string Write(OutputType type, long value)
        {
            return string.Format("T.{0}={1};", type.GetDefaultDescription(), value);
        }

        public static string Write(OutputType type, byte value)
        {
            return string.Format("T.{0}={1};", type.GetDefaultDescription(), value);
        }

        public static string Write(OutputType type, double value)
        {
            return string.Format("T.{0}={1};", type.GetDefaultDescription(), value);
        }

        public static string Write(OutputType type, float value)
        {
            return string.Format("T.{0}={1};", type.GetDefaultDescription(), value);
        }

        public static string Write(OutputType type, string value, params object[] args)
        {
            return "T." + type.GetDefaultDescription() + "=" + DataTypeExtensions.ToSafeString(args.Length > 0 ? string.Format(value, args) : value) + ";";
        }

        public static string Write(OutputType type, object value)
        {
            return Write(type, value.ToString());
        }
    }

    public enum OutputType
    {
        [Description("success")]
        Success,

        [Description("alert")]
        Alert,

        [Description("script")]
        Script
    }
}