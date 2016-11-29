using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace CoreTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RegNumTest()
        {
            string num = "О 343 РО 116";
            //У555ЕР116
            //У 555 ЕР 116
            num = num.Replace(" ", "");
            Regex rgx = new Regex(@"[А-Я]\d{3}[А-Я]{2}");
            Match m = rgx.Match(num);
            Regex rgxRegion = new Regex(@"\d{2,3}$");
            m = rgxRegion.Match(num);
            Console.WriteLine(m.Value);
        }
    }
}
