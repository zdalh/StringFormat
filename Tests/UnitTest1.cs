using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cmd = new StringFormat.StringFormatter(@"www.g.cn?{\{\[x=\@@x$}&c=2[&a=@a$][&b=@b$321]&d=abc");

            cmd.ClearParameter();
            cmd.AddParameter("x", 1);
            cmd.AddParameter("a", 2);
            cmd.AddParameter("b", 3);
            var strTmp = cmd.Format();
            Assert.AreEqual(strTmp, @"www.g.cn?{[x=@1&c=2&a=2&b=3321&d=abc");

            cmd.ClearParameter();
            cmd.AddParameter("x", 1);
            cmd.AddParameter("a", 2);

            strTmp = cmd.Format();
            Assert.AreEqual(strTmp, @"www.g.cn?{[x=@1&c=2&a=2&d=abc");

            
        }
    }
}
