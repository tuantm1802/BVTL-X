using Common.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BVTL.Tests
{
    [TestClass]
    public class LoginGuardTests
    {
        [TestInitialize]
        public void Init() { LoginGuard.ClearAll(); }

        private static readonly string[] AttackPayloads = new[] {
            "-1 OR 5*5=25 --",
            "-1' OR 5*5=25 or '5krNS02u'='",
            "BlgeAjyo-1; waitfor delay '0:0:15' --",
            "if(now()=sysdate(),sleep(15),0)",
            "BlgeAjyoSr5OHdME' OR 41=(SELECT 41 FROM PG_SLEEP(15))--",
            "(select 198766*667891 from DUAL)",
            "@@jiKCY",
            "BlgeAjyo'\"",
            "BlgeAjyo????%2527%2522\\'\\\"",
            "(select(0)from(select(sleep(15)))v)/*'+(select(0)from(select(sleep(15)))v)+'\"+(select(0)from(select(sleep(15)))v)+\"*/"
        };

        private static readonly string[] NormalNames = new[] {
            "admin",
            "tuantm",
            "nguyen.van_a-01",
            "user@scdi.org.vn",
            "BlgeAjyo"
        };

        [TestMethod]
        public void AttackPayloads_AreSuspiciousAndInvalid()
        {
            foreach (var payload in AttackPayloads)
            {
                // Controller chặn khi IsSuspicious hoặc sai định dạng; mỗi payload phải bị ít nhất 1 lớp bắt
                Assert.IsTrue(LoginGuard.IsSuspicious(payload) || !LoginGuard.IsValidUserName(payload), payload);
                Assert.IsTrue(LoginGuard.IsSuspicious(payload), payload);
            }
        }

        [TestMethod]
        public void NormalUserNames_AreAccepted()
        {
            foreach (var name in NormalNames)
            {
                Assert.IsFalse(LoginGuard.IsSuspicious(name), name);
                Assert.IsTrue(LoginGuard.IsValidUserName(name), name);
            }
        }

        [TestMethod]
        public void InvalidFormats_AreRejected()
        {
            Assert.IsFalse(LoginGuard.IsValidUserName(null));
            Assert.IsFalse(LoginGuard.IsValidUserName(""));
            Assert.IsFalse(LoginGuard.IsValidUserName("ab"));
            Assert.IsFalse(LoginGuard.IsValidUserName("has space"));
            Assert.IsFalse(LoginGuard.IsValidUserName(new string('a', 51)));
        }

        [TestMethod]
        public void IpIsBlockedAfterMaxFailures()
        {
            string ip = "203.0.113.10";
            int retry;
            for (int i = 0; i < LoginGuard.MaxFailPerIp - 1; i++)
            {
                LoginGuard.RegisterIpFailure(ip);
                Assert.IsFalse(LoginGuard.IsIpBlocked(ip, out retry));
            }
            LoginGuard.RegisterIpFailure(ip);
            Assert.IsTrue(LoginGuard.IsIpBlocked(ip, out retry));
            Assert.IsTrue(retry >= 1);
            Assert.IsFalse(LoginGuard.IsIpBlocked("203.0.113.11", out retry), "IP khác không bị ảnh hưởng");
        }

        [TestMethod]
        public void LoopbackIsNeverBlocked()
        {
            int retry;
            for (int i = 0; i < 20; i++) LoginGuard.RegisterIpFailure("::1");
            Assert.IsFalse(LoginGuard.IsIpBlocked("::1", out retry));
        }

        [TestMethod]
        public void UserLockedAfterMaxFailures_AndResetOnSuccess()
        {
            int retry;
            for (int i = 0; i < LoginGuard.MaxFailPerUser; i++) LoginGuard.RegisterUserFailure("Admin");
            Assert.IsTrue(LoginGuard.IsUserLocked("admin", out retry), "Không phân biệt hoa/thường");
            LoginGuard.ResetUser("admin");
            Assert.IsFalse(LoginGuard.IsUserLocked("admin", out retry));
        }

        [TestMethod]
        public void SanitizeForLog_StripsControlCharsAndTruncates()
        {
            Assert.AreEqual("a b c", LoginGuard.SanitizeForLog("a\r\nb\tc").Replace("  ", " "));
            Assert.AreEqual(50, LoginGuard.SanitizeForLog(new string('x', 300), 50).Length);
            Assert.AreEqual("", LoginGuard.SanitizeForLog(null));
        }
    }
}
