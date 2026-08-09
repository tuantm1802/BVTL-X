using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BVTL.Tests
{
    [TestClass]
    public class BaoCaoTuanLogicTests
    {
        private int MapWeekToMonth(int weekNum)
        {
            int monthNum = (weekNum - 1) / 4 + 1;
            if (monthNum > 12) monthNum = 12;
            return monthNum;
        }

        [TestMethod]
        public void MapWeekToMonth_Week1_ShouldReturnMonth1()
        {
            Assert.AreEqual(1, MapWeekToMonth(1));
        }

        [TestMethod]
        public void MapWeekToMonth_Week4_ShouldReturnMonth1()
        {
            Assert.AreEqual(1, MapWeekToMonth(4));
        }

        [TestMethod]
        public void MapWeekToMonth_Week5_ShouldReturnMonth2()
        {
            Assert.AreEqual(2, MapWeekToMonth(5));
        }

        [TestMethod]
        public void MapWeekToMonth_Week48_ShouldReturnMonth12()
        {
            Assert.AreEqual(12, MapWeekToMonth(48));
        }

        [TestMethod]
        public void MapWeekToMonth_Week53_ShouldClampToMonth12()
        {
            Assert.AreEqual(12, MapWeekToMonth(53));
        }
    }
}
