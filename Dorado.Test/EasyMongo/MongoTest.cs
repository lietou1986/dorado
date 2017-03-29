using Dorado.EasyMongo.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;

namespace Dorado.EasyMongo.Test
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class MongoTest
    {
        public class CoalmineMetaDataMap : EntityMap<CoalmineMetaData>
        {
            public CoalmineMetaDataMap()
            {
                Collection("CoalmineMetaData");

                Property(n => n.ID).Identity();
                Property(n => n.N).DefaultValue("");
                Property(n => n.S).DefaultValue(1);
                Property(n => n.C).DefaultValue(DateTime.Now);
            }
        }

        public class CoalmineMetaData
        {
            public Guid ID { get; set; }

            public string N { get; set; }

            public int S { get; set; }

            public DateTime C { get; set; }
        }

        public MongoTest()
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
        public void Add()
        {
            MongoServerSettings settings = new MongoServerSettings
            {
                Server = new MongoServerAddress("127.0.0.1", 27017),
                SafeMode = SafeMode.False,
                ConnectTimeout = new TimeSpan(1, 1, 1)
            };
            MongoServer mongoServer = new MongoServer(settings);
            MongoDatabase database = mongoServer.GetDatabase("test");
            var map = new CoalmineMetaDataMap();
            var collection = new EntityCollection<CoalmineMetaData>(database, map.GetDescriptor(), true);

            //for ( int j = 0; j < 10; j++ )
            //{
            for (int i = 0; i < 100000; i++)
            {
                CoalmineMetaData entity = new CoalmineMetaData { ID = Guid.NewGuid(), N = "测试", S = 1, C = DateTime.Now };
                collection.InsertOnSubmit(entity);
            }
            collection.SubmitChanges();

            //}
        }

        [TestMethod]
        public void Update()
        {
        }

        [TestMethod]
        public void Delete()
        {
        }
    }
}