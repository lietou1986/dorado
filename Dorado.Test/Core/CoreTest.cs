using System;
using System.Collections.Generic;
using Dorado.Core;
using Dorado.Core.Cache;
using Dorado.Core.GlobalTimer.TimerStrategies;
using Dorado.Core.Logger;
using Dorado.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test.Core
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class CoreTest
    {
        private static readonly Cache<string, object> _memory = new Cache<string, object>();
        private static readonly GlobalTimer<ITask> _globalTimer = new GlobalTimer<ITask>(TimeSpan.FromSeconds(1));
        private static readonly TaskDispatcher<ITask> _taskDispatcher = new TaskDispatcher<ITask>();
        private static readonly ObjectPool<object> _objectPool = new ObjectPool<object>(_CreateObject, 100);

        public CoreTest()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性

        //
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion 附加测试特性

        [TestMethod]
        public void Main()
        {
            LoggerWrapper.Logger.Error("Hellow World!");

            // 1、缓存
            _memory.AddOfRelative("aaa", new object(), TimeSpan.FromSeconds(60));
            _memory.AddOfTermly("bbb", new object(), DateTime.Now.AddHours(1));
            object aaa = _memory.Get("aaa");

            // 2、定时器
            _globalTimer.Add(new EveryDayTimerStrategy(new[] { TimeSpan.Parse("12:00:00") }), new TaskFuncAdapter(_MyFunc), true);
            // 3、任务调度器
            foreach (ITask task in _GetTasks())
            {
                _taskDispatcher.Add(task);
            }
            _taskDispatcher.WaitForAllCompleted();

            // 4、对象缓存池
            object obj = _objectPool.Accquire();
            _objectPool.Release(obj);

            Console.ReadLine();
        }

        private static IEnumerable<ITask> _GetTasks()
        {
            yield break;
        }

        private static void _MyFunc()
        {
        }

        private static object _CreateObject()
        {
            return new object();
        }

        [TestMethod]
        public void Test1()
        {
            Guid guid = CommonUtility.GenerateGuid();
            string value = guid.ToString();
            Console.WriteLine(value);
            Assert.IsNotNull(value);
        }
    }
}