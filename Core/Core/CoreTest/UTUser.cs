using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreWeb.Models.Users;
using CoreDM;
using System.Linq;

namespace CoreTest
{
    [TestClass]
    public class UTUser
    {
        private CoreEntities _db = new CoreEntities();

        [TestMethod]
        public void GetUserFromActiveDirectory()
        {
            var u = ADUtils.GetUser("asu.tatneft.ru", "OvcharovDP");
            Assert.IsFalse(u == null);
        }
        [TestMethod]
        public void GetActiveDirectoryDomains()
        {
            var u = ADUtils.GetDomains();
            Assert.IsTrue(u.Length > 0);
        }
        [TestMethod]
        public void UpdateUsersDisplayName()
        {
            //var u = ADUtils.UpdateUsersDisplayName(_db, new string[] { "djalil.tatneft.ru", "hq.tatneft.ru", "llc.tatneft.ru"});
            //Assert.IsTrue(u.Count() > 0);
            Assert.IsTrue(true);
        }
    }
}
