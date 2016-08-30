using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreDM;
using CoreAPI.Const;

namespace CoreTest
{
    [TestClass]
    public class UTConst
    {
        /// <summary>
        /// Проверка определения идентификатора группы по коду
        /// </summary>
        [TestMethod]
        public void GroupLoader()
        {
            var l = new GroupIDLoader(new CoreEntities());
            Assert.IsTrue(l.Load("8701B02D-5329-44CD-BD87-121B3C8CCA31") > 0);
        }
        /// <summary>
        /// Менеджер загрузки значений константы из группы
        /// </summary>
        [TestMethod]
        public void ConstManager_GroupLoader()
        {
            var l = new GroupIDLoader(new CoreEntities());
            Assert.IsTrue(ConstManager.Get("8701B02D-5329-44CD-BD87-121B3C8CCA31", l) > 0);
        }
    }
}
