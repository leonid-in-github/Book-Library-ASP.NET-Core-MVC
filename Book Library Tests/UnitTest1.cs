using Book_Library_EF_Core_Proxy_Class_Library.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Book_Library_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestLogin()
        {
            var login = "login1";
            var password = "123123123";
            var sessionId = "TEST";
            var accountId =
                    dbBookLibraryProxy.Account.Login(sessionId, login, password);

            Assert.IsTrue(accountId > 0);
        }
    }
}
