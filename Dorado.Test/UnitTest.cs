using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Queue;
using Dorado.Platform.Services;
using Dorado.Platform.Utils;
using Dorado.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Test()
        {
            string pinyin = PinYin.ConvertToAllSpell("哈哈").ToLower();
            Assert.IsNotNull(pinyin);
        }

        [TestMethod]
        public void Test1()
        {
            string xml = System.IO.File.ReadAllText("E:\\Download\\schema.xml");
            schema schema = XmlSerializerWrapper<schema>.Import(xml);
            Assert.IsNotNull(schema);

           
            string json = new JsonNetSerializer().Serialize(schema);
            Assert.IsNotNull(json);
        }

    }
}