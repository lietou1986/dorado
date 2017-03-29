using Dorado.Core;
using Dorado.Core.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test.Core
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            LoggerWrapper.Logger = new Logger<LogItem>(new DBLogWriter<LogItem>("Data Source=.\\SqlExpress;Initial Catalog=Dorado_Platform_5isolar_Log;Integrated Security=True;MultipleActiveResultSets=True;"));
            for (int i = 0; i < 100000; i++)
            {
                LoggerWrapper.Logger.Error("出错了", "日志记录测试日志记录测试");
                LoggerWrapper.Logger.Error("出错了", "日志记录测试日志记录测试");
                LoggerWrapper.Logger.Error("出错了", "日志记录测试日志记录测试");
                LoggerWrapper.Logger.Error("出错了", "日志记录测试日志记录测试");
                LoggerWrapper.Logger.Error("出错了", "日志记录测试日志记录测试");
            }
        }
    }
}