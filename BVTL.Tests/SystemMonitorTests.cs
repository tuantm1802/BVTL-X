using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.SystemMonitor;
using System;

namespace BVTL.Tests
{
    [TestClass]
    public class SystemMonitorTests
    {
        [TestMethod]
        public void UserOnlineModel_WhenInactiveUnder2Minutes_ShouldBeOnline()
        {
            var user = new UserOnlineModel
            {
                LastActiveTime = DateTime.Now.AddSeconds(-30)
            };

            Assert.IsTrue(user.IsOnline);
            Assert.IsFalse(user.IsIdle);
        }

        [TestMethod]
        public void UserOnlineModel_WhenInactive5Minutes_ShouldBeIdle()
        {
            var user = new UserOnlineModel
            {
                LastActiveTime = DateTime.Now.AddMinutes(-5)
            };

            Assert.IsFalse(user.IsOnline);
            Assert.IsTrue(user.IsIdle);
        }

        [TestMethod]
        public void LoginHistoryModel_SessionDuration_ShouldFormatCorrectly()
        {
            var loginTime = new DateTime(2026, 8, 15, 8, 0, 0);
            var logoutTime = new DateTime(2026, 8, 15, 9, 35, 20);

            var history = new LoginHistoryModel
            {
                LoginTime = loginTime,
                LogoutTime = logoutTime
            };

            Assert.AreEqual("1 giờ 35 phút", history.SessionDurationText);
        }

        [TestMethod]
        public void LoginHistoryModel_WhenLogoutIsNull_ShouldIndicateActive()
        {
            var history = new LoginHistoryModel
            {
                LoginTime = DateTime.Now,
                LogoutTime = null
            };

            Assert.AreEqual("Đang hoạt động / Tự hết hạn", history.SessionDurationText);
        }
    }
}
