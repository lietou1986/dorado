using Dorado.Configuration;
using Dorado.Core.Queue;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test
{
    [TestClass]
    public class MsmqTest
    {
        [TestMethod]
        public void SendTest()
        {
            using (IQueue<string> queue = new MsmqQueue<string>(AppSettingProvider.Get("MointorDataQueue")))
            {
                bool result = queue.Push("test1");
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void ReceiveTest()
        {
        }
    }
}